namespace ArmsFW.Services.Email
{
    public class NotificacaoProvider
    {
        public string Nome { get; set; }
        public string Host { get; set; }
        public string DisplayName { get; set; }
        public string EmailTeste { get; set; }
        public string Password { get; set; }
        public string Sender { get; set; }
        public int Port { get; set; }
        public bool UseSsl { get; set; }
        public string DiretorioTemplates { get; set; }
    }

}
