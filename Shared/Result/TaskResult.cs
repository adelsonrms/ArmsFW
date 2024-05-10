using System;
using System.Collections.Generic;
using System.Linq;

namespace ArmsFW.Services.Shared
{
	public class TaskResult
	{
		private Dictionary<int, Exception> _exceptions;

		public bool Success { get; set; }

		public int StatusCode { get; set; }

		public string Message { get; set; }

		public dynamic Data { get; set; }

		public Dictionary<int, object> Steps { get; set; }

		public Dictionary<int, string> Errors { get; set; }

		public Dictionary<int, InfoException> Exceptions { get; set; }

		public int ErrorCount { get; private set; }

		public Dictionary<int, object> ResultSuccess { get; set; }

		public RedirectTo RedirectToUrl { get; set; }

		public TaskResult()
		{
			Errors = new Dictionary<int, string>();
			Steps = new Dictionary<int, object>();
			Exceptions = new Dictionary<int, InfoException>();
			ResultSuccess = new Dictionary<int, object>();
		}

		public TaskResult(bool success, string message, dynamic data)
		{
			Success = success;
			Message = message;
			Data = (object)data;
			Errors = new Dictionary<int, string>();
			Steps = new Dictionary<int, object>();
			Exceptions = new Dictionary<int, InfoException>();
			ResultSuccess = new Dictionary<int, object>();
		}

		public override string ToString()
		{
			return $"Sucesso : {Success} - Mensagem : {Message}";
		}

		public TaskResult SetResultInfo(bool success, string info, object data)
		{
			Success = success;
			Message = info;
			Data = data;
			return this;
		}

		public void AddException(Exception ex)
		{
			_exceptions = _exceptions ?? new Dictionary<int, Exception>();
			_exceptions.Add(_exceptions.Count, ex);
			AddError("Uma Exception foi capturada ! Verifique a lista de Exceptions para mais detalhes. Detalhe : " + ex.Message);
		}

		public void AddError(Exception ex)
		{
			AddException(ex);
		}

		public void AddError(string ErroMessage)
		{
			Errors = Errors ?? new Dictionary<int, string>();
			Errors.Add(Errors.Count, ErroMessage);
			Success = false;
		}

		public void AddSuccess(object obj)
		{
			ResultSuccess = ResultSuccess ?? new Dictionary<int, object>();
			ResultSuccess.Add(ResultSuccess.Count, obj);
			Success = false;
		}

		public static TaskResult Create()
		{
			return new TaskResult();
		}

		public void AddStep(string stepName, TaskResult tskResultFromStep)
		{
			Steps.Add(Steps.Count, new
			{
				stepName = stepName,
				status = (tskResultFromStep.Success ? "Sucesso" : "Erro"),
				TaskResultMensagem = tskResultFromStep.Message
			});
		}

		public TaskResult GetResult(TaskResult result)
		{
			return this.SetResultInfo(result.Success, result.Message, result.Data);
		}

		public TaskResult GetResult(bool success, string info, object data)
		{
			return SetResultInfo(success, info, data);
		}

		public TaskResult GetResult()
		{
			TaskResult taskResult = new TaskResult();
			taskResult.Success = Success;
			taskResult.StatusCode = StatusCode;
			taskResult.Message = Message;
			taskResult.Data = (object)(Data ?? new
			{
				info = "Não há dados para ser retornado"
			});
			taskResult.Exceptions = ExceptionExtensions.GetInnerExceptions(_exceptions);
			taskResult.ErrorCount = Errors.Count();
			taskResult.Errors = Errors;
			taskResult.Steps = Steps;
			taskResult.ResultSuccess = ResultSuccess;
			if (taskResult.ErrorCount > 0)
			{
				taskResult.Message = taskResult.Errors[0];
				taskResult.StatusCode = 400;
			}
			return taskResult;
		}

		public static TaskResult Sucesso(string message = "", dynamic data = null)
		{
			return new TaskResult(true, message, data);
		}

		public static TaskResult Erro(string message = "", dynamic data = null)
		{
			return new TaskResult(true, message, data);
		}
	}
}
