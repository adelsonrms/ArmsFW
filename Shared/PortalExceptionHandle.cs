using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ArmsFW.Services.Shared
{
    [Serializable]

    public class PortalExceptionHandle : PortalExceptionHandle<object>
    {
        public PortalExceptionHandle(Exception ex) : base(ex) { }
        public PortalExceptionHandle(string ex) : base(ex) { }
    }

    public class PortalExceptionHandle<T> : Exception where T : class
    {
        private Exception _ex;

        public PortalExceptionHandle()
        {
            GetInnerExceptionsInfo(base.GetBaseException());
        }

        public PortalExceptionHandle(string message, T context) : base(message)
        {
            TypeContext = context;
            GetInnerExceptionsInfo(base.GetBaseException());
        }

        public PortalExceptionHandle(Exception ex, T context) : base(ex.Message, ex)
        {
            TypeContext = context;
            GetInnerExceptionsInfo(ex);
        }

        public PortalExceptionHandle(Exception ex)
        {
            this._ex = ex;
            GetInnerExceptionsInfo(ex);
        }

        public PortalExceptionHandle(Exception ex, object envRuntime) : base(ex.Message, ex)
        {
            this._ex = ex;
            this.EnvironmentRuntime = envRuntime;
            GetInnerExceptionsInfo(ex);
        }
        public PortalExceptionHandle(string message) : base(message)
        {
            GetInnerExceptionsInfo(base.GetBaseException());
        }

        public PortalExceptionHandle(string message, object envRuntime) : base(message)
        {
            this.EnvironmentRuntime = envRuntime;
            GetInnerExceptionsInfo(base.GetBaseException());
        }


        public PortalExceptionHandle(string message, Exception innerException) : base(message, innerException)
        {
            GetInnerExceptionsInfo(base.GetBaseException());
        }

        protected PortalExceptionHandle(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        private void GetInnerExceptionsInfo(Exception ex)
        {
            PreviousInnerExceptions = new InnerExceptions(ex);
            PreviousInnerExceptionsMessagem = PreviousInnerExceptions.ToString();
        }

        public InnerExceptions PreviousInnerExceptions { get; set; }
        public string PreviousInnerExceptionsMessagem { get; set; }

        public dynamic EnvironmentRuntime { get; set; }

        public T TypeContext { get; set; }
    }

    public class InnerExceptionsInfo
    {
        public InnerExceptionsInfo(Exception ex) => this.Exceptions = ex.GetInnerExceptions();
        public Dictionary<int, InfoException> Exceptions { get; set; }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var ex in Exceptions)
            {
                sb.AppendLine($"{ex.Value.ToString()}");
            }
            
            return sb.ToString();
        }
    }
}