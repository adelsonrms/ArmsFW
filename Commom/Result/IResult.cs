namespace ArmsFW.Services.Shared
{
	public interface IResult<TData>
	{
		bool Status { get; set; }

		string Message { get; set; }

		TData Data { get; set; }
	}


	public interface IResultResponse<TData>: IResult<TData>
	{
		int StatusCode { get; set; }
	}
}
