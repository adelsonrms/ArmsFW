using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ArmsFW.Lib.Web.HttpRest;
using ArmsFW.Lib.Web.Json;
using ArmsFW.Services.Shared;
using Newtonsoft.Json.Linq;

namespace ArmsFW.Services.Azure
{
	public class Office365UserService
	{
		private MSGraph _graphService;

		private readonly string _email;

		private readonly ApiCredential _appClient;

		private readonly List<AssignedLicence> _licencas;

		


		public Office365UserService()
		{
			_licencas = CarregarLicencas();
			_appClient = new ApiCredential();
			_graphService = new MSGraph(_appClient);
		}

		public Office365UserService(ApiCredential appClient, string email):this()
		{
			_email = email;
			_appClient = appClient;
			_graphService = new MSGraph(_appClient, _email);
		}

		public Office365UserService(string email) : this(new ApiCredential(), email)
		{
		}

		public ApiCredential AppClient => _appClient;
		public async Task<OfficeUser> GetOfficeUser(string email, bool getPhoto, bool checkMail)
		{
			OfficeUser officeUser = new OfficeUser();
			int countErro2 = 0;
			try
			{
				string mensagem = "";
				
				User user = (officeUser.User = await GetUser(email));

				if (user.IsValid())
				{
					mensagem += "User - Perfil recuperado com sucesso\n";

					if (getPhoto)
					{
						string text = await GetPhoto(email, "48x48");
						if (text.Contains("Error"))
						{
							text = string.Empty;
						}
						if (!string.IsNullOrEmpty(text))
						{
							mensagem += " | Photo - foto do perfil encontrada\n";
						}
						else
						{
							countErro2++;
						}

						user.Photo = text;

						officeUser.Photo = text;
					}

					if (checkMail)
					{
						var cont = await GetCountMessages(email);
						officeUser.QtdMensagens = cont;
						mensagem += $" | Messages - Total de Mensagens nao lidas : {cont}\n";
					}

				}
				else
				{
					countErro2++;
					mensagem += $"Usuário '{email}' não existe";
				}

				officeUser.Success = user.IsValid();
				officeUser.ValidationMessage = mensagem ?? "";

				return officeUser;
			}
			catch (Exception ex)
			{
				officeUser.Success = false;
				officeUser.ValidationDetail = new
				{
					error = ex.Message
				};
				officeUser.ValidationMessage = "Não foi possivel recuperar informações sobre o perfil do usuário";
				throw;
			}
		}

		public async Task<List<User>> GetUsers()
		{
			var request = await _graphService.GetUsers<User>();

            if (request.Success)
            {
				//Com informacoes adicionais para cada usuario
				var users = JSON.Carregar<dynamic>(request.ContentAsString()) as List<User>;

				users.ForEach(u =>
				{
					if (u.AssignedLicenses?.Count > 0)
					{
						u.AssignedLicenses.ForEach(lic => lic.Nome = ObtemNomeDaLicenca(lic.SkuId));
					};
				});

				return users;
			}
			return new List<User>();
		}

		public async Task<List<User>> GetUsersDeleted()
		{
			

			dynamic @object = JSON.Carregar<User>((await _graphService.GetUsersDeleted<User>()).ContentAsString()) as User; ;
			if (@object != null)
			{
				return @object;
			}
			return new List<User>();
		}

		public async Task<User> GetUser(string userId)
		{
			_graphService.UserId = userId;

			var request = await _graphService.GetUser<User>();

			if (request.Success)
			{
				//Com informacoes adicionais para cada usuario
				var usr = JSON.Carregar<dynamic>(request.ContentAsString()) as User;

				if (usr.AssignedLicenses?.Count > 0)
				{
					usr.AssignedLicenses.ForEach(lic => lic.Nome = ObtemNomeDaLicenca(lic.SkuId));
				};

				return usr;
			}
			return new User();
		}

		public async Task<User> GetUser()
		{
			return await GetUser(_email);
		}



		/// <summary>
		/// Cria uma nova conta, caso ainda não exista, Se for informado um ID, atualiza a conta
		/// </summary>
		/// <param name="user"></param>
		/// <returns>retornar uma Result<User> contendo as informações da cbonta criada</returns>
		public async Task<Result<User>> CreateOrUpdateUser(string userId, GraphUser user)
		{
			ResponseOld response;
			//Atualiza uma conta existente
			_graphService.UserId = userId;
			
			try
			{
				if (!string.IsNullOrEmpty(userId))
				{
					response = await _graphService.UpdateUser(user);
				}
				else
				{
					//Cria uma nova
					response = await _graphService.CreateUser(user);
				}

				if (response.Success)
				{
					//Objeto do usuario criado.
					var usr = JSON.Carregar<User>(response.ContentAsString());
					return ResultBase<User>.Sucesso(response.Description, usr);
				}
				else
				{
					var usr = await GetUser(userId);
					return ResultBase<User>.Erro($"{response.Conteudo}", usr);
				}
			}
            catch (Exception ex)
            {
				return ResultBase<User>.Erro($"Falha na criação/atualização da conta {ex.Message}", null);
            }
		}

