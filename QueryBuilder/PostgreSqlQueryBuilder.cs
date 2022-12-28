using System.Text;
using Contracts;
using SqlKata;
using SqlKata.Execution;

namespace QueryBuilder;

public class PostgreSqlQueryBuilder
{
    public GridRowsResponse Build(string table, QueryFactory _db, GridRowsRequest request)
    {
        var result = new GridRowsResponse();
        var query = _db.Query(table);

        SelectSql(query, request);
        WhereSql(query, request);
        GroupBySql(query, request);

        if (!request.PivotMode)
        {
            query.Limit(request.EndRow - request.StartRow + 1)
            .Offset(request.StartRow);
            OrderBySql(query, request);
        }

        var pivotQuery = PivotColumns(_db, query, request);
        PivotOrderBy(pivotQuery, request);
        PivotGroupBy(pivotQuery, request);
        PivotSort(_db, pivotQuery, request);

        if (request.PivotMode)
        {
            pivotQuery.Limit(request.EndRow - request.StartRow + 1)
            .Offset(request.StartRow);
            OrderBySql(pivotQuery, request);
        }

        var fullJson = request.PivotMode ? CreatePivotJsonSql(_db, pivotQuery) : CreateJsonSql(_db, query);

        var jsonResult = fullJson.Get<string>().FirstOrDefault() ?? string.Empty;
        result.Data = jsonResult.Length == 0 ? "[]" : jsonResult;

        // Get the list of secondary columns, those dynamically created by the pivot
        // Badminton|gold, Badminton|silver, Badminton|bronze, Basketball|gold, Basketball|silver, Basketball|bronze
        if (request.PivotMode && request.PivotCols.Count > 0)
        {
            var q = new List<string>();
            request.PivotCols.ForEach(p => q.Add($"\"{p.Field}\""));

            var pivotColumns = string.Join(" || '|' || ", q);

            var listedPivotColumns = string.Join(", ", q);

            var secondaryColumnInnerQuery = _db.Query()
                .FromRaw(table)
                .SelectRaw($"Distinct on({listedPivotColumns}) {listedPivotColumns}");

            // It is missing a where clause here, to include the Pivot values

            var secondaryColumnQuery = _db.Query(string.Empty)
                .From(secondaryColumnInnerQuery.As("secondary_columns"))
                .SelectRaw($"{pivotColumns}")
                .WhereRaw($"{pivotColumns} IS NOT NULL");

            PivotColumnsOrderBy(secondaryColumnQuery, request.PivotCols);

            var scl = secondaryColumnQuery.Get<string>().ToList();

            result.SecondaryColumns = (from s in scl
                                       from v in request.ValueCols
                                       select $"{s}|{v.Field}"
                                       ).ToList();
        }

        return result;
    }
    private void PivotColumnsOrderBy(Query query, List<ColumnVO> pivotCols)
    {
        if (pivotCols.Count > 0)
        {
            pivotCols.ForEach(p =>
            {
                var sortModel = new SortModel
                {
                    ColId = p.Field,
                    Sort = "asc"
                };
                query.OrderByRaw($"\"{p.Field}\" {sortModel.Sort} nulls last");
            });
        }
    }

    private void PivotSort(QueryFactory _db, Query query, GridRowsRequest request)
    {
        if (request.PivotMode)
        {
            var sortQuery = _db.Query(string.Empty)
                .From(query.As("sorted_data"))
                .SelectRaw("data");

            // sort the columns out of the Json query
            // ORDER BY data -> '2022|year' desc null last
            request.SortModel.Where(sort =>
                sort.ColId.Contains('|')
            ).ToList()
            .ForEach(s => sortQuery.OrderByRaw($"data -> '{s.ColId}' {s.Sort} null last"));
        }
    }

    private void PivotGroupBy(Query query, GridRowsRequest request)
    {
        // Add the group by fields
        if (request.PivotMode)
        {
            request.RowGroupCols.Take(request.GroupKeys.Count + 1).ToList()
                .ForEach(r => query.GroupBy(r.Field));
        }
    }

    private void PivotOrderBy(Query query, GridRowsRequest request)
    {
        // If we are pivoting, we need to order by the pivot columns
    }

    private Query PivotColumns(QueryFactory _db, Query query, GridRowsRequest request)
    {
        if (request.PivotMode)
        {
            var pivotFields = new StringBuilder();
            request.PivotCols.ForEach(p => pivotFields.Append('\"').Append(p.Field).Append("\" || '|' || "));

            // Add the group by fields
            var queryString = new List<string>();
            request.RowGroupCols.Take(request.GroupKeys.Count + 1).ToList()
                .ForEach(r => queryString.Add($"json_object_agg('{r.Field}', \"{r.Field}\")::jsonb"));

            // Add the aggregate values
            request.ValueCols.ForEach(v => queryString.Add($"json_object_agg({pivotFields}'{v.Field}', \"{v.Field}\")::jsonb"));

            // Concatenate the json objects
            var jsonConcat = string.Join(" || ", queryString) + " AS data";
            Console.WriteLine(jsonConcat);

            return _db.Query(string.Empty)
                .From(query.As("the_data"))
                .SelectRaw(jsonConcat);
        }
        return query;
    }

    private void GroupBySql(Query query, GridRowsRequest request)
    {
        if (request.PivotMode && request.PivotCols.Count > 0)
        {
            var include = request.PivotCols.Where(
                p => !request.RowGroupCols.Take(request.GroupKeys.Count + 1)
                    .Any(r => r.Id == p.Id))
                    .ToList();

            include.ForEach(p =>
            {
                query.SelectRaw($"coalesce(text(\"{p.Field}\"), 'Unknown') AS \"{p.Field}\"");
                query.GroupBy(p.Field);
            });
        }
    }

