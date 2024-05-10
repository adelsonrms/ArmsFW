using System;
using System.Collections.Generic;
using System.Text;

namespace ArmsFW.Services.Shared
{
	public static class ExceptionExtensions
	{
		public static Dictionary<int, InfoException> GetInnerExceptions(Dictionary<int, Exception> exceptions)
		{
			Dictionary<int, InfoException> exceptions2 = new Dictionary<int, InfoException>();
			if (exceptions == null)
			{
				return new Dictionary<int, InfoException>();
			}
			foreach (KeyValuePair<int, Exception> exception in exceptions)
			{
				int nivel = 1;
				getInnerMessage(exception.Value, ref nivel, ref exceptions2);
			}
			return exceptions2;
		}

		public static Dictionary<int, InfoException> GetInnerExceptions(this Exception exception)
		{
			try
			{
				return GetInnerExceptions(new Dictionary<int, Exception> { { 1, exception } });
			}
			catch (Exception ex)
			{
				return new Dictionary<int, InfoException> { 
				{
					1,
					getInfoException(ex, 1)
				} };
			}
		}

		private static void getInnerMessage(Exception ex, ref int nivel, ref Dictionary<int, InfoException> exceptions)
		{
			exceptions = exceptions ?? new Dictionary<int, InfoException>();
			exceptions.Add(exceptions.Count, getInfoException(ex, nivel));
			if (ex.InnerException != null)
			{
				nivel++;
				getInnerMessage(ex.InnerException, ref nivel, ref exceptions);
			}
		}
	
		private static InfoException getInfoException(Exception ex, int nivel)
		{
			return new InfoException
			{
				Leval = nivel,
				Message = ex.Message,
				Source = ex.Source,
				StackTrace = ex.StackTrace,
				ExceptionType = ex.GetType().Name
			};
		}

		public static void EmitirException<T>(this Exception ex, T context) where T : class
		{
			PortalExceptionHandle<T> portalExceptionHandle = new PortalExceptionHandle<T>(ex, context);
			throw new Exception(portalExceptionHandle.PreviousInnerExceptionsMessagem, portalExceptionHandle);
		}

		public static void EmitirException(this Exception ex)
		{
			PortalExceptionHandle<object> portalExceptionHandle = new PortalExceptionHandle<object>(ex);
			throw new Exception(portalExceptionHandle.PreviousInnerExceptionsMessagem, portalExceptionHandle);
		}

		public static string ColetarMensagensPilhaExceptions(this Exception ex)
        {
			StringBuilder sb = new StringBuilder();
			foreach (var exInfo in ex.GetInnerExceptions())
			{
				sb.AppendLine($"{exInfo.Value.Source} - {exInfo.Value.Message}");
			}
			return sb.ToString();
		}

		
	}
}
