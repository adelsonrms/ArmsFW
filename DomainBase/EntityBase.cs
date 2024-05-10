using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using app.core.Domain;
using app.core.Domain.Entities;

namespace ArmsFW.Domain
{
	public class EntityBase : EntityBase<object>
	{
	}
	public class EntityBase<T> : RowTracking, IEFEntity
	{
		[Key]
		[Column("id")]
		public int Id { get; set; }

		[Ignore]
		[NotMapped]
		public string EFGuid { get; set; }
		public string usr_criacao { get; set; }
		public DateTime? dt_criacao { get; set; }

		internal static T Criar()
		{
			return Activator.CreateInstance<T>();
		}

		public bool IsPersist()
		{
			if (!string.IsNullOrEmpty(EFGuid))
			{
				return Id != 0;
			}
			return false;
		}

		public override string ToString()
		{
			return $"{Id}";
		}
	}
}
