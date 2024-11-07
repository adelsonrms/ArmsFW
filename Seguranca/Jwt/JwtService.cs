using ArmsFW.Lib.Web.Json;
using ArmsFW.Services.Extensions;
using ArmsFW.Services.Shared.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Contem objetos para tratativa de security
/// </summary>
namespace ArmsFW.Security
{

    /// <summary>
    /// Implementa os serviços de criação e manipulaç~~ao de tokens 
    /// </summary>
    public class JwtService : IJwtService
    {
        public static JwtService Default => new JwtService();

        private readonly JwtTokenSettings _settings;
        internal JwtTokenSettings JwtSettings => _settings;

        public List<TokenBlackList> BlackList { get; private set; }
        public readonly string BlackListStore;

        public JwtService(IOptions<JwtTokenSettings> JwtTokenSettings) : this()
        {
            _settings = JwtTokenSettings.Value;
        }

        public JwtService()
        {
            //Carrega as configurações para geração do token do appsettings
            _settings = AppSettings.GetSection<JwtTokenSettings>("JwtTokenSettings");

            //Caso não possua as configurações, assume o padrão
            //Apenas a chave de secret será usada
            if (_settings == null) _settings = new JwtTokenSettings { };

            BlackListStore = Path.GetDirectoryName(GetType().Assembly.Location) + "\\TokenBlackList.json";

            //Atualiza a lista de blacklist
            CarregarBlackList();
        }
         
        public TokenValidationParameters parametrosDoToken { get; set; } = new TokenValidationParameters();

        /// <summary>
        /// Dado a instancia de um Principal (Identidade), gera um token JWT apos a validação das credencias passando as suas Claims
        /// </summary>
        /// <param name="user">Instancia do ClaimsPrincipal logado</param>
        /// <returns></returns>
        public async Task<JwtToken> GerarToken(ClaimsPrincipal user) => await GerarToken(user.Claims);
        /// <summary>
        /// Gera um token passando as Claims desejada de um usuario
        /// </summary>
        /// <param name="userClaims">Lista de Claims</param>
        /// <returns></returns>
        public async Task<JwtToken> GerarToken(IEnumerable<Claim> userClaims)
        {
            JwtToken tokenGerado = new JwtToken();

            try
            {
                //Recupera o nome do usuario da claim ClaimTypes.Name
                var userName = userClaims?.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
                List<Claim> claims;
                DateTime expiration;

                MontaPayload(userClaims, userName, out claims, out expiration);

                var tokenHandler = new JwtSecurityTokenHandler();

                #region NET7
                //var key = Encoding.UTF8.GetBytes(_settings.Secret!);
                //var tokenDescriptor = new SecurityTokenDescriptor
                //{
                //    Subject = new ClaimsIdentity(claims),
                //    Expires = expiration,
                //    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                //};
                //var secToken = tokenHandler.CreateToken(tokenDescriptor);
                #endregion

                //Monta as informações do token
                JwtSecurityToken token = new JwtSecurityToken(
                   issuer: _settings.ValidIssuer,
                   audience: _settings.ValidAudience,
                   claims: claims,
                   expires: expiration,
                   signingCredentials: GerarCredencial(_settings.Secret)
                );

                tokenGerado = new JwtToken()
                {
                    DtEmissao = DateTime.Now,
                    Token = tokenHandler.WriteToken(token), //Gera a string do token
                    DtExpiracao = expiration,
                    ID = Guid.NewGuid().ToString(),
                    Status = eTokenStatus.Valido,
                    Claims = claims
                };

                tokenGerado.Message = $"Token gerado com sucesso por '{userName}'. Valido para autenticação até as {expiration.ToString("HH:mm")} do dia {expiration.ToShortDateString()}";
            }
            catch (Exception ex)
            {
                tokenGerado.ID = "";
                tokenGerado.Message = $"Ocorreu inesperada ao gerar o Token. Detalhe {ex.Message}";
            }

            return await Task.FromResult(tokenGerado);
        }

        public JwtToken Decodificar(string token)
        {
            var tokenGerado = new JwtToken();
            var tokenConfig = _settings;

            token = token.Trim();

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(tokenConfig.Secret));

            var creds = GerarCredencial(tokenConfig.Secret); //new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            SecurityToken validatedToken;

            var validator = new JwtSecurityTokenHandler();


