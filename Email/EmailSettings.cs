namespace ArmsFW.Services.Email
{
    public class EmailSettings
    {
        public string UserMailSender { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; } = 587;
        public bool UseSsl { get; set; } = true;
        public bool UseDefaultCredentials { get; set; } = false;
        public string EmailTeste { get; set; }
        public string CC { get;  set; }
        public string CCO { get;  set; }
        public string DiretorioTemplates { get; set; }
        public string AzureEmailServiceConnectionString { get; set; }
    }
}
