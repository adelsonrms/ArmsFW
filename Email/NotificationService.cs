using ArmsFW.Services.Shared.Settings;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ArmsFW.Services.Logging;
using System.ComponentModel;
using ArmsFW.Core;
using ArmsFW.Core.Types;
using System.Reflection;
using Azure.Communication.Email;

namespace ArmsFW.Services.Email
{
    public enum eTipoRecipiente
    {
        De, Para, ComCopia, ComCopiaOculta
    }

    public class NotificationService : IEmailSender, ISmsSender, IDisposable
    {
        public static bool Sending { get; set; }

        #region Propriedados do Email
        private SmtpClient _smtpClient;

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
        private async Task GravarLog(string msg)
        {

            var log = new EntradaLog { Mensagem = msg, Origem = "Email Service", IdLog = this.LogId };
            LogServices.GravarLog(log, this.LogFile);
        }

        public SendCompletedEventHandler OnCompleted { get; set; }
        public Action<EmailResponse> ColetarResponse { get; set; }

        //private readonly EmailSettings _mailSettings;

        private readonly NotificacaoProvider _mailSettings;

        /// <summary>
        /// Inicializa a classe com as configurações via Injação de Dependencia
        /// </summary>
        /// <param name="emailSettings">Opções Recebido via ConfigureServices do appsettings</param>
        public NotificationService(IOptions<NotificacaoOptions> emailSettings) : this()
        {
            if (emailSettings.Value.NotificacaoProviders?.Count>0)
            {
                _mailSettings = emailSettings.Value.NotificacaoProviders.FirstOrDefault(x => x.Nome == emailSettings.Value.ServicoDeEmail);
                _smtpClient = new SmtpClient(_mailSettings.Host, _mailSettings.Port);
            }
        }

        public NotificationService()
        {
            this.LogFile = $@"{Aplicacao.Diretorio}\_logs\log.json";
            var notificacaoOptios = AppSettings.GetSection<NotificacaoOptions>("NotificacaoOptions") ?? new NotificacaoOptions { NotificacaoProviders  = new System.Collections.Generic.List<NotificacaoProvider>()};
            _mailSettings = notificacaoOptios.NotificacaoProviders.FirstOrDefault(x => x.Nome == notificacaoOptios.ServicoDeEmail);

            if (!notificacaoOptios.Ativo)
            {
                LogServices.GravarLog(
                    "O serviço de notificações por email está desativado", 
                    $"({Assembly.GetCallingAssembly().ManifestModule.Name}) > {this.GetType().FullName}"
                );
            }

            if (_mailSettings != null)
            {
                _smtpClient = new SmtpClient(_mailSettings.Host, _mailSettings.Port);

                this.OnCompleted += this.Smtp_SendCompleted;
            }
            else
            {
                LogServices.GravarLog(
                  "Não há configurações de envio de emails para a aplicação. Verifique o AppSettings",
                  $"({Assembly.GetCallingAssembly().ManifestModule.Name}) > {this.GetType().FullName}"
              );
            }
        }

