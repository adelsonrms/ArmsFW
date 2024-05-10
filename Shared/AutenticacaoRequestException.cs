using Microsoft.AspNetCore.Http;
using System;
using System.Runtime.Serialization;

namespace ArmsFW.Services.Shared
{
    [Serializable]
    internal class AutenticacaoRequestException : Exception
    {
        public AutenticacaoRequestException()
        {
        }

        public AutenticacaoRequestException(string message) : base(message)
        {
        }

        public AutenticacaoRequestException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AutenticacaoRequestException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public HttpContext Context { get; set; }
    }
}
