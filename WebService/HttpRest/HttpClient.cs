using ArmsFW.Lib.Web.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ArmsFW.Lib.Web.HttpRest
{
    public static class RestApiExtensions
    {
        public static FormUrlEncodedContent ToEncodedContent(this string parameters)
        {
            List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
            foreach (string item in parameters.Split("&".ToCharArray()).ToList())
            {
                string[] array = item.Split("=".ToCharArray());

                if (array.Length==2)
                {
                    list.Add(new KeyValuePair<string, string>(array[0], array[1]));
                }
                
            }

            return new FormUrlEncodedContent(list);
        }
    }

    public class HttpWebClient : HttpRestClientBase
    {
        private HttpClient client;

        #region Inicialização do Client
        public HttpWebClient(string endPoint) => StartClient(endPoint);
        public static HttpWebClient CreateNewClient(string endPoint) => new HttpWebClient(endPoint);
        private HttpWebClient StartClient(string endPoint)
        {
            base.EndPoint = Uri.UnescapeDataString(endPoint);
            client = new System.Net.Http.HttpClient();
            base.Client = client;
            return this;
        }
        #endregion

        public override void Dispose() => client.Dispose();
        
        #region Execução generica da requisição
        public override async Task<ResponseOld> SendRequestAsync(string method, string contentType = "") => await SendRequestAsync<string>(method, contentType);
        public override async Task<ResponseOld> SendRequestAsync<TDataResponse>(string method, string contentType = "")
        {
            HttpResponseMessage response = new HttpResponseMessage();
            ResponseOld _responseOutput = new ResponseOld();

            try
            {
                HttpRequestMessage request = new HttpRequestMessage(method: GetMethod(), requestUri: this.EndPoint) { };

                #region Define os parametros para cabeçalhos, query string ou body
                AddRequestParams(request, this.HeadersParams, ParameterType.HttpHeader);
                AddRequestParams(request, this.QueryStringParams, ParameterType.QueryString);
                AddRequestParams(request, this.BodyStringParams, ParameterType.RequestBody);
                #endregion

                //Sobescreve o content-type padrao
                if (!string.IsNullOrEmpty(contentType))
                {
                    request.Content.Headers.Add("Content-Type", contentType);
                }

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
            catch (Exception e)
            {
                _responseOutput = new ResponseOld
                {
                    Status = HttpStatusCode.NoContent,
                    Description = e.Message,
                    HttpResponse = response
                };

                _responseOutput.SetDataObject(e);

                return await Task.FromResult(_responseOutput);
            }
        }

        private async Task<ResponseOld> TratarResponse(HttpResponseMessage response)
        {
            try
            {
                string HttpContent = await response.Content.ReadAsStringAsync();

                var ResultResponse = new ResponseOld
                {
                    Success = response.IsSuccessStatusCode,
                    Conteudo = HttpContent,
                    Status = response.StatusCode,
                    Description = response.ReasonPhrase,
                    HttpResponse = response
                };

                if (HttpContent.IsJson()) ResultResponse.SetDataObject(JSON.JsonToObject<object>(HttpContent));

                return ResultResponse;
            }
            catch (Exception ex)
            {
                return new ResponseOld
                {
                    Success = false,
                    Conteudo = $"Falha na requisição REST. Detalhe : {ex.Message}",
                    Status = HttpStatusCode.BadRequest,
                    Description = ex.Source,
                    HttpResponse = response
                };
            }
        }

        #endregion

        #region Chamadas Static
        public static async Task<ResponseOld> SendRequestAsync(string endPoint, string method, string contentType = "") => await CreateNewClient(endPoint).SendRequestAsync(method);
        public static async Task<ResponseOld> SendRequestAsync(string endPoint, string method, string bodyContent, string contentType = "")
        {
            var _client = CreateNewClient(endPoint);
            _client.AddParameter("body",bodyContent, ParameterType.RequestBody);
            return await _client.SendRequestAsync(method);
        }
        public static async Task<ResponseOld> SendRequestAsync<TResponse>(string endPoint, string method, string contentType = "") => await CreateNewClient(endPoint).SendRequestAsync<TResponse>(method);
        #endregion


        protected override void AddRequestParams(dynamic req, Dictionary<string, string> parameters, ParameterType type = ParameterType.QueryString)
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
    }
}
