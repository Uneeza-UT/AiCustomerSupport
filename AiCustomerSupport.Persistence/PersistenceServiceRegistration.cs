using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using AiCustomerSupport.Persistence.DatabaseContext;
using AiCustomerSupport.Application.Contracts.Persistence;
using AiCustomerSupport.Persistence.Repositories;

namespace AiCustomerSupport.Persistence
{
    public static class PersistenceServiceRegistration
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });


            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IConversationRepository, ConversationRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IIntentRepository, IntentRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();


            return services;
        } 
    }
}
