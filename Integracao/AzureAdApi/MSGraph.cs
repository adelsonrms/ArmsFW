using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using ArmsFW.Lib.Web.HttpRest;
using ArmsFW.Lib.Web.Json;
using Newtonsoft.Json.Linq;

namespace ArmsFW.Services.Azure
{
	public class MSGraph
	{
		private readonly string MSGraphEndPointBeta = "https://graph.microsoft.com/beta";
		private readonly string MSGraphEndPointV1 = "https://graph.microsoft.com/v1.0";

		private AccessToken _accessToken;

		private readonly AzureApiService _apiService;

		private ApiCredential AppClient { get; set; }

		private AccessToken AccessToken
		{
			get
			{
				if (_accessToken == null)
				{
					_accessToken = new AccessToken();
				}
				return _accessToken;
			}
			set
			{
				_accessToken = value;
			}
		}

		public string ODataQuery { get; set; }

		public string UserId { get; set; }

		public string EndPoint { get; set; }

		public MSGraph()
		{
            if (AppClient!=null)
            {
				_apiService = AzureApiService.Create(AppClient);
			}
		}

		public MSGraph(string userId):this()
		{
			UserId = userId;
		}

		public MSGraph(ApiCredential appClient) : this()
		{
			AppClient = appClient;
			_accessToken = new AccessToken();
			_apiService = AzureApiService.Create(AppClient);
		}

		public MSGraph(ApiCredential appClient, string userId) : this()
		{
			UserId = userId;
			AppClient = appClient;
			_accessToken = new AccessToken();
			_apiService = AzureApiService.Create(AppClient);
		}

		public MSGraph(AccessToken accessToken) : this()
		{
			AccessToken = accessToken;
		}

		public async Task<ResponseOld> CreateUser(GraphUser user) 
		{
			ResponseOld ResultResponse = new ResponseOld();
			try
			{
				AccessToken = await _apiService.RequisitarAccessToken(AppClient, newToken: true);

				ResultResponse = await ExecuteLocalRequest();

				if (ResultResponse.Status == HttpStatusCode.Created)
				{
					dynamic @object = JSON.Carregar<dynamic>(ResultResponse.ContentAsString());// ResultResponse.Data.Object;
				}
				else if (ResultResponse.Status == HttpStatusCode.Unauthorized)
				{
					AccessToken = await _apiService.RequisitarAccessToken(AppClient);
					ResultResponse = await ExecuteLocalRequest();
				}else
				{
					return ResultResponse;
				}
			}
			catch
			{
			}
			
			return ResultResponse;

			async Task<ResponseOld> ExecuteLocalRequest()
			{
				EndPoint = string.Empty;

				RequestInfo request = new RequestInfo("POST");
				request.ContentBody = user.ToJson();
				request.BearerToken = AccessToken.access_token;
				request.ContentType = "application/json";

				 AzureApiResponse<User> response = await AzureApiService.EnviarRequest<User>(getUserEndPoint(), request);
				
				return response.Response;
			}

		}

		//

		public async Task<ResponseOld> AddManager(string managerUserId)
		{
			ResponseOld ResultResponse = new ResponseOld();
			try
			{
				//Recupera o token de autorização
				AccessToken = await _apiService.RequisitarAccessToken(AppClient, newToken: true);

				//Prepara e executa o request
				ResultResponse = await ExecuteRequest();

				//Retorno esperado. OK (200) sem nada no conteudo
				if (ResultResponse.Status == HttpStatusCode.OK && string.IsNullOrEmpty(ResultResponse.Conteudo))
				{
					return ResultResponse;
				}
				//Nao autorizado. Tenta  novamente com um novo Token
				else if (ResultResponse.Status == HttpStatusCode.Unauthorized)
				{
					AccessToken = await _apiService.RequisitarAccessToken(AppClient);
					ResultResponse = await ExecuteRequest();
				}
				else
				{
					return ResultResponse;
				}
			}
			catch
			{
			}

			return ResultResponse;

			//Envia o Request
			async Task<ResponseOld> ExecuteRequest()
			{
				EndPoint = string.Empty;

				RequestInfo request = new RequestInfo("PUT");
				request.ContentBody = $"{{\"@odata.id\": \"https://graph.microsoft.com/v1.0/users/{managerUserId}\"}}";
				request.BearerToken = AccessToken.access_token;
				

				AzureApiResponse<User> response = await AzureApiService.EnviarRequest<User>($"{getUserEndPoint()}/manager/$ref", request);

				return response.Response;
			}

		}