    private Query CreateJsonSql(QueryFactory _db, Query query)
    {
        var jsonQuery = _db.Query(string.Empty)
            .From(query.As("the_data"))
            .SelectRaw(@"row_to_json(""the_data"") AS data");

        var fullJson = _db.Query(string.Empty)
            .From(jsonQuery.As("x"))
            .SelectRaw("json_agg(data) AS data");

        return fullJson;
    }

    private Query CreatePivotJsonSql(QueryFactory _db, Query query)
    {
        var fullJson = _db.Query(string.Empty)
            .From(query.As("x"))
            .SelectRaw("json_agg(data) AS data");

        return fullJson;
    }

    private void SelectSql(Query query, GridRowsRequest request)
    {
        var rowGroupCols = request.RowGroupCols;
        var valueCols = request.ValueCols;
        var groupKeys = request.GroupKeys;

        if (IsDoingGrouping(rowGroupCols, groupKeys) || request.PivotMode)
        {
            var rowGroupCol = rowGroupCols[groupKeys.Count];
            var colsToSelect = new List<string>() { rowGroupCol.Id };

            valueCols.ForEach(valueCol =>
            {
                if (request.PivotMode)
                {
                    query.SelectRaw(valueCol.AggFunc + "(\"" + valueCol.Id + "\") AS " + valueCol.Id);
                }
                else
                {
                    query.SelectRaw(valueCol.AggFunc + "(\"" + valueCol.Id + "\") AS \"" + valueCol.Id + "\"");
                }
            }
            );

            request.RowGroupCols.Take(request.GroupKeys.Count + 1).ToList()
            .ForEach(r =>
            {
                query.Select(r.Field);
                query.GroupBy(r.Field);
            });
        }
    }

    private void WhereSql(Query query, GridRowsRequest request)
    {
        for (int i = 0; i < request.GroupKeys.Count; i++)
        {
            query.Where(request.RowGroupCols[i].Field, request.GroupKeys[i]);
        }

        FilterSql(query, request.FilterModel);
    }

    private void FilterSql(Query query, Dictionary<string, ColumnFilter> filterModel)
    {
        filterModel.Where(f => f.Value.FilterType == "number").ToList()
            .ForEach(filter => AddNumericFilter(filter.Key, query, filter.Value));

        filterModel.Where(f => f.Value.FilterType == "text").ToList()
            .ForEach(filter => AddTextFilter(filter.Key, query, filter.Value));

        filterModel.Where(f => f.Value.FilterType == "set").ToList()
            .ForEach(filter => AddSetFilter(filter.Key, query, filter.Value));

        filterModel.Where(f => f.Value.FilterType == "boolean").ToList()
            .ForEach(filter => AddBooleanFilter(filter.Key, query, filter.Value));
    }

    private void AddBooleanFilter(string field, Query query, ColumnFilter columnFilter)
    {
        throw new NotImplementedException();
    }

    private void AddSetFilter(string field, Query query, ColumnFilter columnFilter)
    {
        query.WhereIn(field, columnFilter.Values);
    }

    private void AddTextFilter(string field, Query query, ColumnFilter columnFilter)
    {
        _ = columnFilter.Type switch
        {
            "contains" => query.WhereContains(field, columnFilter.Filter),
            "notContains" => query.WhereNotContains(field, columnFilter.Filter),
            "startsWith" => query.WhereStarts(field, columnFilter.Filter),
            "endsWith" => query.WhereEnds(field, columnFilter.Filter),
            _ => query.Where(field, GetOperatorForFilterType(columnFilter.Type), columnFilter.Filter)
        };
    }

    private void AddNumericFilter(string field, Query query, ColumnFilter columnFilter)
    {
        if (columnFilter.Type == "inRange")
        {
            query.WhereBetween(field, int.Parse(columnFilter.Filter), int.Parse(columnFilter.FilterTo ?? "0"));
        }
        else
        {
            var op = GetOperatorForFilterType(columnFilter.Type);
            query.Where(field, op, int.Parse(columnFilter.Filter));
        }
    }

    private string GetOperatorForFilterType(string filterType)
    {
        return filterType switch
        {
            "equals" => "=",
            "notEqual" => "!=",
            "lessThan" => "<",
            "lessThanOrEqual" => "<=",
            "greaterThan" => ">",
            "greaterThanOrEqual" => ">=",
            _ => "",
        };
    }

    private void OrderBySql(Query query, GridRowsRequest request)
    {
        if (request.SortModel.Count == 0)
        {
            return;
        }
        var rowGroupCols = request.RowGroupCols;
        var valueCols = request.ValueCols;
        var groupKeys = request.GroupKeys;

        var isGrouping = IsDoingGrouping(rowGroupCols, groupKeys);

        var groupColIds = request.RowGroupCols.Take(groupKeys.Count + 1)
                            .Select(groupCol => groupCol.Id).ToList();

        var valueColIds = request.ValueCols
                            .ConvertAll(valCol => valCol.Id);

        request.SortModel.ForEach(s =>
        {
            if (isGrouping && groupColIds.IndexOf(s.ColId) < 0 &&
                 valueColIds.IndexOf(s.ColId) < 0
             )
            {
                // ignore if it is grouping and the request is asking to sort by a column that is not
                // in the group or aggregation columns
            }
            else
            {
                query.OrderByRaw($"\"{s.ColId}\" {s.Sort} nulls last");
            }
        }
        );
    }

    private bool IsDoingGrouping(List<ColumnVO> rowGroupCols, List<string> groupKeys)
    {
        // we are not doing grouping if at the lowest level
        return rowGroupCols.Count > groupKeys.Count;
    }

}
