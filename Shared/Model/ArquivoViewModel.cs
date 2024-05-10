using System;
using System.IO;
using ArmsFW.Services.Extensions.FileSystem;

namespace ArmsFW.Services.Shared.Model
{
	public class ArquivoViewModel
	{
		public string Id { get; set; }

		public string Nome { get; set; }

		public bool Store => !string.IsNullOrEmpty(Id);

		public string Status { get; set; }

		public string Pasta { get; set; }

		public string FilePath { get; set; }

		public DateTime Data { get; set; }

		public int Tamanho { get; set; }

		public dynamic Parent { get; set; }

		public string NomeArquivoId => Nome.Replace(" ", "").Replace("-", "").Replace("/", "")
			.Replace("+", "")
			.Replace("\\", "")
			.Replace("|", "");

		public ArquivoViewModel(string filename)
		{
			if (File.Exists(filename))
			{
				FilePath = filename;
				Nome = filename.GetFileName();
				Pasta = filename.GetFolder();
				Tamanho = filename.GetLength();
				Data = File.GetCreationTime(filename);
			}
		}

		public static ArquivoViewModel CriarStore(string filename)
		{
			return new ArquivoViewModel(filename)
			{
				Id = Guid.NewGuid().ToString().Replace(" ", "")
					.Replace("-", "")
			};
		}

		public bool Existe()
		{
			return File.Exists(FilePath);
		}

		public byte[] GetConteudo()
		{
			try
			{
				return File.ReadAllBytes(FilePath);
			}
			catch (Exception)
			{
			}
			return new byte[0];
		}

		public override string ToString()
		{
			return base.ToString();
		}

		internal bool Delete()
		{
			try
			{
				File.Delete(FilePath);
				return true;
			}
			catch
			{
			}
			return false;
		}
	}
}
