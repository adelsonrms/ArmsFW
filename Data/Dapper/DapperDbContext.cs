using ArmsFW.Data.Contexts;
using ArmsFW.Services.Shared;
using ArmsFW.Services.Shared.Settings;
using Microsoft.Data.SqlClient;
using System;

namespace ArmsFW.Data
{
	public enum ConnectionState
	{
		Opened,
		Close
	}

	public class DapperDbContext : IDisposable
	{
		private SqlConnection _cnn;

		public string ConnectionStrig { get; private set; }

		public ConnectionState ConnectionState
		{
			get
			{
				if (_cnn != null) return (ConnectionState)_cnn.State;
				return ConnectionState.Close;
			}
		}

		public SqlConnection Connection
		{
			get
			{
				if (_cnn == null) Connect();
				return _cnn;
			}
		}

		private string getNewConnection(string connectionString)
		{
			return new SqlConnectionStringBuilder(connectionString)
			{
				ApplicationName = "Arms FW Utils",
				ConnectRetryCount = 5,
				CurrentLanguage = "english",
				ConnectRetryInterval = 5,
				MaxPoolSize = 200,
				Pooling = true
			}.ConnectionString;
		}

		public DapperDbContext()
		{
			ConnectionStrig = getNewConnection(DbConfig.ConnectionString);
			//Connect();
		}

		public DapperDbContext(string connectionString)
		{
			ConnectionStrig = connectionString;
		}

		private SqlConnection Connect()
		{
			ConnectionStrig = getNewConnection(ConnectionStrig);
			return Connect(ConnectionStrig);
		}

		private SqlConnection Connect(string stringConnection)
		{
			//App.GravarLog("Iniciando a conexao", "DapperDbContext.Connect()", "db.txt");

			try
			{
                if (_cnn!=null)
                {
					if (_cnn.State == System.Data.ConnectionState.Open)
					{
						//App.GravarLog($"Ja existe uma conexao ativa...{_cnn.ConnectionString}", "DapperDbContext.Connect()", "db.txt");
						return _cnn;
					}
				}

				//App.GravarLog($"Cria uma nova conexao...{stringConnection}", "DapperDbContext.Connect()", "db.txt");
				ConnectionStrig = getNewConnection(stringConnection); 
				
				if (ConnectionState == ConnectionState.Close)
				{
					//LogServices.Debug("SqlConnection - Nova conexao");
					_cnn = new SqlConnection(ConnectionStrig);
				
					//LogServices.Debug("SqlConnection - Inicia a conexao...");
					_cnn.Open();
					//LogServices.Debug("SqlConnection - OK, conexao aberta!");
				}
				else
                {
					//LogServices.Debug("SqlConnection - Conexao ja ativa");
				}
				return _cnn;
			}
			catch(Exception ex)
			{
				App.GravarLog($"Ocorreu uma falha na inicializa��o da conex�o", "DapperDbContext.Connect()", "db.txt");
				App.GravarLog($"Excessao > {ex.ColetarMensagensPilhaExceptions()}", "DapperDbContext.Connect()", "db.txt");

				_cnn = null;
			}

			App.GravarLog($"N�o foi possivel inicializar a conexao com o DB", "DapperDbContext.Connect()", "db.txt");
			return null;
		}

		private SqlConnection GetConnection(bool bForceNew = false)
		{
			if (bForceNew)
			{
				CloseConnection();
			}
			return Connect();
		}

		private bool CloseConnection()
		{
			bool ret = true;
			try
			{
				SqlConnection cnn = _cnn;

				if (cnn != null)
				{

					cnn.Close();
					//LogServices.Debug("SqlConnection - Conexao encerrada");
					return ret;
				}

				return ret;
			}
			catch
			{
				return false;
			}
			finally
			{
				_cnn = null;
				_cnn?.Dispose();
			}
		}

		public void Dispose()
		{
			CloseConnection();
		}

		public bool ExecuteCommand(SqlCommand cmdCommand)
		{
			SqlTransaction tran = Connection.BeginTransaction();
			try
			{
				cmdCommand.Transaction = tran;
				cmdCommand.Connection = Connection;
				cmdCommand.BeginExecuteNonQuery();
				tran.Commit();
				return true;
			}
			catch
			{
				tran.Rollback();
				return false;
			}
		}

		internal bool FinalizeConnection()
		{
			return CloseConnection();
		}
	}
}
