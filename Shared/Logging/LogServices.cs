using ArmsFW.Core;
using System.Collections.Generic;
using System.IO;
using System;
using System.Reflection;
using System.Threading.Tasks;
using ArmsFW.Services.Session;
using ArmsFW.Services.Shared.Settings;

namespace ArmsFW.Services.Logging
{
    public static class LogExceptionsExtensions
    {
        public static async Task<string> Logar(this Exception ex) => LogServices.GravarLog($"{ex.TargetSite.Name} >> {ex.Message}").Result.Mensagem;
    }
    /// <summary>
    /// classe de serviços de log, separação e composição de relatórios de erros baseados na exception e em sua stack
    /// </summary>
    public class LogServices
    {
        #region Logs de Texto
        public string SessionID = App.SessionID;
        public string UserID => SessionService.GetUser().UserName;

        public static void Debug(string mensagem) => System.Diagnostics.Debug.WriteLine($"{DateTime.Now} - {mensagem}");

        public static async Task<EntradaLog> GravarLog(string logMessage, string contexto = "", string fileLog = "", bool criarNovo = false, bool backup = true)
        {
            //Direciona para o modelo de log Json

            var entrada = new EntradaLog(logMessage, contexto);

            await GravarLog(entrada, fileLog, criarNovo, backup);

            return entrada;
        }


        public static async Task<EntradaLog> GravarLog(EntradaLog log, string fileLog = "", bool criarNovo = false, bool backup = true)
        {
            try
            {
                if (string.IsNullOrEmpty(fileLog)) fileLog = $@"{Aplicacao.Diretorio}\_logs\log.json";

                FileLog objFile = GetFileLog(backup, fileLog);

                int ultimaLinha = 0;
                string msg = "";

                string fileToLog = objFile.FileName;

                if (objFile.FileLogOrigem != null)
                {
                    ultimaLinha = objFile.FileLogOrigem.GetCountLines() + 1;
                }
                else
                {
                    ultimaLinha = objFile.GetCountLines() + 1;
                }

                var mensagem = log.ToJson(false) + ",";

                Debug(log.Mensagem);

                if (criarNovo)
                {
                    File.WriteAllLines(fileToLog, new List<string> { mensagem }, encoding: System.Text.Encoding.UTF8);
                }
                else
                {
                    File.AppendAllLines(fileToLog, new List<string> { mensagem }, encoding: System.Text.Encoding.UTF8);
                }
            }
            catch
            {
            }

            return log;
        }

        public static async Task LimparLog(string fileLog)
        {
            try
            {
                File.CreateText(fileLog).Close();
            }
            catch
            {
            }
        }

        public static async Task<LogView> CarregarLogView(string fileLog)
        {
            LogView lv = new LogView();

            try
            {
                var conteudo = File.ReadAllText(fileLog);

                var json_log_view = $"{{\"Arquivo\": \"{""}\",\"Linhas\": [{conteudo}]}}";

                lv = JSON.Carregar<LogView>(json_log_view);

                if (lv==null)
                {
                    await LogServices.LimparLog(fileLog);
                    lv = new LogView { Arquivo = fileLog,  };
                    lv.AdicionarEntrada(msg: $"O conteudo desse log é invalido. Nao esta no formato Json. O logo será redefido (limpar)");
                }
            }
            catch (Exception ex)
            {
                lv.Linhas.Add(new EntradaLog { Detalhes = (await ex.Logar()), Mensagem = ex.Message, Origem = ex.TargetSite.Name });
            }

            return lv ?? new LogView();
        }

        private static FileLog GetFileLog(bool backup, string _file)
        {
            FileLog fl = new FileLog();

            string fileLog = _file;

            string dirLog = $"{Path.GetDirectoryName(_file)}";

            if (!Directory.Exists(dirLog)) Directory.CreateDirectory(dirLog);

            fl.Diretorio = dirLog;
            fl.FileName = $"{dirLog}\\{Path.GetFileName(_file)}";

            if (backup) fl = CriarBackup(fl);

            return fl;
        }

