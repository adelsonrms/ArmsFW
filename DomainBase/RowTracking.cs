using System;
using System.ComponentModel.DataAnnotations.Schema;
using ArmsFW.Services.Shared.Settings;

namespace app.core.Domain
{
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
