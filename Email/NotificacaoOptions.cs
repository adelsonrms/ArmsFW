using System.Collections.Generic;

namespace ArmsFW.Services.Email
{
    /// <summary>
    /// Abstração para as configurações globais de notificacao. Pega do appsettings.json
    /// </summary>
    public class NotificacaoOptions
    {
        public bool Ativo { get; set; }
        public string ServicoDeEmail { get; set; }
        public string ServicoDeSms { get; set; }
        public List<NotificacaoProvider> NotificacaoProviders { get; set; }
    }

}
