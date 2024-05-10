
using ArmsFW.Core;
using ArmsFW.Services.Logging;
using System;

namespace ArmsFW.Infra.Data
{
    [Serializable]
	internal class RepositoryException : Exception
	{
		private Exception ex;

		public RepositoryException(string message)
			: base(message)
		{
			LogServices.GravarLog(message, "EFException", $@"{Aplicacao.Diretorio}\_logs\EF_Exceptions.txt");
		}

		public RepositoryException(string message, Exception innerException)
			: base(message, innerException)
		{
			ex = innerException;
			LogServices.GravarLog(message, "EFException", $@"{Aplicacao.Diretorio}\_logs\EF_Exceptions.txt");
		}
	}
}
