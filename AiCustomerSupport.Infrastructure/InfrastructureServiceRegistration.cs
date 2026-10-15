using AiCustomerSupport.Application.Contracts.Email;
using AiCustomerSupport.Application.Contracts.Services;
using AiCustomerSupport.Application.Models.AISettings;
using AiCustomerSupport.Application.Models.Email;
using AiCustomerSupport.Infrastructure.EmailService;
using AiCustomerSupport.Infrastructure.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Resend;

namespace AiCustomerSupport.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

            services.AddHttpClient<ResendClient>();
            services.Configure<ResendClientOptions>(options =>
            {
                options.ApiToken = configuration.GetValue<string>("EmailSettings:ApiKey");
            });


            services.Configure<GeminiSettings>(configuration.GetSection("GeminiSettings"));

            services.AddTransient<IResend, ResendClient>();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddScoped<IConversationService, ConversationService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IIntentService, IntentService>();     
            services.AddScoped<INotificationService, NotificationService>();     
            services.AddScoped<IFileStorageService, FileStorageService>();
            services.AddHttpClient<IAIService, AIService>();
            services.AddScoped<IChatHubService, ChatHubService>();
            services.AddSingleton<IUserIdProvider, SignalRUserIdProvider>();

            return services;
        }
    }
}
