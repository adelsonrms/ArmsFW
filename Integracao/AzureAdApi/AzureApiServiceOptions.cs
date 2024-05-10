namespace ArmsFW.Services.Azure
{
	public class AzureApiServiceOptions
	{
		public string Resource { get; set; }

		public string TenantID { get; set; }

		public string EndPointAccessToken { get; set; }

		public ApiCredential Credencial { get; set; }
	}
}
