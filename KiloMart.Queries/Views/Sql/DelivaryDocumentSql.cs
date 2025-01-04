using KiloMart.Core.Sql;

namespace KiloMart.Queries.Views.Sql;

public partial class Sql 
{
    public static string DelivaryDocumentSql => @"
    SELECT
        [Id],
        [Name],
        [Url],
        [Delivary],
        [DocumentType]
    FROM 
        dbo.[DelivaryDocument]";

    public static DbQuery<DelivaryDocumentSqlResponse> DelivaryDocumentSqlQuery 
    => new(DelivaryDocumentSql);
}

public class DelivaryDocumentSqlResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Url { get; set; } = null!;
    public int Delivary { get; set; }
    public byte DocumentType { get; set; }
}
