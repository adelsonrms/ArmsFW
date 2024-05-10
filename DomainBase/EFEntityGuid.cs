using System;
using System.ComponentModel.DataAnnotations.Schema;
using app.core.Domain.Entities;

namespace ArmsFW.Domain
{
	public class EFEntityGuid : Entity<string>
	{
		[Ignore]
		public virtual string UsuarioInclusao { get; set; }

		[Ignore]
		public virtual DateTime? MomentoInclusao { get; set; }

		[Ignore]
		public virtual string UsuarioEdicao { get; set; }

		[Ignore]
		public virtual DateTime? MomentoEdicao { get; set; }

		[Ignore]
		[NotMapped]
		public string Atualizacao => MomentoEdicao.Value.ToString("dd-MM-yyyy hh:mm");

		public EFEntityGuid()
		{
			MomentoInclusao = DateTime.Now;
			MomentoEdicao = DateTime.Now;
		}

		public bool Exists()
		{
			if (!string.IsNullOrEmpty(base.EFGuid))
			{
				return Id != "0000.0000.0000.0000.0000.000";
			}
			return false;
		}

		public bool IsValid()
		{
			return Exists();
		}

		public new bool IsPersist()
		{
			if (!string.IsNullOrEmpty(base.EFGuid))
			{
				return Id != "0000.0000.0000.0000.0000.000";
			}
			return false;
		}

		public override string ToString()
		{
			return Id ?? "";
		}
	}
}
