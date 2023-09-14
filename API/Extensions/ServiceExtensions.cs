using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Data.Repositories;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;

namespace API.Extensions
{
    public static class ServiceExtensions
    {
        //Application Services
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {            
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(config.GetConnectionString("DefaultConnection"));
            });

            services.AddScoped<IRecordsRepository, RecordsRepository>();
            services.AddScoped<ISensorsRepository, SensorsRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddAutoMapper(typeof(AutomapperProfiles).Assembly);
            services.AddScoped<ISensorSimulator, SensorSimulator>();

            return services;
        }

        //Identity Services
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
        {
            //services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(config.GetSection("AzureAd"));

            return services;
        }
    }
}