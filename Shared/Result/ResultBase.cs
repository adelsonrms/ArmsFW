using Microsoft.AspNetCore.Mvc;
using System;

namespace ArmsFW.Services.Shared
{
    /// <summary>
    /// Representação do objeto ResultBase com tipo padrão object
    /// </summary>
    public abstract class ResultBase : ResultBase<object>
    {
        public static ActionResult<ResultResponse> Ok(string message = "", dynamic data = null) => CriarResponseOK(message, data);
        public static ActionResult<ResultResponse> Created(string message = "", dynamic data = null) => CriarResponseCriado(message, data);
        public static ActionResult<ResultResponse> BadRequest(string message = "", dynamic data = null) => CriarResponseErroAplicacao(message, data);

        public static ActionResult<ResultResponse> CriarResponseOK(string message = "", dynamic data = null) => CriarResponse(message, data, 200);
		public static ActionResult<ResultResponse> CriarResponseErro(string message = "", dynamic data = null) => ErroResult(message, data, 200);

		public static ActionResult<ResultResponse> CriarResponseCriado(string message = "", dynamic data = null) => CriarResponse(message, data, 201);
		public static ActionResult<ResultResponse> CriarResponseErroAplicacao(string message = "", dynamic data = null) => CriarResponse(message, data, 400);
		public static ActionResult<ResultResponse> CriarResponseNaoAutenticado(string message = "", dynamic data = null) => CriarResponse(message, data, 401);
		public static ActionResult<ResultResponse> CriarResponseNegado(string message = "", dynamic data = null) => CriarResponse(message, data, 403);
		public static ActionResult<ResultResponse> CriarResponseNaoExiste(string message = "", dynamic data = null) => CriarResponse(message, data, 404);
		public static ActionResult<ResultResponse> CriarResponseErroServidor(string message = "", dynamic data = null) => CriarResponse(message, data, 500);
		
		public static ActionResult<ResultResponse> CriarResponse(string message = "", int statusCode = 200) => CriarResponse(message, null, statusCode);
		public static ActionResult<ResultResponse> CriarResponse(string message = "", dynamic data = null, int statusCode = 200)
        {
			//Qualquer statusCode diferente de 200 será considerado erro
            if (statusCode>299)
            {
				return ErroResult(message, data, statusCode);
            }
            else
            {
				//Status Code : 200, 201, 203
				return SucessoResult(message, data, statusCode);
			}
        }

		public static ActionResult<ResultResponse<TDataBody>> CriarResponseBody<TDataBody>(string message, TDataBody data, int statusCode = 200)
		{
			//Qualquer statusCode diferente de 200 será considerado erro
			if (statusCode != 200)
			{
				return ErroResultData(message, data, statusCode);
			}
			else
			{
				//Status Code : 200, 201, 203
				return SucessoResultData(message, data, statusCode);
			}
		}

	}
    /// <summary>
    /// Retorna um objeto contendo informações para tratativa de requisições api
    /// </summary>
    /// <typeparam name="TData">Tipo Generic para representar a instancia do objeto desejado</typeparam>
    public abstract class ResultBase<TData>
	{
		/// <summary>
		/// Instancia ativa do tipo generico TData
		/// </summary>
		public Result<TData> Result { get; set; }

		#region Tratativa de Retorno de sucesso para API's

		/// <summary>
		/// Retorna um Result<Data> com status = true representando sucesso no retorno
		/// </summary>
		/// <param name="message">Mensagem a ser mostrada no retorno</param>
		/// <param name="data">Objeto contendo os dados do retorno</param>
		/// <returns></returns>
		public static ResultResponse SucessoResponse(string message = "", dynamic data = null) => new ResultResponse(true, message, data, 200);
		public static ResultResponse<TData> SucessoResponseData(string message = "", dynamic data = null) => new ResultResponse<TData>(true, message, data, 200);

		/// <summary>
		/// Retorno de um Result<object> sem tratar o objeto de retorno
		/// </summary>
		/// <param name="message">Mensagem a ser mostrada no retorno</param>
		/// <returns></returns>
		public static ResultResponse SucessoResponse(string message = "") => SucessoResponse(message, null);


