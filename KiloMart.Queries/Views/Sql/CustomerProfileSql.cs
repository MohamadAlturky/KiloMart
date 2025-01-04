using KiloMart.Core.Sql;

namespace KiloMart.Queries.Views.Sql;

public partial class Sql 
{
    public static string CustomerProfileSql => @"
    SELECT
        [Id],
        [Customer],
        [FirstName],
        [SecondName],
        [NationalName],
        [NationalId]
    FROM 
        dbo.[CustomerProfile]";

    public static DbQuery<CustomerProfileSqlResponse> CustomerProfileSqlQuery 
    => new(CustomerProfileSql);
}

public class CustomerProfileSqlResponse
{
    public int Id { get; set; }
    public int Customer { get; set; }
    public string FirstName { get; set; } = null!;
    public string SecondName { get; set; } = null!;
    public string NationalName { get; set; } = null!;
    public string NationalId { get; set; } = null!;
}
