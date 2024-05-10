using System.Collections.Generic;
using System.Net.Mail;
using Microsoft.AspNetCore.Http;

namespace ArmsFW.Services.Email
{
    public class EmailRequest
    {
        public string De { get; set; }
        public string Para { get; set; }
        public string CC { get; set; }
        public string CCO { get; set; }
        public string Assunto { get; set; }
        public string Corpo { get; set; }
        public List<IFormFile> Anexos { get; set; }
        public SendCompletedEventHandler OnCompleted { get; set; }
        public string LogId { get; set; }
    }
}
