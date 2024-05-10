using ArmsFW.Core;
using ArmsFW.Services.Shared;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArmsFW.Domain.Entities
{
    [Table("Usuario", Schema = Aplicacao.SchemaPadrao)]
    public class UsuarioLocal
    {
        [Key]
        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        [NotMapped]
        public Nome NomeDoFuncionario => new Nome(this.Nome);
        [NotMapped]
        public string Nome { get; set; }
        [NotMapped]
        public string Perfil { get; set; }
        [NotMapped]
        public string Gestor { get; set; }
        [NotMapped]
        public string Flags { get; set; }
        [NotMapped]
        public bool? Ativo { get; set; }

        public string GetGestor()
        {
            if (string.IsNullOrEmpty(Gestor)) return "";
            return Gestor;
        }

        public bool ContemPerfil(string nomePerfil)
        {
            if (!string.IsNullOrEmpty(Perfil)) return Perfil.Contains(nomePerfil);
            return false;
        }

        public bool ContemFlag(string flag)
        {
            if (!string.IsNullOrEmpty(Flags)) return Flags.Contains(flag);
            return false;
        }

        public override string ToString()
        {
            return $"ID : ({Id}) | {this.Email} - {Nome} / Ativo {Ativo}";
        }

        [NotMapped]
        public bool Exists
        {
            get
            {
                bool result = false;
                if (Id != null && Id.ToString() != "00000000-0000-0000-0000-000000000000")
                {
                    result = true;
                }
                return result;
            }
        }

        [NotMapped]
        public bool IsExcluido { get; internal set; }


        public static UsuarioLocal Criar(string nome, string email) => Criar(nome, email, string.Empty);

        public static UsuarioLocal Criar(string nome, string email, string passwrod)
        {
            return new UsuarioLocal
            {
                Nome = nome,
                //UserName = email,
                //PasswordHash = passwrod,
                //Email = email
            };
        }

        public static UsuarioLocal Criar(string nome) => Criar(nome, nome);


        public string GerarSenhaTemporaria()
        {
            string novaSenha = Guid.NewGuid().ToString().Substring(0, 4).ToLower();
            novaSenha += $"#{Guid.NewGuid().ToString().Substring(0, 4).ToUpper()}";
            return novaSenha;
        }
    }

}


