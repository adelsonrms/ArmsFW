using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArmsFW.Domain { 

	public class EFEntity : Entity<int>
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
		public string Atualizacao
		{
			get
			{
				if (MomentoEdicao.HasValue)
				{
					return MomentoEdicao.Value.ToString("dd-MM-yyyy HH:mm");
				}
				return "";
			}
		}

		public EFEntity()
		{
			MomentoEdicao = DateTime.Now;
			MomentoInclusao = DateTime.Now;
		}

		public new bool IsPersist()
		{
			if (!string.IsNullOrEmpty(base.EFGuid))
			{
				return Id != 0;
			}
			return false;
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
	}

	internal class IgnoreAttribute : Attribute
	{
	}
}
