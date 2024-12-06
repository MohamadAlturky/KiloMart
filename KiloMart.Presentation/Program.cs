// using Microsoft.AspNetCore.Authentication.JwtBearer;
// using Microsoft.IdentityModel.Tokens;
// using System.Text;
// using Microsoft.OpenApi.Models;
// using KiloMart.Core.Configurations;
// using KiloMart.Presentation.Authorization;
// using KiloMart.Presentation.RealTime;
// using KiloMart.Core.Authentication;
// using KiloMart.DataAccess.EFCore.Configuration;
// using KiloMart.Core.Repositories;
// using KiloMart.Core.Settings;
// using KiloMart.DataAccess.EFCore.Repositories;


// var builder = WebApplication.CreateBuilder(args);

// builder.Services.AddHttpContextAccessor();
// builder.Services.AddScoped<IUserContext, UserContext>();
// builder.Services.AddSingleton<IAppSettingsRepository, AppSettingsRepository>();
// builder.Services.AddSingleton<IAppSettingsProvider, AppSettingsProvider>();
// builder.Services.AddHostedService<FFF.NotificationService>();
// #region Authentication
// // Add JWT Authentication configuration
// builder.Services.AddAuthentication(options =>
// {
//     options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//     options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
// }).AddJwtBearer(options =>
// {
//     options.TokenValidationParameters = new TokenValidationParameters
//     {
//         ValidateIssuer = true,
//         ValidateAudience = true,
//         ValidateLifetime = true,
//         ValidateIssuerSigningKey = true,
//         ValidIssuer = builder.Configuration["Jwt:Issuer"],
//         ValidAudience = builder.Configuration["Jwt:Audience"],
//         IssuerSigningKey = new SymmetricSecurityKey(
//             Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
//     };
// });
// #endregion
// builder.Services.AddControllers();
// builder.Services.AddSignalR();
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen(options =>
// {
//     options.SwaggerDoc("v1", new OpenApiInfo { Title = "KiloMart API", Version = "v1" });
    
//     options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//     {
//         Description = @"JWT Authorization header using the Bearer scheme. 
//                       Enter 'Bearer' [space] and your token in the text input below.
//                       Example: 'Bearer 12345abcdef'",
//         Name = "Authorization",
//         In = ParameterLocation.Header,
//         Type = SecuritySchemeType.Http,
//         Scheme = "Bearer",
//         BearerFormat = "JWT"
//     });

//     options.AddSecurityRequirement(new OpenApiSecurityRequirement
//     {
//         {
//             new OpenApiSecurityScheme
//             {
//                 Reference = new OpenApiReference
//                 {
//                     Type = ReferenceType.SecurityScheme,
//                     Id = "Bearer"
//                 }
//             },
//             new string[] {}
//         }
//     });
// });

// builder.Services.AddDataAccess(builder.Configuration);
// builder.Services.AddEFDataAccess(builder.Configuration.GetConnectionString("DefaultConnection")!);
// // builder.Services.AddSignalR();

// // configuration
// GuardAttribute.SECRET_KEY = builder.Configuration["Jwt:Key"]!;
// GuardAttribute.ISSUER = builder.Configuration["Jwt:Issuer"]!;
// GuardAttribute.AUDIENCE = builder.Configuration["Jwt:Audience"]!;

// // builder.Services.AddCors(options =>
// // {
// //     options.AddDefaultPolicy(policy =>
// //     {
// //         policy
// //             .AllowAnyOrigin() 
// //             .AllowAnyHeader()
// //             .AllowAnyMethod();
// //     });
// // });
// // builder.Services.AddCors(options =>
// // {
// //     options.AddDefaultPolicy(policy =>
// //     {
// //         policy
// //             .WithOrigins("http://localhost:3001") // Allow requests from localhost:3001
// //             .AllowAnyHeader()
// //             .AllowAnyMethod();
// //     });
// // });

// var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
// // Configure CORS policy
// builder.Services.AddCors(options =>
// {
//     options.AddPolicy(name: MyAllowSpecificOrigins,
//     policy =>
//     {
//         policy.WithOrigins("http://localhost:3001") // Allow requests from this origin
//               .AllowAnyHeader()                    // Allow any headers
//               .AllowAnyMethod() 
//               .AllowCredentials();
//     });
// });

// var app = builder.Build();

// app.UseSwagger();
// app.UseSwaggerUI();

// app.UseStaticFiles();
// app.UseHttpsRedirection();
// app.UseCors(MyAllowSpecificOrigins); // Use the defined CORS policy
// app.UseAuthentication();
// app.UseAuthorization();

// app.MapControllers();

// // app.MapHub<NotificationHub>("/notificationHub");
// app.MapHub<FFF.NotificationHub>("/notificationHub");

// app.Run();


using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using KiloMart.Core.Configurations;
using KiloMart.Presentation.Authorization;
using KiloMart.Presentation.RealTime;
using KiloMart.Core.Authentication;
using KiloMart.DataAccess.EFCore.Configuration;
using KiloMart.Core.Repositories;
using KiloMart.Core.Settings;
using KiloMart.DataAccess.EFCore.Repositories;
using KiloMart.Presentation.Middlewares;
using KiloMart.Presentation.Tracking;

var builder = WebApplication.CreateBuilder(args);

// Dependency Injection
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContext, UserContext>();
builder.Services.AddSingleton<IAppSettingsRepository, AppSettingsRepository>();
builder.Services.AddSingleton<IAppSettingsProvider, AppSettingsProvider>();
builder.Services.AddSingleton<DriversTrackerService>();
// builder.Services.AddHostedService<FFF.NotificationService>();

// JWT Configuration
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience))
{
    throw new InvalidOperationException("JWT configuration is missing or invalid.");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

// Controllers, SignalR, and Swagger
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "KiloMart API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter 'Bearer' followed by a space and your token.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

// CORS Policy
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins, policy =>
    {
        policy.WithOrigins("http://localhost:3001")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Add Data Access
builder.Services.AddDataAccess(builder.Configuration);
builder.Services.AddEFDataAccess(builder.Configuration.GetConnectionString("DefaultConnection")!);

// Set JWT values for GuardAttribute
GuardAttribute.SECRET_KEY = jwtKey;
GuardAttribute.ISSUER = jwtIssuer;
GuardAttribute.AUDIENCE = jwtAudience;

builder.Services.AddTransient<ExceptionHandlingMiddleware>();

var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseCors(MyAllowSpecificOrigins);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<NotificationHub>("/notificationHub");

app.Run();
