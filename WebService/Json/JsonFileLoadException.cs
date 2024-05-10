using System;
using System.Runtime.Serialization;

namespace ArmsFW.Lib.Web.Json
{
	[Serializable]
	internal class JsonFileLoadException<TObjeto> : Exception where TObjeto : class
	{
		public TObjeto Objeto { get; set; }

		public string JsonFIle { get; set; }

		public dynamic JsonException { get; internal set; }

		public JsonFileLoadException()
		{
		}

		public JsonFileLoadException(string message)
			: base(message)
		{
		}

		public JsonFileLoadException(string message, Exception innerException, TObjeto result)
			: base(message, innerException)
		{
			Objeto = result;
			Source = "ArmsFW.Lib.Exceptions";
		}

		protected JsonFileLoadException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
