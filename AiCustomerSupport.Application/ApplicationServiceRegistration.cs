using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace AiCustomerSupport.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg =>
            {
                cfg.AddMaps(Assembly.GetExecutingAssembly());
            });

            return services;
        }
    }
}
