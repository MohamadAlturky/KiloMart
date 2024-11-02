using Microsoft.Data.SqlClient;
using SqlKata.Compilers;
using SqlKata.Execution;
using System.Data;

namespace KiloMart.DataAccess.Contracts;

public class DbFactory(string connectionString) : IDbFactory
{

    public IDbConnection CreateDbConnection()
    {
        return new SqlConnection(connectionString);
    }

    public QueryFactory CreateSqlQuery()
    {
        var connection = new SqlConnection(connectionString);
        var compiler = new SqlServerCompiler();
        return new QueryFactory(connection, compiler);
    }
}