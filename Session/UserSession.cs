namespace ArmsFW.Services.Session
{
	public class UsuarioDaSessao
	{
		public string Email { get; set; }

		public string Id { get; set; }

		public string Name { get; set; }
        public string UserName { get;  set; }
        public string Nome { get;  set; }
		public string Perfil { get;  set; }
		public string Token { get;  set; }

        public override string ToString()
		{
			return Email + " - " + Name;
		}
	}
}
