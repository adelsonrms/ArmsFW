//using System;

//namespace ArmsFW.Infra.Data
//{
//    public class DapperDataContext : IDisposable
//	{
//		private readonly string ConnectionString;

//		private DapperDbContext _db;

//		protected virtual DapperDbContext Db => _db;

//		public DapperDataContext()
//		{
//			_db = new DapperDbContext();
//		}

//		public DapperDataContext(string connectionString)
//		{
//			ConnectionString = connectionString;
//			_db = new DapperDbContext(connectionString);
//		}

//		protected DapperRepository<TEntidade> InicializaRepositorio<TEntidade>() where TEntidade : class
//		{
//			try
//			{
//				if (_db == null)
//				{
//					_db = new DapperDbContext();
//				}
//				return new DapperRepository<TEntidade>(_db);
//			}
//			catch (Exception value)
//			{
//				Console.WriteLine(value);
//				throw;
//			}
//		}

//		public void Dispose()
//		{
//			_db.Dispose();
//		}
//	}
//}
