using ArmsFW.HttpRest;
using ArmsFW.Lib.Web.HttpRest;
using ArmsFW.Lib.Web.Json;
using ArmsFW.Services.Azure;
using ArmsFW.Services.Shared.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ArmsFW.Integracao.DevOps
{
    public class AzureDevOpsService
    {
		#region Funcionalidades

		public async Task<Response> CriarWorkItem(WorkItemPayloadRequet itemPayload)
		{
			Response ResultResponse = new Response();

			try
			{
                var url = MontarUriRest($"/{this.Settings.TeamProject.id}/_apis/wit/workitems/{("$" + itemPayload.Tipo)}");

                using (var http = new HttpClient())
				{
					//Autenticacao
					http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($":{AppClient.EncryptSecret}")));

                    //Envio do Request
                    var httpResponse = await http.PostAsync(
						url,
						content: new StringContent(itemPayload.Campos.ToJson(), Encoding.UTF8, "application/json-patch+json")
					);

					//Serializa o json retornado
					ResultResponse = await TratarResponse(httpResponse);

                    itemPayload.Response = JSON.JsonToObject<WorkItemPayloadResponse>(ResultResponse.Conteudo) as WorkItemPayloadResponse;
                }
			}
			catch
			{
			}

			return ResultResponse;
		}

        public async Task<Response> GravarLista(string idLista, params string[] items)
        {
            Response ResultResponse = new Response();

            try
            {
                Uri url = MontarUriRest($"/_apis/work/processes/lists/{idLista}");

                string payload = new { items = items }.ToJson();

                using (var http = IniciarRequisicao())
                {
                    //Envio do Request
                    var httpResponse = await http.PutAsync(
                        url,
                        content: new StringContent(payload, Encoding.UTF8, "application/json")
                    );

                    //Serializa o json retornado
                    ResultResponse = await TratarResponse(httpResponse);
                }
            }
            catch
            {
            }

            return ResultResponse;
        }

        public async Task<Response> ObterLista(string idLista)
        {
            Response ResultResponse = new Response();

            try
            {

                //Lista Exemplo: Clientes : https://dev.azure.com/tecnun/_apis/work/processes/lists/a9cff423-4110-4df0-9671-94bfd81e8ede
                Uri url = MontarUriRest($"/_apis/work/processes/lists/{idLista}");

                using (var http = IniciarRequisicao())
                {
                    //Envio do Request
                    var httpResponse = await http.GetAsync(url);

                    //Serializa o json retornado
                    ResultResponse = await TratarResponse(httpResponse);
                }
            }
            catch
            {
            }

            return ResultResponse;
        }

        private HttpClient IniciarRequisicao()
        {
            var http = new HttpClient();

            //Autenticacao
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($":{AppClient.EncryptSecret}")));

            return http;
        }

        #endregion

        #region Suporte
        private readonly ApiCredential AppClient;
        private AccessToken _accessToken;

        public DevOpsApiSettings Settings { get; }
        
        public string Url => $"https://dev.azure.com/{Settings.Organizacao}";

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

        public AzureDevOpsService()
        {
            AppClient = new ApiCredential();

            this.Settings = AppSettings.GetSection<DevOpsApiSettings>("DevOpsApiSettings");

            if (Settings==null)
            {
                return;
            }

            //Dados para autentica as apis
            AppClient.Username = this.Settings.Autenticacao.Usuario;
            AppClient.EncryptSecret = this.Settings.Autenticacao.Token;
        }

        public AzureDevOpsService(string organizacao) : this() => this.Settings.Organizacao = new IdNome(organizacao);

        private async Task<Response> TratarResponse(HttpResponseMessage response)
        {
            try
            {
                string HttpContent = await response.Content.ReadAsStringAsync();

                var ResultResponse = new Response
                {
                    Conteudo = HttpContent,
                    Status = response.StatusCode,
                };

                //if (HttpContent.IsJson()) ResultResponse.SetDataObject(JSON.JsonToObject<object>(HttpContent));

                return ResultResponse;
            }
            catch (Exception ex)
            {
                return new Response
                {
                    Conteudo = $"Falha na requisição REST. Detalhe : {ex.Message}",
                    Status = HttpStatusCode.BadRequest,
                };
            }
        }

        private string MontarUrl(string organizacao, string projeto, string tipo = "")
        {
            organizacao = organizacao ?? this.Settings.Organizacao.id;
            tipo = (!string.IsNullOrEmpty(tipo) ? "$" + tipo : "");

            string url = $"{this.Settings.DevOpsService}/{organizacao}/{projeto}/_apis/wit/workitems/{tipo}?api-version={this.Settings.Versao}";
            return url;

        }

        private Uri MontarUriRest(string complemento = "", string organizacao=null)
        {
            organizacao = organizacao ?? this.Settings.Organizacao.id;

            string url = $"{this.Settings.DevOpsService}/{organizacao}[complemento]?api-version={this.Settings.Versao}".Replace("[complemento]", complemento);
            return new Uri(url);
        }



        #endregion
    }

    public class ResponseList
    {
        public List<string> items { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public bool isSuggested { get; set; }
        public string url { get; set; }
    }

    public class WorkItemPayloadRequet
    {
        public WorkItemPayloadRequet()
        {
			Campos = new List<CampoPayload>();
            Response = new WorkItemPayloadResponse();
        }

        public List<CampoPayload> Campos { get; set; }
        public string Organizacao { get;  set; }
        public string Projeto { get;  set; }
        public string Tipo { get;  set; }
		public string ApiVersao { get;  set; } = "1.0";
        public WorkItemPayloadResponse Response { get; internal set; }
        public string code_request { get; set; }

        public void AdicionarCampo(string campo, string valor)
        {
            var lista = Campos ?? new List<CampoPayload>();

            lista.Add(new CampoPayload { Operacao = "add", Caminho = $"/fields/{campo}", Valor = valor });
        }

        public void AdicionarTitulo(string valor) => AdicionarCampo("System.Title", valor);
        public void AssociarAoUsuario(string valor) => AdicionarCampo("System.AssignedTo", valor);
        public void AdicionarComentario(string valor) => AdicionarCampo("System.History", valor);
    }

    public class AzureDevOpsApiResponse
    {
        public AzureDevOpsApiResponse()
        {
            Request = new WorkItemPayloadRequet();
            Response = new WorkItemPayloadResponse();
        }
        public WorkItemPayloadRequet Request { get; set; }
        public WorkItemPayloadResponse Response { get; set; }

        public bool Status { get; set; }
        public string Mensagem { get; set; }

    }
    public class CampoPayload
	{
		[JsonProperty("op")]
		[JsonPropertyName("op")]
		public string Operacao { get; set; }

		[JsonProperty("path")]
		[JsonPropertyName("path")]
		public string Caminho { get; set; }

		[JsonProperty("value")]
		[JsonPropertyName("value")]
		public string Valor { get; set; }
	}

    public class Autenticacao
    {
        public string Usuario { get; set; }
        public string Token { get; set; }
    }

    public class DevOpsApiSettings
    {
        public DevOpsApiSettings()
        {
            Organizacao = new IdNome();
            TeamProject = new IdNome();
            Processo = new IdNome();
            Autenticacao = new Autenticacao();
            Listas = new List<IdNome>();
            DevOpsService = "https://dev.azure.com";
        }
        public IdNome Organizacao { get; set; }
        public IdNome TeamProject { get; set; }
        public string Versao { get; set; }
        public IdNome Processo { get; set; }
        public string TipoWorkItem { get; set; }
        public Autenticacao Autenticacao { get; set; }
        public List<IdNome> Listas { get; set; }
        public string DevOpsService { get; set; }
    }

    public class IdNome
    {
        public override string ToString()
        {
            return this.id;
        }
        public IdNome()
        {

        }
        public IdNome(string _id)
        {
            this.id = _id;
        }

        public IdNome(string _id, string _nome)
        {
            this.id = _id;
            this.nome = _nome;
        }

        public string id { get; set; }
        public string nome { get; set; }
    }

}