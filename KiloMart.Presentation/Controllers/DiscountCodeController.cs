using Dapper;
using KiloMart.Commands.Services;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Controllers
{
    [ApiController]
    [Route("api/discountcode")]
    public class DiscountCodeController : AppController
    {
        public DiscountCodeController(IDbFactory dbFactory, IUserContext userContext)
            : base(dbFactory, userContext)
        {
        }

        [HttpPost]
        public async Task<IActionResult> Insert(DiscountCodeInsertModel model)
        {
            var result = await DiscountCodeService.Insert(_dbFactory, _userContext.Get(), model);

            if (result.Success)
            {
                return CreatedAtAction(nameof(Insert), new { id = result.Data.Id }, result.Data);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, DiscountCodeUpdateModel model)
        {
            model.Id = id;

            var result = await DiscountCodeService.Update(_dbFactory, _userContext.Get(), model);

            if (result.Success)
            {
                return Ok(result.Data);
            }
            else
            {
                if (result.Errors.Contains("Not Found"))
                {
                    return NotFound();
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }
        }


        [HttpGet("list")]
        // [Guard([Roles.Admin])]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            using var connection = _dbFactory.CreateDbConnection();
            connection.Open();
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

            return Ok(discountCodes.ToArray());
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
