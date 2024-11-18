namespace ArmsFW.Services.Shared
{
	public class InfoException
	{
		public int Leval { get; set; }

		public string Source { get; set; }

		public string Message { get; set; }

		public string StackTrace { get; set; }

		public string ExceptionType { get; set; }

		public string Method { get; internal set; }

		public override string ToString()
		{
			return $"\r\n## ({Leval}) {ExceptionType}\r\n..........................................................................................................................................\r\nMensagem            : {Message}\r\n..........................................................................................................................................\r\nMetodo              : {Method}";
		}
	}
}
