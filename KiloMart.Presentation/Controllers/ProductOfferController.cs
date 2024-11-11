using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.DataAccess.Database;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Controllers;

[ApiController]
[Route("api/product-offer")]
public class ProductOfferController : ControllerBase
{
    private readonly IDbFactory _dbFactory;
    private readonly IUserContext _userContext;

    public ProductOfferController(IDbFactory dbFactory, IUserContext userContext)
    {
        _dbFactory = dbFactory;
        _userContext = userContext;
    }

    [HttpPost("add")]
    public async Task<ActionResult<int>> AddProductOfferAsync([FromBody] AddProductOfferDto model)
    {
        var providerId = _userContext.Get().Party;
        if (!model.Validate().Success)
        {
            return BadRequest(model.Validate().Errors);
        }

        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        try
        {
            var id = await Db.InsertProductOfferAsync(connection,
                model.Product,
                model.Price,
                model.OffPercentage,
                model.FromDate,
                model.ToDate,
                model.Quantity,
                providerId);

            return CreatedAtAction(nameof(GetProductOfferAsync), new { id }, id);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Errors = new List<string>() { ex.Message } });
        }
    }

    [HttpPut("edit")]
    public async Task<ActionResult> UpdateProductOfferAsync([FromQuery] int id, [FromBody] UpdateProductOfferDto model)
    {
        if (!model.Validate().Success)
        {
            return BadRequest(model.Validate().Errors);
        }

        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        var providerId = _userContext.Get().Party;
        var offer = await Db.GetProductOfferByIdAsync(id, connection);
        if (offer is null)
        {
            return NotFound();
        }
        if (providerId != offer.Provider)
        {
            return Unauthorized("can't edit another provider products");
        }
        try
        {
            var updated = await Db.UpdateProductOfferAsync(connection,
                id,
                model.Product,
                model.Price,
                model.OffPercentage,
                model.FromDate,
                model.ToDate,
                model.Quantity,
                providerId,
                model.IsActive);

            return Ok(new { UpdatedData = updated });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Errors = new List<string>() { ex.Message } });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductOffer>> GetProductOfferAsync(int id)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        var productOffer = await Db.GetProductOfferByIdAsync(id, connection);
        return productOffer is not null ? Ok(productOffer) : NotFound();
    }
}


public class AddProductOfferDto
{
    public int Product { get; set; }
    public decimal Price { get; set; }
    public decimal OffPercentage { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public float Quantity { get; set; }

    public (bool Success, string[] Errors) Validate()
    {
        var errors = new List<string>();
        if (Quantity <= 0)
            errors.Add("Quantity must be greater than zero");
        if (OffPercentage < 0 || OffPercentage > 100)
            errors.Add("Off percentage must be between 0 and 100");
        if (Price <= 0)
            errors.Add("Price must be greater than zero");
        return (errors.Count == 0, errors.ToArray());
    }
}

public class UpdateProductOfferDto : AddProductOfferDto
{
    public bool IsActive { get; set; }
}
