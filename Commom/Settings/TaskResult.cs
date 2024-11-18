//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace ArmsFW.Services.Shared
//{
//    public class TaskResult
//    {

//        private Dictionary<int, Exception> _exceptions;


//        public TaskResult()
//        {
//            Errors = new Dictionary<int, string>();
//            Steps = new Dictionary<int, object>();
//            Exceptions = new Dictionary<int, InfoException>();
//            ResultSuccess = new Dictionary<int, object>();
//        }

//        public TaskResult(bool success, string message, dynamic data)
//        {

//            this.Success = success;
//            this.Message = message;
//            this.Data = data;

//            Errors = new Dictionary<int, string>();
//            Steps = new Dictionary<int, object>();
//            Exceptions = new Dictionary<int, InfoException>();
//            ResultSuccess = new Dictionary<int, object>();
//        }

//        public bool Success { get; set; }
//        public int StatusCode { get; set; }
//        public string Message { get; set; }
//        public dynamic Data { get; set; }
//        public override string ToString() => $"Sucesso : {Success} - Mensagem : {Message}";
//        public Dictionary<int, object> Steps { get; set; }
//        public Dictionary<int, string> Errors { get; set; }
//        public Dictionary<int, InfoException> Exceptions { get; set; }
//        public int ErrorCount { get; private set; }
//        public Dictionary<int, object> ResultSuccess { get; set; }
//        public TaskResult SetResultInfo(bool success, string info, object data)
//        {
//            Success = success;
//            Message = info;
//            Data = data;
//            return this;
//        }
//        public void AddException(Exception ex)
//        {
//            this._exceptions = this._exceptions ?? new Dictionary<int, Exception>();
//            this._exceptions.Add(this._exceptions.Count, ex);
//            this.AddError($"Uma Exception foi capturada ! Verifique a lista de Exceptions para mais detalhes. Detalhe : {ex.Message}");
//        }
//        public void AddError(string ErroMessage)
//        {
//            this.Errors = this.Errors ?? new Dictionary<int, string>();
//            this.Errors.Add(this.Errors.Count, ErroMessage);
//            Success = false;
//        }
//        public void AddSuccess(object obj)
//        {
//            this.ResultSuccess = this.ResultSuccess ?? new Dictionary<int, object>();
//            this.ResultSuccess.Add(this.ResultSuccess.Count, obj);
//            Success = false;
//        }
//        public static TaskResult Create() => new TaskResult();
//        public void AddStep(string stepName, TaskResult tskResultFromStep)
//        {
//            this.Steps.Add(this.Steps.Count,
//                new
//                {
//                    stepName = stepName,
//                    status = (tskResultFromStep.Success ? "Sucesso" : "Erro"),
//                    TaskResultMensagem = tskResultFromStep.Message
//                }
//               );
//        }

//        public TaskResult GetResult(TaskResult result) => this.SetResultInfo(result.Success, result.Message, result.Data);

//        public TaskResult GetResult(bool success, string info, object data) => this.SetResultInfo(success, info, data);

//        public TaskResult GetResult()
//        {
//            var tsk = new TaskResult();

//            tsk.Success = this.Success;
//            tsk.StatusCode = this.StatusCode;
//            tsk.Message = this.Message;
//            tsk.Data = this.Data ?? new { info = "Não há dados para ser retornado" };

//            tsk.Exceptions = ExceptionExtensions.GetInnerExceptions(this._exceptions);

//            tsk.ErrorCount = this.Errors.Count();
//            tsk.Errors = this.Errors;
//            tsk.Steps = this.Steps;
//            tsk.ResultSuccess = this.ResultSuccess;

//            if (tsk.ErrorCount>0) {
//                tsk.Message = tsk.Errors[0];
//                tsk.StatusCode = 400;
//            }
//            return tsk;
//        }

//    }
//}
