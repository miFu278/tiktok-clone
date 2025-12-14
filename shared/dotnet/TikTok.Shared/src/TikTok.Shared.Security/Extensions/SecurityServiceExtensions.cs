using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TikTok.Shared.Security.Interfaces;
using TikTok.Shared.Security.Models;
using TikTok.Shared.Security.Services;

namespace TikTok.Shared.Security.Extensions
{
    public static class SecurityServiceExtensions
    {
        public static IServiceCollection AddSharedSecurity(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Configure options
            services.Configure<TokenOptions>(configuration.GetSection("Jwt"));
            services.Configure<EncryptionOptions>(configuration.GetSection("Encryption"));

            // Register services
            services.AddSingleton<ITokenService, JwtTokenService>();
            services.AddSingleton<IEncryptionService, AesEncryptionService>();
            services.AddSingleton<IPasswordHasher, Argon2PasswordHasher>();

            return services;
        }

        public static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var tokenOptions = configuration.GetSection("Jwt").Get<TokenOptions>();

            if (tokenOptions == null || string.IsNullOrWhiteSpace(tokenOptions.SecretKey))
                throw new InvalidOperationException("JWT configuration is missing or invalid.");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = tokenOptions.Issuer,
                    ValidAudience = tokenOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(tokenOptions.SecretKey)),
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers["Token-Expired"] = "true";
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            return services;
        }
    }
}
