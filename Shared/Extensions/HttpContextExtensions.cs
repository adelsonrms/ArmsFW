using ArmsFW.Security;
using ArmsFW.Services.Session;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Security.Claims;
using System.Text;

namespace ArmsFW.Web.Http
{
    /// <summary>
    /// Funcoes adicionais a instancia do HttpContext
    /// </summary>
    public static class HttpContextInstance
    {
        private static Microsoft.AspNetCore.Http.IHttpContextAccessor m_httpContextAccessor;

        public static void Configure(Microsoft.AspNetCore.Http.IHttpContextAccessor httpContextAccessor)
        {
            m_httpContextAccessor = httpContextAccessor;
        }
        public static Microsoft.AspNetCore.Http.HttpContext Current
        {
            get
            {

                if (m_httpContextAccessor==null)
                {
                    return null;
                }
                return m_httpContextAccessor.HttpContext;
            }
        }


        /// <summary>
        /// Recupera o valor de um Header
        /// </summary>
        /// <param name="request">Instancia do HttpRequest</param>
        /// <param name="chave">Nome da chave do Header</param>
        /// <returns></returns>
        public static string PegarValorDoHeader(this HttpRequest request, string chave)
        {
            if (request.Headers.TryGetValue(chave, out var valor)) return valor;

            return null;
        }
        /// <summary>
        /// Tenta recupera o valor do Session ID em um header SessaoID. Caso seja informado, pode ser usando para validações
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string SessaoID(this HttpRequest request) => PegarValorDoHeader(request, "X-SESSION-ID");
        /// <summary>
        /// Recupera a identificação do dispositivo que invocou a requisicao. Caso nao ache, pega do header do 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string Origem(this HttpRequest request) => PegarValorDoHeader(request, "X-CLIENT_APPID") ?? PegarValorDoHeader(request, "User-Agent");
        /// <summary>
        /// Retorna o valor do header X-CLIENT_DEVICE_TYPE o qual deve ter a identificação do dispositivo
        /// </summary>
        /// <param name="request">Instancia do HttpRequest</param>
        /// <returns>String contendo a identificacao do dispositivo</returns>
        public static string TipoOrigem(this HttpRequest request)
        {

            var tipo = PegarValorDoHeader(request, "X-CLIENT_DEVICE_TYPE");

            //Se nao informado, tenta pegar o padrao
            if (string.IsNullOrEmpty(tipo))
            {
                if (Origem(request).ToLower().Contains("postman")) return "desktop";
                if (Origem(request).ToLower().Contains("Edg")) return "web";
                if (Origem(request).ToLower().Contains("Chrome")) return "web";
            }

            return tipo;
        }
        /// <summary>
        /// Caso seja enviado, recupera informações de localização de origem (Localtion) do dispositivo no header X-CLIENT_LOCATIO
        /// </summary>
        /// <param name="request">Instancia do HttpRequest</param>
        /// <returns>String contendo as coordenadas </returns>
        public static string Localizacao(this HttpRequest request) => PegarValorDoHeader(request, "X-CLIENT_LOCATION");
        /// <summary>
        /// Retorna o valor do autorization
        /// </summary>
        /// <param name="request">Instancia do HttpRequest</param>
        /// <returns></returns>
        public static string Autorization(this HttpRequest request) => PegarValorDoHeader(request, "Authorization");
        /// <summary>
        /// Retorna True se for identificado um conteudo no Header Authorization
        /// </summary>
        /// <param name="request">Instancia do HttpRequest</param>
        /// <returns></returns>
        public static bool Autenticado(this HttpRequest request) => !string.IsNullOrEmpty(Autorization(request));
        /// <summary>
        /// Avalia o conteudo dp Header de Authorization e determina qual o tipo foi identificado : Bearer, Basic ou code
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Retorna um AutenticacaoRequest contendo as informações da autenticacao do Request</returns>
        /// <remarks></remarks>
        /// <example></example>
        /// <exception cref="AutenticacaoRequestException"></exception>
        public static AutenticacaoRequest PegarAutenticacao(this HttpRequest request)
        {

            try
            {
                //Retorna a autenticacao no Header
                var authorization = PegarValorDoHeader(request, "Authorization");

                if (!string.IsNullOrEmpty(authorization))
                {
                    //Trata o Token JWT recebido.
                    if (authorization.StartsWith("Bearer"))
                    {
                        return new AutenticacaoRequest
                        {
                            type = eAuthType.TokenJwt,
                            code = authorization.ToString().Replace("Bearer ", "")
                        };
                    }

                    //Trata usuario e senha recebido
                    if (authorization.ToString().StartsWith("Basic"))
                    {
                        var user_pwd_b64 = authorization.ToString().Replace("Basic ", "");

                        string user_pwd = Encoding.GetEncoding("iso-8859-1").GetString(Convert.FromBase64String(user_pwd_b64));

                        int seperatorIndex = user_pwd.IndexOf(':');

                        return new AutenticacaoRequest
                        {
                            type = eAuthType.UserPassword,
                            userName = user_pwd.Substring(0, seperatorIndex),
                            password = user_pwd.Substring(seperatorIndex + 1)
                        };
                    }
                }


                //Determina a autenticação via ClientSecret
                bool localizaClientSecret = AutenticationService.PegaClientSecret(request.Headers, request.Query, out var client, out var secret, out var source);

                //Se achou o ClientSecret, retorna o schema
                if (localizaClientSecret)
                {
                    //Nao é Bearer nem Basic, retorna somenet o codigo
                    return new AutenticacaoRequest
                    {
                        type = eAuthType.ClientSecret,
                        userName = client,
                        password = secret
                    };
                }


                localizaClientSecret = AutenticationService.PegarApiKeyNaQueryString(request.Query, out var api_key);

                //Se achou o ClientSecret, retorna o schema
                if (localizaClientSecret)
                {
                    //Nao é Bearer nem Basic, retorna somenet o codigo
                    return new AutenticacaoRequest
                    {
                        type = eAuthType.ApiKey,
                        userName = "API Key",
                        password = api_key
                    };
                }



            }
            catch
            {

            }

            //Nao é Bearer nem Basic, retorna somenet o codigo
            return new AutenticacaoRequest
            {
                type = eAuthType.NaoAutenticado
            };
        }
        public static string Caminho(this HttpRequest request) => $"{request.Method} => {request.Host}{request.Path.Value}";
        public static JwtToken UserToken(this HttpContext http)
        {
            if (http.Request.Autenticado())
            {
                var auth = http.Request.PegarAutenticacao();

                if (auth.type == eAuthType.TokenJwt)
                {
                    var jwt = new JwtService().Decodificar(auth.code);

                    return jwt;
                }
            }

            return null;
        }

