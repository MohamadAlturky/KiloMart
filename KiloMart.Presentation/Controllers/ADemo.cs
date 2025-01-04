// using KiloMart.Core.Authentication;
// using KiloMart.Core.Contracts;
// using KiloMart.Core.Sql;
// using KiloMart.DataAccess.Database;
// using KiloMart.Domain.Register.Utils;
// using KiloMart.Presentation.Authorization;
// using KiloMart.Presentation.Services;
// using Microsoft.AspNetCore.Mvc;

// namespace KiloMart.Presentation.Controllers;

// [ApiController]
// [Route("api/tests")]
// public class AController(
//     IDbFactory dbFactory,
//     IUserContext userContext)
//     : AppController(dbFactory, userContext)
// {
//     [HttpGet("get")]
//     public async Task<IActionResult> Get(
//     [FromQuery] int? id,
//     [FromQuery] bool? isActive,
//     [FromQuery] string? displayName,
//     [FromQuery] int offset = 0,
//     [FromQuery] int take = 10)
//     {
//         using var connection = _dbFactory.CreateDbConnection();
//         connection.Open();
//         var query = new Query<Party>("SELECT [Id], [DisplayName], [IsActive] FROM [dbo].[Party]")
//                     .OrderBy("[Id]");
        
        
//         // Add where conditions based on provided parameters
//         if (id.HasValue)
//         {
//             query.Where("[Id] = @Id", new { Id = id });
//         }
//         if (isActive.HasValue)
//         {
//             query.Where("[IsActive] = @IsActive", new { IsActive = isActive });
//         }
//         if (!string.IsNullOrEmpty(displayName))
//         {
//             query.Where("[DisplayName] LIKE @DisplayName", new { DisplayName = $"%{displayName}%" });
//         }

//         // Order by DisplayName ascending
//         // query.OrderBy("[DisplayName] ASC");

//         // Apply paging using query parameters
//         query.Page(offset: offset, take: take);

//         var count = await query.CountAsync(connection);
//         var parties = await query.ToListAsync(connection);
//         return Ok(
//             new { 
//                 count, parties,
//                 CountSql = query.BuildCountSql(),
//                 Sql = query.BuildSql(),
//                  });
//     }
// }
// public class Party
// {
//     public int Id { get; set; }
//     public string DisplayName { get; set; }
//     public bool IsActive { get; set; }
// }