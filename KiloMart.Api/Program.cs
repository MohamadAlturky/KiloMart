using KiloMart.Api.Authentication;
using KiloMart.DataAccess.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDataAccess(builder.Configuration);

//#region Authentication
//builder.Services.AddAuthentication()
//    .AddBearerToken(IdentityConstants.BearerScheme);
//builder.Services.AddAuthorizationBuilder();
//#endregion

#region Authentication
builder.Services.AddAuthentication().AddBearerToken(IdentityConstants.BearerScheme, options =>
{
    options.BearerTokenExpiration = TimeSpan.FromMinutes(3600);
});
builder.Services.AddAuthorization();
#endregion

#region DbContext
builder.Services.AddDbContext<MemberShipDataContext>(config =>
{
    config.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")!);
});
#endregion

#region Identity
builder.Services.AddIdentityCore<MemberShipUser>(options =>
{
    options.SignIn.RequireConfirmedEmail = true;
})
.AddEntityFrameworkStores<MemberShipDataContext>()
.AddApiEndpoints();
#endregion

var app = builder.Build();

app.MapIdentityApi<MemberShipUser>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

