namespace app.core.Services.Rest
{
	public class AccessToken
	{
		public string token_type { get; set; }

		public string expires_in { get; set; }

		public string access_token { get; set; }

		public string ext_expires_in { get; set; }

		public string refresh_token { get; set; }
		public dynamic Response { get; set; }

		public string ErrorMessage { get; internal set; }

		public dynamic Error { get; internal set; }

		public bool IsValid()
		{
			return !string.IsNullOrEmpty(access_token);
		}
	}
}
