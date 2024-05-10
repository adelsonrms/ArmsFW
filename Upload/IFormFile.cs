using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ArmsFW.Services.Upload
{
	public interface IFormFile
	{
		string ContentType { get; }

		string ContentDisposition { get; }

		IHeaderDictionary Headers { get; }

		long Length { get; }

		string Name { get; }

		string FileName { get; }

		void CopyTo(Stream target);

		Task CopyToAsync(Stream target, CancellationToken cancellationToken = default(CancellationToken));

		Stream OpenReadStream();
	}
}