        public static UsuarioDaSessao UsuarioDoToken(this HttpContext context)
        {
            if (context == null) return new UsuarioDaSessao();

            var token = context.UserToken();

            if (token != null && token.Valido)
            {
                return new UsuarioDaSessao
                {
                    Id = token.PegarValor("Usuario.UserId"),
                    Email = token.PegarValor("Usuario.Email"),
                    Name = token.PegarValor("Usuario.Name"),
                    Token = token.PegarValor("Usuario.Nome")
                };
            }

            return new UsuarioDaSessao();
        }

        public static UsuarioDaSessao UsuarioDoContexto(this HttpContext context)
        {
            if (context == null) return new UsuarioDaSessao();

            var usrSaida = new UsuarioDaSessao
            {
                Id = context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? context.User.Identity.Name,
                Email = context.User.Identity.Name,
                Name = context.User.Identity.Name,
                UserName = context.User.Identity.Name,
                Nome = context.User.FindFirstValue(ClaimTypes.GivenName) ?? context.User.Identity.Name,
                //Token = context.User.FindFirstValue("Token") ?? ""
            };

            usrSaida.Token = PegarToken(usrSaida.Id);

            if (string.IsNullOrEmpty(usrSaida.Nome))
            {
                usrSaida.Nome = context.User.Identity.Name;
            }

            return usrSaida;
        }

        public static string PegarToken(string id)
        {
            try
            {
                return System.IO.File.ReadAllText($"{Path.GetTempPath()}\\userSession\\{id}.json");
            }
            catch
            {
            }

            return string.Empty;
        }
    }
}