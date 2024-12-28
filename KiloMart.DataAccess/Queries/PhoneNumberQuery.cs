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
            public static async Task<PhoneNumberApiResponseForMine[]> GetPhoneNumbersByParty(
                  IDbConnection connection, 
                  int partyId)
            {
                  var phoneNumbers = await connection.QueryAsync<PhoneNumberApiResponseForMine>(
                        "SELECT [Id], [Value] FROM PhoneNumber WHERE Party = @partyId",
                        new { partyId });
                  return phoneNumbers.ToArray();
            }

            public static async Task<(PhoneNumberApiResponse[] Data, int TotalCount)> GetAllPhoneNumbersPaginated(
                  IDbConnection connection,
                  int page = 1,
                  int pageSize = 10)
            {
                  int skip = (page - 1) * pageSize;

                  var countSql = """
                                 SELECT COUNT(*) FROM PhoneNumber ph 
                                     INNER JOIN Party p ON ph.Party = p.Id 
                                         WHERE p.IsActive = 1;
                                 """;
                  var count = await connection.ExecuteScalarAsync<int>(countSql);

                  var sql = """
                            SELECT 
                                ph.Id AS PhoneNumberId,
                                ph.Value AS NumberValue,
                                p.Id AS PersonId,
                                p.DisplayName AS PersonName
                                    FROM PhoneNumber ph
                                        INNER JOIN Party p ON ph.Party = p.Id
                                            WHERE p.IsActive = 1
                                                ORDER BY ph.Id
                                OFFSET @skip ROWS FETCH NEXT @pageSize ROWS ONLY
                            """;
                  var phoneNumbers = await connection.QueryAsync<PhoneNumberApiResponse>(sql,
                        new { skip, pageSize });

                  return (phoneNumbers.ToArray(), count);
            }
      }


      public class PhoneNumberApiResponse
      {
            public int PhoneNumberId { get; set; }
            public string NumberValue { get; set; } = null!;
            public int PersonId { get; set; }
            public string PersonName { get; set; } = null!;
      }

      public class PhoneNumberApiResponseForMine
      {
            public int Id { get; set; }
            public string Value { get; set; } = null!;
      }

}
