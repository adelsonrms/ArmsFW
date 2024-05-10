using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ArmsFW;
using ArmsFW.Services.Shared.Settings;
using Newtonsoft.Json;

namespace app.core.Domain
{
    public class EntityTracking
	{
		public EntityTracking()
		{
			Criacao = DateTime.Now;
			Ativo = true;
			UsuarioCriacao = App.Session.User.Email;
		}

		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Key]
		[Column("id")]
		public int Id { get; set; }

		[Column("id_usuario_criacao")]
		public string UsuarioCriacao { get; set; }

		[Column("dt_criacao")]
		public DateTime? Criacao { get; set; }

		[Column("id_usuario_atualizacao")]
		public string UsuarioAtualizacao { get; set; }

		[Column("dt_atualizacao")]
		public DateTime? Atualizacao { get; set; }

		#region Campos de Controle de Registro
		[JsonIgnore]
		[Column("ds_situacao")]
		public string JsonSituacao { get; set; }

		[Column("fl_ativo")]
		public bool Ativo { get; set; }

		[JsonIgnore]
		[Column("ds_historico")]
		public string HistoricoDeAlteracao { get; set; }

		[JsonIgnore]
		[NotMapped]
		public Situacao Situacao { get; set; }

		public Situacao CarregarSituacao()
		{
			this.Situacao = new Situacao(JsonSituacao);
			return this.Situacao;
		}

		public bool IsValid()
		{
			return Id != 0;
		}

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


	public class RowTracking
	{
		[Column("id_usrCriacao")]
		public string UsuarioCriacao { get; set; }

		[Column("dt_criacao")]
		public DateTime? Criacao { get; set; }

		[Column("id_usrAtualizacao")]
		public string UsuarioAtualizacao { get; set; }

		[Column("dt_atualizacao")]
		public DateTime? Atualizacao { get; set; }

		[Column("fl_ativo")]
		public bool Ativo { get; set; }

		public RowTracking()
		{
			Criacao = DateTime.Now;
			Ativo = true;
			UsuarioCriacao = App.Session.User.Email;
		}
	}

    
}
