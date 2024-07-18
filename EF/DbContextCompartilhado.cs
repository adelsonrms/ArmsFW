using ArmsFW.Infra.Data.Contexts;
using ArmsFW.Infra.Data.Extensions;
using ArmsFW.Services.Shared.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System;

namespace Core.Data
{
    /// <summary>
    /// Contexto somente para as classes de uso compartilhado
    /// </summary>
    public class DbContextCompartilhado : SqlDbContext, IDisposable
    {
        #region DbSets de Negocio

        #endregion

        #region DbSets de Infra (Cmpartilhado)
        #endregion

        #region Estrutura do DbContext da Aplicação
        public static DbContextCompartilhado GetDbContext() => Create();
        public static DbContextCompartilhado Create() => new DbContextCompartilhado(DbConfig.PegarConexaoConfig<DbContextCompartilhado>(DbConfig.ConnectionString));
        public DbContextCompartilhado(DbContextOptions<DbContextCompartilhado> options)
            : base(options)
        {
        }
        public DbContextCompartilhado()
            : base(DbConfig.PegarConexaoConfig<DbContextCompartilhado>(DbConfig.ConnectionString))
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .AplicarMapeamentoFluent()
                .HasDefaultSchema(App.Config.Get("EFConfig:schema"));
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
            {
                options.EnableSensitiveDataLogging(true);
                options.EnableDetailedErrors(true);

                options.ConfigureWarnings(
                b => b.Log(
                        (RelationalEventId.ConnectionOpened, LogLevel.Information),
                        (RelationalEventId.ConnectionClosed, LogLevel.Information))
                    );

                options.UseLoggerFactory(
                    LoggerFactory.Create(log =>
                    {
                        log.AddConsole(optLog =>
                        {
                            optLog.FormatterName = "EFLog";
                        });
                    })
                    );
            }
        }
        #endregion

    }
}