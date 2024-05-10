using System.Collections.Generic;

namespace ArmsFW.Services.Shared
{
	public class Nome : ValueObject
	{
		private string _nome;

		private string[] _nomeSplit;

		public string PrimeiroNome => getPrimeiroNome();

		public string SobreNome => getSobreNome();

		public string UltimoNome => getUltimoNome();

		public string NomeCompleto => $"{PrimeiroNome} {SobreNome} {UltimoNome}";

		public string NomeCurto => $"{PrimeiroNome} {UltimoNome}";

		public Nome(string nome)
		{
			_nome = nome;
			_nomeSplit = splitNome();
		}

		private string getPrimeiroNome()
		{
			return splitNome()[0];
		}

		private string getUltimoNome()
		{
			return _nomeSplit[_nomeSplit.Length - 1];
		}

		private string getSobreNome()
		{
			string _sn = "";
			for (int i = 1; i < _nomeSplit.Length - 1; i++)
			{
				_sn += _nomeSplit[i];
			}
			return _sn;
		}

		private string[] splitNome()
		{
			if (_nome != null)
			{
				return _nome?.Split(" ".ToCharArray());
			}
			return new string[1] { "" };
		}

		public override string ToString()
		{
			return NomeCompleto;
		}

		protected override IEnumerable<object> GetAtomicValues()
		{
			yield return PrimeiroNome;
			yield return SobreNome;
			yield return UltimoNome;
			yield return NomeCompleto;
			yield return NomeCurto;
		}
	}
}
