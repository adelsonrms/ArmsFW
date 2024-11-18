using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ArmsFW.Core;
using ArmsFW.Services.Extensions;
using ArmsFW.Services.Extensions.FileSystem;
using ArmsFW.Services.Logging;
using ArmsFW.Services.Session;
using ArmsFW.Services.Shared.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ArmsFW.Services.Shared.Settings
{
    public static class App
	{
		private static string ctx = "ArmsFW.Services.Shared.Settings.App";

		private static Config _AppConfig;

		private static SessionService _Session;

		private static FileStore _fileStore;
        private static string _sessionID;
		private static string _userID;

		public static IConfiguration Configuration { get; set; }

		public static string wwwroot => "";
		public static string Ambiente => AppSettings.Ambiente;

		public static string ConnectionString => AppSettings.ConnectionString;
        public static string MySqlConnectionString => App.Config.Get($"ConnectionStrings:mysql");
        public static string EfContext => App.Config.Get($"EFConfig:Tipo");

        public static bool HostHomologacao(string url)
        {
            string termos = App.Config.Get($"AppSettings:TermosHomologacao");

            var arrTermos = termos.Split(",").ToList();

            foreach (var termo in arrTermos)
            {
                if (url.Contains(termo))
                {
                    return true;
                }
            }
            return false;
        }

        public static AppSettings Config => AppSettings.Instance;

		public static Config Configuracoes
		{
			get
			{
				_AppConfig = _AppConfig ?? new Config().Carregar();
				return _AppConfig;
			}
			set
			{
				_AppConfig = value;
			}
		}

		public static SessionService Session
		{
			get
			{
				return _Session ?? SessionService.Default;
			}
			set
			{
				_Session = value;
			}
		}

        public static FileStore FileStore
		{
			get
			{
				return _fileStore ?? FileStore.Default;
			}
			set
			{
				_fileStore = value;
			}
		}

		public static ILogger<object> Logger { get; set; }

		public static string ContentPath { get; set; }
		public static string ContentWeb { get; set; }

		public static string ApplicationPath { get; set; }

		public static string AppLogFile
		{
			get
			{
				return Configuracoes.Pegar("AppLogFile");
			}
			set
			{
				Configuracoes.Salvar("AppLogFile", value);
			}
		}

        public static bool IsDev
		{
			get
			{
				return AppSettings.Environment.IsDevelopment();
			}
		}

		public static string SessionID
		{
			get
			{
				return _sessionID ?? "";
			}
			set
			{
				_sessionID = value;
			}
		}

		public static string UsuarioDaSessao
		{
			get
			{
				return _userID ?? "";
			}
			set
			{
				_userID = value;
			}
		}

		public static bool GravarLog(string msg, string contexto = "", string fileLog = "", bool criarNovo = false, bool backup = true)
		{
			try
			{
				

				if (Configuracoes.Pegar("EnableDebug") == "true")
				{
					LogServices.Debug($"{contexto} | {msg}");

					var diretorio = Aplicacao.Diretorio;

					contexto = ((contexto == "") ? "Aplicação" : contexto);
					if (string.IsNullOrEmpty(fileLog))
					{
						fileLog = AppLogFile;
						if (string.IsNullOrEmpty(fileLog)) fileLog = $@"{diretorio}\_logs\log.json";
					}
                    else
                    {
						//Se nao tem barra, entao so vem o nome do arquivo, sava no diretorio raiz de logs
                        if (!fileLog.Contains("\\")) fileLog = $@"{ApplicationPath}\_logs\{fileLog}";
					}

					LogServices.GravarLog(msg, fileLog, contexto, criarNovo, backup);

					return true;
				}

			}
			catch (Exception ex)
			{
				ex.Logar();
			}

			return false;
		}

		public static Config LoadSettings(string fileStore)
		{
			return Settings.Config.Default.Carregar(fileStore);
		}

		public static string PastaImagems()
		{
			return ContentPath + "\\img";
		}

		public static string ObtemCaminhoImagem(string id, string subpasta = "funcionarios", bool incluirTagAtualizacao = false)
		{
			_ = id + ".png";

			string caminhoDoArquivo = "";
			string caminho = "/img" + ((!string.IsNullOrEmpty(subpasta)) ? ("/" + subpasta) : "");

            if (File.Exists(ContentWeb + caminho + "/" + id + ".png"))
			{
				caminhoDoArquivo = caminho + "/" + id + ".png";
			}
			else if (File.Exists(ContentWeb + caminho + "/" + id + ".jpg"))
			{
				caminhoDoArquivo = caminho + "/" + id + ".jpg";
			}
			else if (File.Exists(ContentWeb + caminho + "/generico.jpg"))
			{
				caminhoDoArquivo = caminho + "/generico.jpg";
			}
			if (incluirTagAtualizacao)
			{
				caminhoDoArquivo = caminhoDoArquivo + "?dt=" + File.GetCreationTime(ContentWeb + "/" + caminhoDoArquivo).ToString("yyyyMMddhhmmss");
			}
			return caminhoDoArquivo;
		}

		public static string BackupFilePorTamanho(string file, long tamanho)
		{
			try
			{
				FileInfo fileInfo = new FileInfo(file);
				if (!fileInfo.Exists)
				{
					return file;
				}
				if (fileInfo.Length > tamanho)
				{
					string text = file.Replace(fileInfo.Name, fileInfo.Name.GetFileName(eFileNameType.NameWithoutExtension) + "_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + "." + fileInfo.Name.GetExtension());
					File.Move(file, text);
					GravarLog("...Continuação do log de : " + text, ctx + ".BackupFilePorTamanho()", file, criarNovo: true, backup: false);
					return file;
				}
				return file;
			}
			catch (Exception)
			{
				return file;
			}
		}

		public static void LimparArquivosAntigos(string pasta, long tamanho)
		{
			try
			{
				IEnumerable<ArquivoViewModel> enumerable = ListaArquivosPorPasta(pasta, "*.txt").Result.Where((ArquivoViewModel x) => x.Data < DateTime.Today);
				if (enumerable.Count() <= 0)
				{
					return;
				}
				GravarLog("Limpando arquivos antigos...Inferir a data de hoje", ctx + ".LimparArquivosAntigos()");
				foreach (ArquivoViewModel item in enumerable)
				{
					if (item.Delete())
					{
						GravarLog("Excluído...." + item.Nome, ctx + ".LimparArquivosAntigos()");
					}
				}
			}
			catch (Exception)
			{
			}
		}

		public static async Task<List<ArquivoViewModel>> ListaArquivosPorPasta(string pasta, string tipo)
		{
			string text = "ListaArquivosPorPasta()";
			if (!Directory.Exists(pasta))
			{
				GravarLog("A pasta/diretorio nao existe. Pesquisa cancelada...", ctx + "." + text);
				return await Task.FromResult(new List<ArquivoViewModel>());
			}
			IEnumerable<string> enumerable;
			try
			{
				enumerable = Directory.EnumerateFiles(pasta, tipo, SearchOption.AllDirectories);
			}
			catch (Exception ex)
			{
				GravarLog("EXCEPTION ! Falha inesperada ao executar Directory.EnumerateFile(" + pasta + ", " + tipo + ") : Source : " + ex.Source + " / Mensagem : " + ex.Message + " / Stack : " + ex.StackTrace, ctx + ".ListaArquivosPorPasta()");
				return await Task.FromResult(new List<ArquivoViewModel>());
			}
			if (enumerable != null && enumerable.Count() == 0)
			{
				GravarLog("Nenhum arquivo " + tipo + " encontrado. arquivos?.Count()==0. Consulta cancelada. Retorna a lista vazia !", ctx + "." + text);
				return await Task.FromResult(new List<ArquivoViewModel>());
			}
			return await Task.FromResult(enumerable.Select((string f) => FileStore.getFileByPath(f)).ToList());
		}

		public static void ClearFiles(string directory)
		{
			try
			{
				Directory.Delete(directory, recursive: true);
				Directory.CreateDirectory(directory);
			}
			catch
			{
			}
		}

		public static TaskResult SalvarArquivo(byte[] conteudo, string fileName)
		{
			TaskResult taskResult = TaskResult.Create();
			try
			{
				fileName = fileName.Replace("/", "\\");
				
				if (File.Exists(fileName)) File.Delete(fileName);

				conteudo.GetStream().SaveToDiskAsync(fileName);
				return taskResult.GetResult(success: true, "O arquivo foi gravado com sucesso no disco!", fileName);
			}
			catch (Exception ex)
			{
				return taskResult.GetResult(success: false, "Exception - Ocorreu uma falha nao tratada (Exception) ao salvar o arquivo no disco", ex.Message);
			}
		}

		public static TaskResult SalvarImagem(byte[] conteudo, string nomeArquivo, string subPasta = "")
		{
			TaskResult taskResult = TaskResult.Create();
			try
			{
				string text = PastaImagems() + ((!string.IsNullOrEmpty(subPasta)) ? ("\\" + subPasta) : "");
				if (!Directory.Exists(text))
				{
					Directory.CreateDirectory(text);
				}
				string fileName = text + "\\" + nomeArquivo;
				return SalvarArquivo(conteudo, fileName);
			}
			catch (Exception ex)
			{
				return taskResult.GetResult(success: false, "Exception - Ocorreu uma falha nao tratada (Exception) ao fazer o upload da imagem", ex.Message);
			}
		}

		public static T Instanciar<T>(Type p)
		{
			try
			{
				return (T)Activator.CreateInstance(p);
			}
			catch (Exception)
			{

			}
			return default(T);
		}

		public static List<Type> CarregarClasses<TClassType>(Assembly arquivo = null) 
        {
            if (arquivo==null)
            {
				arquivo = Assembly.GetExecutingAssembly();
			}

			var classes = (from t in arquivo.GetExportedTypes()
						  where t.IsClass && typeof(TClassType).IsAssignableFrom(t)
						  select t).ToList();

			return classes;
		}
	}
}
