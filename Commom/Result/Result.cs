using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ArmsFW.Services.Shared
{
    public class Result: Result<object> { }
    public class Result<TData> : IResult<TData>
	{
        #region Contrução
        public Result()
        {
            DateTime = System.DateTime.Now;
        }

        public Result(bool status)
            : this()
        {
            Status = status;
        }

        public Result(bool status, string msg)
            : this(status)
        {
            Message = msg;
        }

        public Result(bool status, string msg, TData data)
            : this(status, msg)
        {
            Data = data;
        } 
        #endregion

        public bool Status { get; set; }

		public string Message { get; set; }
        [JsonIgnore]
        public string Acao { get; set; } = "novo";

        public TData Data { get; set; }

		public DateTime? DateTime { get; set; }
		public ValidationResult Validation { get; set; }
    }

    public class ResultResponse<TData> : Result<TData>, IResultResponse<TData>
    {
        #region Contrução
        public ResultResponse()
        {
            DateTime = System.DateTime.Now;
        }

        public ResultResponse(bool status)
            : this()
        {
            Status = status;
        }

        public ResultResponse(bool status, string msg)
            : this(status)
        {
            Message = msg;
        }

        public ResultResponse(bool status, string msg, dynamic data, int statusCode = 200)
            : this(status, msg)
        {
            Data = data;
            StatusCode = statusCode;
        }
        #endregion
        public int StatusCode { get; set; }
    }

    public class ResultResponse : Result<object>, IResultResponse<object>
    {
        #region Contrução
        public ResultResponse()
        {
            DateTime = System.DateTime.Now;
        }

        public ResultResponse(bool status)
            : this()
        {
            Status = status;
        }

        public ResultResponse(bool status, string msg)
            : this(status)
        {
            Message = msg;
        }

        public ResultResponse(bool status, string msg, dynamic data, int statusCode = 200)
            : this(status, msg)
        {
            Data = data;
            StatusCode = statusCode;
        }
        #endregion
        public int StatusCode { get; set; }
    }
}