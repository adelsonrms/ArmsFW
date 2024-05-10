namespace ArmsFW.Services.Azure
{
	public class AzureErrorRequest
	{
		public string error { get; set; }

		public string error_description { get; set; }

		public string error_codes { get; set; }

		public string timestamp { get; set; }

		public string trace_id { get; set; }

		public string correlation_id { get; set; }

		public string error_uri { get; set; }

        public string Descricao { get; set; }
	}
}
