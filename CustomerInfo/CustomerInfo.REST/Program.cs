using CustomerInfo.REST.Data;
using CustomerInfo.REST.Models;
using CustomerInfo.REST.Services;
using CustomerInfo.REST.Validation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

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

// For demo purposes only
builder.Services.AddDbContext<AppDbContext>(
    options => options.UseInMemoryDatabase("demoDB")
);

var app = builder.Build();

// !!!For demo purposes only!!!, ensure that OnModelCreating is called and thus the DB is seeded
using (var serviceScope = app.Services.CreateScope())
{
    var dbContext = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

// Configure the HTTP request pipeline.
/*if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}*/


// Use Swagger in all environments for now
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { } // Needed for unit tests of controllers