		public async Task<ResponseOld> UpdatePhoto(dynamic base64Foto)
		{
			ResponseOld ResultResponse = new ResponseOld();
			try
			{
				//Recupera o token de autorização
				AccessToken = await _apiService.RequisitarAccessToken(AppClient, newToken: true);

				//Prepara e executa o request
				ResultResponse = await ExecuteRequest();

				//Retorno esperado. OK (200) sem nada no conteudo
				if (ResultResponse.Status == HttpStatusCode.OK && string.IsNullOrEmpty(ResultResponse.Conteudo))
				{
					return ResultResponse;
				}
				//Nao autorizado. Tenta  novamente com um novo Token
				else if (ResultResponse.Status == HttpStatusCode.Unauthorized)
				{
					AccessToken = await _apiService.RequisitarAccessToken(AppClient);
					ResultResponse = await ExecuteRequest();
				}
				else
				{
					return ResultResponse;
				}
			}
			catch
			{
			}

			return ResultResponse;

			async Task<ResponseOld> ExecuteRequest()
			{
				EndPoint = string.Empty;

				RequestInfo request = new RequestInfo("PUT");
				request.ContentBody = base64Foto.ToString();
				request.BearerToken = AccessToken.access_token;
				request.ContentType = "image/jpeg";

				AzureApiResponse<User> response = await AzureApiService.EnviarRequest<User>($"{getUserEndPoint()}/photo/$value", request);

				return response.Response;
			}

		}

		public async Task<ResponseOld> UpdateUser(GraphUser user)
		{
			ResponseOld ResultResponse = new ResponseOld();
			try
			{
				AccessToken = await _apiService.RequisitarAccessToken(AppClient, newToken: true);

				ResultResponse = await ExecuteLocalRequest();

				if (ResultResponse.Status == HttpStatusCode.NoContent)
				{
					ResultResponse = await GetUser<User>();

					//ResultResponse.SetDataObject(u);
				}
				else if (ResultResponse.Status == HttpStatusCode.Unauthorized)
				{
					AccessToken = await _apiService.RequisitarAccessToken(AppClient);
					ResultResponse = await ExecuteLocalRequest();
				}
				else
				{
					return ResultResponse;
				}
			}
			catch (Exception ex)
			{
				ResultResponse.Conteudo = $"UpdateUser() - Erro desconhecido ao processar a requisição. {ex.Message}";
			}

			return ResultResponse;

			async Task<ResponseOld> ExecuteLocalRequest()
			{
				EndPoint = string.Empty;

				RequestInfo request = new RequestInfo("PATCH");
				request.ContentBody = user.ToJson();
				request.BearerToken = AccessToken.access_token;
				request.ContentType = "application/json";

				AzureApiResponse<User> response = await AzureApiService.EnviarRequest<User>(getUserEndPoint(), request);

				return response.Response;
			}
		}
		public async Task<ResponseOld> GetUser<TUser>() where TUser : new()
		{
			new TUser();
			ResponseOld ResultResponse = new ResponseOld();
			try
			{
				AccessToken = await _apiService.RequisitarAccessToken(AppClient, newToken: false);
				ResultResponse = await ExecuteLocalRequest();
				if (ResultResponse.Status == HttpStatusCode.OK)
				{
					dynamic @object = JSON.Carregar<dynamic>(ResultResponse.ContentAsString());
					_ = (TUser)@object;
				}
				else
				{
					if (ResultResponse.Status != HttpStatusCode.Unauthorized)
					{
						return ResultResponse;
					}
					AccessToken = await _apiService.RequisitarAccessToken(AppClient);
					ResultResponse = await ExecuteLocalRequest();
				}
			}
			catch (Exception ex)
			{
				ResultResponse.Conteudo = $"GetUser() - Erro desconhecido ao processar a requisição. {ex.Message}";
			}

			return ResultResponse;
			async Task<ResponseOld> ExecuteLocalRequest()
			{
				EndPoint = string.Empty;
				using RestClient client = new RestClient(getUserEndPoint());
				client.SetTokenAuthorization(AccessToken.access_token);

				var response = ArmsFW.HttpRest.WebClient.GetAsync(getUserEndPoint(), AccessToken.access_token);
				return await client.GetAsync<TUser>();
			}
		}

		public async Task<ResponseOld> GetUsers<TUser>() where TUser : new()
		{
			new TUser();
			ResponseOld ResultResponse = new ResponseOld();
			try
			{
				AccessToken = _apiService.RequisitarAccessToken(AppClient, newToken: false).Result;
				
				ResultResponse = await ExecuteLocalRequest();
				
				if (ResultResponse.Status == HttpStatusCode.OK)
				{
					ResultResponse = await TrataResponse();
				}
				else
				{
					if (ResultResponse.Status != HttpStatusCode.Unauthorized)
					{
						return ResultResponse;
					}

					AccessToken = _apiService.RequisitarAccessToken(AppClient).Result;
					ResultResponse = await ExecuteLocalRequest();
					ResultResponse = await TrataResponse();
				}
			}
			catch
			{
			}

			return ResultResponse;


			async Task<ResponseOld> ExecuteLocalRequest()
			{
				EndPoint = string.Empty;
				using RestClient client = new RestClient(MSGraphEndPointBeta + "/users/" + ODataQuery);
				client.SetTokenAuthorization(AccessToken.access_token);
				return await client.GetAsync<ODataCollection>();
			}


			async Task<ResponseOld> TrataResponse()
			{
				JObject val = (JObject)JSON.JsonToObject<object>(ResultResponse.HttpResponse.Content);
				//ResultResponse.Data.Object = val.GetValue("value").ToObject<List<TUser>>();
				return await Task.FromResult(ResultResponse);
			}
		}

