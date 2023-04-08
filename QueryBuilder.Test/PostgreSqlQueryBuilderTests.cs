using SqlKata;
using Contracts;
using NSubstitute;
using SqlKata.Compilers;
using FluentAssertions;
using System.Globalization;

namespace QueryBuilder.Test;

public class PostgreSqlQueryBuilderTests
{
    private readonly PostgreSqlQueryBuilder _postgreSqlQueryBuilder;
    private readonly PostgresCompiler _compiler = new();

    public PostgreSqlQueryBuilderTests()
    {
        // You can now mock the IQueryFactoryWrapper interface and pass it to the constructor
        var queryFactoryWrapperMock = Substitute.For<IQueryFactoryWrapper>();
        _postgreSqlQueryBuilder = new PostgreSqlQueryBuilder(queryFactoryWrapperMock);
    }

    [Fact]
    public void TestAddBooleanFilter()
    {
        var query = new Query("test_table");
        var columnFilter = new ColumnFilter
        {
            FilterType = "boolean",
            Filter = "true",
        };

        PostgreSqlQueryBuilder.AddBooleanFilter("is_active", query, columnFilter);

        var expectedSql = "SELECT * FROM \"test_table\" WHERE \"is_active\" = true";
        string actualSql = GetSqlFrom(query);

        actualSql.Should().Be(expectedSql);
    }

    [Fact]
    public void TestAddSetFilter()
    {
        var query = new Query("test_table");
        var columnFilter = new ColumnFilter
        {
            FilterType = "set",
            Values = new List<string> { "value1", "value2", "value3" }
        };

        PostgreSqlQueryBuilder.AddSetFilter("category", query, columnFilter);

        var expectedSql = "SELECT * FROM \"test_table\" WHERE \"category\" IN ('value1', 'value2', 'value3')";
        string actualSql = GetInlinedQueryString(_compiler.Compile(query));

        actualSql.Should().Be(expectedSql);
    }

    [Fact]
    public void TestAddTextFilter()
    {
        var query = new Query("test_table");
        var columnFilter = new ColumnFilter
        {
            FilterType = "text",
            Type = "startsWith",
            Filter = "test"
        };

        PostgreSqlQueryBuilder.AddTextFilter("name", query, columnFilter);

        var expectedSql = "SELECT * FROM \"test_table\" WHERE \"name\" ilike 'test%'";
        string actualSql = GetInlinedQueryString(_compiler.Compile(query));

        actualSql.Should().Be(expectedSql);
    }

    [Fact]
    public void TestAddNumericFilter()
    {
        var query = new Query("test_table");
        var columnFilter = new ColumnFilter
        {
            FilterType = "number",
            Type = "inRange",
            Filter = "10",
            FilterTo = "20"
        };

        PostgreSqlQueryBuilder.AddNumericFilter("price", query, columnFilter);
        var expectedSql = "SELECT * FROM \"test_table\" WHERE \"price\" BETWEEN 10 AND 20";
        string actualSql = GetInlinedQuery(_compiler.Compile(query));

        actualSql.Should().Be(expectedSql);
    }

    private static string GetSqlFrom(Query query)
    {
        var compiler = new PostgresCompiler();
        var result = compiler.Compile(query);
        var actualSql = result.Sql;
        return actualSql;
    }

    private string GetInlinedQueryString(SqlResult compiledQuery)
    {
        string queryString = compiledQuery.ToString();
        var bindings = compiledQuery.Bindings;

        for (int i = 0; i < bindings.Count; i++)
        {
            string placeholder = $"@p{i}";
            string value = bindings[i] is string ? $"'{bindings[i]}'" : bindings[i].ToString();
            queryString = queryString.Replace(placeholder, value);
        }

        return queryString;
    }

    private string GetInlinedQuery(SqlResult compiledQuery)
    {
        string queryString = compiledQuery.ToString();
        var bindings = compiledQuery.Bindings;

        for (int i = 0; i < bindings.Count; i++)
        {
            string placeholder = $"@p{i}"; // Named placeholders use @p0, @p1, etc.
            string value;

            if (bindings[i] is string)
            {
                value = $"'{bindings[i]}'";
            }
            else if (bindings[i] is int || bindings[i] is double || bindings[i] is decimal || bindings[i] is float)
            {
                value = Convert.ToString(bindings[i], CultureInfo.InvariantCulture);
            }
            else
            {
                value = bindings[i].ToString();
            }

            queryString = queryString.Replace(placeholder, value);
        }

        return queryString;
    }

}
