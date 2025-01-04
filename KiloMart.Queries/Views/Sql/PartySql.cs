using KiloMart.Core.Sql;

namespace KiloMart.Queries.Views.Sql;

public partial class Sql 
{
    public static string PartySql => @"
    SELECT
        [Id],
        [IsActive],
        [DisplayName]
    FROM 
        dbo.[Party]";

    public static DbQuery<PartySqlResponse> PartySqlQuery 
    => new(PartySql);
}

public class PartySqlResponse
{
    public int Id { get; set; }
    public bool IsActive { get; set; }
    public string DisplayName { get; set; } = null!;
}
