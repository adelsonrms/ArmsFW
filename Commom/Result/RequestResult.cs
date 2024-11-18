namespace ArmsFW.Services.Shared
{
	public class RequestResult<TData>
	{
		public int StatusCode { get; set; }

		public string Mensagem { get; set; }

		public TData Dados { get; set; }
	}
}
