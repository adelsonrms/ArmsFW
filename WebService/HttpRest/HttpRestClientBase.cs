using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArmsFW.Lib.Web.HttpRest
{
	public abstract class HttpRestClientBase : IHttpRestClient, IDisposable
	{
		public dynamic Client { get; set; }

		public string EndPoint { get; set; }

		public Dictionary<string, string> HeadersParams { get; set; } = new Dictionary<string, string>();


		public Dictionary<string, string> QueryStringParams { get; set; } = new Dictionary<string, string>();


		public Dictionary<string, string> BodyStringParams { get; set; } = new Dictionary<string, string>();


		public void AddParameter(string name, string value, ParameterType type)
		{
			if (type == ParameterType.HttpHeader && !string.IsNullOrEmpty(value))
			{
				HeadersParams.Add(name, value);
			}
			if (type == ParameterType.RequestBody && !string.IsNullOrEmpty(value))
			{
				BodyStringParams.Add(name, value);
			}
			if (type == ParameterType.QueryString && !string.IsNullOrEmpty(value))
			{
				QueryStringParams.Add(name, value);
			}
		}

		protected void AddHeader(string name, string value)
		{
			AddParameter(name, value, ParameterType.HttpHeader);
		}

		public void SetTokenAuthorization(string bearerToken)
		{
			AddHeader("Authorization", "Bearer " + bearerToken);
		}

		protected abstract void AddRequestParams(dynamic req, Dictionary<string, string> parameters, ParameterType type = ParameterType.QueryString);

		public abstract void Dispose();

		public async Task<ResponseOld> GetAsync<TResponse>()
		{
			return await SendRequestAsync<TResponse>("GET");
		}

		public async Task<ResponseOld> GetAsync()
		{
			return await GetAsync<object>();
		}

		public async Task<ResponseOld> GetAsync(string url, string token = "")
		{
			AddHeader("Authorization", "Bearer " + token);
			return await GetAsync<object>();
		}

		public async Task<ResponseOld> GetHeadAsync<TResponse>()
		{
			return await SendRequestAsync<TResponse>("HEAD");
		}

		public async Task<ResponseOld> PostAsync<TResponse>()
		{
			return await SendRequestAsync<TResponse>("POST");
		}

		public async Task<ResponseOld> PutAsync<TResponse>()
		{
			return await SendRequestAsync<TResponse>("PUT");
		}

		public async Task<ResponseOld> PatchAsync<TResponse>()
		{
			return await SendRequestAsync<TResponse>("PATCH");
		}

		public async Task<ResponseOld> DeleteAsync<TResponse>()
		{
			return await SendRequestAsync<TResponse>("DELETE");
		}

		public abstract Task<ResponseOld> SendRequestAsync(string method, string contentType = "");

		public abstract Task<ResponseOld> SendRequestAsync<TResponse>(string method, string contentType = "");
	}
}
