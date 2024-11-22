using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.DataAccess.Database;
using KiloMart.Presentation.Controllers;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/customer/cart")]
public class CartCustomerController(IDbFactory dbFactory, IUserContext userContext)
    : AppController(dbFactory, userContext)
{
    [HttpPost("add")]
    public async Task<IActionResult> AddCartItem([FromBody] CartItemRequest request)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        var cartId = await Db.InsertCartAsync(connection, request.Product, request.Quantity, _userContext.Get().Party);
        return CreatedAtAction(nameof(GetCartById), new { id = cartId }, null);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCartItem(long id, [FromBody] UpdateCartItemRequest request)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        var cart = await Db.GetCartByIdAsync(id, connection);
        if (cart is null)
        {
            return NotFound();

        }
        if (cart.Customer != _userContext.Get().Party)
        {
            return Unauthorized();
        }
        cart.Product = request.Product ?? cart.Product;
        cart.Quantity = request.Quantity ?? cart.Quantity;

        var updated = await Db.UpdateCartAsync(connection, id, cart.Product, cart.Quantity, _userContext.Get().Party);

        if (!updated)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCartItem(long id)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        var cart = await Db.GetCartByIdAsync(id, connection);
        if (cart is null)
        {
            return NotFound();

        }
        if (cart.Customer != _userContext.Get().Party)
        {
            return Unauthorized();
        }
        var deleted = await Db.DeleteCartAsync(connection, id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCartById(long id)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        var cartItem = await Db.GetCartByIdAsync(id, connection);

        if (cartItem is null)
            return NotFound();

        return Ok(cartItem);
    }

    [HttpGet("mine")]
    public async Task<IActionResult> GetCartsByCustomer()
    {
        int customerId = _userContext.Get().Party;
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        var carts = await Db.GetCartsByCustomerAsync(customerId, connection);

        return Ok(carts);
    }
    [HttpGet("mine-with-products-info")]
    public async Task<IActionResult> GetCartsByCustomerWithProductsInfo([FromQuery] byte language)
    {
        int customerId = _userContext.Get().Party;
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        var carts = await Db.GetCartsByCustomerWithProductsInfoAsync(customerId,language, connection);

        return Ok(carts);
    }

    [HttpDelete("mine")]
    public async Task<IActionResult> DeleteMine()
    {
        int customerId = _userContext.Get().Party;
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        var deleted = await Db.DeleteAllCartsByCustomerAsync(connection, customerId);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}

public class CartItemRequest
{
    public int Product { get; set; }
    public float Quantity { get; set; }
}

public class UpdateCartItemRequest
{
    public int? Product { get; set; }
    public float? Quantity { get; set; }
}