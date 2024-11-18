using ArmsFW.Data.Extensions;
using ArmsFW.Services.Logging;
using ArmsFW.Services.Shared.Settings;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System;
using System.Data.Common;

namespace ArmsFW.Data.Contexts
{
    public interface ISqlDbContext: IDisposable 
    {
        DataAccess Conexao { get; }
    }

    public class SqlDbContext : DbContext, IDisposable, ISqlDbContext
    {
        private readonly DbConnection db_connection;
        //public SqlDbContext() : this(DbConfig.PegarConexaoConfig<SqlDbContext>())
        //{
        //}
        public SqlDbContext(DbContextOptions options) : base(options)
        {
            try
            {
                db_connection = db_connection ?? this.Database.GetDbConnection() ?? new SqlConnection();
            }
            catch (Exception ex)
            {
                ex.Logar("SqlDbContext() - Falha ao configurar o DbContext do EF");
            }
            //LogServices.Debug($"EF.DbContextBase : {options.ContextType} - Conexao inicializada com : SERVER : {conn.DataSource} | DB : {conn.Database} | PROVIDER : ({this.Database.ProviderName})");
        }

        public DataAccess Conexao => new DataAccess();

        public SqlConnection Connection;

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

        public override string ToString()
        {
            return $"EF DbConext : (SqlDbContext) > {base.Database.GetConnectionString()}"; 
        }
    }
}
