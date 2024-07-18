using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ArmsFW;
using ArmsFW.Services.Shared.Settings;
using Newtonsoft.Json;

namespace app.core.Domain;

public class EntityTracking
{
    public EntityTracking()
    {
        Criacao = DateTime.Now;
        UsuarioCriacao = App.Session.User.Email;
    }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("UsuarioCriacao")]
    public string UsuarioCriacao { get; set; }

    [Column("DataCriacao")]
    public DateTime? Criacao { get; set; }

    [Column("UsuarioAtualizacao")]
    public string UsuarioAtualizacao { get; set; }

    [Column("DataAtualizacao")]
    public DateTime? Atualizacao { get; set; }

    #region Campos de Controle de Registro
    [JsonIgnore]
    [Column("Historico")]
    public string HistoricoDeAlteracao { get; set; }

    [JsonIgnore]
    [Column("Opcoes")]
    public string Opcoes { get; set; }
        
		
    [JsonIgnore]
    [NotMapped]
    public Situacao Situacao { get; set; }

    public bool Exists()
    {
        return Id != 0;
    }

    public override string ToString()
    {
        return $"{Id}";
    }

    #endregion
}