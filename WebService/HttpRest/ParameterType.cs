namespace ArmsFW.Lib.Web.HttpRest
{
	public enum ParameterType
	{
		Cookie,
		GetOrPost,
		UrlSegment,
		HttpHeader,
		RequestBody,
		QueryString,
		QueryStringWithoutEncode
	}
}
