using SqlKata;
using SqlKata.Execution;
using System.Data;

namespace KiloMart.DataAccess.Contracts;

public interface IDbFactory
{
    IDbConnection CreateDbConnection();
    QueryFactory CreateSqlQuery();
}
