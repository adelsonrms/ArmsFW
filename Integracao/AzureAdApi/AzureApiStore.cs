using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ArmsFW.Services.Azure;
using Newtonsoft.Json;

namespace ArmsFW.Services.Store
{
	public class AzureApiStore
	{
		public string FilePath { get; set; }

		public DateTime UpdateDateTime { get; set; }

		public AccessToken AccessToken { get; set; } = new AccessToken();


		public bool Loaded { get; private set; }

		public List<ApiCredential> Credentials { get; set; } = new List<ApiCredential>();


		public static AzureApiStore Default => new AzureApiStore().Load();

		public string EndPointAccessToken { get; set; }

		public string Resource { get; set; }

		public ApiCredential Credencial { get; set; }

		public string TenantID { get; set; }

		public void AtualizarToken(AccessToken token)
		{
			AccessToken = token;
			SalvarNoStore();
		}

		public AzureApiStore Load(string file = "")
		{
			try
			{
				if (string.IsNullOrEmpty(FilePath))
				{
					if (string.IsNullOrEmpty(file))
					{
						file = Path.GetDirectoryName(GetType().Assembly.Location) + "\\apiCredencialStore.json";
					}
					FilePath = file;
				}
				else
				{
					file = FilePath;
				}
				if (File.Exists(file))
				{
					string text = File.ReadAllText(file);
					if (!string.IsNullOrEmpty(text))
					{
						AzureApiStore azureApiStore = JsonConvert.DeserializeObject<AzureApiStore>(text);
						azureApiStore.UpdateDateTime = File.GetCreationTime(file);
						azureApiStore.Loaded = true;
						azureApiStore.FilePath = file;
						FilePath = file;
						return azureApiStore;
					}
					return this;
				}
				Credentials = new List<ApiCredential>();
				UpdateDateTime = File.GetCreationTime(file);
				Loaded = true;
				SalvarNoStore();
				return this;
			}
			catch
			{
				return this;
			}
		}

		public AzureApiStore RegistrarCredencial(ApiCredential ApiCredential)
		{
			List<ApiCredential> credentials = Credentials;


			if (!credentials.Any(cf => cf.AppKey == ApiCredential.AppKey))
			{
				this.Credentials.Add(ApiCredential);
				SalvarNoStore();
			}
			return this;
		}

		public ApiCredential ObterCredencialPorChave(string key)
		{
			ApiCredential apiCredential = Load().Credentials.FirstOrDefault((ApiCredential cf) => cf.AppKey == key);
			if (apiCredential == null)
			{
				return new ApiCredential();
			}
			return apiCredential;
		}

		public void RegistrarCredencial(string id, string secret)
		{
			RegistrarCredencial(new ApiCredential(id, secret));
		}

		private AzureApiStore SalvarNoStore()
		{
			AzureApiStore azureApiStore = this;
			try
			{
				string contents = JsonConvert.SerializeObject((object)azureApiStore);
				if (File.Exists(azureApiStore.FilePath))
				{
					File.Delete(azureApiStore.FilePath);
				}
				Directory.CreateDirectory(Path.GetDirectoryName(azureApiStore.FilePath));
				File.WriteAllText(azureApiStore.FilePath, contents);
				azureApiStore = Load(azureApiStore.FilePath);
				azureApiStore.FilePath = FilePath;
				return azureApiStore;
			}
			catch (Exception)
			{
				return azureApiStore;
			}
		}

		internal AzureApiStore InicializarAzureApiServices(AzureApiServiceOptions opt)
		{
			Credentials = new List<ApiCredential>();
			TenantID = opt.TenantID;
			EndPointAccessToken = opt.EndPointAccessToken;
			Resource = opt.Resource;
			Credencial = opt.Credencial;
			Credencial.TenanId = opt.TenantID;
			Credencial.Resoruce = opt.Resource;
			RegistrarCredencial(opt.Credencial);
			SalvarNoStore();
			return this;
		}
	}
}
