using GoDaddyDDNS.Abstractions;
using GoDaddyDDNS.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GoDaddyDDNS.Extensions
{
    internal static class DependencyInjectionExtensions
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddHttpClient<IPublicIPTracker, PublicIPTracker>();
            services.AddHttpClient<IGoDaddyConnector, GoDaddyConnector>();
            services.AddSingleton<IDynamicDNSService, DynamicDNSService>();
        }
    }
}
