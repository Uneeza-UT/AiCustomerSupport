using AiCustomerSupport.Application.Contracts.Identity;
using AiCustomerSupport.Application.Contracts.Persistence;
using AiCustomerSupport.Application.Models.Identity;
using AiCustomerSupport.Identity.DbContext;
using AiCustomerSupport.Identity.Models;
using AiCustomerSupport.Identity.Repositories;
using AiCustomerSupport.Identity.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AiCustomerSupport.Identity
{
    public static class IdentityServiceRegistration
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            services.AddDbContext<ApplicationIdentityDbContext>(options =>
            {
                options
                    .UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
                    .ConfigureWarnings(warning => warning.Ignore(RelationalEventId.PendingModelChangesWarning);
            });


            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
            .AddDefaultTokenProviders();


            services.AddTransient<IAuthService, AuthService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IUserService, UserService>();



            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                        ValidIssuer = configuration["JwtSettings:Issuer"],
                        ValidAudience = configuration["JwtSettings:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                            configuration["JwtSettings:Key"]))
                    };


                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            if (!string.IsNullOrWhiteSpace(accessToken) &&
                                context.HttpContext.Request.Path.StartsWithSegments("/hubs/support-chat"))
                            {
                                context.Token = accessToken;
                            }

                            return Task.CompletedTask;
                        },

                        OnForbidden = async context =>
                        {
                            context.Response.StatusCode = StatusCodes.Status403Forbidden;
                            context.Response.ContentType = "application/json";

                            var endpoint = context.HttpContext.GetEndpoint();
                            var authorizeData = endpoint?.Metadata.GetOrderedMetadata<IAuthorizeData>();

                            var requiredRoles = authorizeData?
                                .Where(x => !string.IsNullOrWhiteSpace(x.Roles))
                                .SelectMany(x => x.Roles!.Split(','))
                                .Select(x => x.Trim())
                                .Distinct()
                                .ToList();

                            var message = requiredRoles?.Any() == true
                                ? $"You must have the {string.Join(" or ", requiredRoles)} role to perform this action."
                                : "You do not have permission to perform this action.";

                            await context.Response.WriteAsJsonAsync(new
                            {
                                StatusCode = 403,
                                message
                            });
                        }
                    };
                });

            return services;
        }
    }
}
