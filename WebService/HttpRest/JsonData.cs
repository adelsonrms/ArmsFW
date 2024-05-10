using ArmsFW.Lib.Web.Json;

namespace ArmsFW.Lib.Web.HttpRest
{
	public class JsonData
	{
		public dynamic Object { get; set; }

		public string Json => Object.ToJson();
	}
}
