using ArmsFW.Lib.Web.Json;
using ArmsFW.Services.Logging;
using ArmsFW.Services.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ArmsFW.Services.Shared.Settings
{
    public class Config : IConfig
    {

        //Metodos que buscam as informacoes direto na classe
        public static string Pegar(string config) => Config.Default.Pegar(config);
        public static void Salvar(string config, string valor) => Config.Default.Salvar(config, valor);

        public string Arquivo { get; set; }//=> Path.GetDirectoryName(GetType().Assembly.Location) + "\\configuracao.json";

        public static Config Default => new Config();

        public List<Configuracao> Configuracoes { get; set; } = new List<Configuracao>();

        public List<Menu> Menu { get; set; } = new List<Menu>();

        
        public DateTime DataCriacao { get; set; }

        public Config()
        {
            //Arquivo = Path.GetDirectoryName(GetType().Assembly.Location) + "\\configuracao.json";
            DataCriacao = DateTime.Now;
        }

        public Config(string fileSettings)
        {
            Arquivo = fileSettings;
            Carregar(fileSettings);
        }

        public Config Carregar(string file = "")
        {
            try
            {
                //Arquivo
                if (string.IsNullOrEmpty(Arquivo))
                {
                    if (string.IsNullOrEmpty(file)) file = Path.GetDirectoryName(GetType().Assembly.Location) + "\\configuracao.json";
                }
                else
                {
                    file = Arquivo;
                }

                this.Arquivo = file;

                if (File.Exists(file))
                {
                    var jr = JSON.Carregar<Config>(file);

                    if (jr!=null)
                    {
                        Config appConfigService = jr;
                        appConfigService.Arquivo = file;
                        Configuracoes = appConfigService.Configuracoes;
                        Menu = appConfigService.Menu;
                    }
                }

                return this;
            }
            catch (Exception ex) { File.AppendAllTextAsync($"{typeof(Config).Assembly.Location}_log.txt", $"{ex.Source} - {ex.Message}"); }

            return this;
        }

        public static ConfiguracaoBase<T> Load<T>(string file = "") where T : class
        {
            ConfiguracaoBase<T> settingsBase = new ConfiguracaoBase<T>();

            T val2 = (settingsBase.Settings = Activator.CreateInstance<T>());

            try
            {
                if (string.IsNullOrEmpty(file))
                {
                    file = $"{Path.GetDirectoryName(typeof(T).Assembly.Location)}\\{typeof(T).Name}.json";
                }
                if (File.Exists(file))
                {
                    string text = File.ReadAllText(file);
                    if (!string.IsNullOrEmpty(text))
                    {
                        val2 = JsonConvert.DeserializeObject<T>(text);
                        if (val2 != null)
                        {
                            settingsBase.FilePathSettings = file;
                            settingsBase.Settings = val2;
                            settingsBase.Result = TaskResult.Sucesso("Configurações carregadas com sucesso de " + Path.GetFileName(file));

                            return settingsBase;
                        }
                        settingsBase.Result = TaskResult.Erro("Não foi localizado as informações mapeadas do objeto " + typeof(T).Name + " no arquivo " + Path.GetFileName(file));
                        return settingsBase;
                    }
                    return settingsBase;
                }
                settingsBase.Result = TaskResult.Erro("O arquivo nao foi encontrado ! Arquivo : " + file);
                return settingsBase;
            }
            catch (Exception ex)
            {
                ex.Logar();
                settingsBase.Result = TaskResult.Erro("Falha ao carregar as configurações. Detalhe " + ex.Message);
                return settingsBase;
            }
        }

        public void Salvar(string configName, string configValue, bool gravarNoFileStore = true)
        {
            List<Configuracao> settings = Configuracoes;
            Configuracao appSetting = settings.Find((Configuracao cf) => cf.Name == configName);
            if (appSetting == null)
            {
                settings.Add(new Configuracao(configName, configValue));
            }
            else
            {
                appSetting.Value = configValue;
            }

            if (gravarNoFileStore) GravarConfiguracoesNoArquivo();
        }

        public string Pegar(string configName, bool pegarNoCache = false)
        {
            try
            {
                if (pegarNoCache)
                {
                    if (Configuracoes.Any((Configuracao cf) => cf.Name == configName))
                    {
                        return Configuracoes.First((Configuracao cf) => cf.Name == configName).Value;
                    }
                    return string.Empty;
                }

                if (!File.Exists(Arquivo)) return "";

                Carregar(Arquivo);

                if (Configuracoes.Any((Configuracao cf) => cf.Name == configName))
                {
                    return Configuracoes.First((Configuracao cf) => cf.Name == configName).Value;
                }
                return string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public Config GravarConfiguracoesNoArquivo()
        {
            Config appConfigService = this;

            try
            {
                string contents = JsonConvert.SerializeObject((object)appConfigService, Formatting.Indented);

                if (string.IsNullOrEmpty(appConfigService.Arquivo))
                {
                    return appConfigService;
                }

                if (File.Exists(appConfigService.Arquivo)) File.Delete(appConfigService.Arquivo);


                //Directory.CreateDirectory(Path.GetDirectoryName(appConfigService.FileSettings));

                File.WriteAllText(appConfigService.Arquivo, contents);

                //appConfigService = Load(appConfigService.FileSettings);
                return appConfigService;
            }
            catch (Exception)
            {
                return appConfigService;
            }
        }
    }

    public interface IConfig
    {
        string Pegar(string config, bool pegarNoCache = false);
        void Salvar(string config, string valor, bool gravarNoFileStore = true);
    }
}
