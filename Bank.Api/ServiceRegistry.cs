using Bank.Api.Contracts;
using Bank.Api.Data;
using Bank.Api.Helpers;
using Bank.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bank.Api
{
    public static class ServiceRegistry
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApppDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("sqlConnection")));


            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<CodeGeneratorHelper>();
            services.AddHttpContextAccessor();



            return services;
        }
    }
}
