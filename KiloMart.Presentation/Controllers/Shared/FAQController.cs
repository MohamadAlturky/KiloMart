using Microsoft.AspNetCore.Mvc;
using KiloMart.DataAccess.Database;
using KiloMart.Core.Contracts;
using KiloMart.Core.Authentication;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Authorization;

namespace KiloMart.Presentation.Controllers.Shared;

[ApiController]
[Route("api/faq")]
public class FAQController : AppController
{
    public FAQController(IDbFactory dbFactory, IUserContext userContext)
        : base(dbFactory, userContext) { }

    // [HttpGet("{id}")]
    // public async Task<IActionResult> GetById(int id)
    // {
    //     using var connection = _dbFactory.CreateDbConnection();
    //     connection.Open();

    //     var faq = await Db.GetFAQByIdAsync(id, connection);
    //     if (faq is null)
    //     {
    //         return DataNotFound();
    //     }

    //     return Success(faq);
    // }

    [HttpGet("all/list")]
    public async Task<IActionResult> List(
        [FromQuery] byte type,
        [FromQuery] byte language)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var faqs = await Db.GetAllFAQsByTypeAsync(connection, type, language);

        return Success(new
        {
            faqs = faqs,
            TotalCount = faqs.Count()
        });
    }

    [HttpPost("admin/create")]
    [Guard([Roles.Admin])]
    public async Task<IActionResult> Create([FromBody] FAQDto faqDto)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var newId = await Db.InsertFAQAsync(connection, faqDto.Question, faqDto.Answer, faqDto.Language, faqDto.Type);

        return Success(new { Created = faqDto });
    }

    [HttpPut("admin/edit/{id}")]
    [Guard([Roles.Admin])]
    public async Task<IActionResult> Update(int id, [FromBody] FAQDto faqDto)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var result = await Db.UpdateFAQAsync(connection, id, faqDto.Question, faqDto.Answer, faqDto.Language, faqDto.Type);

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

        var result = await Db.DeleteFAQAsync(connection, id);

        if (result)
        {
            return Success();
        }

        return DataNotFound();
    }
}

public class FAQDto
{
    public string Question { get; set; } = null!;
    public string Answer { get; set; } = null!;
    public byte Language { get; set; }
    public byte Type { get; set; }
}