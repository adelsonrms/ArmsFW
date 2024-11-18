namespace ArmsFW.Services.Shared
{
	public class RedirectTo
	{
		public string Controller { get; set; }

		public string Action { get; set; }

		public string Url { get; set; }

		public bool Success { get; set; }

		public dynamic RouterArgs { get; set; }

		public RedirectTo()
		{
			Url = "/";
		}

		public RedirectTo(string redirectTo)
		{
			Url = redirectTo;
			string url = Url;
			if (url != null && url.Split("/".ToCharArray()).Length > 1)
			{
				string url2 = Url;
				Controller = ((url2 != null) ? url2.Split("/".ToCharArray())[1] : null);
				string url3 = Url;
				Action = ((url3 != null) ? url3.Split("/".ToCharArray())[2] : null);
			}
		}
	}
}
