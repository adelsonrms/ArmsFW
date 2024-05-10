using System.Collections.Generic;
using ArmsFW.Lib.Web.HttpRest;

namespace ArmsFW.Services.Azure
{
	public class GraphEntity
	{
		public string Id { get; set; }

		public string ODataType { get; set; }

		public IDictionary<string, object> AdditionalData { get; set; }

		public ResponseOld Response { get; set; }

		public bool IsValid()
		{
			return !string.IsNullOrEmpty(Id);
		}
	}
}
