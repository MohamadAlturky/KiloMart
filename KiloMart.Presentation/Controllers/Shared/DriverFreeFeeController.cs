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
    [Route("api/driverfreefee")]
    public class DriverFreeFeeController(IDbFactory dbFactory, IUserContext userContext) : AppController(dbFactory, userContext)
    {
        [HttpPost("admin/create")]
        [Guard([Roles.Admin])]
        public async Task<IActionResult> Create([FromBody] DriverFreeFeeDto dto)
        {
            using var connection = _dbFactory.CreateDbConnection();
            connection.Open();

            var newId = await Db.InsertDriverFreeFeeAsync(connection, dto.StartDate, dto.EndDate, dto.IsActive);
            return Success(new { id = newId, dto });
        }

        [HttpPut("admin/edit/{id}")]
        [Guard([Roles.Admin])]
        public async Task<IActionResult> Update(int id, [FromBody] DriverFreeFeeDto dto)
        {
            using var connection = _dbFactory.CreateDbConnection();
            connection.Open();

            var result = await Db.UpdateDriverFreeFeeAsync(connection, id, dto.StartDate, dto.EndDate, dto.IsActive);
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

            var result = await Db.DeleteDriverFreeFeeAsync(connection, id);
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

            var fee = await Db.GetDriverFreeFeeByIdAsync(id, connection);
            if (fee != null)
            {
                return Success(fee);
            }

            return DataNotFound();
        }

        [HttpPut("admin/deactivate/{id}")]
        [Guard([Roles.Admin])]
        public async Task<IActionResult> Deactivate(int id)
        {
            using var connection = _dbFactory.CreateDbConnection();
            connection.Open();

            var result = await Db.DeActivateDriverFreeFeeAsync(connection, id);
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

            var activeFees = await Db.GetActiveDriverFreeFeesAsync(connection);

            if (activeFees != null)
            {
                return Success(activeFees);
            }

            return DataNotFound(); // Return 404 if no active fees are found
        }
        [HttpGet("admin/all")]
        [Guard([Roles.Admin])]
        public async Task<IActionResult> GetAllAsync()
        {
            using var connection = _dbFactory.CreateDbConnection();
            connection.Open();

            var activeFees = await Db.GetAllAsync(connection);

            if (activeFees != null)
            {
                return Success(activeFees);
            }

            return DataNotFound(); // Return 404 if no active fees are found
        }
    }

    public class DriverFreeFeeDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}
