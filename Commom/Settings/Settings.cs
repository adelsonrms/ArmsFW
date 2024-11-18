using ArmsFW.Services.Extensions;
using ArmsFW.Services.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ArmsFW.Services.Shared.Settings
{
    public enum RuntimeMode
    {
        Producao = 1, Desenvolvimento = 2
    }

    //public static class App_Old
    //{
    //    public static IConfiguration Configuration { get; set; }
    //    private static AppConfigService _AppConfig;

    //    public static string wwwroot => "";

    //    public static AppSettings Config => AppSettings.Instance;
    //    public static AppConfigService Settings { get => _AppConfig; set { _AppConfig = value; } }
    //    public static AppConfigService LoadSettings(string fileStore)
    //    {
    //        _AppConfig = AppConfigService.Default.Load(fileStore);
    //        return _AppConfig;
    //    }

    //    public static ILogger<object> Logger { get; set; }
    //    public static string ContentPath { get; set; }

    //    public static string PastaImagems() => $@"{App.ContentPath}\img";

    //    public static string ObtemCaminhoImagem(string id, string subpasta = "funcionarios", bool incluirTagAtualizacao = false)
    //    {
    //        string nomeArquivo = $"{id}.png";
    //        string urlArquivo = "";
    //        string caminhoImagem = $"/img{(!string.IsNullOrEmpty(subpasta) ? "/" + subpasta : "")}";

    //        //Tenta primeiro .PNG
    //        if (System.IO.File.Exists($"{App.ContentPath}{caminhoImagem}/{id}.png"))
    //        {
    //            urlArquivo = $"{caminhoImagem}/{id}.png";
    //        }
    //        else if (System.IO.File.Exists($"{App.ContentPath}{caminhoImagem}/{id}.jpg"))
    //        {
    //            //Se nao existir, tenta .JPG
    //            urlArquivo = $"{caminhoImagem}/{id}.jpg";
    //        }
    //        else if (File.Exists($"{App.ContentPath}{caminhoImagem}/generico.jpg"))
    //        {
    //            //Se nao existir, tenta o generico
    //            urlArquivo = $"{caminhoImagem}/generico.jpg";
    //        }

    //        //Isso faz com que o timestamp de gravação do arquivo (Data/Hora) seja incluido na URL, forçando assim com que a mesma seja atualizada no cache a cada request 
    //        if (incluirTagAtualizacao)
    //        {
    //            urlArquivo += $"?dt={File.GetCreationTime($"{App.ContentPath}/{urlArquivo}").ToString("yyyyMMddhhmmss")}";
    //        }

    //        return urlArquivo;
    //    }


    //    public static TaskResult SalvarArquivo(byte[] conteudo, string fileName)
    //    {
    //        var result = TaskResult.Create();

    //        try
    //        {
    //            if (File.Exists(fileName)) File.Delete(fileName);

    //            var dados = conteudo.GetStream().SaveToDiskAsync(fileName);

    //            if (dados.Length == conteudo.Length)
    //            {
    //                //var fu = new FileUpload(fileName);

    //                return result.GetResult(true, "O arquivo foi gravado com sucesso no disco!", fileName);
    //            }
    //            else
    //            {
    //                return result.GetResult(false, "Ocorreu um erro ao salvar o arquivo", null);
    //            }
    //        }
    //        catch (System.Exception ex)
    //        {
    //            return result.GetResult(false, "Exception - Ocorreu uma falha nao tratada (Exception) ao salvar o arquivo no disco", ex.Message);
    //        }
    //    }

    //    public static TaskResult SalvarImagem(byte[] conteudo, string nomeArquivo, string subPasta = "")
    //    {
    //        var result = TaskResult.Create();

    //        try
    //        {
    //            var Pasta = $@"{App.PastaImagems()}{(!string.IsNullOrEmpty(subPasta) ? "\\" + subPasta : "")}";

    //            //Cria o sub-diretorio, caso nao exista
    //            if (!Directory.Exists(Pasta)) Directory.CreateDirectory(Pasta);

    //            var fileName = $"{Pasta}\\{nomeArquivo}";

    //            return SalvarArquivo(conteudo, fileName);
    //        }
    //        catch (System.Exception ex)
    //        {
    //            return result.GetResult(false, "Exception - Ocorreu uma falha nao tratada (Exception) ao fazer o upload da imagem", ex.Message);
    //        }
    //    }


    //}

    public class AppSettings
    {
        private static IConfiguration _configuration;
        
        private static IHostEnvironment _environment;
        
        private static IHostEnvironment _hostenvironment;

        public static AppSettings Instance => new AppSettings();

        public AppSettings(){

            _configuration = AppSettings.Configuration;
            _environment = AppSettings.Environment;
            _hostenvironment = AppSettings.HostEnvironment;
        }
        public AppSettings(IConfiguration configuration, IHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }

        public static string Ambiente => App.Config.Get("EFConfig:Conexao");
        public static string ConnectionString => App.Config.Get($"ConnectionStrings:{App.Config.Get("EFConfig:Conexao")}");

        public static IHostEnvironment HostEnvironment { get; set; }
        public static IConfiguration Configuration { get => _configuration; set { _configuration = value; } }
        public static IHostEnvironment Environment { get => _environment; set { _environment = value; } }
        public static IConfigurationSection GetSection(string key) => _configuration?.GetSection(key);
        public static List<T> GetSectionList<T>(string sessao) => GetSection(sessao).Get<List<T>>();

        //public static T GetSection<T>(string sessao) => GetSection(sessao).Get<T>();

        public static T GetSection<T>(string sessao) => GetSection(sessao).Get<T>();

        public static T GetSection<T>() => GetSection(typeof(T).Name).Get<T>();

        public AppConfig Runtime => new AppConfig();

        public DataAccess DataAccess => new DataAccess();
        public MailServer MailServer => new MailServer();
        public Startup Startup => new Startup();

        public string AuthServer {
            get
            {
                switch (DataAccess.DBAmbiente)
                {
                    case "0": return Get("AuthServer_Dev:Authority");
                    case "1": return Get("AuthServer_Producao:Authority");
                    case "2": return Get("AuthServer_Homologacao:Authority");
                    default:
                        return Get("AuthServer_Dev:Authority");
                }
            }
        }

        public string AppId
        {
            get
            {
                switch (DataAccess.DBAmbiente)
                {
                    case "0": return Get("AuthServer_Dev:AppId");
                    case "1": return Get("AuthServer_Producao:AppId");
                    case "2": return Get("AuthServer_Homologacao:AppId");
                    default:
                        return Get("AuthServer_Dev:AppId");
                }
            }
        }

        public TRetorno GetValor<TRetorno>(string chave) => _configuration.GetValue<TRetorno>(chave);
        public string Get(string chave) => _configuration.GetValue<string>(chave);

        public List<T> Get<T>(string chave) => GetSectionList<T>(chave);

        public T GetType<T>(string chave) => GetSection<T>(chave);

    }

    public class TPASettings
    {
        public bool DesabilitarLancamentosDiasNaoUteis { get; set; } = false;
    }

    public class DataAccess
    {
        public string DBAmbiente => App.Config.Runtime.Ambiente;

        [JsonIgnore]
        public string ConnectionStringOld
        {
            get
            {
                string cs = "";
                cs = AppSettings.GetSection($"ConnectionStrings:{DBAmbiente}").Value;
                return cs;
            }
        }

        public string Ambiente
        {
            get
            {
                return AppSettings.GetSection($"AppSettings:AmbienteNome").GetChildren().ToList()[DBAmbiente.ToInt()].Value;
            }
        }
        public string Servidor => PegarValorConexao("Data Source") ?? PegarValorConexao("Server");

        public string DB => PegarValorConexao("Initial Catalog") ?? PegarValorConexao("Database");
        public string Usuario => PegarValorConexao("User ID");

        private string PegarValorConexao(string chave)
        {
            var arrC = ConnectionStringOld.Split(";".ToCharArray());

            foreach (var item in arrC)
            {
                if (item.ToLower().Split("=".ToCharArray())[0] == chave.ToLower())
                {
                    return item.Split("=".ToCharArray())[1];
                }
            }

            return null;
        }

        public override string ToString()
        {
            return $"{Ambiente} | Servidor : {this.Servidor} | DB : {this.DB} (Usuario : {this.Usuario})";
        }
    }
    public class MailServer
    {
        public  string SMTP_SERVIDOR = AppSettings.Configuration.GetSection("MailServer").Get<SMTPMail>().SMTP_SERVIDOR;
        public  string SMTP_USUARIO => AppSettings.Configuration.GetSection("MailServer").Get<SMTPMail>().SMTP_USUARIO;
        public  string SMTP_SENHA = AppSettings.Configuration.GetSection("MailServer").Get<SMTPMail>().SMTP_SENHA;
        public  string SMTP_DISPLAYNAME = AppSettings.Configuration.GetSection("MailServer").Get<SMTPMail>().SMTP_DISPLAYNAME;
    }
    public class Startup
    {
        public  bool EnableDeveloperExceptionPage => AppSettings.Configuration.GetSection("Startup").GetValue<bool>("EnableDeveloperExceptionPage");
        public  bool EnableLogger => AppSettings.Configuration.GetSection("Startup").GetValue<bool>("EnableLogger");
    }
    public class AppConfig
    {
        public string Ambiente => AppSettings.GetSection("AppSettings").GetValue<string>("Ambiente");
        public RuntimeMode Mode => Ambiente == "prod" ? RuntimeMode.Producao : RuntimeMode.Desenvolvimento;
    }
    public class SMTPMail
    {
        public string SMTP_SERVIDOR { get; set; }
        public string SMTP_USUARIO { get; set; }
        public string SMTP_SENHA { get; set; }
        public string SMTP_DISPLAYNAME { get; set; }
    }
}