        private static FileLog CriarBackup(FileLog fileLog)
        {
            if (fileLog.Existe())
            {
                //Logs acima de 1 MB, cria um novo arquivo
                if (fileLog.Tamanho() > 10000000L)
                {
                    string arqBkp = fileLog.FileName.Replace("log.json", "log_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".json");
                    fileLog.FileLogOrigem = new FileLog { FileName = arqBkp };
                    File.Move(fileLog.FileName, arqBkp);
                }
                else
                {
                    fileLog.FileLogOrigem = null;
                }
            }
            return fileLog;
        }
        #endregion

        #region métodos estáticos públicos para tratamento de erro

        /// <summary>
        /// pega uma exception e varre sua inner exception para montar uma lista até que não haja mais inner exception
        /// </summary>
        /// <param name="err"></param>
        /// <returns></returns>
        public static List<Exception> GetExceptionList(Exception err)
        {
            List<Exception> result = new List<Exception>();
            var tmp = err;


            while (tmp != null)
            {

                result.Add(tmp);
                tmp = tmp.InnerException;

            }


            return result;
        }


        /// <summary>
        /// pega todas as informações da lista de exceptions e transforma em uma lista de strings
        /// </summary>
        /// <param name="errors"></param>
        /// <returns></returns>
        public static List<string> GetExceptionMessages(List<Exception> errors)
        {
            var result = new List<string>();

            foreach (Exception err in errors)
            {
                if (err != null)
                {
                    result.Add("Mensagem:".PadRight(80, '-'));
                    result.Add(err.Message);
                    result.Add("Source:".PadRight(80, '-'));
                    result.Add(err.Source);
                    result.Add("StackTrace:".PadRight(80, '-'));
                    result.Add(err.StackTrace);

                    if (err is DbEntityValidationException)
                    {
                        result.AddRange(ExtraiErrosDbEntityValidation(err as DbEntityValidationException));
                    }
                }
            }

            return result;
        }


        /// <summary>
        /// pega todos os erros de validação de entidades e transforma-os em uma lista de strings
        /// </summary>
        /// <param name="err"></param>
        /// <returns></returns>
        public static List<string> ExtraiErrosDbEntityValidation(DbEntityValidationException err)
        {
            var result = new List<string>();

            if ((err is DbEntityValidationException) && (err != null))
            {
                //foreach (var eve in err.EntityValidationErrors)
                //{
                //    string entity = eve.Entry.Entity.GetType().Name;
                //    string state = eve.Entry.State.ToString();
                //    string mensagem = string.Format("-Entidade do tipo \"{0}\" no estado \"{1}\" tem os seguintes erros de validação:", entity, state);

                //    result.Add(mensagem);

                //    foreach (var ve in eve.ValidationErrors)
                //    {
                //        string prop = ve.PropertyName;
                //        string erro = ve.ErrorMessage;
                //        string valor = "";
                //        try
                //        {
                //            valor = eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName).ToString();
                //        }
                //        catch
                //        {
                //        }
                //        string detalhe = string.Format("--Property: \"{0}\", Valor: \"{1}\", Erro: \"{2}\"", prop, valor, erro);

                //        result.Add(detalhe);
                //    }
                //}
            }


            return result;

        }


        /// <summary>
        /// loga uma exception
        /// </summary>
        /// <param name="err"></param>
        /// <param name="rethrow"></param>
        //public static string LogarException(Exception err)
        //{
        //    string mensagens = "";
        //    try
        //    {
        //        mensagens = ConcatenaExceptions(err);
        //    }
        //    catch (Exception err2)
        //    {
        //        mensagens = "Não foi possível concatenar todas as mensagens de erro\r\n" + err.ToString() + "\r\n" + err2.ToString();

        //        System.Diagnostics.Debug.WriteLine(mensagens);
        //        System.Diagnostics.Trace.WriteLine(mensagens);
        //        Console.WriteLine(mensagens);
        //    }

        //    try
        //    {
        //        GravarLog(mensagens);

        //        string codeBase = Assembly.GetExecutingAssembly().Location;
        //        UriBuilder uri = new UriBuilder(codeBase);
        //        string pathAssembly = Uri.UnescapeDataString(uri.Path);
        //        string diretorioAssembly = Path.GetDirectoryName(pathAssembly);
        //        string diretorioAplicacao = Directory.GetParent(diretorioAssembly).FullName;
        //        string caminhoArquivoLog = Path.Combine(diretorioAplicacao, "_logs\\log.txt");

        //        System.Diagnostics.Debug.WriteLine(mensagens);
        //        System.Diagnostics.Trace.WriteLine(mensagens);
        //        Console.WriteLine(mensagens);

        //        if (!Directory.Exists(Path.GetDirectoryName(caminhoArquivoLog)))
        //        {
        //            Directory.CreateDirectory(Path.GetDirectoryName(caminhoArquivoLog));
        //        }

        //        using (StreamWriter sw = new StreamWriter(caminhoArquivoLog, true))
        //        {
        //            sw.WriteLine(string.Format("Data: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")).PadRight(80, '-'));
        //            sw.WriteLine(ConcatenaExceptions(err));
        //            sw.WriteLine("=".PadRight(80, '='));
        //            sw.Flush();
        //            sw.Close();
        //        }
        //    }
        //    catch (Exception err2)
        //    {
        //        mensagens = "Não foi possível logar o erro.\r\n" + err2.ToString() + "\r\n" + mensagens;
        //        System.Diagnostics.Debug.WriteLine(mensagens);
        //        System.Diagnostics.Trace.WriteLine(mensagens);
        //        Console.WriteLine(mensagens);
        //    }

        //    return mensagens;
        //}


        /// <summary>
        /// loga um texto / evento qualquer
        /// </summary>
        /// <param name="texto">string - texto a logar</param>
        public static string Logar(string texto)
        {
            return  LogServices.GravarLog(texto).Result.Mensagem;

            //try
            //{

            //    string codeBase = Assembly.GetExecutingAssembly().Location;
            //    UriBuilder uri = new UriBuilder(codeBase);
            //    string pathAssembly = Uri.UnescapeDataString(uri.Path);
            //    string diretorioAssembly = Path.GetDirectoryName(pathAssembly);
            //    string diretorioAplicacao = Directory.GetParent(diretorioAssembly).FullName;
            //    string caminhoArquivoLog = Path.Combine(diretorioAplicacao, "App_Data\\Log.txt");


            //    if (!Directory.Exists(Path.GetDirectoryName(caminhoArquivoLog)))
            //    {
            //        Directory.CreateDirectory(Path.GetDirectoryName(caminhoArquivoLog));
            //    }

            //    using (StreamWriter sw = new StreamWriter(caminhoArquivoLog, true))
            //    {
            //        sw.WriteLine(string.Format("Data: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")).PadRight(80, '-'));
            //        sw.WriteLine(texto);
            //        sw.WriteLine("=".PadRight(80, '='));
            //        sw.Flush();
            //        sw.Close();
            //    }


            //}
            //catch (Exception err2)
            //{
            //    err2.Logar();
            //}
        }

        /// <summary>
        /// concatena todas as exceptions em uma string com \r\n separando
        /// </summary>
        /// <param name="err"></param>
        /// <returns></returns>
        public static string ConcatenaExceptions(Exception err)
        {
            var mensagens = GetExceptionMessages(GetExceptionList(err));
            return string.Join("\r\n", mensagens.ToArray());
        }

        /// <summary>
        /// para mensagens de erros de validação na própria tela, pega só os erros de validação
        /// </summary>
        /// <param name="err"></param>
        /// <returns></returns>
        public static string ConcatenaErrosDbEntityValidation(DbEntityValidationException err)
        {
            var mensagens = ExtraiErrosDbEntityValidation(err);
            return string.Join("\r\n", mensagens.ToArray());

        }
        #endregion
    }

    public class LogView
    {
        public string Arquivo { get; set; }
        public List<EntradaLog> Linhas { get; set; } = new List<EntradaLog>();

        public void AdicionarEntrada(string msg, string origem = "", string detalhes = "")
        {
            this.Linhas = this.Linhas ?? new List<EntradaLog>();

            var entrada = new EntradaLog(msg)
            {
                Origem = origem,
                Detalhes = detalhes
            };
            
            this.Linhas.Add(entrada);
        }
    }
    public class EntradaLog
    {
        public EntradaLog()
        {
            Origem = "LogView";
            IdLog = "--";
        }
        public EntradaLog(string msg, string origem = "", string detalhes = ""):this()
        {
            Mensagem = msg;
            Origem = origem;
            Detalhes = detalhes;
            SessaoID = App.SessionID;
            UsuarioDaSessao = App.UsuarioDaSessao;
        }

        public int Linha { get; set; }
        public DateTime DataHora { get; set; } = DateTime.Now;
        public string Origem { get; set; }
        public string IdLog { get; set; }
        public string Mensagem { get; set; }
        public string Detalhes { get; set; }
        public string SessaoID { get; set; }
        public string UsuarioDaSessao { get; set; }
    }

}
