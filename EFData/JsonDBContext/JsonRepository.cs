using ArmsFW.Domain;
using ArmsFW.Infra.Data.Contexts;
using ArmsFW.Infra.Data.Extensions;
using ArmsFW.Infra.Data.JsonStore;
using ArmsFW.Services.Extensions;
using ArmsFW.Services.Session;
using ArmsFW.Services.Shared;
using ArmsFW.Services.Shared.Pesquisa;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ArmsFW.Infra.Data.JsonStore.Old
{
    public class JsonRepository<TEntity> : IJsonRepository<TEntity, string>, IDisposable where TEntity : IJsonEntity
	{
		public readonly UsuarioDaSessao _user = SessionService.GetUser();

		protected readonly JsonDbContext _Db;

		public JsonRepository() => _Db = JsonDbContext.Create();
		public JsonRepository(JsonDbContext db)=> _Db = db;

		public Result<TEntity> Inserir(TEntity obj)
		{
			try
			{
				//Gera o 
				obj.Id = Guid.NewGuid().ToString();

				//Logica para salvar os dados no store
				_Db.Set<TEntity>().Add(obj);
				CommitChanges();
				obj.fl_status = "novo";

				return ResultBase<TEntity>.Sucesso($"Item {obj.ToString()} acresentado com sucesso na lista", obj);
			}
			catch (Exception ex)
			{
				throw new RepositoryException($"EF Exception > Falha inesperada ao INSERIR (INSERT) a entidade no DB. Metodo : Inserir({typeof(TEntity).Name}): " + ex.ColetarMensagensPilhaExceptions());
			}
		}

		public Result<TEntity> Excluir(string id)
		{
		
			try
			{
				var obj = ObterPorId(id).Data;

				_Db.Set<TEntity>().Remove(obj);

				CommitChanges();
				return ResultBase<TEntity>.Sucesso($"Item excluido da lista");
			}
			catch (Exception ex)
			{
				throw new RepositoryException($"EF Exception > Falha inesperada ao EXCLUIR (DELETE) a entidade no DB. Metodo : Excluir({typeof(TEntity).Name}): " + ex.ColetarMensagensPilhaExceptions());
			}
		}

		public Result<TEntity> Alterar(TEntity obj)
		{
			try
			{
				//Localiza a tabela no Cache
				var tabela = _Db.Set<TEntity>().Find(x => x.ToString() == obj.ToString()) ;

				//Mescla os dados do objeto Origem > Destino
				var destino = obj.Merge(tabela);

				CommitChanges();

				destino.fl_status = "alterado";

				return ResultBase<TEntity>.Sucesso($"Item {obj.ToString()} atualizado com sucesso na lista", destino);
			}
			catch (Exception ex)
			{
				throw new RepositoryException($"EF Exception > Falha inesperada ao ATUALIZAR (UPDATE) a entidade no DB. Metodo : Alterar({typeof(TEntity).Name}): " + ex.ColetarMensagensPilhaExceptions());
			}
		}

		public List<TEntity> ObterTodos()
		{
			try
			{
				return _Db.Set<TEntity>().ToList();
			}
			catch (Exception ex)
			{
				throw new RepositoryException($"EF Exception > Falha inesperada ao consultar a LISTA de entidades no DB. Metodo : List<{typeof(TEntity).Name}> ObterTodos(): " + ex.ColetarMensagensPilhaExceptions());
			}
		}

		public Result<TEntity> ObterPorId(string id)
		{
			TEntity entity = default(TEntity);

			try
			{
				entity = _Db.Set<TEntity>().Find(x => x.ToString() == id.ToString());

				if (entity != null)
				{
					return ResultBase<TEntity>.Sucesso($"Item encontrado na base", entity);
				}

				return ResultBase<TEntity>.Erro($"Item {id} nao existe");

			}
			catch (Exception ex)
			{
				throw new RepositoryException($"EF Exception > Falha inesperada ao consultar a entidade no EF. Metodo : {typeof(TEntity).Name} ObterPorId({id}): " + ex.ColetarMensagensPilhaExceptions());
			}
		}

		public List<TEntity> DbSet => _Db.Set<TEntity>();

        public UsuarioDaSessao Usuario { get => _user; set => throw new NotImplementedException(); }

        public int Contar(Func<TEntity, bool> filtro) => DbSet.Count(filtro);
		public bool Existe(Func<TEntity, bool> filtro)
        {
			try
			{
				var retorno = DbSet.Any(filtro);
				return retorno;

			}
			catch (Exception ex)
			{
				throw new RepositoryException($"EF > {typeof(TEntity).Name}.Existe(filtro) - Falha na pesquisa dos dados. " + ex.ColetarMensagensPilhaExceptions());
			}
		}

		public bool Existe(string id) {

			try
			{
				var item = DbSet.Any(x => x.ToString() == id.ToString());

				return item ;
			}
			catch (Exception ex)
			{
				throw new RepositoryException($"EF > {typeof(TEntity).Name}.Existe({id}) - Falha na pesquisa dos dados. " + ex.ColetarMensagensPilhaExceptions());
			}
		}

		public List<TEntity> Pesquisar(Func<TEntity, bool> filtro)
        {
            try
            {
				var retorno = DbSet.Where(filtro).ToList();
				return retorno;

			}
			catch (Exception ex)
            {
				throw new RepositoryException($"EF > {typeof(TEntity).Name}.Pesquisar(Where(filtro)) - Falha na pesquisa dos dados. " + ex.ColetarMensagensPilhaExceptions());
			}
		}
		public TEntity PesquisarSingle(Func<TEntity, bool> filtro)
        {
			try
			{
				var retorno = DbSet.FirstOrDefault(filtro); 
				return retorno;

			}
			catch (Exception ex)
			{
				throw new RepositoryException($"EF > {typeof(TEntity).Name}.PesquisarSingle(Where(filtro)) - Falha na pesquisa dos dados. " + ex.ColetarMensagensPilhaExceptions());
			}
		}

		public Pesquisa<TEntity> PesquisarPaginada(IQueryable<TEntity> lista, PaginacaoRequest paginacao)
		{
			var resultado = lista.ToListaPaginada(paginacao);
			return resultado;
		}

		private bool CommitChanges()
		{
			try
			{
				var dados = this.DbSet;

				_Db.GravarStore<TEntity>(dados);

				return true;
			}
			catch (Exception ex)
			{
				var pilha = ex.ColetarMensagensPilhaExceptions();
				throw new Exception("EF Exception > Ocorreu um erro ao enviar as atualizações para o banco de dados. Metodo : CommitChanges()." + pilha);
			}
		}

		public void Submit()
		{
			CommitChanges();
		}

        public void Dispose()
        {
			//_Db.Dispose();

		}
    }
}
