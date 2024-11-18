using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace ArmsFW.Services.Shared.Model
{
	public class FileStore
	{
		public string FilePath { internal get; set; }

		public DateTime UpdateDateTime { internal get; set; }

		public bool Loaded { get; private set; }

		public List<ArquivoViewModel> Files { get; set; }

		public static FileStore Default => new FileStore();

		public FileStore Load(string file = "")
		{
			try
			{
				if (string.IsNullOrEmpty(FilePath))
				{
					if (string.IsNullOrEmpty(file))
					{
						file = Path.GetDirectoryName(Default.GetType().Assembly.Location) + "\\fileStore.json";
					}
					FilePath = file;
				}
				else
				{
					file = FilePath;
				}
				if (File.Exists(file))
				{
					string text = File.ReadAllText(file);
					if (!string.IsNullOrEmpty(text))
					{
						FileStore fileStore = JsonConvert.DeserializeObject<FileStore>(text);
						fileStore.UpdateDateTime = File.GetCreationTime(file);
						fileStore.Loaded = true;
						fileStore.FilePath = file;
						FilePath = file;
						return fileStore;
					}
					return this;
				}
				Files = new List<ArquivoViewModel>();
				UpdateDateTime = File.GetCreationTime(file);
				Loaded = true;
				Persist();
				return this;
			}
			catch
			{
				return this;
			}
		}

		public ArquivoViewModel addFile(ArquivoViewModel arquivoViewModel)
		{
			List<ArquivoViewModel> files = Files;
			if (files.Find((ArquivoViewModel cf) => cf.FilePath.Equals(arquivoViewModel.FilePath)) == null)
			{
				files.Add(arquivoViewModel);
				Persist();
			}
			return getFileByPath(arquivoViewModel.FilePath);
		}

		public ArquivoViewModel getFileByPath(string filePath)
		{
			ArquivoViewModel arquivoViewModel = Load().Files.FirstOrDefault((ArquivoViewModel cf) => cf.FilePath == filePath);
			if (arquivoViewModel == null)
			{
				return new ArquivoViewModel(filePath);
			}
			return arquivoViewModel;
		}

		public void addFile(string filePah)
		{
			addFile(new ArquivoViewModel(filePah));
		}

		private FileStore Persist()
		{
			FileStore fileStore = this;
			try
			{
				string contents = JsonConvert.SerializeObject((object)fileStore);
				if (File.Exists(fileStore.FilePath))
				{
					File.Delete(fileStore.FilePath);
				}
				Directory.CreateDirectory(Path.GetDirectoryName(fileStore.FilePath));
				File.WriteAllText(fileStore.FilePath, contents);
				fileStore = Load(fileStore.FilePath);
				fileStore.FilePath = FilePath;
				return fileStore;
			}
			catch (Exception)
			{
				return fileStore;
			}
		}
	}
}
