using ArmsFW.Infra.Data.Extensions;
using ArmsFW.Services.Logging;
using ArmsFW.Services.Shared.Settings;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System;

namespace ArmsFW.Infra.Data.Contexts
{
    public interface ISqlDbContext: IDisposable 
    {
        DataAccess Conexao { get; }
    }

    public class SqlDbContext : DbContext, IDisposable, ISqlDbContext
    {
        public SqlDbContext() : this(DbConfig.PegarConexaoConfig<SqlDbContext>())
        {
        }
        public SqlDbContext(DbContextOptions options) : base(options)
        {
            try
            {
                var conn = this.Database.GetDbConnection() ?? new SqlConnection();
            }
            catch (Exception ex)
            {
                App.GravarLog("SqlDbContext() - Falha ao configurar o DbContext do EF");
                ex.Logar();
            }
            //LogServices.Debug($"EF.DbContextBase : {options.ContextType} - Conexao inicializada com : SERVER : {conn.DataSource} | DB : {conn.Database} | PROVIDER : ({this.Database.ProviderName})");
        }

        public DataAccess Conexao => AppSettings.Instance.DataAccess;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .AplicarMapeamentoFluent()
                .HasDefaultSchema("dbo");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
            {
                options.EnableSensitiveDataLogging(true);
                options.EnableDetailedErrors(true);
            }
        }
    }
}
