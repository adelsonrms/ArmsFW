using System;
using System.Runtime.Serialization;

namespace ArmsFW.Data
{
	[Serializable]
	public class DapperRepositoryExceptionHandle : Exception
	{
		public string Ambiente { get; set; }

		public object EnviromentRuntimeMethod { get; set; }

		public DapperRepositoryExceptionHandle()
		{
		}

		public DapperRepositoryExceptionHandle(string message)
			: base(message)
		{
		}

		public DapperRepositoryExceptionHandle(string message, Exception innerException, object envRuntime)
			: base(message, innerException)
		{
			EnviromentRuntimeMethod = envRuntime;
		}

		protected DapperRepositoryExceptionHandle(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