		/// <summary>
		/// Retorna um Result<Data> com status = true representando sucesso no retorno
		/// </summary>
		/// <param name="message">Mensagem a ser mostrada no retorno</param>
		/// <param name="data">Objeto contendo os dados do retorno</param>
		/// <returns></returns>
		public static Result<TData> Sucesso(string message = "", dynamic data = null) => new Result<TData>(true, message, data);
		/// <summary>
		/// Retorno de um Result<object> sem tratar o objeto de retorno
		/// </summary>
		/// <param name="message">Mensagem a ser mostrada no retorno</param>
		/// <returns></returns>
		public static Result<TData> Sucesso(string message = "") => Sucesso(message, null);
		/// <summary>
		/// Para retorno de API. Retorna um IActionResult (ObjectResult.StatusCode = 200) contendo informações de sucesso. No contendo será retorna um Result com status  = true
		/// </summary>
		/// <param name="message">Mensagem a ser mostrada no retorno</param>
		/// <returns>IActionResult (ObjectResult.StatusCode = 200) com um Result<object>.Status = true</returns>
		public static ActionResult<ResultResponse> SucessoResult(string message = "") => SucessoResult(message, null);
		/// <summary>
		/// Para retorno de API. Retorna um IActionResult (ObjectResult.StatusCode = 200) contendo informações de sucesso. No contendo será retorna um Result com status  = true
		/// </summary>
		/// <param name="message">Mensagem a ser mostrada no retorno</param>
		/// <param name="data">Instancia do objeto que será passado para o Result<TData></param>
		/// <returns></returns>
		public static ActionResult<ResultResponse> SucessoResult(string message = "", dynamic data = null, int statusCode = 200)
        {
			var retorno = SucessoResponse(message, data);

			retorno.StatusCode = statusCode;

			var result = new ObjectResult(retorno) { StatusCode = statusCode };

			return result;
		}

		public static ActionResult<ResultResponse<TDataResult>> SucessoResultData<TDataResult>(string message, TDataResult data, int statusCode)
		{
			var retorno = SucessoResponseData(message, data);

			retorno.StatusCode = statusCode;

			var result = new ObjectResult(retorno) { StatusCode = statusCode };

			return result;
		}

		#endregion

		#region Tratativas de Erro
		public static ResultResponse ErroResponse(string message = "", int statusCode = 200) => ErroResponse(message, null, statusCode);
		public static ResultResponse ErroResponse(string message = "", dynamic data = null, int statusCode = 200)
		{
			ResultResponse result;

			//No caso de rrors, se o Data for ValidationResult, direciona o valor para as validações
			if ((data is ValidationResult))
			{
				result = new ResultResponse(false, message);
				result.Validation = data;
			}
			else
			{
				result = new ResultResponse(false, message, data);
			}

			result.StatusCode = statusCode;

			return result;
		}

		public static ResultResponse<TDataBody> ErroResponseData<TDataBody>(string message = "", dynamic data = null, int statusCode = 200)
		{
			ResultResponse<TDataBody> result;

			//No caso de rrors, se o Data for ValidationResult, direciona o valor para as validações
			if ((data is ValidationResult))
			{
				result = new ResultResponse<TDataBody>(false, message);
				result.Validation = data;
			}
			else
			{
				result = new ResultResponse<TDataBody>(false, message, data);
			}

			result.StatusCode = statusCode;

			return result;
		}

		public static Result<TData> Erro(string message = "")=> Erro(message, null);
		/// <summary>
		/// Tratativa para retorno de Resultado de Erro
		/// </summary>
		/// <param name="message"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		public static Result<TData> Erro(string message = "", dynamic data = null) {

			Result<TData> result;

			//No caso de rrors, se o Data for ValidationResult, direciona o valor para as validações
			if ((data is ValidationResult))
            {
				result = new Result<TData>(false, message);
				result.Validation = data;
            }
            else
            {
				result = new Result<TData>(false, message, data);
			}
			return result;
		}
		/// <summary>
		/// Retorno de API - Retorna um IActionResult com status code 200 porem com valor de status False
		/// </summary>
		/// <param name="message">Mensagem do erro</param>
		/// <returns>IActionResult (ObjectResult.StatusCode = 200) com um Result<object>.Status = true</returns>
		public static ActionResult<ResultResponse> ErroResult(string message = "") => ErroResult(message, null);
		/// <summary>
		/// Retorno de API - Retorna um IActionResult com status code 200 porem com valor de status False
		/// </summary>
		/// <param name="message">Mensagem do erro</param>
		/// <param name="data">Instancia do objeto que será passado para o Result<TData></param>
		/// /// <returns>IActionResult (ObjectResult.StatusCode = 200) com um Result<data>.Status = true</returns>
		public static ActionResult<ResultResponse> ErroResult(string message = "", dynamic data = null, int statusCode = 200) {

			var retorno = ErroResponse(message, data);

			retorno.StatusCode = statusCode;

			var result = new ObjectResult(retorno) { StatusCode = statusCode };

			return result;

		}

		public static ActionResult<ResultResponse<TDataBody>> ErroResultData<TDataBody>(string message, TDataBody data, int statusCode = 200)
		{

			var retorno = ErroResponseData<TDataBody>(message, data);

			retorno.StatusCode = statusCode;

			var result = new ObjectResult(retorno) { StatusCode = statusCode };

			return result;

		}
		#endregion
	}
}
