using System;
using System.Security.Cryptography;
using System.Text;

namespace ArmsFW.Services.Shared.Util
{
	public class GuidUtil
	{
		public static Guid GetGuidFromStrng(string value)
		{
			return new Guid(MD5.Create().ComputeHash(Encoding.Default.GetBytes(value)));
		}
	}
}
