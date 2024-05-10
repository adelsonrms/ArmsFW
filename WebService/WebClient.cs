using ArmsFW.Lib.Web.HttpRest;
using ArmsFW.Lib.Web.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ArmsFW.HttpRest
{
    public class WebClient : IDisposable
    {
        public string EndPoint { get; set; }

        public HttpClient client;

        #region Parametros da Requisicao

        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> QueryString { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> BodyPayload { get; set; } = new Dictionary<string, string>();

        public void AdicionarParametro(string name, string value, ParameterType type)
        {
            if (type == ParameterType.HttpHeader && !string.IsNullOrEmpty(value))
            {
                Headers.Add(name, value);
            }
            if (type == ParameterType.RequestBody && !string.IsNullOrEmpty(value))
            {
                BodyPayload.Add(name, value);
            }
            if (type == ParameterType.QueryString && !string.IsNullOrEmpty(value))
            {
                QueryString.Add(name, value);
            }
        }
        #endregion

        #region Inicialização do Client
        public WebClient() => InicializaHttpClient("");
        public WebClient(string endPoint) => InicializaHttpClient(endPoint);
        public static WebClient CriarWebClient(string endPoint) => new WebClient(endPoint);
        private WebClient InicializaHttpClient(string endPoint)
        {
            if (!string.IsNullOrEmpty(endPoint)) EndPoint = Uri.UnescapeDataString(endPoint);
            client = new System.Net.Http.HttpClient();
            return this;
        }
        #endregion

        #region Execução generica da requisição
        private async Task<Response> EnviarRequest(string method, string contentType = "") => await EnviarRequest<Response>(method, contentType);
        private async Task<Response> EnviarRequest<TDataResponse>(string method, string contentType = "")
        {
            HttpResponseMessage response = new HttpResponseMessage();
            Response _responseOutput = new Response();

            try
            {
                HttpRequestMessage request = new HttpRequestMessage(method: GetMethod(), requestUri: this.EndPoint) { };

                #region Define os parametros para cabeçalhos, query string ou body
                ConfigurarHeaders(request, this.Headers, ParameterType.HttpHeader);
                ConfigurarHeaders(request, this.QueryString, ParameterType.QueryString);
                ConfigurarHeaders(request, this.BodyPayload, ParameterType.RequestBody);
                #endregion

                //Sobescreve o content-type padrao
                if (!string.IsNullOrEmpty(contentType)) request.Content.Headers.Add("Content-Type", contentType);

                //**** Metodo Resposavel por enviar o Request Assincrono
                response = await client.SendAsync(request);

                _responseOutput = await TratarResponse(response);

                return _responseOutput;

                HttpMethod GetMethod()
                {
                    switch (method)
                    {
                        case "POST": return HttpMethod.Post;
                        case "DELETE": return HttpMethod.Delete;
                        case "PUT": return HttpMethod.Put;
                        case "PATCH": return HttpMethod.Patch;
                        case "HEAD": return HttpMethod.Head;
                        default:
                        case "GET": return HttpMethod.Get;
                    }

                }
            }
            catch (Exception ex)
            {
                _responseOutput = new Response
                {
                    Status = HttpStatusCode.BadRequest,Conteudo = ex.Message
                };

                return await Task.FromResult(_responseOutput);
            }
        }

        #region CRUD - HTTP (GET, POST, PUT, PACTH, DELETE)

        public async Task<Response> GetAsync<TResponse>()
        {
            return await EnviarRequest<Response>("GET");
        }

        public async Task<Response> GetAsync()
        {
            return await GetAsync<object>();
        }

        public async Task<Response> GetRequestAsync(string url, string token = "")
        {
            EndPoint = EndPoint ?? url;
            AdicionarParametro("Authorization", "Bearer " + token, ParameterType.HttpHeader);
            return await GetAsync<object>();
        }

        public async Task<Response> PostRequestAsync<TResponse>(string url, string token = "")
        {
            EndPoint = EndPoint ?? url;
            AdicionarParametro("Authorization", "Bearer " + token, ParameterType.HttpHeader);
            return await EnviarRequest<TResponse>("POST");
        }

        public async Task<Response> PutRequestAsync<TResponse>(string url, string token = "")
        {
            EndPoint = EndPoint ?? url;
            AdicionarParametro("Authorization", "Bearer " + token, ParameterType.HttpHeader);
            return await EnviarRequest<TResponse>("PUT");
        }

        public async Task<Response> PatchAsync<TResponse>(string url, string token = "")
        {
            EndPoint = EndPoint ?? url;
            AdicionarParametro("Authorization", "Bearer " + token, ParameterType.HttpHeader);
            return await EnviarRequest<TResponse>("PATCH");
        }

        public async Task<Response> DeleteRequestAsync<TResponse>(string url, string token = "")
        {
            EndPoint = EndPoint ?? url;
            AdicionarParametro("Authorization", "Bearer " + token, ParameterType.HttpHeader);
            return await EnviarRequest<TResponse>("DELETE");
        }

        private async Task<Response> TratarResponse(HttpResponseMessage response)
        {
            try
            {
                string HttpContent = await response.Content.ReadAsStringAsync();

                var ResultResponse = new Response
                {
                    Conteudo = HttpContent.ToString(),
                    Status = response.StatusCode,
                    Url = response.RequestMessage.RequestUri.ToString()
                };

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
        #endregion

        #endregion

        #region Chamadas Static

        public static async Task<Response> GetAsync(string url, string token = "") => await CriarWebClient(url).GetRequestAsync(url, token);
        public static async Task<Response> PostAsync<TResponse>(string url, string token = "") => await CriarWebClient(url).PostRequestAsync<TResponse>(url, token);

        public static async Task<Response> PostAsync(string url, string payload = "", string token = "")
        {
            var _client = CriarWebClient(url);
            _client.AdicionarParametro("body", payload, ParameterType.RequestBody);
            _client.AdicionarParametro("Authorization", "Bearer " + token, ParameterType.HttpHeader);
            return await _client.EnviarRequest("POST");
        }

        public static async Task<Response> PutAsync(string url, string payload = "", string token = "")
        {
            var _client = CriarWebClient(url);
            _client.AdicionarParametro("body", payload, ParameterType.RequestBody);
            _client.AdicionarParametro("Authorization", "Bearer " + token, ParameterType.HttpHeader);

            return await _client.EnviarRequest("PUT");
        }
        public static async Task<Response> DeleteAsync<TResponse>(string url, string token = "") => await CriarWebClient(url).DeleteRequestAsync<TResponse>(url, token);

        public static async Task<Response> EnviarRequestAsync(string endPoint, string method, string bodyContent, string contentType = "")
        {
            var _client = CriarWebClient(endPoint);
            _client.AdicionarParametro("body", bodyContent, ParameterType.RequestBody);
            return await _client.EnviarRequest(method);
        }
        public static async Task<Response> EnviarRequestAsync<TResponse>(string endPoint, string method, string contentType = "") => await CriarWebClient(endPoint).EnviarRequest<TResponse>(method);
        public static async Task<Response> EnviarRequestAsync(string endPoint, string method, string contentType = "") => await CriarWebClient(endPoint).EnviarRequest(method);
        
        #endregion

        private void ConfigurarHeaders(dynamic req, Dictionary<string, string> parameters, ParameterType type = ParameterType.QueryString)
        {
            var _req = req as HttpRequestMessage;

            if (parameters != null && req != null)
            {
                foreach (KeyValuePair<string, string> par in parameters)
                {
                    switch (type)
                    {
                        case ParameterType.Cookie:
                            break;
                        case ParameterType.GetOrPost:
                            break;
                        case ParameterType.UrlSegment:
                            break;
                        case ParameterType.HttpHeader:
                            _req.Headers.Add(par.Key, par.Value);
                            break;
                        case ParameterType.RequestBody:

                            if (!string.IsNullOrEmpty(par.Value))
                            {
                                //Representação String de Json
                                if (par.Value.StartsWith("{") && par.Value.EndsWith("}"))
                                {
                                    _req.Content = new StringContent(par.Value, Encoding.UTF8, "application/json");
                                }
                                else if (par.Value.StartsWith("data:"))
                                {
                                    _req.Content = new StreamContent(new MemoryStream(Convert.FromBase64String(par.Value.Split(",".ToCharArray())[1])));
                                    _req.Content.Headers.Add("Content-Type", "image/jpeg");
                                }
                                else
                                {
                                    _req.Content = par.Value.ToEncodedContent();
                                }
                            }

                            break;
                        case ParameterType.QueryString:
                            break;
                        case ParameterType.QueryStringWithoutEncode:
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

    public class Response
    {
        public HttpStatusCode Status { get; set; }
        public string Conteudo { get; set; }
        public string Url { get; set; }
        public override string ToString() => $"Status ! {Status} | Retorno : {Conteudo?.Substring(0, 30)}";
        public string Mensagem
        {
            get
            {
                if (Sucesso)
                {
                    return $"Solicitação realizada com sucesso ";
                }
                else
                {
                    return $"Falha desconhecida ({Status}) na solicitação. Detalhe : {Conteudo}";
                }
            }
        }

        public bool Sucesso => Status == HttpStatusCode.OK || Status == HttpStatusCode.Created;
    }
}
