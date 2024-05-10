using System.ComponentModel.DataAnnotations.Schema;

namespace ArmsFW.Domain
{
	public class EntityBaseIdNome : EntityBase
	{
	}
	public class EntityBaseIdNome<T> : EntityBase<T> where T:class
	{
		[Column("ds_nome")]
		public string Nome { get; set; }

		public EntityBaseIdNome()
		{
		}

		public EntityBaseIdNome(int id, string nome)
		{
			base.Id = id;
			Nome = nome;
		}

		public override string ToString()
		{
			return $"ID : ({base.Id}) - {Nome}";
		}
	}
}
