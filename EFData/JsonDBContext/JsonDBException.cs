
using ArmsFW.Core;
using ArmsFW.Services.Logging;
using System;

namespace ArmsFW.Infra.Data.JsonStore.Old
{
    [Serializable]
	public class JsonDBException : Exception
	{
		private Exception ex;

		public JsonDBException(string message)
			: base(message)
		{
			LogServices.GravarLog(message, "EFException", $@"{Aplicacao.Diretorio}\_logs\EF_Exceptions.txt");
		}

		public JsonDBException(string message, Exception innerException)
			: base(message, innerException)
		{
			ex = innerException;
			LogServices.GravarLog(message, "EFException", $@"{Aplicacao.Diretorio}\_logs\EF_Exceptions.txt");
		}
	}
}
