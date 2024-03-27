using CustomerInfo.REST.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CustomerInfo.REST.Services.CustomerInfoServices;
using CustomerInfo.REST.Services.ApiKeyServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Customer Information API",
        Description = "An API for managing Customers",
        Version = "v1"
    });
});
builder.Services.AddProblemDetails();

builder.Services.AddScoped<ICustomerInfoService, CustomerInfoService>();
builder.Services.AddScoped<IApiKeyService, ApiKeyService>();

builder.Services.AddDbContext<AppDbContext>(
    options => options.UseInMemoryDatabase("CustomerInfoDB")
);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value))
        };
    });


var app = builder.Build();

// !!!For demo purposes only!!!, ensure that OnModelCreating is called and thus the DB is seeded
using (var serviceScope = app.Services.CreateScope())
{
    var dbContext = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}


// Use Swagger in all environments for now
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { } // Needed for unit tests of controllers