using ArmsFW.Domain;
using ArmsFW.Services.Logging;
using ArmsFW.Services.Shared.Settings;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ArmsFW.Infra.Data
{
    public class DapperRepository : DapperRepository<object>
	{
	}
    public class DapperRepository<TEntity> : IDapperRepository<TEntity>
    {
        private DapperDbContext _context;
        public DapperDbContext Context { get; private set; }

        public SqlConnection DB => _context.Connection;
        private string table = "";

        private string sqlCommand;

        private DbEntity<object>.TEntityBase<TEntity> entity;

        private TEntity retorno;

        public DapperRepository()
        {
            _context = new DapperDbContext(AppSettings.ConnectionString);
        }

        public DapperRepository(DapperDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TEntity>> ObterTodos()
        {
            IEnumerable<TEntity> result = null;
            try
            {
                DbEntity<object>.TEntityBase<TEntity> Entity = new DbEntity<object>.TEntityBase<TEntity>(null, table);
                sqlCommand = Entity.Select(all: true);
                result = await QuerySelect<TEntity>(sqlCommand);
                return result;
            }
            catch (Exception ex)
            {
                EmitirException(ex, "Db.ObterTodos", null, null);
                return result;
            }
        }
        public TEntity ObterPorId(int id)
        {
            try
            {
                string sqlCommand = new DbEntity<object>.TEntityBase<TEntity>(null, table).SelectById(id.ToString());
                retorno = QuerySelectSingle<TEntity>(sqlCommand);
            }
            catch (Exception ex)
            {
                EmitirException(ex, "Db.ObterPorId", null, null);
            }
            return retorno;
        }

        public TEntity Inserir(object obj)
        {
            entity = new DbEntity<object>.TEntityBase<TEntity>(obj, table);
            try
            {
                SqlConnection connection = _context.Connection;
                connection.ExecuteAsync(entity.Insert());
                _context.FinalizeConnection();
                connection?.CloseAsync();

            }
            catch (Exception ex)
            {
                EmitirException(ex, "Db.Insert", obj, retorno);
            }
            return retorno;
        }

        public TEntity ObtemPorNome(string nome)
        {
            return ObtemPor("Nome", nome);
        }

        public TEntity ObtemPorEmail(string email)
        {
            return ObtemPor("EmailProfissional", email);
        }

        public TEntity ObtemPorGuid(string idGuid)
        {
            return ObtemPor("IdGuid", idGuid);
        }

        public TEntity ObtemPor(string coluna, string valor)
        {
            try
            {
                DbEntity<object>.TEntityBase<TEntity> Entity = new DbEntity<object>.TEntityBase<TEntity>(null, table);
                sqlCommand = Entity.SelectByCollumn(coluna, valor);
                using (_context)
                {
                    SqlConnection connection = _context.Connection;
                    string sql = sqlCommand;
                    var param = new { };
                    CommandType? commandType = CommandType.Text;
                    retorno = connection.Query<TEntity>(sql, param, null, buffered: true, null, commandType).FirstOrDefault();

                    _context.FinalizeConnection();

                    connection?.CloseAsync();

                }
            }
            catch (Exception ex)
            {
                EmitirException(ex, "Db.ObtemPor", null, null);
            }
            return retorno;
        }

        public Retorno QuerySelectSingle<Retorno>(string command)
        {
            Retorno result = default(Retorno);
            try
            {
                using (_context)
                {
                    //LogServices.Debug("Dapper.QuerySelectSingle<Retorno>() - INICIO");

                    SqlConnection connection = _context.Connection;
                    var param = new { };
                    CommandType? commandType = CommandType.Text;
                    result = connection.QueryFirstOrDefault<Retorno>(command, param, null, null, commandType);
                    _context.FinalizeConnection();

                    //LogServices.Debug("Dapper.QuerySelectSingle<Retorno>() - FIM");

                    return result;
                }
            }
            catch (Exception ex)
            {
                ex.Logar();
                return result;
            }
        }

        public async Task<IEnumerable<Retorno>> QuerySelect<Retorno>(string command)
        {
            IEnumerable<Retorno> result = null;
            try
            {
                using (_context)
                {
                    //LogServices.Debug("Dapper.QuerySelect<Retorno>() - Inicio....");

                    SqlConnection connection = _context.Connection;
                    var param = new { };
                    CommandType? commandType = CommandType.Text;
                    result = await connection.QueryAsync<Retorno>(command, param, null, null, commandType);
                    _context.FinalizeConnection();

                    connection?.CloseAsync();

                    //LogServices.Debug("Dapper.QuerySelect<Retorno>() - FIM");

                }
                return result;
            }
            catch (Exception ex)
            {
                ex.Logar();
                return result;
            }
        }

        public async Task<IEnumerable<TEntity>> QuerySelect(string command)
        {
            IEnumerable<TEntity> result = null;
            try
            {
                using (_context)
                {
                    //LogServices.Debug("Dapper.QuerySelect() - Inicio....");

                    SqlConnection connection = _context.Connection;
                    object obj;
                    if (connection == null)
                    {
                        obj = null;
                    }
                    else
                    {
                        var param = new { };
                        CommandType? commandType = CommandType.Text;
                        obj = connection.QueryAsync<TEntity>(command, param, null, null, commandType);
                    }
                    result = await (Task<IEnumerable<TEntity>>)obj;
                    _context.FinalizeConnection();
                    connection?.CloseAsync();

                    //LogServices.Debug("Dapper.QuerySelect() - Fim !");
                }
                return result;
            }
            catch (Exception ex)
            {
                ex.Logar();
                return result;
            }
        }

        public async Task<SQLCommandResult> Execute(string command)
        {
            SQLCommandResult sqlResult = new SQLCommandResult();
            try
            {
                using (_context)
                {
                    SqlConnection connection = _context.Connection;
                    var param = new { };
                    CommandType? commandType = CommandType.Text;

                    while (connection == null)
                    {
                        connection = _context.Connection;
                    }

                    if (await connection.ExecuteAsync(command, param, null, null, commandType) != 0)
                    {
                        sqlResult = new SQLCommandResult
                        {
                            CodeReturn = -1,
                            Message = "Comando processado com sucesso"
                        };
                    }
                    _context.FinalizeConnection();
                    connection?.CloseAsync();
                }
                return sqlResult;
            }
            catch (Exception ex)
            {
                ex.Logar();

                throw new QueryCommandException(ex.Message, ex)
                {
                    SQLCommand = command,
                    SQLCommandResult = new SQLCommandResult
                    {
                        CodeReturn = 0,
                        Message = "Erro ao processar o comando : " + ex.Message
                    }
                };
            }
        }

        private void EmitirException(Exception ex, string contexto, dynamic obj, dynamic retorno)
        {
            var EnvironmentDetails = new
            {
                DbContext = _context,
                Entity = entity,
                Entidade = (object)obj,
                Tabela = table,
                Retorno = (object)retorno,
                Operacao = contexto,
                SQLCrudCommands = entity.SqlCrud
            };
            throw new DapperRepositoryExceptionHandle("Ocorreu um Exception inesperada. Detalhe " + ex.Message, ex, EnvironmentDetails);
        }
    }
}
