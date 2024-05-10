namespace ArmsFW.Services.Azure
{
	public class RequestInfo
	{
		public string ContentType { get; internal set; } = "application/json";

		public string ContentBody { get; internal set; }

		public string Method { get; internal set; } = "GET";
        public string BearerToken { get; internal set; }

        public RequestInfo()
		{
		}

		public RequestInfo(string method)
		{
			Method = method;
		}
	}
}
