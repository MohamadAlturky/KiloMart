using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.DataAccess.Database;
using KiloMart.Presentation.Commands;
using Microsoft.AspNetCore.Mvc;


namespace KiloMart.Presentation.Controllers;

[ApiController]
public class CardController : AppController
{
    public CardController(IDbFactory dbFactory, IUserContext userContext)
        : base(dbFactory, userContext)
    {
    }

    // Models
    public class CardModel
    {
        public int? Id { get; set; }
        public string HolderName { get; set; } = null!;
        public string Number { get; set; } = null!;
        public string SecurityCode { get; set; } = null!;
        public DateOnly ExpireDate { get; set; }
        public int Customer { get; set; }

        public (bool Success, string[] Errors) Validate()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(HolderName))
                errors.Add("Holder name is required");

            if (string.IsNullOrWhiteSpace(Number))
                errors.Add("Card number is required");

            if (string.IsNullOrWhiteSpace(SecurityCode))
                errors.Add("Security code is required");

            if (ExpireDate == default)
                errors.Add("Expire date is required");

            if (Customer <= 0)
                errors.Add("Customer ID is required");

            return (errors.Count == 0, errors.ToArray());
        }
    }

    // Actions
    [HttpPost("card")]
    public async Task<ActionResult<CardModel>> CreateCard(CardModel model)
    {
        var (isValid, errors) = model.Validate();
        if (!isValid)
            return BadRequest(new { Errors = errors });

        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var newId = await Db.InsertCardAsync(connection, model.HolderName, model.Number, model.SecurityCode, model.ExpireDate, model.Customer);

        var card = new CardModel
        {
            Id = newId,
            HolderName = model.HolderName,
            Number = model.Number,
            SecurityCode = model.SecurityCode,
            ExpireDate = model.ExpireDate,
            Customer = model.Customer
        };

        return Ok(card);
    }

    [HttpPut("card/{id}")]
    public async Task<ActionResult<bool>> UpdateCard(int id, CardModel model)
    {
        var (isValid, errors) = model.Validate();
        if (!isValid)
            return BadRequest(new { Errors = errors });

        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        return await Db.UpdateCardAsync(connection, id, model.HolderName, model.Number, model.SecurityCode, model.ExpireDate, model.Customer, true);
    }

    [HttpDelete("card/{id}")]
    public async Task<ActionResult<bool>> DeleteCard(int id)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        return await Db.DeleteCardAsync(connection, id);
    }

    [HttpGet("card/{id}")]
    public async Task<IActionResult> GetCard(int id)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        var result = await Db.GetCardByIdAsync(id, connection);
        return Ok(result);
    }
}
