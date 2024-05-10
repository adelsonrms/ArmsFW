using System;
using System.IO;
using System.Threading.Tasks;
using ArmsFW.Services.Extensions;
using ArmsFW.Services.Extensions.FileSystem;
using ArmsFW.Services.Shared;
using ArmsFW.Services.Shared.Exceptions;
using ArmsFW.Services.Shared.Settings;

namespace ArmsFW.Services.Upload
{
	public class UploadService
	{
		public static async Task<TaskResult> UploadForm(IFormFile file, string pastaDestino = null, string nomeDestino = null)
		{
			TaskResult taskResult = TaskResult.Create();
			FileUpload arquivo = null;
			try
			{
				arquivo = new FileUpload(file.FileName);
				pastaDestino = pastaDestino ?? (App.ContentPath + "\\tmp");
				nomeDestino = nomeDestino ?? (DateTime.Now.ToString("yyyyMMdd-hhmmss") + "." + file.FileName.GetExtension());
				if (!Directory.Exists(pastaDestino))
				{
					Directory.CreateDirectory(pastaDestino);
				}
				Path.Combine(pastaDestino, nomeDestino);
				arquivo = await new FileManager("", pastaDestino, file).SaveUploadedFileAsync(nomeDestino);
				taskResult.Message = "Arquivo foi enviado (upload) com sucesso.";
				taskResult.Success = true;
			}
			catch (Exception ex)
			{
				taskResult.Message = ex.Message;
				taskResult.AddException(new ApplicationtException("Ocorreu uma Expception no upload do arquivo'", ex, new
				{
					Rotina = "UploadService.Upload()",
					Variaveis = new { file, pastaDestino, nomeDestino },
					FileUpload = arquivo
				}));
			}
			taskResult.Data = arquivo;
			return taskResult;
		}

		public static async Task<TaskResult> UploadBase64(string base64Code, string destination)
		{
			TaskResult tsk = TaskResult.Create();
			try
			{
				if (!string.IsNullOrEmpty(base64Code))
				{
					if (base64Code.StartsWith("data:"))
					{
						base64Code = base64Code.Split(",".ToCharArray())[1] ?? "";
					}
					if (base64Code.EndsWith(";"))
					{
						base64Code = base64Code.Substring(0, base64Code.Length - 1);
					}
					byte[] array = Convert.FromBase64String(base64Code);
					if (string.IsNullOrEmpty(destination))
					{
						destination = App.ContentPath + "\\files\\" + Path.GetTempFileName();
					}
					destination = destination.Replace("/", "\\");
					TaskResult taskResult = App.SalvarArquivo(array, destination);
					if (taskResult.Success)
					{
						string newFile = (string)taskResult.Data;
						if (File.Exists(newFile))
						{
							FileUpload fileUpload = (await new UploadService().GetFile(array, newFile)).Data;
							if (fileUpload.Exists())
							{
								return tsk.SetResultInfo(success: true, "O arquivo foi salvo com sucesso no disco", new
								{
									arquivo = fileUpload.Name,
									status = "Ok"
								});
							}
							return tsk.SetResultInfo(success: false, "Ocorreu uma falha inesperada em UploadService.GetFile()", new
							{
								arquivo = Path.GetFileName(newFile),
								status = "erro"
							});
						}
						return tsk.SetResultInfo(success: false, "Ocorreu uma falha inesperada em : (string)rstFile.Data", new
						{
							arquivo = Path.GetFileName(newFile),
							status = "erro"
						});
					}
					return tsk.SetResultInfo(success: false, "Ocorreu uma falha inesperada em : App.SalvarArquivo(conteudoBytes, destination)", new
					{
						arquivo = "(nao existe)",
						status = "erro"
					});
				}
				return tsk.SetResultInfo(success: false, "O código Base64 do arquivo nao foi informado", new
				{
					arquivo = "(Base64 invalido)",
					status = "erro"
				});
			}
			catch (Exception ex)
			{
				return tsk.SetResultInfo(success: false, ex.Message ?? "", new
				{
					arquivo = "(Falha)",
					status = "erro"
				});
			}
		}

		public async Task<TaskResult> GetFile(byte[] file)
		{
			return await GetFile(file, Path.GetTempFileName());
		}

		public async Task<TaskResult> GetFile(byte[] file, string fileName)
		{
			TaskResult taskResult = TaskResult.Create();
			FileUpload fileUpload = null;
			try
			{
				fileUpload = new FileUpload(fileName)
				{
					StreamContent = file.GetStream()
				};
				fileUpload.BinaryContent = file;
				fileUpload.Size = fileUpload.BinaryContent.Length;
				taskResult.Message = $"Arquivo processado com sucesso. Tamanho : {file.Length}";
				taskResult.Success = true;
			}
			catch (Exception ex)
			{
				taskResult.Message = ex.Message;
				taskResult.AddException(new ApplicationtException("Ocorreu uma Expception ao obter o Stream do arquivo'", ex, new
				{
					Rotina = "UploadService.GetStreamFormFile()",
					FileUpload = fileUpload
				}));
			}
			taskResult.Data = fileUpload;
			return await Task.FromResult(taskResult.GetResult());
		}
	}
}
