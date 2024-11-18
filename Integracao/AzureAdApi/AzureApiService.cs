using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ArmsFW.Core.Types;
using ArmsFW.HttpRest;
using ArmsFW.Lib.Web.HttpRest;
using ArmsFW.Lib.Web.Json;
using ArmsFW.Services.Shared;
using ArmsFW.Services.Shared.Settings;
using ArmsFW.Services.Store;

namespace ArmsFW.Services.Azure
{
    public class AzureApiService : AzureApiServiceOptions
	{
		private readonly string C_MS_LOGIN_SERVICE = "https://login.microsoftonline.com/{0}/oauth2/token";

		private readonly string C_MS_DEFAULT_TENANT = "common";

		public List<ApiCredential> AppCredencias { get; set; } = new List<ApiCredential>();

		public AzureApiStore Store { get; set; }

		public AccessToken AccessToken { get; set; }

		public AzureApiService() => CarregarStore();

		public AzureApiService(ApiCredential appClient)
			: this()
		{
			base.Credencial = appClient;
			Store.RegistrarCredencial(base.Credencial);
		}

		public void RegistrarCredencialAplicativo(string appId, string appSecret, string resource = "")
		{
			ApiCredential apiCredential = ApiCredential.CriarCredencialAplicativo(appId, appSecret);
			apiCredential.Resoruce = (string.IsNullOrEmpty(resource) ? base.Resource : resource);
			apiCredential.TenanId = base.TenantID;
			Store.RegistrarCredencial(apiCredential);
			CarregarStore();
		}

		public void RegistrarCredencialUsuario(string appId, string usuario, string senha, string resource = "")
		{
			ApiCredential apiCredential = ApiCredential.CriarCredencialUsuario(appId, usuario, senha);
			apiCredential.Resoruce = (string.IsNullOrEmpty(resource) ? base.Resource : resource);
			apiCredential.TenanId = base.TenantID;
			Store.RegistrarCredencial(apiCredential);
			CarregarStore();
		}

		private void CarregarStore()
		{
			Store = AzureApiStore.Default;
			base.EndPointAccessToken = Store.EndPointAccessToken;
			base.Resource = Store.Resource;
			base.TenantID = Store.TenantID;
			AppCredencias = Store.Credentials;
			base.Credencial = Store.Credencial;
			if (string.IsNullOrEmpty(base.EndPointAccessToken))
			{
				base.EndPointAccessToken = C_MS_LOGIN_SERVICE;
			}
			if (string.IsNullOrEmpty(base.TenantID))
			{
				base.TenantID = C_MS_DEFAULT_TENANT;
			}
		}

        public ApiCredential ObtemCredencialPorUsuario(string userName)=> AppCredencias.FirstOrDefault(c => c.Username.Equals(userName)) ?? new ApiCredential();

		public ApiCredential ObtemCredencialPorTipo(string tipo) => AppCredencias.FirstOrDefault(c => c.Grant_Type.Equals(tipo)) ?? new ApiCredential();

		public ApiCredential ObtemCredencialId(string id) => AppCredencias.FirstOrDefault(c => c.AppId.Equals(id)) ?? new ApiCredential();

		internal static AzureApiService Create(ApiCredential appClient) => new AzureApiService(appClient);

		public async Task<AccessToken> RequisitarAccessToken(bool newToken = true) => await RequisitarAccessToken(base.Credencial, newToken);

		public async Task<AccessToken> RequisitarAccessToken(ApiCredential appClient, bool newToken = true)
		{
			AccessToken result = new AccessToken();
			if (!newToken)
			{
				result = Store.AccessToken;
				if (!string.IsNullOrEmpty(result.access_token))
				{
					return result;
				}
			}
			try
			{
				RequestInfo requestToken = new RequestInfo("POST");
				requestToken.ContentBody = appClient.QueryString;
				requestToken.ContentType = null;

				var retornoToken = new AzureApiServices().BuscarTokenDoUsuario(appClient.Username, appClient.Password);

				AzureApiResponse<AccessToken> response = await EnviarRequest<AccessToken>(GetAccessTokenUrl(appClient), requestToken);
				

				result.Response = response.Response;
				if (response.Response.Success)
				{
					string access_token = response.Response.Conteudo;

					result = JSON.Carregar<AccessToken>(access_token);
					Store.AtualizarToken(result);
				}
				else
				{
					result.ErrorMessage = response.ErrorRequest.error_description;
					result.Error = response.ErrorRequest;
				}
				return result;
			}
			catch (Exception ex)
			{
				result.ErrorMessage = "Falha inesperada na requisição do token : " + ex.Message;
			}
			return result;
		}

		private string GetAccessTokenUrl(ApiCredential appClient)
		{
			string text = (string.IsNullOrEmpty(appClient.TenanId) ? base.TenantID : appClient.TenanId);
			text = (string.IsNullOrEmpty(text) ? C_MS_DEFAULT_TENANT : text);
			return string.Format(base.EndPointAccessToken, text);
		}

