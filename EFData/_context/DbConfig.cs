//using ArmsFW.Services.Logging;
//using ArmsFW.Services.Shared.Settings;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
//using System;

//namespace ArmsFW.Infra.Data.Contexts
//{
//	public static class DbConfig
//	{
//		public static string ConnectionString => AppSettings.ConnectionString;
//        public static string EfContext => App.Config.Get($"DB:Tipo");
//        public static string Schema => App.Config.Get($"DB:Schema");
//        public static string Conexao => App.Config.Get($"DB:conexao");
//        public static DbContextOptions<TContext> PegarConexaoConfig<TContext>() where TContext : DbContext => PegarConexaoConfig<TContext>(ConnectionString);
//        /// <summary>
//        /// Determina as configuração de conexao com o Contexto o EF de acordo com as oções selecionadas no AppSettings.
//        /// Bancos de Suporte : SQL SERVER (padrao), ou MySQL
//        /// </summary>
//        /// <typeparam name="TContext"></typeparam>
//        /// <param name="_ConnectionString"></param>
//        /// <returns></returns>
//        public static DbContextOptions<TContext> PegarConexaoConfig<TContext>(string _ConnectionString) where TContext: DbContext

//        {
//            DbContextOptionsBuilder<TContext> builder = PegarConexaoConfigBuilder<TContext>(_ConnectionString);// new DbContextOptionsBuilder<TContext>();
//			return builder.Options;
//		}

//        public static DbContextOptionsBuilder<TContext> PegarConexaoConfigBuilder<TContext>(string _ConnectionString) where TContext : DbContext

//        {
//            DbContextOptionsBuilder<TContext> builder = new DbContextOptionsBuilder<TContext>();

//            try
//            {
//                if (!string.IsNullOrEmpty(_ConnectionString))
//                {
//                    switch (App.EfContext)
//                    {
//                        case "mysql":
//                            builder.UseMySql(_ConnectionString, ServerVersion.AutoDetect(_ConnectionString), b => b.MigrationsAssembly("cetec-core")).EnableSensitiveDataLogging();
//                            break;
//                        case "mongodb": // MONGO DB (a ser implementado)

//                            break;
//                        default: //SQL SERVER
//                            builder.UseSqlServer(_ConnectionString, b => b.MigrationsAssembly("cetec-web")).EnableSensitiveDataLogging();
//                            break;
//                    };
//                }
//                else
//                {
//                    App.GravarLog("ArmsFW.Infra.Data.Contexts.DbConfig.SqlDbOptions() - Falha ao carregar a string de Conexão. A string esta vazia");
//                }
//            }
//            catch (System.Exception ex)
//            {
//                ex.Logar();
//            }
//            return builder;
//        }
//        /// <summary>
//        /// Constroi um DbContextOptionsBuilder especifico para AddDbContext do Identity
//        /// </summary>
//        /// <param name="connectionString"></param>
//        /// <returns></returns>
//        public static Action<DbContextOptionsBuilder> MontarBuilderIdentity(string connectionString)
//        {
//            return builder =>
//            {

//                if (!string.IsNullOrEmpty(connectionString))
//                {
//                    switch (App.EfContext)
//                    {
//                        case "mysql":
//                            builder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), b => b.MigrationsAssembly("cetec-core")).EnableSensitiveDataLogging();
//                            break;
//                        case "mongodb": // MONGO DB (a ser implementado)

//                            break;
//                        default: //SQL SERVER
//                            builder.UseSqlServer(connectionString, b => b.MigrationsAssembly("cetec-web")).EnableSensitiveDataLogging();
//                            break;
//                    };
//                }
//            };
//        }
//    }
//}
