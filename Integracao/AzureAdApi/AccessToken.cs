namespace ArmsFW.Services.Azure
{
	public class AccessToken
	{
		public string token_type { get; set; }

		public string expires_in { get; set; }

		public string expires_on { get; set; }

		public string ext_expires_in { get; set; }

		public string not_before { get; set; }

		public string access_token { get; set; }

		public string refresh_token { get; set; }

		public string resource { get; set; }

		public dynamic Response { get; set; }

		public string ErrorMessage { get; internal set; }

		public dynamic Error { get; internal set; }

		public AccessToken()
		{
		}

		public AccessToken(string accessToken)
			: this()
		{
			access_token = accessToken;
		}

		public bool IsValid()
		{
			return !string.IsNullOrEmpty(access_token);
		}

		public override string ToString()
		{
			return (IsValid() ? ("Token (" + token_type + "): " + access_token.Substring(0, 20) + "....") : ("Nao há token ! (" + ErrorMessage + ")")) ?? "";
		}
	}
}
