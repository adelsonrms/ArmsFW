using System;
using System.IO;

namespace ArmsFW.Services.Upload
{
	public class FileUpload
	{
		public string Name { get; private set; }

		public string FullName { get; private set; }

		public string FolderPath { get; private set; }

		public int Size { get; set; }

		public string Extension { get; private set; }

		public byte[] BinaryContent { get; set; }

		public Stream StreamContent { get; set; }

		public DateTime CreationTime { get; private set; }

		public FileUpload(string arquivo)
		{
			DefinirInfoArquivo(arquivo);
		}

		private void DefinirInfoArquivo(string arquivo)
		{
			if (!string.IsNullOrEmpty(arquivo))
			{
				try
				{
					Name = Path.GetFileName(arquivo);
					FullName = arquivo;
					FolderPath = Path.GetDirectoryName(arquivo);
					Extension = Path.GetExtension(arquivo);
					CreationTime = File.GetCreationTime(arquivo);
				}
				catch
				{
				}
			}
		}

		private string ObtemExtensaoDoArquivo(string filePath)
		{
			string text = InverteString(filePath);
			return InverteString(text.Substring(0, text.IndexOf(".", StringComparison.Ordinal)));
		}

		private string InverteString(string s)
		{
			char[] array = s.ToCharArray();
			Array.Reverse(array);
			return new string(array);
		}

		public bool Exists()
		{
			return File.Exists(FullName);
		}
	}
}