		public async Task<ResponseOld> GetUsersDeleted<TUser>() where TUser : new()
		{
			new TUser();
			ResponseOld ResultResponse = new ResponseOld();
			try
			{
				AccessToken = await _apiService.RequisitarAccessToken(AppClient, newToken: false);
				ResultResponse = await ExecuteLocalRequest();
				if (ResultResponse.Status == HttpStatusCode.OK)
				{
					ResultResponse = await TrataResponse();
				}
				else
				{
					if (ResultResponse.Status != HttpStatusCode.Unauthorized)
					{
						return ResultResponse;
					}
					AccessToken = await _apiService.RequisitarAccessToken(AppClient);
					ResultResponse = await ExecuteLocalRequest();
					ResultResponse = await TrataResponse();
				}
			}
			catch
			{
			}
			return ResultResponse;
			async Task<ResponseOld> ExecuteLocalRequest()
			{
				EndPoint = MSGraphEndPointV1 + "/directory/deletedItems/microsoft.graph.user";
				using RestClient client = new RestClient(EndPoint);
				client.SetTokenAuthorization(AccessToken.access_token);
				return await client.GetAsync<ODataCollection>();
			}
			async Task<ResponseOld> TrataResponse()
			{
				JObject val = (JObject)JSON.JsonToObject<object>(ResultResponse.HttpResponse.Content);
				ResultResponse.SetDataObject(val.GetValue("value").ToObject<List<TUser>>());
				return await Task.FromResult(ResultResponse);
			}
		}

		public async Task<ResponseOld> GetMessages<TMessage>() where TMessage : new()
		{
			new List<TMessage>();
			ResponseOld ResultResponse = new ResponseOld();
			try
			{
				AccessToken = await _apiService.RequisitarAccessToken(AppClient, newToken: false);
				ResultResponse = await ExecuteLocalRequest();
				if (ResultResponse.Status == HttpStatusCode.OK)
				{
					dynamic val = JSON.JsonToObject<TMessage>(ResultResponse.HttpResponse.Content);
					ResultResponse.SetDataObject(val);
				}
				else
				{
					if (ResultResponse.Status != HttpStatusCode.Unauthorized)
					{
						return ResultResponse;
					}
					AccessToken = await _apiService.RequisitarAccessToken(AppClient);
					ResultResponse = await ExecuteLocalRequest();
					dynamic val2 = JSON.JsonToObject<object>(ResultResponse.HttpResponse.Content);
					ResultResponse.SetDataObject(val2);
				}
			}
			catch
			{
			}
			return ResultResponse;
			async Task<ResponseOld> ExecuteLocalRequest()
			{
				EndPoint = (string.IsNullOrEmpty(EndPoint) ? (getUserEndPoint() + "/mailFolders/inbox/messages" + ODataQuery) : EndPoint);
				using RestClient client = new RestClient(EndPoint);
				client.SetTokenAuthorization(AccessToken.access_token);
				return await client.GetAsync<TMessage>();
			}
		}

		public async Task<ResponseOld> GetPhoto<TPhoto>(string tamanho) where TPhoto : new()
		{
			new ResponseOld();
			AccessToken = await _apiService.RequisitarAccessToken(AppClient);
			return await ExecuteLocalRequest();
			async Task<ResponseOld> ExecuteLocalRequest()
			{
				//EndPoint. Nao utilizar o beta
				EndPoint = getUserEndPoint(false) + "/photos/" + (tamanho ?? "48x48") + "/$value";

				using RestClient client = new RestClient(EndPoint);
				client.SetTokenAuthorization(AccessToken.access_token);
				return await client.GetAsync<TPhoto>();
			}
		}

		private string getUserEndPoint(bool beta = true)
		{
			AppClient = AppClient ?? new ApiCredential();
			return (beta? MSGraphEndPointBeta : MSGraphEndPointV1) + "/users/" + UserId;
		}

		public AccessToken GetToken()
		{
			return _apiService.RequisitarAccessToken(AppClient).Result;
		}

		public AccessToken GetTokenByPassword(string password)
		{
			try
			{
				ApiCredential apiCredential = AppClient.Clone() as ApiCredential;
				apiCredential.Grant_Type = "password";
				apiCredential.Password = password;
				apiCredential.Username = UserId;
				return _apiService.RequisitarAccessToken(apiCredential).Result;
			}
			catch
			{
				return new AccessToken();
			}
		}
	}
}
