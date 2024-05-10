using ArmsFW.Services.Shared;
using System.Collections.Generic;

namespace ArmsFW.Services.Shared
{
	public class Email : ValueObject
	{
		public string Endereco { get; set; }

		public string Dominio => Endereco.Split("@".ToCharArray())[1];

		public Email()
		{
		}

		public Email(string endereco)
		{
			Endereco = endereco;
		}

		public override string ToString()
		{
			return Endereco;
		}

		protected override IEnumerable<object> GetAtomicValues()
		{
			yield return Endereco;
			yield return Dominio;
		}
	}
}