            //Garante que a string seja de um Jwt Token valido
            if (validator.CanReadToken(token))
            {
                ClaimsPrincipal principal = null;
                try
                {
                    // Identifica o Principal associado ao token
                    principal = validator.ValidateToken(token, GetTokenValidationParameters(), out validatedToken);

                    // Verifica se há a claim
                    var t = validator.ReadJwtToken(token);

                    tokenGerado.ID = t.Claims.FirstOrDefault(x => x.Type == "jti").Value;
                    tokenGerado.DtExpiracao = t.ValidTo;

                    if (t.Claims.Any(x => x.Type == "dtEmissao")) tokenGerado.DtEmissao = Convert.ToDateTime(t.Claims.FirstOrDefault(x => x.Type == "dtEmissao").Value);
                    tokenGerado.Claims = t.Claims.ToList();

                    tokenGerado.Token = token;

                    CarregarBlackList();

                    //Apos a devida decodificação do token, verifica se o mesmo nao esta na blackList
                    if (EstaNaBlackList(token))
                    {
                        tokenGerado.Status = eTokenStatus.Bloquado;
                        tokenGerado.Message = $"Esse token esta Bloquado e não é mais valido para autenticação";
                    }
                    else
                    {
                        tokenGerado.Status = eTokenStatus.Valido;
                        tokenGerado.Message = $"Token emitido por : {principal.Claims.Where(c => c.Type == ClaimTypes.Name).First().Value}";
                    }
                }
                catch (SecurityTokenExpiredException ex)
                {
                    tokenGerado.Status = eTokenStatus.Expirado;
                    tokenGerado.Message = $"Token expirado em ({ex.Expires}). Requisite um novo token.";
                }
                catch (SecurityTokenInvalidAudienceException ex)
                {
                    tokenGerado.Status = eTokenStatus.ClienteInvalido;
                    tokenGerado.Message = $"O Cliente ({ex.InvalidAudience}) que informou o Token não esta autorizado a valida-lo";
                }
                catch (SecurityTokenInvalidIssuerException ex)
                {
                    tokenGerado.Status = eTokenStatus.EmissorInvalido;
                    tokenGerado.Message = $"O host emissor (Issuer) que informou o Token não esta autorizado a valida-lo. Detalhe : {ex.Message}";
                }
                catch (SecurityTokenInvalidSigningKeyException ex)
                {
                    tokenGerado.Status = eTokenStatus.AssinanteInvalido;
                    tokenGerado.Message = $"Falha na validação da assinatura do Token. Detalhe : {ex.Message}";
                }
                catch (Exception ex)
                {
                    tokenGerado.Status = eTokenStatus.Excessao;
                    tokenGerado.Message = $"Exception - Falha desconhecida na validação do Token. Detalhe {ex.Message}";
                }
            }
            else
            {
                tokenGerado.Status = eTokenStatus.Desconhecido;
                tokenGerado.Message = $"Token informado é invalido";
            }

            return tokenGerado;
        }

        private static void MontaPayload(IEnumerable<Claim> userClaims, string userName, out List<Claim> claims, out DateTime expiration)
        {
            //Claims (dados) do usuario no token
            claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.UniqueName, (userName?? "")),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("DtEmissao", DateTime.Now.ToString()),
                    new Claim("AppEmissor", "ArmsFW")
                };

            var validade_recorrencia = userClaims?.FirstOrDefault(x => x.Type == "Validade.Recorrencia")?.Value;
            var validade_valor = userClaims?.FirstOrDefault(x => x.Type == "Validade.Valor")?.Value;

            if (string.IsNullOrEmpty(validade_valor)) validade_valor = "1";

            //Gera o dia de expiracao a partir de hj
            switch (validade_recorrencia)
            {
                case "mes": expiration = DateTime.Now.AddMonths(validade_valor.ToInt()); break;
                case "ano": expiration = DateTime.Now.AddYears(validade_valor.ToInt()); break;
                default: expiration = DateTime.Now.AddDays(validade_valor.ToInt()); break;
            }

            //Demais claims do usuário
            claims.Add(new Claim("DtExpiracao", $"{expiration}"));
            claims.AddRange(userClaims);
        }

        private SigningCredentials GerarCredencial(string secret)
        {
            //Chave de encriptação (Senha)
            var keyCripto = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));

            //Credenciais
            var credenciais = new SigningCredentials(keyCripto, SecurityAlgorithms.HmacSha256Signature);

            return credenciais;
        }

        private bool EstaNaBlackList(string token) => this.BlackList.Any(t => t.Token == token);

        public List<TokenBlackList> CarregarBlackList()
        {
            BlackList = new List<TokenBlackList>();

            var file = BlackListStore;

            if (File.Exists(file))
            {
                var json = JSON.LoadFromFile<List<TokenBlackList>>(file);

                BlackList = json.Result;

                return BlackList ?? new List<TokenBlackList>();
            }
            return new List<TokenBlackList>();
        }

        private void AdicionarNaBlackList(string token, string motivo)
        {
            if (!this.BlackList.Any(t => t.Token == token)) this.BlackList.Add(new TokenBlackList { Token = token, Motivo = motivo });
        }

        public void RemoverDaBlackList(string token)
        {
            var t = PegarTokenNaBlackList(token);

            if (t != null) this.BlackList.Remove(t);
        }

        public TokenBlackList BloquerToken(string token, string motivo)
        {
            //Adiciona o token na black List
            AdicionarNaBlackList(token, motivo);

            //Atualiza o store
            SalvarBlackListNoStore(this.BlackList);

            return PegarTokenNaBlackList(token);
        }

        public void DesbloquarToken(string token)
        {
            //Adiciona o token na black List
            RemoverDaBlackList(token);

            //Atualiza o store
            SalvarBlackListNoStore(this.BlackList);
        }

        private TokenBlackList PegarTokenNaBlackList(string token) => this.BlackList.FirstOrDefault(t => t.Token == token);

        public void SalvarBlackListNoStore(List<TokenBlackList> tokenBlackLists)
        {
            if (File.Exists(BlackListStore)) File.Delete(BlackListStore);

            File.WriteAllText(BlackListStore, tokenBlackLists.ToJson());
        }

        public TokenValidationParameters GetTokenValidationParameters()
        {
            var tokenConfig = _settings;

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(tokenConfig.Secret));

            parametrosDoToken.IssuerSigningKey = key;
            parametrosDoToken.ValidateIssuer = tokenConfig.ValidateIssuer;
            parametrosDoToken.ValidateIssuerSigningKey = true;
            parametrosDoToken.ValidIssuer = tokenConfig.ValidIssuer;

            parametrosDoToken.ValidAudiences = tokenConfig.ValidAudiences;
            parametrosDoToken.ValidAudience = tokenConfig.ValidAudience;
            parametrosDoToken.ValidateAudience = tokenConfig.ValidateAudience;

            return parametrosDoToken;
        }
    }
}