		public async Task<Result<User>> CreateNewUser(dynamic user)
        {
			ResponseOld response = await _graphService.CreateUser(user);

            if (response.Success)
            {
				var usr = JSON.Carregar<User>(response.ContentAsString());
				return ResultBase<User>.Sucesso($"Conta '{usr?.UserPrincipalName}' criada com sucesso ! ID : {usr.Id}", usr);
            }
            else
            {
				return ResultBase<User>.Erro($"{response.Conteudo}", null);
			}

		}

		public async Task<Result<User>> UpdateUser(string userId, dynamic user)
		{
			_graphService.UserId = userId;

			ResponseOld response = await _graphService.UpdateUser(user);

			if (response.Success)
			{
				var usr = await GetUser(userId);

				return ResultBase<User>.Sucesso($"Conta de usuario atualizada com sucesso !", usr);
			}
			else
			{
				return ResultBase<User>.Erro($"{response.Conteudo}", new User());
			}

		}

		public async Task<Result<bool>> UpdateUserPhoto(string userId, string base64Foto)
		{
			_graphService.UserId = userId;

			ResponseOld response = await _graphService.UpdatePhoto(base64Foto);

			if (response.Success)
			{
				return ResultBase<bool>.Sucesso($"A nova imagem foi atualizada com sucesso");
			}
			else
			{
				return ResultBase<bool>.Erro($"Falha na atualização da foto da conta. Detalhe : {response.Conteudo}");
			}
		}

		public async Task<Result<bool>> AddManager(string userId, string managerUserId)
		{
			_graphService.UserId = userId;

			ResponseOld response = await _graphService.AddManager(managerUserId);

			if (response.Success)
			{
				return ResultBase<bool>.Sucesso($"O gerente foi atribuido com sucesso ao usuario", true);
			}
			else
			{
				return ResultBase<bool>.Erro($"Falha na atualização do gerente. Detalhe : {response.Conteudo}", false);
			}
		}

		public async Task<List<Message>> GetMessages()
		{
			return await GetMessages(_email);
		}

		public async Task<List<Message>> GetMessages(string userId)
		{
			_graphService.UserId = userId;
			List<Message> list;
			try
			{
				list = new List<Message>();
				bool flag = true;
				string text = "&$filter=isRead eq false";
				string text2 = "";
				string text3 = "&$orderby=receivedDateTime desc";
				string text4 = "&$top=10";
				_graphService.ODataQuery = "?" + text3 + text + text2 + text4;
				_graphService.EndPoint = "";
				while (flag)
				{
					flag = await GetList();
				}
				return list;
			}
			catch
			{
				return null;
			}
			async Task<bool> GetList()
			{
				try
				{
					var o =  JSON.Carregar<User>((await _graphService.GetMessages<object>()).ContentAsString());
					dynamic @object = o;
					JObject val = ((@object is JObject) ? @object : null);
					if (val != null)
					{
						JToken obj2 = ((JToken)val).SelectToken("['value']");
						((IEnumerable<JToken>)((obj2 is JArray) ? obj2 : null)).ToList().ForEach(delegate(JToken m)
						{
							list.Add(m.ToObject<Message>());
						});
						JToken obj3 = ((JToken)val).SelectToken("['@odata.nextLink']");
						JValue val2 = (JValue)(object)((obj3 is JValue) ? obj3 : null);
						if (val2 != null)
						{
							_graphService.EndPoint = Uri.UnescapeDataString(((object)val2).ToString()).Replace("+", " ");
							return true;
						}
					}
					return false;
				}
				catch
				{
					return false;
				}
			}
		}

		public async Task<int> GetCountMessages(string userId)
		{
			_graphService.UserId = userId;
			int contagem = 0;
			try
			{
				_graphService.ODataQuery = "?&$filter=isRead eq false&$count=true";
				_graphService.EndPoint = "";

				try
				{
					var o = JSON.Carregar<object>((await _graphService.GetMessages<object>()).ContentAsString());
					dynamic @object = o;
					
					JObject odataMensagens = ((@object is JObject) ? @object : null);

					if (odataMensagens != null)
					{
						JToken proximoLink = odataMensagens.SelectToken("['@odata.count']");
						JValue link = (JValue)((proximoLink is JValue) ? proximoLink : null);
						contagem = Convert.ToInt32(link);
					}
				}
				catch
				{
				}

				return contagem;
			}
			catch
			{
				return 0;
			}
			
		}

