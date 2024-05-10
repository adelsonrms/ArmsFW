using ArmsFW.Services.Session;
using ArmsFW.Services.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArmsFW.Infra.Data.Repositories
{
    public interface IRepository<T, TKey> : IDisposable where T : class
	{
		Result<List<T>> ObterTodos();

		Result<T> ObterPorId(TKey id);

		Result<T> Inserir(T entity);

		Result<int> Excluir(T entity);

		Result<T> Alterar(T entity);

		Result<object> Submit();

		int Contar(Func<T, bool> filtro);
		bool Existe(Func<T, bool> filtro);
		List<T> Pesquisar(Func<T, bool> filtro);
		T PesquisarSingle(Func<T, bool> filtro);
		UsuarioDaSessao User { get; set; }
          
        Task<SQLCommandResult> Execute(string sqlCommand);
	}
	public interface IRepository<T> : IRepository<T, int> where T : class {	}
}
