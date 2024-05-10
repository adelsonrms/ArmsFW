using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Funcionalidades do Identity
/// </summary>
namespace ArmsFW.Infra.Identity
{
    //public class Usuario : IdentityUser
    //{
    //    public Usuario()
    //    {
    //        dt_criacao = DateTime.Now;
    //    }
    //    public string Name { get; set; }
    //    public string Nome { get; set; }
    //    [NotMapped]
    //    public string Perfil { get; set; }
    //    [NotMapped]
    //    public string Status { get; set; }
    //    [NotMapped]
    //    public string SenhaTemporaria { get; set; }
    //    public string Descricao { get; set; }
    //    #region Metodos de Manutenção

    //    public override string ToString() => $"Login ID : ({Id}) | {Email} - {Nome}";

    //    [NotMapped]
    //    public bool Exists
    //    {
    //        get
    //        {
    //            bool result = false;
    //            if (base.Id != null && base.Id.ToString() != "00000000-0000-0000-0000-000000000000")
    //            {
    //                result = true;
    //            }
    //            return result;
    //        }
    //    }

    //    public static Usuario Criar(string nome, string email) => Criar(nome, email, string.Empty);

    //    public static Usuario Criar(string nome, string email, string passwrod)
    //    {
    //        return new Usuario
    //        {
    //            Nome = nome,
    //            UserName = email,
    //            PasswordHash = passwrod,
    //            Email = email
    //        };
    //    }

    //    public static Usuario Criar(string nome) => Criar(nome, nome);

    //    public DateTime? dt_criacao { get; private set; }
    //    public DateTime? dt_atualizacao { get; set; }
    //    [NotMapped]
    //    public string MensagemUltimaAtualizacao { get; set; }
    //    public string GerarSenhaTemporaria()
    //    {
    //        string novaSenha = Guid.NewGuid().ToString().Substring(0, 4).ToLower();
    //        novaSenha += $"#{Guid.NewGuid().ToString().Substring(0, 4).ToUpper()}";
    //        return novaSenha;
    //    }
    //    [NotMapped]
    //    public bool IsExcluido
    //    {
    //        get
    //        {

    //            if (!string.IsNullOrEmpty(Status))
    //            {
    //                return Status.Equals("excluido");
    //            }

    //            return false;

    //        }
    //    }
    //    [NotMapped]
    //    public bool IsAtivo
    //    {
    //        get
    //        {

    //            if (!string.IsNullOrEmpty(Status))
    //            {
    //                return Status.Equals("ativo");
    //            }

    //            return false;

    //        }
    //    }
    //    [NotMapped]
    //    public bool IsBloqueado
    //    {
    //        get
    //        {

    //            if (!string.IsNullOrEmpty(Status))
    //            {
    //                return Status.Equals("bloqueado");
    //            }

    //            return false;

    //        }
    //    }
    //    [NotMapped]
    //    public string Token { get; set; }

    //    public void DefinirAtualizacao(string msg)
    //    {
    //        MensagemUltimaAtualizacao = msg;
    //        this.dt_atualizacao = DateTime.Now;
    //    }

    //    public void Excluir()
    //    {
    //        this.Status = "excluido";
    //        this.DefinirAtualizacao("O usuario foi excluido (nao deletado) da base !");
    //    }

    //    public void Bloquear()
    //    {
    //        this.Status = "bloqueado";
    //        this.DefinirAtualizacao("O usuario teve o acesso bloqueado !");

    //    }

    //    public void Habilitar()
    //    {
    //        this.Status = "ativo";
    //        this.DefinirAtualizacao("O acesso do usuario foi restaurado !");
    //    }

    //    #endregion
    //}
}