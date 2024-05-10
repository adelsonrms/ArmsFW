using ArmsFW.Services.Email;
using GSK.ArtWork.Web.Auth.CustomProvider;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace ArmsFW.Infra.Identity
{
    /// <summary>
    /// Contem os metodos para instalação e configuração do identity na aplicação
    /// </summary>
    public static class JsonIdentitySetup
    {
        /// <summary>
        /// Registra os serviços do Identity customizado para aplicação
        /// </summary>
        /// <param name="services">Instancia do ServiceContainer para injeção de dependencia</param>
        /// 
        public static IServiceCollection AddIdentityJsonStore(this IServiceCollection services)
        {
            // Classes de gerenciamento do Identity
            services
                .AddIdentity<JsonIdentityUser, JsonIdentityRole>()
                .AddUserStore<JsonUserStore>()
                .AddRoleStore<JsonRoleStore>()
                .AddUserManager<JsonUserManager<JsonIdentityUser>>()
                .AddDefaultTokenProviders();

            services
                .ConfigureApplicationCookie(options => options.LoginPath = $"/sessao/login");

            services.Configure<IdentityOptions>(config =>
            {
                config.User.RequireUniqueEmail = true;

                config.SignIn.RequireConfirmedEmail = false;
                config.SignIn.RequireConfirmedPhoneNumber = false;

                //Politica de Senha
                config.Password.RequireDigit = true;
                config.Password.RequiredLength = 8;
                config.Password.RequireLowercase = false;
                config.Password.RequireUppercase = false;
                config.Password.RequireNonAlphanumeric = false;
            });

            //Registro dos serviços utilizados no DI
            services.AddTransient<JsonIdentityDbContext>();
            //services.AddTransient<IAccountManager<JsonIdentityUser>, AccountManager<JsonIdentityUser>>();
            //services.AddTransient<ILoginService, LoginService>();
            
            services.AddTransient<IEmailSender, EmailService>();
            services.AddTransient<ISmsSender, EmailService>();

            return services;
        }
    }
}
