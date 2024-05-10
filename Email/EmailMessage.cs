using ArmsFW.Core.Types;
using System.IO;
using System.Linq;
using System.Text;

namespace ArmsFW.Services.Email
{
    public class EmailMessage : IEmailMessage
    {
        public EmailMessage()
        {

        }

        public string Para { get; set; }
        public string Assunto { get; set; }
        public string Corpo { get; set; }
        public StringList Variaveis { get; set; }
        public string TemplateId { get; set; }

		public EmailMessage ConfiguraVariaveis(StringList variaveis, string tags)
		{
			Corpo = EmailService.ConfiguraVariaveis(Corpo, variaveis);
			Assunto = EmailService.ConfiguraVariaveis(Assunto, variaveis);
			return this;
		}

		public EmailMessage ConfiguraVariaveis() => ConfiguraVariaveis(Variaveis, "{{;}}");
	}
}
