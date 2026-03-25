using Sample.Api.Helpers;
using Sample.Common.DTOs;
using Sample.Common.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using Microsoft.OpenApi;

namespace Sample.Api.Config
{
    public static class APIConfigurationExtensions
    {
        public static IApplicationBuilder UseAPIExceptionHandler(this IApplicationBuilder app)
        {
            return app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var handler = context.Features.Get<IExceptionHandlerPathFeature>();
                    var response = GetErrorResponse(handler);
                    var content = ErrorToString(response);
                    context.Response.StatusCode = response.StatusCode;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(content);
                });
            });
        }

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

        private static ErrorResponse GetErrorResponse(IExceptionHandlerPathFeature handler)
        {
            var response = new ErrorResponse
            {
                StatusCode = handler?.Error switch
                {
                    DomainException => StatusCodes.Status400BadRequest,
                    NotFoundException => StatusCodes.Status404NotFound,
                    UnauthorizedException => StatusCodes.Status401Unauthorized,
                    ForbiddenException => StatusCodes.Status403Forbidden,
                    DuplicateKeyException => StatusCodes.Status409Conflict,
                    _ => StatusCodes.Status500InternalServerError
                },
                Message = handler?.Error?.Message ?? "An error occurred in the application"
            };

            return response;
        }

        private static string ErrorToString(ErrorResponse response)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
            };

            return JsonConvert.SerializeObject(response, Formatting.None, settings);
        }
    }
}