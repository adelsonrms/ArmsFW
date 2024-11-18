using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ArmsFW.Services.Logging
{
    [Serializable]
    public class DbEntityValidationException : Exception
    {
        public DbEntityValidationException()
        {
        }

        public DbEntityValidationException(string message) : base(message)
        {
        }

        public DbEntityValidationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DbEntityValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public IEnumerable<object> EntityValidationErrors { get; internal set; }
    }
}