//using ArmsFW.Infra.Data.Contexts;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using System;

///// <summary>
///// Funcionalidades do Identity
///// </summary>
//namespace ArmsFW.Infra.Identity.Remote
//{
//    /// <summary>
//    /// Autor       : Adelson RM Silva
//    /// Objetivo    : Contem as funções de configurações do Identity em uma aplicação
//    /// </summary>
//    public static class IdentityConfigExtensions
//    {
//        /// <summary>
//        /// Objetivo    : Instala o ServiceCollection (DI) os serviços do identity usando a string padrão do appsettings.json
//        /// </summary>
//        /// <param name="services"></param>
//        /// <returns></returns>
//        public static IServiceCollection AddIdentityArmsFW(this IServiceCollection services) => AddIdentityArmsFW(services, null);
//        /// <summary>
//        /// Autor       : Adelson RM Silva
//        /// Objetivo    : Instala o ServiceCollection (DI) os serviços do identity informando uma string de conexao
//        /// </summary>
//        /// <param name="services"></param>
//        /// <param name="connectionString">String de conexao onde estará o DB</param>
//        /// <returns></returns>
//        public static IServiceCollection AddIdentityArmsFW(this IServiceCollection services, string connectionString)
//        {
//            //Registra o DbContext do EF que vai armazenar as informações do Identity
//            services.AddDbContext<IdentityContext>(options => DbConfig.PegarConexaoConfig<IdentityContext>(connectionString));

//            //Registra os objetos de armazenamento do Identity
//            services.AddIdentity<Usuario, IdentityRole>(options => {

//                options.Password.RequireDigit = true;
//                options.Password.RequiredLength = 8;
//                options.Password.RequireLowercase = false;
//                options.Password.RequireUppercase = false;
//                options.Password.RequireNonAlphanumeric = false;

//                options.SignIn.RequireConfirmedEmail = false;
//                options.SignIn.RequireConfirmedPhoneNumber = false;

//                options.User.RequireUniqueEmail = true;
//            })
//            .AddEntityFrameworkStores<IdentityContext>()
//            .AddRoles<IdentityRole>()
//            .AddSignInManager<SignInManager<Usuario>>()
//            .AddUserManager<UserManager<Usuario>>()
//            .AddRoleManager<RoleManager<IdentityRole>>()
//            .AddDefaultTokenProviders();

//            services.AddTransient<IIdentityService, IdentityService>();

//            return services;
//        }

//        public static void AddFormsAutentication(this IServiceCollection services, IConfiguration configuration)
//        {
//            services.AddAuthentication(o =>
//            {
//                o.DefaultScheme = IdentityConstants.ApplicationScheme;
//                o.DefaultSignInScheme = IdentityConstants.ExternalScheme;
//            }).AddCookie("Identity.Application")
//                .AddCookie("Identity.External")
//                .AddCookie("Identity.TwoFactorUserId");

//            services.ConfigureApplicationCookie(Cookies =>
//            {
//                Cookies.Cookie.HttpOnly = true;
//                Cookies.Cookie.Expiration = TimeSpan.FromMinutes(5.0);
//                Cookies.LoginPath = "/Account/Login";
//                Cookies.LogoutPath = "/Account/Logout";
//                Cookies.AccessDeniedPath = "/Account/AcessoNegado";
//                Cookies.SlidingExpiration = true;
//            });
//        }
//    }
//}