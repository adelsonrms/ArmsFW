using Microsoft.Data.SqlClient;
using System.Collections.Generic;

using System.Threading.Tasks;

namespace ArmsFW.Data
{
    public interface IDapperRepository<TEntity>
    {
        DapperDbContext Context { get; }
        SqlConnection DB { get; }

        Task<SQLCommandResult> Execute(string command);
        TEntity Inserir(object obj);
        TEntity ObtemPor(string coluna, string valor);
        TEntity ObtemPorEmail(string email);
        TEntity ObtemPorGuid(string idGuid);
        TEntity ObtemPorNome(string nome);
        TEntity ObterPorId(int id);
        Task<IEnumerable<TEntity>> ObterTodos();
        Task<IEnumerable<TEntity>> QuerySelect(string command);
        Task<IEnumerable<Retorno>> QuerySelect<Retorno>(string command);
        Retorno QuerySelectSingle<Retorno>(string command);
    }
}