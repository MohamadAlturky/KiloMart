using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;

namespace KiloMart.Requests.Queries;
public static partial class Query
{

      public static async Task<List<Card>> CardById(IDbFactory factory, UserPayLoad payLoad)
      {
            using var connection = factory.CreateDbConnection();
            connection.Open();
            const string query = @"SELECT * FROM dbo.Card where Id=@Id";

            var result = await connection.QueryAsync<Card>(query, new
            {
                  Id = 5
            });
            return result.ToList();

      }

      public class Card
      {

      }
}

