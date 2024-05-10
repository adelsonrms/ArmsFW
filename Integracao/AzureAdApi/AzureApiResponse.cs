using System;
using ArmsFW.Lib.Web.HttpRest;

namespace ArmsFW.Services.Azure
{
	public class AzureApiResponse<TDataResponse>
	{
		public TDataResponse Data { get; private set; }

		public AzureErrorRequest ErrorRequest { get; private set; }

		public Exception Exception { get; private set; }

		public bool Success
		{
			get
			{
				if (Response != null)
				{
					return Response.Success;
				}
				return false;
			}
		}

		public ResponseOld Response { get; internal set; }

		internal static AzureApiResponse<TDataResponse> CriarResponse()
		{
			return new AzureApiResponse<TDataResponse>();
		}

		internal void addData(TDataResponse @object)
		{
			Data = @object;
		}

		internal AzureErrorRequest addErro(string content)
		{
			ErrorRequest = AzureApiService.GetAzureErrorRequest(content);
			if (ErrorRequest == null)
			{
				ErrorRequest = new AzureErrorRequest
				{
					error_description = Response.Description
				};
			}
			return ErrorRequest;
		}

		internal void addExeption(string message)
		{
			Exception = new Exception(message);
		}
	}
}
