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
	public class EntityBase<T> : IEFEntity
	{
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("id")]
        public int Id { get; set; }

		[NotMapped]
		public string EFGuid { get; set; }

        [Column("UsuarioCriacao")]
        public string UsuarioCriacao { get; set; }

        [Column("DataCriacao")]
        public DateTime? Criacao { get; set; }

        [Column("UsuarioAtualizacao")]
        public string UsuarioAtualizacao { get; set; }

        [Column("DataAtualizacao")]
        public DateTime? Atualizacao { get; set; }

        [Column("Historico")]
        public string HistoricoDeAlteracao { get; set; }

        
        [Column("Opcoes")]
        public string Opcoes { get; set; }

        [NotMapped]
        public Situacao Situacao { get; set; }

        public bool Exists()
        {
            return Id != 0;
        }


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