        public static string ConsultaErro(AzureErrorRequest erroAzure)
        {
            if (erroAzure!=null)
            {
                if (!string.IsNullOrEmpty(erroAzure.error_description)) return AppSettings.GetSection<string>("AzureApiErrors:" + erroAzure.error_description.Split(":".ToCharArray())[0].ToString());
			}
			return "";
		}

        public static async Task<AzureApiResponse<TResponse>> EnviarRequest<TResponse>(string endPoint, RequestInfo requstOptions)
		{
			AzureApiResponse<TResponse> result = AzureApiResponse<TResponse>.CriarResponse();
			try
			{
				using HttpWebClient client = new HttpWebClient(endPoint);

				client.BodyStringParams.TryAdd("body", requstOptions.ContentBody);

                if (!string.IsNullOrEmpty(requstOptions.BearerToken))
                {
					client.HeadersParams.TryAdd("Authorization", "Bearer " + requstOptions.BearerToken);
				}

				AzureApiResponse<TResponse> azureApiResponse = result;

				azureApiResponse.Response = await client.SendRequestAsync<TResponse>(requstOptions.Method);

				ResponseOld response = result.Response;
				
				if (response.Success)
				{
					//result.addData(JSON.Carregar<TResponse>(response.ContentAsString()));
				}
				else
				{
					result.addErro(response.Conteudo);
				}
			}
			catch (Exception ex)
			{
				result.addExeption(ex.Message);
			}
			return await Task.FromResult(result);
		}

		public static AzureErrorRequest GetAzureErrorRequest(string error)
		{
            if (string.IsNullOrEmpty(error)) return null;

			var erroRequest = JSON.JsonToObject<AzureErrorRequest>(error.Replace("[", "").Replace("]", ""));
			erroRequest.Descricao = ConsultaErro(erroRequest);

			return erroRequest;
		}

		public static AzureApiService Registrar(Action<AzureApiServiceOptions> actionConfigure)
		{
			AzureApiServiceOptions azureApiServiceOptions = new AzureApiServiceOptions();
			actionConfigure(azureApiServiceOptions);
			AzureApiStore.Default.InicializarAzureApiServices(azureApiServiceOptions);
			return new AzureApiService();
		}
	}


	public class AzureApiServices
    {
		public async Task<OfficeUser> ValidarSenha(string userId, string password)
		{
			OfficeUser validation = new OfficeUser();
			try
			{
				AccessToken accessToken = await BuscarTokenDoUsuario(userId, password);

				if (!accessToken.IsValid())
				{
					validation.ValidationDetail = (object)accessToken.Response;
					validation.ValidationMessage = "Não foi possivel validar o usuário :'" + accessToken.ErrorMessage + "'";
					return validation;
				}
				var usr = await GetUser(userId, accessToken.access_token);
				validation.User = usr;

				validation.Success = validation.User.IsValid();
				validation.ValidationMessage = $"Usuário '{validation.User.Id} - {validation.User.DisplayName}' do Office 365 Valido !";
				return validation;
			}
			catch (Exception ex)
			{
				validation.ValidationMessage = "Ocorreu um erro inesperado na validação do usuário no Diretório Azure Ad do Office 365. Detalhe : " + ex.Message;
				return validation;
			}
		}

		public async Task<User> GetUser(string userId, string token)
        {
            if (string.IsNullOrEmpty(token))
            {
				AccessToken accessToken = await BuscarTokenDoUsuario(userId);
			}

			var response = await WebClient.GetAsync($"https://graph.microsoft.com/v1.0/users/{userId}", token: token);

			var result = JSON.Carregar<User>(response.Conteudo);

			return result;
		}

		public async Task<AccessToken> BuscarTokenDoUsuario(string userId, string password = null)
		{
			try
			{

				string tokenFile = $"\\tokenAzure_{userId}.json";


				if (string.IsNullOrEmpty(password))
                {
					return JSON.Carregar<AccessToken>(File.ReadAllText(tokenFile));
				}
                else
                {
					var queryString = $"";
					string text = "client_id=";
					text = text + "&grant_type=password";
					text = text + "&client_secret=";
					text = text + "&scope=https://graph.microsoft.com/.default";
					text = text + "&username=" + userId + "&password=" + password;

					var response = await WebClient.PostAsync($"https://login.microsoftonline.com/{this.TenantId}/oauth2/v2.0/token", payload: text);
					var token = response.Conteudo;

					File.WriteAllText(tokenFile, token);
					return JSON.Carregar<AccessToken>(token);
				}
				
			}
			catch
			{
				return new AccessToken();
			}
		}

		private string GetAccessTokenUrl(ApiCredential appClient)
		{
			return $"https://login.microsoftonline.com/{appClient.TenanId}/oauth2/token";
		}

		internal string QueryString
		{
			get
			{
				return "";
			}
			set
			{
			}
		}

		public FormUrlEncodedContent ContentBody => ConvertToFormBase(QueryString);

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

	}
}

