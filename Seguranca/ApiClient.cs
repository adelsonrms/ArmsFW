using System.Collections.Generic;

/// <summary>
/// Contem objetos para tratativa de security
/// </summary>
namespace ArmsFW.Security
{
    /// <summary>
    /// Informações para aplicativos de acesso a API
    /// </summary>
    public class ApiClient
    {
        /// <summary>
        /// ID do App (username)
        /// </summary>
        public string ClientID { get; set; }
        /// <summary>
        /// Senha  (password) do app
        /// </summary>
        public string ClientSecret { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }

        public object dt_criacao { get; set; }

        public List<string> Perfil { get; set; }
    }
}


