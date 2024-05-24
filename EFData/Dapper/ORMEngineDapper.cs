//using ArmsFW.Infra.Data;
//using System;
//using System.Collections.Generic;

//namespace Core.Data
//{
//    public interface IEntityApplication<T> where T : class
//	{
//		T ObterPorId(int id);

//		IEnumerable<T> ObterTodos();
//	}

//	public class ORMEngineDapper : ORMEngineDapper<object>
//	{
//	}
//	public class ORMEngineDapper<T> : DapperDataContext, IEntityApplication<T> where T : class
//	{
//		public DapperRepository<T> Rep { get; }

//		public ORMEngineDapper()
//		{
//			Rep = InicializaRepositorio<T>();
//		}

//		public ORMEngineDapper(string connectionString)
//			: base(connectionString)
//		{
//			Rep = InicializaRepositorio<T>();
//		}

//		public IEnumerable<T> ObterTodos()
//		{
//			try
//			{
//				return Rep.ObterTodos().Result;
//			}
//			catch (Exception ex)
//			{
//				throw new Exception("Erro ao obter os dados da lista do Repositorio", ex);
//			}
//		}

//		public void Salvar(T entidade)
//		{
//			try
//			{
//				Rep.Inserir(entidade);
//			}
//			catch (Exception ex)
//			{
//				throw new Exception("Ocorreu um erro ao salvar a Entidade no Banco de dados", ex);
//			}
//		}

//		protected T ObterPorId(int id)
//		{
//			try
//			{
//				return Rep.ObterPorId(id);
//			}
//			catch (Exception ex)
//			{
//				throw new Exception("Ocorreu um erro ao salvar a Entidade no Banco de dados", ex);
//			}
//		}

//		public bool QueryContemRegistros(string sqlCommand)
//		{
//			return Rep.QuerySelectSingle<bool>(sqlCommand);
//		}

//        T IEntityApplication<T>.ObterPorId(int id)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
