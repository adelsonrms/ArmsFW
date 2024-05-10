using System;
using System.IO;
using ArmsFW.Services.Shared;
using ArmsFW.Services.Shared.Settings;

namespace ArmsFW.Services.Session
{
	public class SessionService
	{
		private static Config _fileSession;

		public UsuarioDaSessao User
		{
			get
			{
				if (_fileSession == null)
				{
					return new UsuarioDaSessao
					{
						Name = "Nao logado"
					};
				}
				return new UsuarioDaSessao
				{
					Id = _fileSession.Pegar("userId"),
					Email = _fileSession.Pegar("email"),
					Name = _fileSession.Pegar("userName")
				};
			}
			private set
			{
			}
		}
        public string SessaoID { get; }
        public static SessionService Default => new SessionService();

		public bool Online { get; set; }

		public SessionService()
		{
			SessaoID = DateTime.Now.ToString("yyyyMMdd-hhmmss");
		}

		public SessionService(string userId, string userName, string email)
		{
            if (!Directory.Exists(App.ApplicationPath + "\\userSession")) Directory.CreateDirectory(App.ApplicationPath + "\\userSession");

			_fileSession = App.LoadSettings(App.ApplicationPath + "\\userSession\\_" + email + ".json");
			SaveUser(userId, userName, email);
			Online = true;
		}

		public void SaveUser(string userId, string userName, string email)
		{
			_fileSession.Salvar("userId", userId, false);
			_fileSession.Salvar("userName", userName, false);
			_fileSession.Salvar("email", email, false);
			_fileSession.Salvar("data", DateTime.Now.ToString());
			
			_fileSession.GravarConfiguracoesNoArquivo();
		}

		public static UsuarioDaSessao GetUser()
		{
			try
			{
				return new UsuarioDaSessao
				{
					Id = _fileSession?.Pegar("userId", true),
					Email = _fileSession?.Pegar("email", true),
					Name = _fileSession?.Pegar("userName", true)
				};
			}
			catch
			{
			}
			return new UsuarioDaSessao();
		}

		public void ClearSession()
		{
			try
			{
				if (File.Exists(_fileSession?.Arquivo))
				{
					File.Delete(_fileSession.Arquivo);
				}
				Online = false;
			}
			catch (Exception)
			{
				throw;
			}
		}
	}
}
