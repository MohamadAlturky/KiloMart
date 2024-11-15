using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace KiloMart.Requests.Queries
{
    public partial class Query
    {
        public static async Task<DiscountCodeApiResponse[]> GetAllDiscountCodesPaginated(
            IDbConnection connection,
            int page = 1,
            int pageSize = 10)
        {
            int skip = (page - 1) * pageSize;
            var query = @"
            SELECT 
                [d].[Id],
                [d].[Code],
                [d].[Value],
                [d].[Description],
                [d].[StartDate],
                [d].[EndDate],
                [d].[DiscountType],
                [d].[IsActive]
            FROM DiscountCode [d]
            WHERE [d].[IsActive] = 1  
            ORDER BY [d].[Id] 
            OFFSET @skip ROWS FETCH NEXT @pageSize ROWS ONLY;";

            var discountCodes = await connection.QueryAsync<DiscountCodeApiResponse>(
                query,
                new { skip, pageSize });

            return discountCodes.ToArray();
        }
    }
    
    public class DiscountCodeApiResponse
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public byte DiscountType { get; set; }
        public bool IsActive { get; set; }
    }
}
