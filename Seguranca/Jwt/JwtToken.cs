using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

/// <summary>
/// Contem objetos para tratativa de security
/// </summary>
namespace ArmsFW.Security
{
    /// <summary>
    /// Rrepresenta o Token Jwt contendo as suas informações relevantes 
    /// </summary>
    public class JwtToken
    {
        public string Token { get; set; }
        public DateTime? DtExpiracao { get; set; }
        public string ID { get; set; }
        public string Message { get; set; }
        public bool Valido => Status == eTokenStatus.Valido;
        public DateTime? DtEmissao { get; set; }
        public eTokenStatus Status { get; set; } = eTokenStatus.NaoGerado;

        [JsonIgnore]
        public List<Claim> Claims { get; set; }

        public string PegarValor(string chave) => this.Claims.FirstOrDefault(x => x.Type == chave)?.Value;

        public override string ToString()
        {
            return $"Token : {(Valido ? $"Valido, Expira em {DtExpiracao}" : "Não")} | Codigo : {(this.Token?.Substring(0, 30))}...";
        }
    }
}


