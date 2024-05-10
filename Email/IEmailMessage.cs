using ArmsFW.Core.Types;

namespace ArmsFW.Services.Email
{
    public interface IEmailMessage
    {
        string Para { get; set; }
        string Assunto { get; set; }
        string Corpo { get; set; }
        string TemplateId { get; set; }
        StringList Variaveis { get; set; }

        EmailMessage ConfiguraVariaveis(StringList variaveis, string tags);
    }
}
