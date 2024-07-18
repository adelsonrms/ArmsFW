using ArmsFW.Security;
using ArmsFW.Services.Azure;
using ArmsFW.Services.Email;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AplicacaoServiceExtensions
    {
        public static IServiceCollection AddEstruturaCompartilhado(this IServiceCollection services)
        {
            //services.AddTransient<IAnexoService, AnexoService>();
            //services.AddTransient<IAnexoRepository, AnexoRepository>();

            //services.AddTransient<IDominioService, DominioService>();
            //services.AddTransient<IDominioRepository, DominioRepository>();


            return services;
        }
    }


    public static class IntegracaoServiceExtensions
    {
        public static IServiceCollection AddIntegracaoAzureAd(this IServiceCollection services)
        {
            //services.AddTransient<AzureAdUserService, AzureAdUserService>();
            services.AddTransient<Office365UserService, Office365UserService>();

            return services;
        }

        
        public static IServiceCollection AddIntegracaoEmail(this IServiceCollection services, Configuration.IConfiguration configuration)
        {
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            services.Configure<NotificacaoOptions>(configuration.GetSection("NotificacaoOptions"));

            //services.AddScoped<INotificacaoService, NotificacaoService>();
            services.AddScoped<IEmailSender, EmailService>();
            services.AddScoped<AzureEmailService, AzureEmailService>();


            return services;
        }

        public static IServiceCollection AddIntegracaoJWT(this IServiceCollection services, Configuration.IConfiguration configuration)
        {
            services.Configure<JwtTokenSettings>(configuration.GetSection("JwtTokenSettings"));
            services.AddScoped<IJwtService, JwtService>();

            return services;
        }
    }
}
