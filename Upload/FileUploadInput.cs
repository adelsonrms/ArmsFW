using System.Collections.Generic;

namespace ArmsFW.Services.Upload
{
	public class FileUploadInput
	{
		public List<IFormFile> files { get; set; }

		public dynamic data { get; set; }

		public string RedirectTo { get; set; }

		public string RedirectToAction { get; set; }

		public string RedirectToController { get; set; }

		public string base64Code { get; set; }

		public string UploadType { get;  set; }

		public string Destination { get;  set; }
	}
}