		async Task<bool> ConsultarListaDeMensagens(List<Message> list)
		{
			try
			{
				var o = JSON.Carregar<object>((await _graphService.GetMessages<object>()).ContentAsString());
				dynamic @object = o;

				JObject odataMensagens = ((@object is JObject) ? @object : null);

				if (odataMensagens != null)
				{
					JToken valor = odataMensagens.SelectToken("['value']");

					((IEnumerable<JToken>)((valor is JArray) ? valor : null))
						.ToList()
						.ForEach(m =>
						{
							list.Add(m.ToObject<Message>());
						});


					JToken proximoLink = odataMensagens.SelectToken("['@odata.nextLink']");
					JValue link = (JValue)((proximoLink is JValue) ? proximoLink : null);
					
					if (link != null)
					{
						_graphService.EndPoint = Uri.UnescapeDataString(link.ToString()).Replace("+", " ");
						return true;
					}
				}
				return false;
			}
			catch
			{
				return false;
			}
		}

		public async Task<string> GetPhoto(string tamanho)
		{
			string photoBase64 = "";
			try
			{
				photoBase64 = await GetPhoto(_email, tamanho);
				return photoBase64;
			}
			catch
			{
				return photoBase64;
			}
		}

		public async Task<string> GetPhoto(string userId, string tamanho)
		{
			_graphService.UserId = userId;
			dynamic httpResponse = (await _graphService.GetPhoto<ProfilePhoto>(tamanho)).HttpResponse;
			
			if (httpResponse.StatusCode == HttpStatusCode.OK)
			{
				dynamic bytesImagem = httpResponse.RawBytes;

				if (httpResponse.ContentType.Contains("image"))
				{
					return $"data:{httpResponse.ContentType};base64,{Convert.ToBase64String(bytesImagem)}";
				}

				return httpResponse.Content.ReadAsStringAsync().Result;
			}

			if (!(httpResponse.ContentType.Contains("application/json") ? true : false))
			{
				return httpResponse.Content.ReadAsStringAsync().Result;
			}

			return httpResponse.Content;
		}

		public async Task<bool> ValidatePassword(string password)
		{
			return (await GetTokenByPassword(password)).IsValid();
		}

		public async Task<bool> ValidateClientCrendential()
		{
			return await Task.FromResult(!string.IsNullOrEmpty(GetToken()?.access_token));
		}

		public AccessToken GetToken()=> _graphService.GetToken();

		public async Task<AccessToken> GetTokenByPassword(string password)
		{
			return await Task.FromResult(_graphService.GetTokenByPassword(password));
		}

		public async Task<OfficeUser> ValidaLoginOffice365(string email, string password)
		{
			OfficeUser validation = new OfficeUser();
			try
			{
				new User();
				AccessToken accessToken = await GetTokenByPassword(password);
				if (!accessToken.IsValid())
				{
					validation.ValidationDetail = (object)accessToken.Response;
					validation.ValidationMessage = "Não foi possivel validar o usuário :'" + accessToken.ErrorMessage + "'";
					return validation;
				}
				User user2 = (validation.User = await GetUser());
				validation.Success = user2.IsValid();
				validation.ValidationMessage = $"Usuário '{user2.Id} - {user2.DisplayName}' do Office 365 Valido !";
				return validation;
			}
			catch (Exception ex)
			{
				validation.ValidationMessage = "Ocorreu um erro inesperado na validação do usuário no Diretório Azure Ad do Office 365. Detalhe : " + ex.Message;
				return validation;
			}
		}

		public async Task<OfficeUser> ValidarSenha(string userId, string password)
		{
			OfficeUser validation = new OfficeUser();
			try
			{
				AccessToken accessToken = await GetTokenByPassword(password);

				//Falha
				if (!accessToken.IsValid())
				{
					validation.ValidationDetail = (object)accessToken.Response;
					validation.ValidationMessage = "Não foi possivel validar o usuário :'" + accessToken.ErrorMessage + "'";
					return validation;
				}
				
				//Sucesso
				User user2 = (validation.User = await GetUser());
				validation.Success = user2.IsValid();
				validation.ValidationMessage = $"Usuário '{user2.Id} - {user2.DisplayName}' do Office 365 Valido !";
				return validation;
			}
			catch (Exception ex)
			{
				validation.ValidationMessage = "Ocorreu um erro inesperado na validação do usuário no Diretório Azure Ad do Office 365. Detalhe : " + ex.Message;
				return validation;
			}
		}

		public List<AssignedLicence> CarregarLicencas()
		{
			var file = Path.GetDirectoryName(GetType().Assembly.Location) + "\\officeLicencas.json";

			if (File.Exists(file))
			{
				var json = JSON.LoadFromFile<List<AssignedLicence>>(file);
				return json.Result;

			}
			return new List<AssignedLicence>();
		}

		public string ObtemNomeDaLicenca(string skuId)
		{
            if (_licencas==null)
            {
				return "";
            }

			return _licencas.FirstOrDefault(x => x.SkuId == skuId)?.Nome;
		}
	}
}
