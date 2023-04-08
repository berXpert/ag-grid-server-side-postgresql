using SqlKata;

namespace QueryBuilder;

public interface IQueryFactoryWrapper
{
    Query Query(string table);
    T Get<T>(Query query);
    void SetLogger(Action<SqlResult> logger);
    Query Query();
}