using MessageRouter.Extensions;
using MessageRouter.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MessageRouter
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRabbitMqBroker(this IServiceCollection services, IConfiguration config)
        {
            services
                .AddBroker<RabbitMqBroker>()
                .ConfigureOption<RabbitMqOptions>(config)
                ;
            return services;
        }
    }
}
