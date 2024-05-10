using app.core.Domain;
using ArmsFW.Domain;
using ArmsFW.Infra.Data.Contexts;
using ArmsFW.Infra.Data.Extensions;
using ArmsFW.Lib.Web.Json;
using ArmsFW.Services.Extensions;
using ArmsFW.Services.Logging;
using ArmsFW.Services.Session;
using ArmsFW.Services.Shared;
using ArmsFW.Services.Shared.Pesquisa;
using ArmsFW.Services.Shared.Settings;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ArmsFW.Infra.Data.Repositories
{
    public class Repository<TEntity> : Repository<TEntity, int>, IRepository<TEntity>, IDisposable where TEntity : class
	{
		public Repository() : base(ArmsFWContext.Create()) { }
		public Repository(SqlDbContext db) : base(db){}
	}
	public class Repository<TEntity, TKey> : IRepository<TEntity, TKey>, IDisposable where TEntity : class
	{
		public readonly UsuarioDaSessao _user = SessionService.GetUser();

		protected readonly SqlDbContext _Db;

		public Repository() => _Db = ArmsFWContext.Create();
		public Repository(SqlDbContext db) => _Db = db;


		public Result<TEntity> Inserir(TEntity obj)
		{
			try
			{
				obj.DefinirValor("UsuarioAtualizacao", this.User.Email);

				_Db.Set<TEntity>().Add(obj);

				GravarAlteracoesDB();

				IEFEntity entitySaida = obj as IEFEntity;

				if (entitySaida != null) entitySaida.EFGuid = Guid.NewGuid().ToString();

				return ResultBase<TEntity>.Sucesso($"Um item {obj.ToString()} acresentado com sucesso na lista", obj);
			}
			catch (Exception ex)
			{
				return ResultBase<TEntity>.Erro($"EF Exception > Falha ao executar o metodo {ex.TargetSite.Name}({typeof(TEntity).Name}). Detalhe : {ex.ColetarMensagensPilhaExceptions()}");
			}
		}

		public Result<int> Excluir(TEntity obj)
		{
			try
			{
				_Db.Set<TEntity>().Remove(obj);
				GravarAlteracoesDB();
				return ResultBase<int>.Sucesso($"Items excluidos", 1);
			}
			catch (Exception ex)
			{
				return ResultBase<int>.Erro($"EF Exception > Falha ao executar o metodo {ex.TargetSite.Name}({typeof(TEntity).Name}). Detalhe : {ex.ColetarMensagensPilhaExceptions()}");
			}
		}

		public Result<int> ExcluirPorId(TKey id) => Excluir(ObterPorId(id).Data);

		public Result<TEntity> Alterar(TEntity obj)
		{
			try
			{


				obj.DefinirValor("Atualizacao", DateTime.Now);
				obj.DefinirValor("UsuarioAtualizacao", this.User.Email);

				CriarHistorico(obj);

				_Db.Entry(obj).State = EntityState.Modified;
				GravarAlteracoesDB();
				IEFEntity entitySaida = obj as IEFEntity;
				if (entitySaida != null) entitySaida.EFGuid = Guid.NewGuid().ToString();

				var retorno = ResultBase<TEntity>.Sucesso($"Uma entidade foi atualizada", obj);
				retorno.Acao = "alterado";

				return retorno;
			}
			catch (Exception ex)
			{
				return ResultBase<TEntity>.Erro($"EF Exception > Falha ao executar o metodo {ex.TargetSite.Name}({typeof(TEntity).Name}). Detalhe : {ex.ColetarMensagensPilhaExceptions()}");
			}
		}

		//TODO : Nao ta funcionando. Nao ta pegando os dados do BD. Ta pegando sempre do cache do DbContext
		private void CriarHistorico(TEntity obj)
		{
			try
			{
				EntityTracking entity = obj as EntityTracking;

				if (entity != null)
				{
					var type = obj.GetType();
					string cmd = $"SELECT * FROM {type.PegarNomeDaTabela()} WHERE id = {entity.Id}";
				}
			}
			catch (Exception ex)
			{
				ex.Logar();
			}
		}

		public Result<List<TEntity>> ObterTodos()
		{
			try
			{
				return ResultBase<List<TEntity>>.Sucesso($"A lista foi recuperada com sucesso", _Db.Set<TEntity>().AsNoTracking().ToList());
			}
			catch (Exception ex)
			{
				return ResultBase<List<TEntity>>.Erro($"EF Exception > Falha ao executar o metodo {ex.TargetSite.Name}({typeof(TEntity).Name}). Detalhe : {ex.ColetarMensagensPilhaExceptions()}");
			}
		}

		public Result<TEntity> ObterPorId(TKey id)
		{
			TEntity entity = null;
			try
			{
				entity = _Db.Set<TEntity>().Find(id);
				if (entity != null)
				{
					IEFEntity entitySaida = entity as IEFEntity;
					if (entitySaida != null)
					{
						entitySaida.EFGuid = Guid.NewGuid().ToString();
						return ResultBase<TEntity>.Sucesso($"Entidade encontrada", entitySaida);

					}
					return ResultBase<TEntity>.Sucesso($"Entidade encontrada", entity);
				}

				return ResultBase<TEntity>.Erro($"Nenhuma entidade foi encontrada para o ID : {id}");
			}
			catch (Exception ex)
			{
				return ResultBase<TEntity>.Erro($"EF Exception > Falha ao executar o metodo {ex.TargetSite.Name}({typeof(TEntity).Name}). Detalhe : {ex.ColetarMensagensPilhaExceptions()}");
			}
		}

		public DbSet<TEntity> DbSet => _Db.Set<TEntity>();

		public UsuarioDaSessao User { get => _user; set => throw new NotImplementedException(); }

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

		public bool Existe(TKey id)
		{

			try
			{
				var item = DbSet.Find(id);
				return item != null;
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
				ex.Logar();
			}

			return null;
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

		public List<TEntity> ListarPorSql(string sql)
		{
			FormattableString cmd_projetos_por_contrato = $@"{sql}";

			var items = DbSet.FromSqlInterpolated($@"{sql}").ToList();

			return null;
		}

		private bool GravarAlteracoesDB()
		{
			try
			{
				_Db.SaveChanges();
				return true;
			}
			catch (Exception ex)
			{
				var pilha = ex.ColetarMensagensPilhaExceptions();
				throw new Exception("EF Exception > Ocorreu um erro ao enviar as atualizações para o banco de dados. Metodo : CommitChanges()." + pilha);
			}
		}

		public Result<object> Submit()
		{
			GravarAlteracoesDB();
			return ResultBase.Sucesso($"Uma entidade foi atualizada");
		}

		public void Dispose()
		{
			_Db.Dispose();

		}

		public async Task<SQLCommandResult> Execute(string sqlCommand)
		{
			try
			{
				var cmd = await this._Db.Database.ExecuteSqlRawAsync(sqlCommand);
				return new SQLCommandResult { CodeReturn = cmd, Message = $"O comando foi processado com sucesso. Linhas afetadas ({cmd})" };
			}
			catch (Exception ex)
			{
				ex.Logar();
				return new SQLCommandResult { CodeReturn = 0, Message = $"Falha desconhecida no processamento do comando. Detalhe : {ex.Message}" };
			}
		}
	}
}
