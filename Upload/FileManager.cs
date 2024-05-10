using System;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ArmsFW.Services.Upload
{
	public class FileManager
	{
		public string FolderDestination { get; private set; }

		public string WebPathRoot { get; private set; }

		public IFormFile uploadedFile { get; private set; }

		public FileManager(string webPathRoot, string folderDestination, dynamic source)
		{
			WebPathRoot = webPathRoot;
			FolderDestination = folderDestination;
			uploadedFile = source;
		}

		public async Task<FileUpload> SaveUploadedFileAsync(string destinationFileName)
		{
			try
			{
				if (string.IsNullOrEmpty(destinationFileName))
				{
					destinationFileName = GetFileName(uploadedFile);
				}
				string pathAndFilename = GetPathAndFilename(destinationFileName);
				if (File.Exists(pathAndFilename))
				{
					File.Delete(pathAndFilename);
				}
				using FileStream stream = new FileStream(pathAndFilename, FileMode.Create);
				FileUpload arq = new FileUpload(pathAndFilename);
				await uploadedFile.CopyToAsync(stream);
				BinaryReader binaryReader = new BinaryReader(uploadedFile.OpenReadStream());
				arq.BinaryContent = binaryReader.ReadBytes((int)uploadedFile.Length);
				arq.Size = arq.BinaryContent.Length;
				return arq;
			}
			catch (Exception)
			{
				return new FileUpload("");
			}
		}

		private string EnsureCorrectFilename(string filename)
		{
			if (filename.Contains("\\"))
			{
				filename = filename.Substring(filename.LastIndexOf("\\") + 1);
			}
			return filename;
		}

		private string GetPathAndFilename(string filename)
		{
			string text = Path.Combine(WebPathRoot, FolderDestination);
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text + "\\" + filename;
		}

		private string GetFileName(IFormFile source)
		{
			string filename = ContentDispositionHeaderValue.Parse(source.ContentDisposition).FileName.ToString()
				.Trim('"');
			return EnsureCorrectFilename(filename);
		}
	}
}
