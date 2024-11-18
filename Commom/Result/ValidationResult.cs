using System;
using System.Collections.Generic;
using System.Linq;

namespace ArmsFW.Services.Shared
{
    /// <summary>
    /// Representa uma validação
    /// </summary>
    public class ValidationResult
	{
        public ValidationResult()
        {
			Details = new List<Validation>();
		}
        public ValidationResult(List<Validation> items):this() => Details.AddRange(items);
		public ValidationResult(List<string> items) : this()
        {
            if (items!=null)
            {
				items.ForEach(x =>
				{
					Adicionar(x);
				});
			}
		}

		/// <summary>
		/// Se nao contem erros. Esta validado
		/// </summary>
		public bool valid => (Count == 0);
		/// <summary>
		/// Quantidade total de items validados
		/// </summary>
		public int? Count => Details?.Count;
		/// <summary>
		/// Lista dos items validados
		/// </summary>
		public List<Validation> Details { get; set; }
		/// <summary>
		/// Resumoe os detalhes do item
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"Quantade de Validações : {Count} | Validado {valid}";
		}

        public Validation Adicionar(string x)
        {
            if (!string.IsNullOrEmpty(x))
            {
				var arrItem = x.Split("|".ToCharArray());
				var validacao = new Validation { valid = Convert.ToBoolean(arrItem[0]), item = arrItem[1].ToString(), message = arrItem[2].ToString() };
				Details.Add(validacao);
				return validacao;
			}
			return null;
		}
    }
}


