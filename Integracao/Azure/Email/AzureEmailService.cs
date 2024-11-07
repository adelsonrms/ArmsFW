using ArmsFW.Core.Types;
using ArmsFW.Services.Logging;
using ArmsFW.Services.Shared;
using ArmsFW.Services.Shared.Settings;
using Azure;
using Azure.Communication.Email;
using Microsoft.Extensions.Options;
using SendGrid.Helpers.Mail;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsFW.Services.Email
{
    public class AzureEmailService :  IDisposable
    {
        public static bool Sending { get; set; }

        #region Propriedados do Email
        private readonly EmailClient emailClient;

        private string _destination, _subject, _body, _cc, _cco, _logFile;
        public string Destionation { get => this._destination; set => this._destination = value; }
        public string Subject { get { return this._subject; } set { this._subject = value; } }
        public string Body { get { return this._body; } set { this._body = value; } }
        public string CC { get { return this._cc; } set { this._cc = value; } }
        public string CCO { get { return this._cco; } set { this._cco = value; } }
        public string LogFile { get { return this._logFile; } set { this._logFile = value; } }

        public string LogId { get; private set; }
        public DateTime DtInicio { get; set; }
        public DateTime DtFim { get; set; }
        #endregion
        private async Task GravarLog(string msg) {

            var log = new EntradaLog { Mensagem = msg, Origem= "Azure Email Service", IdLog = this.LogId  };
            await LogServices.GravarLog(log, this.LogFile);
        }

        public Action<EmailResponse> ColetarResponse { get; set; }

        private readonly NotificacaoProvider _mailSettings;

        public NotificacaoOptions Settings { get; private set; }

        public AzureEmailService(IOptions<NotificacaoOptions> emailSettings):this()
        {
            Settings = emailSettings.Value;
            _mailSettings = emailSettings.Value.NotificacaoProviders.FirstOrDefault(x => x.Nome == emailSettings.Value.ServicoDeEmail);
            emailClient = new EmailClient(_mailSettings.Host); 
        }

        public AzureEmailService()
        {
            this.LogFile = $@"{Aplicacao.Diretorio}\_logs\log.json";
            var notificacaoOptios = AppSettings.GetSection<NotificacaoOptions>("NotificacaoOptions");
            Settings = notificacaoOptios;

            if (notificacaoOptios!=null)
            {
                _mailSettings = notificacaoOptios.NotificacaoProviders.FirstOrDefault(x => x.Nome == notificacaoOptios.ServicoDeEmail);

                if (_mailSettings != null)
                {
                    emailClient = new EmailClient(_mailSettings.Host);
                }
            }
        }

        public static async Task<Result<object>> EnviarAsync(string para, string assunto, string corpo)
        {
            try
            {
                using (var emailService = new AzureEmailService())
                {
                    var resultado = await emailService.SendEmailAsync(new EmailRequest { Para = para, Assunto = assunto, Corpo = corpo });

                   return ResultBase.Sucesso("", resultado);
                }
            }
            catch (Exception e)
            {
                await LogServices.GravarLog($"Falha ao enviar o email : {e.Source} - {e.Message}");
                return ResultBase.Erro($"Falha ao enviar o email : {e.Source} - {e.Message}");
            }
        }

        #region Metodos de Envio
        /// <summary>
        /// Envia uma requisição de email atraves de um objeto EmailRequest
        /// </summary>
        /// <param name="mailRequest">Objeto com as informações necessárias para o envio</param>
        /// <returns></returns>
        public async Task<EmailResponse> SendEmailAsync(EmailRequest mailRequest)
        {
            AzureEmailMessage message = null;
            
            DtInicio = DateTime.Now;
            LogId = DtInicio.ToString("yyyyMMddHHmmssfff");
            

            //Sobscreve por um ID enviado pelo cliente no request
            if (!string.IsNullOrEmpty(mailRequest.LogId)) LogId = mailRequest.LogId;

            //Direciona para um email de teste, caso encontre
            if (!string.IsNullOrWhiteSpace(_mailSettings.EmailTeste)) mailRequest.Para = _mailSettings.EmailTeste;

            await GravarLog($"Inicia o envio de email. Para : {mailRequest.Para} | Assunto : {mailRequest.Assunto}");

            try
            {
                string para = mailRequest.Para;

                //Remetente que envia a mensagem. Conforme definições do SMTP no appsettings (EmailSettings)
                mailRequest.De = $"{_mailSettings.Sender}";

                message = CriarMensagem(mailRequest);

                var callbackResponse = new EmailResponse
                {
                    MensagemEnviada = mailRequest,
                    Data = message,
                    Mensagem = "Enviado..",
                    Status = false
                };

                try
                {

                    NotificationService.Sending = true;

                    //Dispara o envio...
                    await GravarLog($"AzureServiceEmailClient > Envia a mensagem....");
                    var emailSendOperation = await emailClient.SendAsync(WaitUntil.Completed, message);

                    await GravarLog($"Aguardando finalização do envio.");
                    EmailSendResult statusMonitor = emailSendOperation.Value;
                    callbackResponse.Mensagem = "Mensagem enviada. ID: " + statusMonitor.Status;
                    await GravarLog($"AzureServiceEmailClient.SendAsync() > Concluído !");
                }
                catch (Exception e)
                {
                   await GravarLog($"|erro|Falha ao enviar o comando AzureServiceEmailClient.SendMailAsync() : {e.Source} - {e.Message}");
                }

                await  GravarLog($"AzureServiceEmailClient > Envio da mensagem concluída !. Aguardando o report do serviço em Smtp_SendCompleted()");
                return callbackResponse;
            }
            catch (Exception e)
            {
                await  GravarLog($"|erro|Falha inesperada no envio da mensagem. Consulte detalhes : {e.Source} - {e.Message}");
                return new EmailResponse { Mensagem = $"Falha inesperada no envio da mensagem. Consulte detalhes : {e.Source} - {e.Message}" };
            }

            #region Funções Locais de Apoio

            AzureEmailMessage CriarMensagem(EmailRequest mailRequest)
            {
                //Cria a mensagem informando usuário remetente e o nome de exibição
                

                var conteudo = new EmailContent(mailRequest.Assunto.Replace("{LogId}", LogId)) { Html = mailRequest.Corpo.Replace("{LogId}", LogId) };

                AzureEmailMessage msg = new AzureEmailMessage(mailRequest.De, mailRequest.Para, conteudo);

                if (mailRequest.Anexos != null)
                {
                    foreach (var file in mailRequest.Anexos)
                    {
                        if (file.Length > 0)
                        {
                            using (var ms = new MemoryStream())
                            {
                                file.CopyTo(ms);
                                var fileBytes = ms.ToArray();
                                EmailAttachment att = new EmailAttachment(file.FileName, "", new BinaryData(fileBytes));
                                msg.Attachments.Add(att);
                            }
                        }
                    }
                }

                return msg;
            }
            #endregion
        }
        /// <summary>
        /// Envia um email informando as informações de : Destinatario, Assunto e Corpo
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        #endregion

        public Task SendSmsAsync(string number, string message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }

        public static string ConfiguraVariaveis(string conteudo, StringList variaveis, string tags = null)
        {
            StringBuilder sb = new StringBuilder(conteudo);
            string tagAbre = "";
            string tagFecha = "";

            if (!string.IsNullOrEmpty(tags))
            {
                tagAbre = tags.Split(";".ToCharArray())[0];
                tagFecha = tags.Split(";".ToCharArray())[1];
            }

            variaveis.ToList().ForEach(v =>
            {
                conteudo = conteudo.Replace($"{tagAbre}{v.Key}{tagFecha}", v.Value?.ToString());
            });

            return conteudo;
        }

        public static string CarregarTemplateDeEmail(string templateId)
        {
            string diretorioBase; string caminhoDoArquivo;

            if (File.Exists(templateId))
            {
                diretorioBase = Path.GetDirectoryName(templateId);
                templateId = Path.GetFileName(templateId);
                caminhoDoArquivo = $@"{diretorioBase}\{templateId}";
            }
            else
            {
                diretorioBase = AppSettings.GetSection<EmailSettings>("EmailSettings").DiretorioTemplates;
                caminhoDoArquivo = $@"{Aplicacao.Diretorio}\{diretorioBase}\{templateId}";
            }

            return CarregarTemplateDeEmail(templateId, caminhoDoArquivo);
        }

        public static string CarregarTemplateDeEmail(string templateId, string caminhoArquivo = null)
        {

            string diretorioBase;

            if (string.IsNullOrEmpty(caminhoArquivo))
            {
                diretorioBase = AppSettings.GetSection<EmailSettings>("EmailSettings").DiretorioTemplates;
                caminhoArquivo = $@"{diretorioBase}\{templateId}";
            }

            if (File.Exists(caminhoArquivo)) return File.ReadAllText(caminhoArquivo);
            return string.Empty;
        }

        public static string CarregarEstiloCss(string arquivo)
        {
            var conteudo = NotificationService.CarregarTemplateDeEmail(arquivo + ".css");

            if (!string.IsNullOrEmpty(conteudo))
            {
                conteudo = conteudo.Replace("\n", "");
                conteudo = conteudo.Replace("\r", "");
                conteudo = conteudo.Replace("." + arquivo, "").Trim();
                conteudo = conteudo.Substring(1, conteudo.Length - 2);
            }

            return conteudo;
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public async Task LimparLog()
        {
            try
            {
                File.CreateText(this.LogFile).Close();
            }
            catch 
            {
            }
        }
    }
}

namespace Azure.Communication.Email
{
    public class AzureEmailMessage : EmailMessage
    {
        public AzureEmailMessage(string senderAddress, string recipientAddress, EmailContent content) : base(senderAddress, recipientAddress, content)
        {
        }

        public AzureEmailMessage(string senderAddress, EmailRecipients recipients, EmailContent content) : base(senderAddress, recipients, content)
        {
        }
    }

}