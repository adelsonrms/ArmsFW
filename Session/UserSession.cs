namespace ArmsFW.Services.Session
{
	public class UsuarioDaSessao
	{
		public string Email { get; set; }

		public string Id { get; set; }

		public string Name { get; set; }
        public string UserName { get; internal set; }
        public string Nome { get; internal set; }
		public string Perfil { get; internal set; }
		public string Token { get; internal set; }

        public override string ToString()
		{
			return Email + " - " + Name;
		}
	}
}
