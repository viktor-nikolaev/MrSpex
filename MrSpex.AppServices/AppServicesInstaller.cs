using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace MrSpex.AppServices
{
    public static class AppServicesInstaller
    {
        public static void AddAppServices(this IServiceCollection services)
        {
            services.AddMediatR(typeof(SetStock));
        }
    }
}