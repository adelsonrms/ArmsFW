using ArmsFW.Services.Shared.Settings;
using System;
using System.Runtime.Serialization;

namespace ArmsFW.Infra.Data
{
    [Serializable]
	public class QueryCommandException : Exception
	{
		public AppSettings AppConfig => AppSettings.Instance;

		public string SQLCommand { get; set; }

		public SQLCommandResult SQLCommandResult { get; set; }

		public QueryCommandException()
		{
		}

		public QueryCommandException(string message, Exception ex)
			: base(message, ex)
		{
		}

		public QueryCommandException(string message)
			: base(message)
		{
		}

		protected QueryCommandException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
