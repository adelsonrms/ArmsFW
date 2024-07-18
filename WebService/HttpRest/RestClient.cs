using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using ArmsFW.Lib.Web.Json;
using RestSharp;

namespace ArmsFW.Lib.Web.HttpRest
{
	public class RestClient : HttpRestClientBase
	{
		public RestClient(string endPoint)
		{
			StartClient(endPoint);
		}

		public static RestClient CreateNewClient(string endPoint)
		{
			return new RestClient(endPoint);
		}

		private RestClient StartClient(string endPoint)
		{
			base.EndPoint = Uri.UnescapeDataString(endPoint);
			base.Client = (object)new RestSharp.RestClient(endPoint);
			return this;
		}

		public override async Task<ResponseOld> SendRequestAsync<TResponse>(string method, string contentType = "")
		{
			new ResponseOld();
			
			try
			{
                //TODO: ARMS, 13/07 - Update Framework - Necessário usar #if condicionais para cada versão do netcore. Os malditos trocam as assinaturas e quebra tudo
                //https://stackoverflow.com/questions/1449925/is-it-possible-to-conditionally-compile-to-net-framework-version

                RestRequest req = new RestRequest(base.EndPoint, GetMethod());
                #if NET5_0
                                  RestResponse<TResponse> webResponse = new RestResponse<TResponse>(); //NET5 (sem construtor) 
                #else
                                RestResponse<TResponse> webResponse = new RestResponse<TResponse>(req); //NET7 (recebe um construtor obrigatorio)
                #endif

                //
                //foreach (KeyValuePair<string, string> parameter in base.HeadersParams)
                //{
                //	req.AddHeader(parameter.Key, parameter.Value);
                //}

                //foreach (KeyValuePair<string, string> parameter in base.BodyStringParams)
                //{
                //	req.AddBody(parameter.Key, parameter.Value);
                //}

                //RestSharp.ParameterType tp;

                //switch (type)
                //{
                //	case ParameterType.GetOrPost:
                //		tp = RestSharp.ParameterType.GetOrPost;
                //		break;
                //	case ParameterType.UrlSegment:
                //		tp = RestSharp.ParameterType.UrlSegment;
                //		break;
                //	case ParameterType.HttpHeader:
                //		tp = RestSharp.ParameterType.HttpHeader;
                //		break;
                //	case ParameterType.RequestBody:
                //		tp = RestSharp.ParameterType.RequestBody;
                //		break;
                //	case ParameterType.QueryString:
                //		tp = RestSharp.ParameterType.QueryString;
                //		break;
                //	default:
                //		tp = RestSharp.ParameterType.HttpHeader;
                //		break;
                //}

                //req.AddParameter(parameter.Key, parameter.Value, tp);


                AdicionaParametros(req, base.HeadersParams, ParameterType.HttpHeader);
				AdicionaParametros(req, base.QueryStringParams);
				AdicionaParametros(req, base.BodyStringParams, ParameterType.RequestBody);

				webResponse = base.Client.Execute<TResponse>(req);
				string content = webResponse.Content;
				
				ResponseOld resp = new ResponseOld();
				resp.Success = webResponse.IsSuccessful;
				resp.Conteudo = content;
				resp.Status = webResponse.StatusCode;
				resp.Description = webResponse.StatusDescription + " - " + webResponse.ErrorMessage;
				resp.HttpResponse = webResponse;

                if (content.IsJson())
                {
					resp.SetDataObject(JSON.JsonToObject<TResponse>(content));
                }
                else
                {
                    if (webResponse.ContentType.Contains("/jpeg") || webResponse.ContentType.Contains("/png") || webResponse.ContentType.Contains("/pdf"))
                        resp.SetDataObject(content); 

                }
  
				return await Task.FromResult(resp);
			}
			catch (Exception ex)
			{
				return await Task.FromResult(new ResponseOld
				{
					Status = HttpStatusCode.NoContent,
					Description = ex.Message
				});
			}
			Method GetMethod()
			{
				if (method == "GET")
				{
					return (Method)0;
				}
				return (Method)1;
			}
		}

		public override async Task<ResponseOld> SendRequestAsync(string method, string contentType = "")
		{
			return await SendRequestAsync<string>(method);
		}

		public static async Task<ResponseOld> SendRequestAsync(string url, string method, string contentType = "")
		{
			return await CreateNewClient(url).SendRequestAsync(method);
		}

		public static async Task<ResponseOld> SendRequestAsync<TResponse>(string url, string method, string contentType = "")
		{
			return await CreateNewClient(url).SendRequestAsync<TResponse>(method);
		}

		public void setBody(string bodyContent)
		{
			AddParameter("body", bodyContent, ParameterType.RequestBody);
		}

		public void setContentType(string contenType)
		{
			AddParameter("Content-Type", contenType, ParameterType.HttpHeader);
		}

		void AdicionaParametros(RestRequest req, Dictionary<string, string> parameters, ParameterType type = ParameterType.QueryString)
		{
			if (!((parameters != null && req != null) ? true : false))
			{
				return;
			}
			foreach (KeyValuePair<string, string> parameter in parameters)
			{
				RestSharp.ParameterType tp;

                switch (type)
                {
                    case ParameterType.GetOrPost:
						tp = RestSharp.ParameterType.GetOrPost;
						break;
                    case ParameterType.UrlSegment:
						tp = RestSharp.ParameterType.UrlSegment;
						break;
                    case ParameterType.HttpHeader:
						tp = RestSharp.ParameterType.HttpHeader;
						break;
                    case ParameterType.RequestBody:
						tp = RestSharp.ParameterType.RequestBody;
						break;
                    case ParameterType.QueryString:
						tp = RestSharp.ParameterType.QueryString;
						break;
                    default:
						tp = RestSharp.ParameterType.HttpHeader;
						break;
                }

                req.AddParameter(parameter.Key, parameter.Value, tp);
			}
		}

		public override void Dispose()
		{
		}

        protected override void AddRequestParams(dynamic req, Dictionary<string, string> parameters, ParameterType type = ParameterType.QueryString)
        {
            throw new NotImplementedException();
        }
    }
}
