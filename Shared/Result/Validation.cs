namespace ArmsFW.Services.Shared
{
    /// <summary>
    /// Representa o detalhe de uma validação. Pode ser um campo, uma regra, etc
    /// </summary>
    public class Validation
    {
		/// <summary>
		/// Retorna True ou false indicando se o item foi validado com sucesso
		/// </summary>
		public bool valid { get; set; }
		/// <summary>
		/// Identificação de um item. Pode ser um campo, uma regra, etc
		/// </summary>
		public string item { get; set; }
		/// <summary>
		/// Mensagem informativa para a validação. Ex. Campo 'Nome' obrigatorio, nao foi preenchido
		/// </summary>
        public string  message { get; set; }
		/// <summary>
		/// Resumoe os detalhes do item
		/// </summary>
		/// <returns></returns>
        public override string ToString()
        {
            return $"item : {item} | Validado {valid} | Detail {message }";
        }
    }
}


