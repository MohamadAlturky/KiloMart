using KiloMart.Core.Sql;

namespace KiloMart.Queries.Views.Sql;

public partial class Sql 
{
    public static string PhoneNumberSql => @"
    SELECT
        [Id],
        [Value],
        [Party],
        [IsActive]
    FROM 
        dbo.[PhoneNumber]";

    public static DbQuery<PhoneNumberSqlResponse> PhoneNumberSqlQuery 
    => new(PhoneNumberSql);
}

public class PhoneNumberSqlResponse
{
    public int Id { get; set; }
    public string Value { get; set; } = null!;
    public int Party { get; set; }
    public bool IsActive { get; set; }
}
