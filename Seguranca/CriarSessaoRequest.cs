using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Seguranca
{
    public class CriarSessaoRequest
    {
        public string tipoOrigem;
        public string location;
        public string code { get; set; }
        public string userId { get; set; }
        public string mensagem { get; set; }
        public string origem { get; set; }
        public string authProvider { get; set; }
        public bool? DevMode { get; set; }
        public List<Claim> claims { get; set; }
        public string userType { get; set; }
    }
}