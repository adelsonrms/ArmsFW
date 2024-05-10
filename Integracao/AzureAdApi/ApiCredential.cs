using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using ArmsFW.Services.Shared.Util;

namespace ArmsFW.Services.Azure
{
	public class ApiCredential : ICloneable
	{
		public string Id { get; set; }

		public string AppName { get; set; }

		public string AppKey => AppId + "@" + TenanId + "." + Grant_Type;

		public string AppId { get; set; }

		public string EncryptSecret { get; set; }

		public string Resoruce { get; set; }

		public string Scopes { get; set; }

		public string Grant_Type { get; set; }

		public string TenanId { get; set; }

		public bool RequireConsent { get; set; }

		public string RedirectUris { get; set; }

		public string PostLogoutRedirectUris { get; set; }

		public bool AllowOfflineAccess { get; set; }

		internal string QueryString
		{
			get
			{
				string text = "client_id=" + AppId;
				text = text + "&grant_type=" + Grant_Type;
				Grant_Type = Grant_Type ?? "";
				string text2 = Grant_Type.ToLower();


				if (!(text2 == "password"))
				{
					if (text2 == "client_credentials")
					{
						text = text + "&client_secret=" + getSecret();
					}
				}
				else
				{
					text = text + "&client_secret=" + getSecret();
					text = text + "&username=" + Username + "&password=" + Password;
				}
				text += ((!string.IsNullOrEmpty(Resoruce)) ? ("&resource=" + Resoruce) : "");
				text += ((!string.IsNullOrEmpty(RedirectUris)) ? ("&redirect_uri=" + RedirectUris) : "");
				return text + ((!string.IsNullOrEmpty(Scopes)) ? ("&scope=" + Scopes) : "");
			}
			set
			{
			}
		}

		public FormUrlEncodedContent ContentBody => ConvertToFormBase(QueryString);

		public bool RequireClientSecret { get; set; }

		public string Username { get; set; } = "";
		public string Password { get; set; } = "";

		public ApiCredential()
		{
			//APP 1: 
			//ID		: ac6aad15-111e-4f79-a816-64c85dba0736
			//Secret	: pzmLMTIM8_%hjiyXH6615^_ (YS1ULwywjQHcC05xYoYuExHvq8lR+LSKiR9lIcn1sCo=)

			//APP 2: (nao habilita para obter informações da photo)
			//ID		: 8d4148ed-6766-40a5-9c43-d4889bb82c1c
			//Secret	: w8~QR1fu5.wVQ1XIKZ.GO.A2-gvPGq4w-Z (Crypto : UG1BtlHZmE3wRfym0Y4cBO2LFHkSHW05oXd724W38kZ0ie/emIVKMWVWMzXK5g17)


			Id = Guid.NewGuid().ToString();
			AppId = "ac6aad15-111e-4f79-a816-64c85dba0736";//"8d4148ed-6766-40a5-9c43-d4889bb82c1c";
			EncryptSecret = "YS1ULwywjQHcC05xYoYuExHvq8lR+LSKiR9lIcn1sCo="; //Chave de Segredo decriptado : "w8~QR1fu5.wVQ1XIKZ.GO.A2-gvPGq4w-Z";
			Scopes = "https://graph.microsoft.com/.default";
			Grant_Type = "client_credentials";
			//Resoruce = "https://tecnuntecnologia.sharepoint.com";
			TenanId = "tecnun.com.br";
		}

		public ApiCredential(string appId, string appSecret)
			: this()
		{
			AppId = appId;
			setSecret(appSecret);
			Grant_Type = "client_credentials";
		}

		public ApiCredential(string appId, string userId, string password)
			: this()
		{
			CriarCredencialUsuario(appId, userId, password);
		}

		public static ApiCredential CriarCredencialAplicativo(string appId, string appSecret)
		{
			ApiCredential apiCredential = new ApiCredential();
			apiCredential.AppId = appId;
			apiCredential.Grant_Type = "client_credentials";
			apiCredential.setSecret(appSecret);
			return apiCredential;
		}

		public static ApiCredential CriarCredencialUsuario(string appId, string userId, string password)
		{
			ApiCredential apiCredential = new ApiCredential();
			apiCredential.AppId = appId;
			apiCredential.Username = userId;
			apiCredential.Grant_Type = "password";
			apiCredential.setSecret(password);
			return apiCredential;
		}

		public void setSecret(string secret)
		{
			EncryptSecret = SecurityUtil.EncryptString(secret);
		}

		public string getSecret()
		{
			return SecurityUtil.DecryptString(EncryptSecret);
		}

		public override string ToString()
		{
			return "Tipo : " + Grant_Type + " / ID : " + AppId;
		}

		private FormUrlEncodedContent ConvertToFormBase(string parameters)
		{
			List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
			foreach (string item in parameters.Split("&".ToCharArray()).ToList())
			{
				string[] array = item.Split("=".ToCharArray());
				list.Add(new KeyValuePair<string, string>(array[0], array[1]));
			}
			return new FormUrlEncodedContent(list);
		}

		public object Clone()
		{
			return MemberwiseClone();
		}
	}
}
