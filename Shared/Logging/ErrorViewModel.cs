using System;

namespace ArmsFW.Logging
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }

    public class ErrorModel
    {
        public string Origem { get; set; }
        public string MsgErro { get; set; }
        public string Detalhe { get; set; }
        public Exception Exception { get; set; }
    }
}
