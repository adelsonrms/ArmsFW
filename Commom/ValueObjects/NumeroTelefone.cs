using System.Collections.Generic;
using ArmsFW.Services.Extensions;

namespace ArmsFW.ValueObjects
{
	public class NumeroTelefone : ValueObject
	{
		private int _numero;

		public string Numero { get; set; }

		public string NumeroFormatado => montarMascara(Numero);

		public NumeroTelefone(string numero)
		{
			_numero = numero.ToInt();
			Numero = numero.Replace(" ", "").Replace("-", "").Replace("(", "")
				.Replace(")", "");
		}

		public string NumeroGlobal()
		{
			if (Numero.StartsWith("55"))
			{
				return "+" + montarMascara(Numero);
			}
			return "+55" + montarMascara(Numero.Replace("+55", "").Replace("+", ""));
		}

		public override string ToString()
		{
			return NumeroGlobal();
		}

		private string montarMascara(string numero)
		{
			string NUMERO = _numero.ToString("{0:(###) ###-####}");
			switch (numero.Length)
			{
			case 8:
				NUMERO = "0000-0000";
				break;
			case 9:
				NUMERO = _numero.ToString("9 0000-0000");
				break;
			case 11:
				NUMERO = _numero.ToString("(00) 9 0000-0000");
				break;
			case 13:
				if (numero.StartsWith("55"))
				{
					NUMERO = _numero.ToString("+00 (00) 9 0000-0000");
				}
				break;
			default:
				NUMERO = _numero.ToString("+55 (11) 9 0000-0000");
				break;
			}
			return NUMERO;
		}

		protected override IEnumerable<object> GetAtomicValues()
		{
			yield return Numero;
			yield return NumeroFormatado;
		}
	}
}
