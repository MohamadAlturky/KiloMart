using Microsoft.AspNetCore.Mvc;
using KiloMart.DataAccess.Database;
using System.Data;
using System.Threading.Tasks;
using KiloMart.Core.Contracts;
using KiloMart.Core.Authentication;
using KiloMart.Presentation.Authorization;
using KiloMart.Domain.Register.Utils;

namespace KiloMart.Presentation.Controllers.Shared
{
    [ApiController]
    [Route("api/deal")]
    public class DealController : AppController
    {
        public DealController(IDbFactory dbFactory, IUserContext userContext) : base(dbFactory, userContext) { }

        [HttpPost("admin/create")]
        [Guard([Roles.Admin])]
        public async Task<IActionResult> Create([FromBody] DealDto dto)
        {
            var (success, errors) = dto.Validate();
            if (!success)
            {
                return Fail(errors);
            }

            using var connection = _dbFactory.CreateDbConnection();
            connection.Open();
            bool hasActiveDeal = await Db.HasActiveDealAsync(dto.Product, dto.StartDate, dto.EndDate, connection);
            if (hasActiveDeal)
            {
                return Fail("there is an intersection with the deals in the db try another start date and end date");
            }
            var newId = await Db.InsertDealAsync(connection, dto.Product, dto.IsActive, dto.OffPercentage, dto.StartDate, dto.EndDate);
            return Success(new { id = newId, dto });
        }

        [HttpPut("admin/edit/{id}")]
        [Guard([Roles.Admin])]
        public async Task<IActionResult> Update(int id, [FromBody] DealDto dto)
        {
            var (success, errors) = dto.Validate();
            if (!success)
            {
                return Fail(errors);
            }
            using var connection = _dbFactory.CreateDbConnection();
            connection.Open();
            bool hasActiveDeal = await Db.HasActiveDealAsync(dto.Product, dto.StartDate, dto.EndDate, connection);
            if (hasActiveDeal)
            {
                return Fail("there is an intersection with the deals in the db try another start date and end date");
            }
            var result = await Db.UpdateDealAsync(connection, id, dto.Product, dto.IsActive, dto.OffPercentage, dto.StartDate, dto.EndDate);
            if (result)
            {
                return Success();
            }

            return DataNotFound();
        }

        [HttpDelete("admin/delete/{id}")]
        [Guard([Roles.Admin])]
        public async Task<IActionResult> Delete(int id)
        {
            using var connection = _dbFactory.CreateDbConnection();
            connection.Open();

            var result = await Db.DeleteDealAsync(connection, id);
            if (result)
            {
                return Success();
            }

            return DataNotFound();
        }

        [HttpGet("admin/{id}")]
        [Guard([Roles.Admin])]
        public async Task<IActionResult> GetById(int id)
        {
            using var connection = _dbFactory.CreateDbConnection();
            connection.Open();

            var deal = await Db.GetDealByIdAsync(id, connection);
            if (deal is not null)
            {
                return Success(deal);
            }

            return DataNotFound();
        }

        [HttpPut("admin/deactivate/{id}")]
        [Guard([Roles.Admin])]
        public async Task<IActionResult> Deactivate(int id)
        {
            using var connection = _dbFactory.CreateDbConnection();
            connection.Open();

            var result = await Db.DeActivateDealAsync(connection, id);
            if (result)
            {
                return Success();
            }

            return DataNotFound();
        }

        [HttpPut("admin/activate/{id}")]
        [Guard([Roles.Admin])]
        public async Task<IActionResult> Activate(int id)
        {
            using var connection = _dbFactory.CreateDbConnection();
            connection.Open();
            var deal = await Db.GetDealByIdAsync(id, connection);
            if (deal is null)
            {
                return DataNotFound();
            }
            bool hasActiveDeal = await Db.HasActiveDealAsync(deal.Product, deal.StartDate, deal.EndDate, connection);
            if (hasActiveDeal)
            {
                return Fail("there is an intersection with the deals in the db try another start date and end date");
            }
            
            var result = await Db.ActivateDealAsync(connection, id);
            if (result)
            {
                return Success();
            }

            return DataNotFound();
        }

        [HttpGet("admin/active")]
        [Guard([Roles.Admin])]
        public async Task<IActionResult> GetActive()
        {
            using var connection = _dbFactory.CreateDbConnection();
            connection.Open();

            var activeDeals = await Db.GetActiveDealsAsync(connection);

            if (activeDeals != null)
            {
                return Success(activeDeals);
            }

            return DataNotFound();
        }
        [HttpGet("admin/active-for-product")]
        [Guard([Roles.Admin])]
        public async Task<IActionResult> GetActiveDealsByProductAsync(
            [FromQuery] int product
        )
        {
            using var connection = _dbFactory.CreateDbConnection();
            connection.Open();

            var activeDeals = await Db.GetActiveDealsByProductAsync(connection, product);

            if (activeDeals != null)
            {
                return Success(activeDeals);
            }

            return DataNotFound();
        }

        [HttpGet("admin/all-for-product")]
        [Guard([Roles.Admin])]
        public async Task<IActionResult> GetAllAsync([FromQuery] int product)
        {
            using var connection = _dbFactory.CreateDbConnection();
            connection.Open();

            var allDeals = await Db.GetAllDealsForProductAsync(connection, product);

            if (allDeals != null)
            {
                return Success(allDeals);
            }

            return DataNotFound(); // Return 404 if no deals are found
        }
    }


    public class DealDto
    {
        public int Product { get; set; }
        public bool IsActive { get; set; }
        public float OffPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public (bool Success, string[] Errors) Validate()
        {
            var errors = new List<string>();

            // Validate Product
            if (Product <= 0)
            {
                errors.Add("Product must be a positive integer.");
            }

            // Validate OffPercentage
            if (OffPercentage < 0 || OffPercentage > 100)
            {
                errors.Add("OffPercentage must be between 0 and 100.");
            }

            // Validate EndDate
            if (EndDate <= StartDate)
            {
                errors.Add("EndDate must be after StartDate.");
            }

            return (errors.Count == 0, errors.ToArray());
        }
    }

}
