using System.Collections.Generic;
using System.Security.Claims;

namespace Seguranca
{
    public static class ClaimsExtensions
    {
        public static void Adicionar(this List<Claim> claims, string nome, object valor)
        {
            if (!string.IsNullOrEmpty(nome) && !string.IsNullOrEmpty(valor?.ToString()))
            {
                claims.Add(new Claim(nome, valor.ToString()));
            }
        }
    }
}