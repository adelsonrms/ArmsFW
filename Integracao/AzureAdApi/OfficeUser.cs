using System.Collections.Generic;

namespace ArmsFW.Services.Azure
{
	public class OfficeUser
	{
		public User User { get; set; }

		public List<Message> Messages { get; set; }
        public int QtdMensagens { get; set; }

        public string Photo { get; set; }

		public bool Success { get; set; }

		public string ValidationMessage { get; set; }

		public dynamic ValidationDetail { get; set; }

		public OfficeUser()
		{
			User = new User();
			Messages = new List<Message>();
		}
	}
}
