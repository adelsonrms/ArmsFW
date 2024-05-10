using System;
using System.Threading.Tasks;

namespace ArmsFW.Lib.Web.HttpRest
{
	public interface IHttpRestClient : IDisposable
	{
		string EndPoint { get; set; }

		Task<ResponseOld> GetAsync<TResponse>();

		Task<ResponseOld> GetAsync();

		Task<ResponseOld> GetHeadAsync<TResponse>();

		Task<ResponseOld> PostAsync<TResponse>();

		Task<ResponseOld> PutAsync<TResponse>();

		Task<ResponseOld> PatchAsync<TResponse>();

		Task<ResponseOld> DeleteAsync<TResponse>();
	}
}
