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
        public static async Task<List<Location>> GetLocationsByParty(IDbConnection connection, int party)
        {
            var query = "SELECT * FROM [dbo].[Location] WHERE [Party] = @Party";
            var result = await connection.QueryAsync<Location>(query, new { Party = party });
            return result.ToList();
        }
    }
    public class Location
    {
        public int Id { get; set; }
        public float Longitude { get; set; }
        public float Latitude { get; set; }
        public string Name { get; set; } = null!;
        public int Party { get; set; }
        public bool IsActive { get; set; }
    }
}
