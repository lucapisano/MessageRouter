using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MessageRouter.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMessageRouter(this IServiceCollection services, IConfiguration config)
        {
            services
                .AddSingleton<RouterEngine>()
                .ConfigureOption<GeneralOptions>(config)
                ;
            return services;
        }
        public static IServiceCollection AddBroker<TBrokerImplementation>(this IServiceCollection services)
            where TBrokerImplementation : class, IBroker
        {
            //TODO: verificare se impostarlo come singleton o scoped
            services.AddSingleton<TBrokerImplementation>();
            services.AddSingleton<IBroker, TBrokerImplementation>(sp=>sp.GetService<TBrokerImplementation>());
            return services;
        }
        #region Implementations
        //TODO: da estrarre in appositi pacchetti
        
        #endregion
        public static IServiceCollection ConfigureOption<ConfigurationType>(this IServiceCollection services, IConfiguration config) where ConfigurationType : class, new()
        {
            services.Configure<ConfigurationType>(config.GetSection(typeof(ConfigurationType).Name));
            return services;
        }
    }
}