        public static void Enviar(string para, string assunto, string corpo)
        {
            try
            {
                using (var ms = new NotificationService())
                {
                    Task.FromResult(ms.SendEmailAsync(para, assunto, corpo));
                }
            }
            catch (Exception e)
            {
                LogServices.GravarLog($"Falha ao enviar o email : {e.Source} - {e.Message}");
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

            if (_mailSettings==null)
            {
                var retorno = 
                new EmailResponse
                {
                    MensagemEnviada = mailRequest,
                    Mensagem = $"Não foi encontradas as configurações de envio de emails no AppSettings...",
                    Status = false
                };

                await GravarLog(retorno.Mensagem);

                return retorno;
            }
            
            MailMessage message = null;

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
                mailRequest.De = $"{_mailSettings.Sender},{_mailSettings.DisplayName}";

                message = CriarMensagem(mailRequest);

                message.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                using (_smtpClient = new SmtpClient(_mailSettings.Host, _mailSettings.Port))
                {
                    #region Configuração do Cliente de Envio SmtpClient
                    _smtpClient.EnableSsl = _mailSettings.UseSsl;
                    _smtpClient.UseDefaultCredentials = false;
                    _smtpClient.Credentials = new NetworkCredential(_mailSettings.Sender, _mailSettings.Password);
                    _smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    #endregion

                    //var secureSocketOptions = SecureSocketOptions.None;
                    //_smtpClient.Connect(_mailSettings.Host, _mailSettings.Port, secureSocketOptions);
                    

                    //Registra os eventos
                    //Evento requisitado pelo cliente
                    if (mailRequest.OnCompleted != null) this.OnCompleted += mailRequest.OnCompleted;

                    _smtpClient.SendCompleted -= this.OnCompleted;

                    //Evento interno do serviço
                    if (this.OnCompleted != null) _smtpClient.SendCompleted += this.OnCompleted;

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
                        await GravarLog($"SmtpClient > Envia a mensagem....");
                        await GravarLog($"****************************************************************");
                        //await GravarLog($"Email Config        : {new { _mailSettings.Host, _mailSettings.Port, UserSender = $"{_mailSettings.DisplayName} - {_mailSettings.UserMailSender}", SSL = _mailSettings.UseSsl }.ToJson(false)}");
                        await GravarLog($"AppSetting Config        : Host:{_mailSettings.Host}, Porta: {_mailSettings.Port}, UserSender = {_mailSettings.DisplayName} - {_mailSettings.Sender}, SSL : {_mailSettings.UseSsl}");
                        await GravarLog($"SmtpClient Config        : Host:{_smtpClient.Host}, Porta: {_smtpClient.Port}, Servidor: {_smtpClient.ServicePoint.Address} , SSL : {_smtpClient.EnableSsl}");

                        await GravarLog($"****************************************************************");

                        _smtpClient.SendAsync(message, callbackResponse);

                        await GravarLog($"SmtpClient.SendAsync() > Solicitação de envio concluído....");


                        if (NotificationService.Sending)
                        {
                            await GravarLog($"Aguardando finalização do envio.");
                        }

                        //Aguandando outro envio...
                        while (NotificationService.Sending) { }

                        await GravarLog($"SmtpClient.SendAsync() > Concluído !");
                    }
                    catch (Exception e)
                    {
                        GravarLog($"|erro|Falha ao enviar o comando _smtpClient.SendMailAsync() : {e.Source} - {e.Message}");
                    }

                    await GravarLog($"SmtpClient > Envio da mensagem concluída !. Aguardando o report do serviço em Smtp_SendCompleted()");

                    //_smtpClient.Disconnect(true);
                    return callbackResponse;

                }
            }
            catch (Exception e)
            {
                await GravarLog($"|erro|Falha inesperada no envio da mensagem. Consulte detalhes : {e.Source} - {e.Message}");
                return new EmailResponse { Mensagem = $"Falha inesperada no envio da mensagem. Consulte detalhes : {e.Source} - {e.Message}" };
            }

            #region Funções Locais de Apoio
            //Função Local para prepara o endereco incluindo ou nao o Display Name
            MailAddress MontarEndereco(string endereco)
            {
                if (string.IsNullOrEmpty(endereco)) return null;

                var email = GetEmail(endereco);
                MailAddress mailAddress = new MailAddress(email.Endereco, email.NomeDeExibicao);

                return mailAddress;
            }

