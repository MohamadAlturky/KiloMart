//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Dapper;

//namespace KiloMart.Requests.Queries
//{
//    public partial class Query
//    {
//        public static async Task<DiscountCodeApiResponse[]> GetAllDiscountCodesPaginated(
//            IDbConnection connection,
//            int page = 1,
//            int pageSize = 10)
//        {
//            int skip = (page - 1) * pageSize;
//            var query = @"
//            SELECT 
//                [d].[Id],
//                [d].[Code],
//                [d].[Value],
//                [d].[Description],
//                [d].[StartDate],
//                [d].[EndDate],
//                [d].[DiscountType],
//                [d].[IsActive]
//            FROM DiscountCode [d]
//            WHERE [d].[IsActive] = 1  
//            ORDER BY [d].[Id] 
//            OFFSET @skip ROWS FETCH NEXT @pageSize ROWS ONLY;";

//            var discountCodes = await connection.QueryAsync<DiscountCodeApiResponse>(
//                query,
//                new { skip, pageSize });

//            return discountCodes.ToArray();
//        }
//    }

//    public class DiscountCodeApiResponse
//    {
//        public int Id { get; set; }
//        public string Code { get; set; } = string.Empty;
//        public decimal Value { get; set; }
//        public string Description { get; set; } = string.Empty;
//        public DateTime StartDate { get; set; }
//        public DateTime? EndDate { get; set; }
//        public byte DiscountType { get; set; }
//        public bool IsActive { get; set; }
//    }
//}

using Dapper;
using System.Data;

namespace KiloMart.Requests.Queries;

public partial class Query
{
    public static async Task<(DiscountCode[] DiscountCodes, int TotalCount)> GetDiscountCodesPaginated(
        IDbConnection connection,
        int page = 1,
        int pageSize = 10)
    {
        int skip = (page - 1) * pageSize;

        // Query to fetch the paginated discount codes
        var dataQuery = @"
        SELECT 
            Id,
            Code,
            Value,
            Description,
            StartDate,
            EndDate,
            DiscountType,
            IsActive
        FROM DiscountCode
        ORDER BY Id
        OFFSET @skip ROWS FETCH NEXT @pageSize ROWS ONLY;";

        // Query to fetch the total count of discount codes
        var countQuery = @"
        SELECT COUNT(*)
        FROM DiscountCode;";

        // Execute both queries
        var discountCodes = await connection.QueryAsync<DiscountCode>(dataQuery, new { skip, pageSize });
        var totalCount = await connection.ExecuteScalarAsync<int>(countQuery);

        return (discountCodes.ToArray(), totalCount);
    }
}
public class DiscountCode
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public decimal Value { get; set; }
    public string Description { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string DiscountType { get; set; } = null!;
    public bool IsActive { get; set; }
}
