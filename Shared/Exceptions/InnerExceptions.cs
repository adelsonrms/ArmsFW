using System;
using System.Collections.Generic;
using System.Text;

namespace ArmsFW.Services.Shared
{
	public class InnerExceptions
	{
		public Dictionary<int, InfoException> Exceptions { get; set; }

		public InnerExceptions(Exception ex)
		{
			Exceptions = ex.GetInnerExceptions();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder("------- BEGIN  STACK DE EXCEPTIONS ------------------------------------------------------------------------------------\n");
			foreach (KeyValuePair<int, InfoException> exception in Exceptions)
			{
				stringBuilder.AppendLine(exception.Value.ToString() ?? "");
			}
			stringBuilder.AppendLine("------- END  STACK DE EXCEPTIONS ------------------------------------------------------------------------------------\n");
			return stringBuilder.ToString();
		}
	}
}
