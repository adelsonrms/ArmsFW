using System;

namespace app.web.Security
{
	public class UserToken
	{
		public string Token { get; set; }

		public DateTime Expiration { get; set; }

		public string TokenID { get; set; }

		public string Message { get;  set; }

		public bool IsValid { get; internal set; }
	}
}
