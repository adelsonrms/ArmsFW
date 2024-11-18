using ArmsFW.Domain;
using ArmsFW.Services.Session;
using ArmsFW.Services.Shared;
using System;
using System.Collections.Generic;

namespace ArmsFW.Data.JsonStore.Old
{
	public interface IJsonRepository<T> : IJsonRepository<T, string> where T : IJsonEntity { }

	public interface IJsonRepository<T, TKey> where T : IJsonEntity
		{
		List<T> ObterTodos();

		Result<T> ObterPorId(TKey id);

		Result<T> Inserir(T entity);

		Result<T> Excluir(TKey id);

		Result<T> Alterar(T entity);

		void Submit();

		int Contar(Func<T, bool> filtro);
		bool Existe(Func<T, bool> filtro);
		List<T> Pesquisar(Func<T, bool> filtro);
		T PesquisarSingle(Func<T, bool> filtro);

		UsuarioDaSessao Usuario { get; set; }

	}
}
