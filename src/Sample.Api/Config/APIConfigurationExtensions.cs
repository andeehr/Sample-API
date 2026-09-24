using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi;
using Sample.Api.Helpers;
using System.Reflection;

namespace Sample.Api.Config
{
    public static class APIConfigurationExtensions
    {
        public static IServiceCollection AddAppValidators(this IServiceCollection services)
        {
            var commonAssembly = Assembly.Load("Sample.Common");

            AssemblyScanner.FindValidatorsInAssembly(commonAssembly)
                .ForEach(result => services.AddTransient(result.InterfaceType, result.ValidatorType));

            return services;
        }

        public static IServiceCollection AddOptionsServices(this IServiceCollection services, params (int number, string title)[] versions)
        {
            return services
                .AddOptions()
                .AddSwaggerGen(c =>
                {
                    c.CustomSchemaIds(x => x.FullName);
                    foreach (var (number, title) in versions)
                        c.SwaggerDoc($"v{number}", new OpenApiInfo { Version = $"v{number}", Title = title });
                });
        }

        public static IServiceCollection AddApiAuthentication(this IServiceCollection services, JwtOptions jwtOptions)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = JwtTokenHelper.CreateParameters(jwtOptions.Issuer, jwtOptions.Audience, jwtOptions.JwtKey);
            });

            return services;
        }
    }
}