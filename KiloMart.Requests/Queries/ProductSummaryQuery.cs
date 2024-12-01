using System.Data;
using Dapper;

namespace KiloMart.Requests.Queries;

public partial class Query
{
    public static async Task<MonthlyPriceSummary[]> GetMonthlyPriceSummary(IDbConnection connection, int numberOfMonths,int product)
    {
        var sql = @"
                CREATE TABLE #Months (
                    Month INT,
                    Year INT
                );

                INSERT INTO #Months (Month, Year)
                SELECT 
                    MONTH(DATEADD(MONTH, -n, GETDATE())) AS Month,
                    YEAR(DATEADD(MONTH, -n, GETDATE())) AS Year
                FROM 
                    (SELECT TOP (@numberOfMonths) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) - 1 AS n
                     FROM master..spt_values) AS Numbers;

                SELECT 
                    MAX(result.Price) AS MaxValue,
                    MIN(result.Price) AS MinValue,
                    [Month],
                    [Year]
                FROM 
                (
                    SELECT * FROM 
                    (
                        SELECT 
                            [Price],
                            MONTH([FromDate]) AS StartDate,
                            YEAR([FromDate]) AS StartDateYear,
                            MONTH(COALESCE([ToDate], GETDATE())) AS EndDate,
                            YEAR(COALESCE([ToDate], GETDATE())) AS EndDateYear
                        FROM [KiloMartMasterDb].[dbo].[ProductOffer]
                        WHERE Product = @product AND YEAR([FromDate]) = YEAR(GETDATE())
                    ) dates
                    INNER JOIN #Months m ON dates.StartDate <= m.Month AND dates.EndDate >= m.Month AND m.Year = dates.StartDateYear
                ) result 
                GROUP BY [Month], [Year];

                DROP TABLE #Months;";

        var results = await connection.QueryAsync<MonthlyPriceSummary>(sql, new { numberOfMonths,product });
        return results.ToArray();
    }
}

public class MonthlyPriceSummary
{
    public decimal MaxValue { get; set; }
    public decimal MinValue { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
}