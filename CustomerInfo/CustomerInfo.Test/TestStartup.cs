using CustomerInfo.REST.Data;
using CustomerInfo.REST.Services.ApiKeyServices;
using CustomerInfo.REST.Services.CustomerInfoServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CustomerInfo.Test
{
    public class TestStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Add DbContext to the test services
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDatabase");
            }, ServiceLifetime.Scoped);


            // Add other services if needed
            services.AddControllers();
            //services.AddScoped<ICustomerInfoService, CustomerInfoService>();
            //services.AddScoped<IApiKeyService, ApiKeyService>();

            /*services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes("DhwDdkAPeY907xizwBRcJqpRKM02zOF6z8uf6hDfmQTfatqso0lsBR1nMYNelEIWI5QzQwyzlyHzCz7NYaMxPP574fSVSMInAnkThhoLGeGtnxcgiEcD22Sqmzo4qIb5")
                            )
                        };
                }); */

        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //app.UseAuthentication();
            //app.UseAuthorization();

        }
    }
}
