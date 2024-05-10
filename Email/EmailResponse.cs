namespace ArmsFW.Services.Email
{
    public class EmailResponse
    {
        public bool Status { get; set; }
        public string Mensagem { get; set; }
        public EmailRequest MensagemEnviada { get; set; }
        public dynamic Data { get; set; }
    }
}
