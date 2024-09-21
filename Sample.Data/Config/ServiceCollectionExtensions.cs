using Microsoft.Extensions.DependencyInjection;
using Sample.Data.Persistence;
using Sample.Data.Persistence.Interfaces;

namespace Sample.Data.Config
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAppRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();

            return services;
        }
    }
}