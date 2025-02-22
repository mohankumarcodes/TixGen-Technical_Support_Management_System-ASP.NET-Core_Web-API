using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TixGen.Data;
using TixGen.Middlewares;
using TixGen.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Logging for application behaviour 
builder.Logging.ClearProviders();       // Removes default logging providers
builder.Logging.AddConsole();           // Adds Console logging
builder.Logging.AddDebug();             // Adds Debug window logging

// Adding DbContext Services
builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Adding Identity for User Management
builder.Services.AddIdentity<User, IdentityRole>()          // Registers the Identity framework with the application
    .AddEntityFrameworkStores<AppDbContext>()               // Uses Entity Framework Core to store user credentials in the database
    .AddDefaultTokenProviders();                            // Provides built-in token support for password resets, email confirmation, etc

// JWT Authentication Configuration
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]))
        //IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your_secret_key_which_should_be_long_enoughtyherhgrgbershtyr"))
    };
});

// Adding Authorization Services
builder.Services.AddAuthorization();
builder.Services.AddControllers();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

// Ensure Roles Exist at Startup
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roleNames = { "Admin", "Agent" };
    foreach (var role in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

// Enable Swagger in development mode
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseLoggingMiddleware(); // Register Custom Logging Middleware here (After Routing, Before Authentication)

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
