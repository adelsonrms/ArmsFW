namespace ArmsFW.Lib.Web.Json
{
	public class JsonResult<TObjeto> where TObjeto : class
	{
		public TObjeto Result { get; set; }

		internal JsonFileLoadException<TObjeto> Exception { get; set; }

		public override string ToString()
		{
			return ((Exception != null) ? ("Sucesso, Conteudo carregado !" + Result.GetType().Name + " (" + Result.ToString() + ")") : ("Erro - " + Exception.Message)) ?? "";
		}
	}
}