            MailMessage CriarMensagem(EmailRequest mailRequest)
            {
                //Cria a mensagem informando usuário remetente e o nome de exibição
                MailMessage msg = new MailMessage();

                msg = AdicionaRemetente(msg, eTipoRecipiente.De, MontarEndereco(mailRequest.De));

                //Email do remetente
                //msg.From = MontarEndereco(mailRequest.De, eTipoRecipiente.De);

                //Monta os endereços para os destinatarios
                if (mailRequest.Para.Contains(";"))
                {
                    mailRequest.Para.Split(";").ToList().ForEach(email => AdicionaRemetente(msg, eTipoRecipiente.Para, MontarEndereco(email)));
                }
                else
                {
                    AdicionaRemetente(msg, eTipoRecipiente.Para, MontarEndereco(mailRequest.Para));
                }

                //Complementa os demais enderecoços :  CC e CCO 
                AdicionaRemetente(msg, eTipoRecipiente.ComCopia, MontarEndereco(mailRequest.CC));
                AdicionaRemetente(msg, eTipoRecipiente.ComCopiaOculta, MontarEndereco(mailRequest.CCO));

                //Atualiza variaveis, caso tenha
                mailRequest.Corpo = mailRequest.Corpo.Replace("{LogId}", LogId);
                mailRequest.Assunto = mailRequest.Assunto.Replace("{LogId}", LogId);

                msg.Subject = mailRequest.Assunto;
                msg.Body = mailRequest.Corpo;
                msg.IsBodyHtml = true;

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
                                Attachment att = new Attachment(new MemoryStream(fileBytes), file.FileName);
                                msg.Attachments.Add(att);
                            }
                        }
                    }
                }

                return msg;
            }

            MailMessage AdicionaRemetente(MailMessage msg, eTipoRecipiente tipo, MailAddress mailAddress)
            {
                if (mailAddress == null) return msg;

                //Acresenta o email na collection correspondentes
                switch (tipo)
                {
                    case eTipoRecipiente.De:
                        msg.From = mailAddress;
                        break;
                    case eTipoRecipiente.Para:
                        msg.To.Add(mailAddress);
                        break;
                    case eTipoRecipiente.ComCopia:
                        msg.CC.Add(mailAddress);
                        break;
                    case eTipoRecipiente.ComCopiaOculta:
                        msg.Bcc.Add(mailAddress);
                        break;
                    default:
                        break;
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
        public async Task<EmailResponse> SendEmailAsync(string destination, string subject, string body) => await SendEmailAsync(new EmailRequest { Para = destination, Assunto = subject, Corpo = body });
        public async Task<EmailResponse> SendEmailAsync(string email) => await SendEmailAsync(email, this._subject, this._body);
        public async Task<EmailResponse> SendEmailAsync(IEmailMessage message) => await SendEmailAsync(message.Para, message.Assunto, message.Corpo);
        public async Task<EmailResponse> SendEmailAsync() => await SendEmailAsync(this._destination, this._subject, this._body);
        #endregion

        private Email GetEmail(string address)
        {
            if (address.Contains(","))
            {
                return new Email() { Endereco = address.Split(",")[0], NomeDeExibicao = address.Split(",")[1] };
            }
            else
            {
                return new Email() { Endereco = address };
            }
        }

        private struct Email
        {
            public string Endereco;
            public string NomeDeExibicao;
        }

        private void Smtp_SendCompleted(object sender, AsyncCompletedEventArgs e)
        {
            EmailResponse response = e.UserState as EmailResponse;

            string mensagem = "";
            DtFim = DateTime.Now;
            NotificationService.Sending = false;

            LogServices.Debug($"Smtp_SendCompleted() - ID de Envio : {LogId} : Um envio de email foi concluído ! Consultando o report enviado pelo SmtpClient...");

            if (e.Error != null)
            {
                mensagem += $"|erro|Falha inesperada ao enviar o email";
                mensagem += $"|erro|{e.Error.Source} - {e.Error.Message}";
                mensagem += $"|erro|{e.Error?.InnerException?.Source} - {e.Error?.InnerException?.Message}";
                GravarLog($"|erro|Smtp_SendCompleted() - Report do SmtpClient : {mensagem}");

                if (response != null)
                {
                    response.Mensagem = mensagem;
                    response.Status = false;
                }
            }
            else
            {
                mensagem += $"SmtpClient informa que o envio foi concluido com sucesso. EmailResponse : {response.MensagemEnviada.Para}";
                if (response != null)
                {
                    response.Mensagem = mensagem;
                    response.Status = true;
                }
                GravarLog($"Smtp_SendCompleted() - Report do SmtpClient : {mensagem}");
            }

            response.Mensagem += $" | Tempo de Envio : {DtFim.Subtract(DtInicio).ToString()}";

            LogServices.Debug(mensagem);

            GravarLog($"Smtp_SendCompleted() - Servidor : {(sender as SmtpClient)?.TargetName}");

            GravarLog($"Smtp_SendCompleted() - Tempo de Envio : {DtFim.Subtract(DtInicio).ToString()}");

            this.ColetarResponse?.Invoke(response);
        }

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
