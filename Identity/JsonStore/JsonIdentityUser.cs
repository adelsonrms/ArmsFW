using Microsoft.AspNetCore.Identity;
using System;
using System.Security.Principal;

namespace ArmsFW.Infra.Identity
{
    public class JsonIdentityUser : IdentityUser, IIdentity
    {
        public JsonIdentityUser()
        {
            DataAtualizacao = DateTime.Now;
        }
        public string AuthenticationType { get; set; }
        public bool IsAuthenticated { get; set; }
        public string Name { get; set; }
        public string Perfil { get; set; }


        public string Status { get; set; }

        public string SenhaTemporaria { get; set; }

        public string Descricao { get; set; }
        public DateTime? DataCadastro { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        
        public string MensagemUltimaAtualizacao { get; set; }
        
        #region Metodos de Manutenção

        public static JsonIdentityUser Criar(string nome, string email)
        {
            return Criar(nome, email, string.Empty);
        }

        public static JsonIdentityUser Criar(string nome, string email, string passwrod)
        {
            return new JsonIdentityUser
            {
                Name = nome,
                UserName = email,
                PasswordHash = passwrod,
                Email = email,
                DataCadastro = DateTime.Now
            };
        }

        public static JsonIdentityUser Criar(string nome)
        {
            return Criar(nome, nome);
        }

        public string GerarSenhaTemporaria()
        {
            string novaSenha = Guid.NewGuid().ToString().Substring(0, 4).ToLower();
            novaSenha += $"#{Guid.NewGuid().ToString().Substring(0, 4).ToUpper()}";
            SenhaTemporaria = novaSenha;
            return novaSenha;
        }

        public bool IsExcluido
        {
            get
            {

                if (!string.IsNullOrEmpty(Status))
                {
                    return Status.Equals("excluido");
                }

                return false;

            }
        }

        public bool IsActive
        {
            get
            {

                if (!string.IsNullOrEmpty(Status))
                {
                    return Status.Equals("ativo");
                }

                return false;

            }
        }
        public bool IsBloqueado
        {
            get
            {

                if (!string.IsNullOrEmpty(Status))
                {
                    return Status.Equals("bloqueado");
                }

                return false;

            }
        }
        public void DefinirAtualizacao(string msg)
        {
            MensagemUltimaAtualizacao = msg;
            this.DataAtualizacao = DateTime.Now;
        }

        public void Excluir()
        {
            this.Status = "excluido";
            this.DefinirAtualizacao("O usuario foi excluido (nao deletado) da base !");
        }

        public void Bloquear()
        {
            this.Status = "bloqueado";
            this.DefinirAtualizacao("O usuario teve o acesso bloqueado !");
            
        }

        public void Habilitar()
        {
            this.Status = "ativo";
            this.DefinirAtualizacao("O acesso do usuario foi restaurado !");

        }
        #endregion
    }
}
