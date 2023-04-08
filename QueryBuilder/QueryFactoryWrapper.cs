using SqlKata;
using SqlKata.Execution;

namespace QueryBuilder;

public class QueryFactoryWrapper : IQueryFactoryWrapper
{
    private readonly QueryFactory _queryFactory;

    public QueryFactoryWrapper(QueryFactory queryFactory)
    {
        _queryFactory = queryFactory;
    }

    public Query Query(string table)
    {
        return _queryFactory.Query(table);
    }

    public Query Query()
    {
        return _queryFactory.Query();
    }

    public T Get<T>(Query query)
    {
        return query.Get<T>().FirstOrDefault();
    }

    public void SetLogger(Action<SqlResult> logger)
    {
        _queryFactory.Logger = logger;
    }
}
