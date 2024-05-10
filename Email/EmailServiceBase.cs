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

namespace ArmsFW.Services.Email
{

    public class EmailServiceBase : IDisposable
    {
        public static bool Sending { get; set; }

        #region Propriedados do Email
        private readonly SmtpClient _smtpClient;

        private string _destination, _subject, _body, _cc, _cco, _logFile;
        
        public string LogFile { get { return this._logFile; } set { this._logFile = value; } }

        public string LogId { get; private set; }
        public DateTime DtInicio { get; set; }
        public DateTime DtFim { get; set; }
        #endregion
        private async Task GravarLog(string msg) {

            var log = new EntradaLog { Mensagem = msg, Origem= "Email Service", IdLog = this.LogId  };
            LogServices.GravarLog(log, this.LogFile);
        }

        public SendCompletedEventHandler OnCompleted { get; set; }
        public Action<EmailResponse> ColetarResponse { get; set; }

        private readonly EmailSettings _mailSettings;

        public EmailServiceBase(IOptions<EmailSettings> emailSettings):this()
        {
            _mailSettings = emailSettings.Value;
            _smtpClient = new SmtpClient(_mailSettings.Host, _mailSettings.Port);
        }

        public EmailServiceBase()
        {
            this.LogFile = $@"{Aplicacao.Diretorio}\_logs\log.json";
            _mailSettings = AppSettings.GetSection<EmailSettings>("EmailSettings");
            if (_mailSettings!=null)
            {
                _smtpClient = new SmtpClient(_mailSettings.Host, _mailSettings.Port);
                this.OnCompleted += this.Smtp_SendCompleted;
            }
        }

        public static void Enviar(string para, string assunto, string corpo)
        {
            try
            {
                using (var ms = new EmailService())
                {
                    Task.FromResult(ms.SendEmailAsync(para, assunto, corpo));
                }
            }
            catch (Exception e)
            {
                LogServices.GravarLog($"Falha ao enviar o email : {e.Source} - {e.Message}");
            }
        }

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
            var conteudo = EmailService.CarregarTemplateDeEmail(arquivo + ".css");

            if (!string.IsNullOrEmpty(conteudo))
            {
                conteudo = conteudo.Replace("\n", "");
                conteudo = conteudo.Replace("\r", "");
                conteudo = conteudo.Replace("." + arquivo, "").Trim();
                conteudo = conteudo.Substring(1, conteudo.Length - 2);
            }

            return conteudo;
        }

        private void Smtp_SendCompleted(object sender, AsyncCompletedEventArgs e)
        {
            EmailResponse response = e.UserState as EmailResponse;

            string mensagem = "";
            DtFim = DateTime.Now;
            EmailService.Sending = false;

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
        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
