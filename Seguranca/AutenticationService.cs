using ArmsFW.Services.Shared.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Contem objetos para tratativa de security
/// </summary>
namespace ArmsFW.Security
{
    public static class AutenticationService
    {

        /// <summary>
        /// Procura pelos headers de autenticação : ClientID e ClientSecret
        /// </summary>
        /// <param name="locationHeader"></param>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        public static bool PegaClientSecret(IDictionary<string, StringValues> locationHeader, IQueryCollection locationQuery, out string apiApp, out string apiKey, out string source)
        {
            //1º Tenta procurar as informações nos Headers, se achou ja sai e valida.
            if (locationHeader.TryGetValue("ClientID", out var extractedApiApp) && locationHeader.TryGetValue("ClientSecret", out var extractedApiKey))
            {
                apiApp = extractedApiApp;
                apiKey = extractedApiKey;
                source = "Hearder";
                return true;
            }

            //Se nao achou, tena procurar na QueryString
            if (locationQuery.TryGetValue("ClientID", out var extractedApiAppQuery) && locationQuery.TryGetValue("ClientSecret", out var extractedApiKeyQuery))
            {
                apiApp = extractedApiAppQuery;
                apiKey = extractedApiKeyQuery;
                source = "QueryString";
                return true;
            }

            apiKey = String.Empty;
            apiApp = String.Empty;
            source = string.Empty;

            return false;
        }


        public static bool ClientExiste(string clientID)
        {
            var clientes = AppSettings.GetSectionList<ApiClient>("ApiClients");

            return clientes.Any(c => c.ClientID == clientID);
        }

        /// <summary>
        /// Busca informações de um usuario aplicativo por nome ou ID
        /// </summary>
        /// <param name="clientID"></param>
        /// <returns></returns>
        public static ApiClient PegarCliente(string clientID)
        {
            try
            {
                return AppSettings.GetSectionList<ApiClient>("ApiClients").FirstOrDefault(c => c.ClientID == clientID || c.Nome == clientID);
            }
            catch
            {
            }
            return null;
        }



        public static bool ValidaClientSecret(string clientID, string clientSecret, string source, out string validacao, out ApiClient app)
        {
            app = null;
            #region Validação dos Input
            if (string.IsNullOrEmpty(clientID))
            {
                validacao = "Necessario informar o ClientID para a requisição. O Fornecimento é obrigatorio.";
                return false;
            }

            if (string.IsNullOrEmpty(clientSecret))
            {
                validacao = "Necessario informar o ClientSecret para a requisição. O Fornecimento é obrigatorio.";
                return false;
            }
            #endregion

            var clientes = App.Config.Get<ApiClient>("ApiClients");

            if (clientes != null)
            {
                var cliente = clientes.FirstOrDefault(c => c.ClientID == clientID);

                if (cliente == null)
                {
                    validacao = $"Cliente API '{clientID}' não cadastrado. Sem permissão para processar a requisição";
                    return false;
                }
                else
                {
                    if (cliente.ClientSecret != clientSecret)
                    {
                        validacao = $"Senha de acesso da API para o cliente '{clientID}' está incorreta. Sem permissão para processar a requisição";
                        return false;
                    }
                    else
                    {
                        app = cliente;
                        //OK - AUTENTICADO
                        validacao = $"OK, Autenticado usando Cliente/Secret em {source}";
                        return true;
                    }
                }
            }

            validacao = $"Nenhum mecanismo Cliente/Secret (Header ou QueryString) foi identificado.  Consulte o administrador do serviço";
            return false;
        }

        public static bool ValidaApiKey(string apiKey, string source, out string validacao)
        {
            #region Validação dos Input

            if (string.IsNullOrEmpty(apiKey))
            {
                validacao = "Necessario informar o ClientSecret para a requisição. O Fornecimento é obrigatorio.";
                return false;
            }
            #endregion

            var keys = AppSettings.GetSectionList<string>("ApiKeys");

            if (keys != null)
            {
                if (keys.Any(c => c == apiKey))
                {
                    //OK - AUTENTICADO
                    validacao = $"OK, Autenticado usando API Key em {source}";
                    return true;
                }
                else
                {
                    validacao = $"Chave de API (Api Key) não encontrada. Requisição negada";
                    return false;
                }
            }

            validacao = $"Nenhum mecanismo Cliente/Secret (Header ou QueryString) foi identificado.  Consulte o administrador do serviço";
            return false;
        }


        /// <summary>
        /// Procura pelos headers de autenticação : ClientID e ClientSecret
        /// </summary>
        /// <param name="location"></param>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        public static bool PegaClientSecretNoHeader(IDictionary<string, StringValues> location, out string apiApp, out string apiKey)
        {

            if (location.TryGetValue("ClientID", out var extractedApiApp) && location.TryGetValue("ClientSecret", out var extractedApiKey))
            {
                apiApp = extractedApiApp;
                apiKey = extractedApiKey;
                return true;
            }


            apiKey = String.Empty;
            apiApp = String.Empty;
            return false;
        }

        /// <summary>
        /// Procura pelas informações de autenticação na queryString
        /// </summary>
        /// <param name="location"></param>
        /// <param name="apiApp"></param>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        public static bool PegarClientSecretNaQueryString(IQueryCollection location, out string apiApp, out string apiKey)
        {

            if (location.TryGetValue("ClientID", out var extractedApiApp) && location.TryGetValue("ClientSecret", out var extractedApiKey))
            {
                apiApp = extractedApiApp;
                apiKey = extractedApiKey;
                return true;
            }
            apiKey = String.Empty;
            apiApp = String.Empty;

            return false;
        }

        public static bool PegarApiKeyNaQueryString(IQueryCollection location, out string apiKey)
        {

            if (location.TryGetValue("X-API-Key", out var extractedApiKey))
            {
                apiKey = extractedApiKey;
                return true;
            }
            apiKey = String.Empty;

            return false;
        }


        public static bool VerificaChaveApi(string extractedApiKey, out string validacao)
        {
            if (string.IsNullOrEmpty(extractedApiKey))
            {
                validacao = "Nenhuma ApiKey informado na requisição. Contate o Administradore, solicite uma ApiKey valida e forneça-a no Header X-API-Key ou em ma queryString de mesmo nome.";
                return false;
            }

            string apiKey = AppSettings.Configuration.GetValue<string>("X-API-Key");

            if (apiKey.Equals(extractedApiKey))
            {
                validacao = "Autenticado usando ApiKey no Header X-API-Key";
                return true;
            }

            validacao = "A ApiKey fornecida não esta autorizada. Contate o Administradore, solicite uma ApiKey valida e forneça-a no Header X-API-Key ou em ma queryString de mesmo nome.";
            return false;
        }

    }
}


