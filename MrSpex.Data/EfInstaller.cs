using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MrSpex.AppServices.ReadModel;
using MrSpex.Domain;
using MrSpex.SharedKernel;

namespace MrSpex.Data
{
    public static class EfInstaller
    {
        public static void AddEfDbAdapters(this IServiceCollection services, string cnnString)
        {
            services.AddDbContext<AvailabilityDbContext>(opts => opts.UseNpgsql(cnnString));

            services.AddScoped<IUnitOfWork, EfUnitOfWork>();
            services.AddScoped<IStockRepository, StockRepository>();
            services.AddScoped<ISalesChannelRepository, SalesChannelRepository>();
            services.AddScoped<IReadRepository, EfReadRepository>();
        }
    }
}