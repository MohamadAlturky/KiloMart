using SqlKata;
using SqlKata.Execution;
using System.Data;

namespace KiloMart.Core.Contracts;

public interface IDbFactory
{
    IDbConnection CreateDbConnection();
    QueryFactory CreateSqlQuery();
}
