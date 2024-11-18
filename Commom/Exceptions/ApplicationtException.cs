using System;
using System.Runtime.Serialization;
using ArmsFW.Services.Shared.Settings;

namespace ArmsFW.Services.Shared.Exceptions
{
	[Serializable]
	public class ApplicationtException : Exception
	{
		public object Ambiente => new
		{
			AppContext.BaseDirectory,
			App.wwwroot,
			AppSettings.Ambiente,
			AppSettings.ConnectionString
		};

		public object Runtime { get; set; }

		public ApplicationtException()
		{
		}

		public ApplicationtException(string message)
			: base(message)
		{
		}

		public ApplicationtException(string message, object envRuntime)
			: base(message)
		{
			Runtime = envRuntime;
		}

		public ApplicationtException(string message, Exception innerException, object envRuntime)
			: base(message, innerException)
		{
			Runtime = envRuntime;
		}

		protected ApplicationtException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
