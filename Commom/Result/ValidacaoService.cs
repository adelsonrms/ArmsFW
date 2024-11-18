using System.Collections.Generic;

namespace ArmsFW.Services.Shared
{
    /// <summary>
    /// Funções para o servico de validação
    /// </summary>
    public class ValidacaoService
    {
		/// <summary>
		/// Cria um resultado de validação com base em uma lista de items ja validados
		/// </summary>
		/// <param name="items"></param>
		/// <returns></returns>
		public ValidationResult CriarValidacao(List<Validation> items) => new ValidationResult(items);
		/// <summary>
		/// Cria um resultado de validação a partir de yna lista de mensagens 
		/// </summary>
		/// <param name="items"></param>
		/// <returns></returns>
		public ValidationResult CriarValidacao(List<string> items) => new ValidationResult(items);
	}
}


