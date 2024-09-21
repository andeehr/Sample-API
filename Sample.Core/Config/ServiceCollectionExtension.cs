using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Sample.Core.Mappers;
using Sample.Core.Services;
using Sample.Core.Services.Interfaces;

namespace Sample.Core.Config
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddAppMappers(this IServiceCollection services)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AllowNullCollections = true;

                cfg.AddProfile<UserProfile>();
            });

            var mapper = config.CreateMapper();
            services.AddSingleton(mapper);

            return services;
        }

        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();

            return services;
        }
    }
}