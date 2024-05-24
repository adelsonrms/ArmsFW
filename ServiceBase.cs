using ArmsFW.AutoMapperExtensions;
using ArmsFW.Services.Logging;
using ArmsFW.Services.Session;
using ArmsFW.Services.Shared;
using ArmsFW.Services.Shared.Pesquisa;
using ArmsFW.Services.Shared.Settings;
using ArmsFW.Web.Http;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ArmsFW.Services
{
    /// <summary>
    /// Classe base para definição de serviços
    /// </summary>
    public abstract class ServiceBase: ValidacaoService, IDisposable
	{
        public readonly UsuarioDaSessao _user = SessionService.GetUser();

        public ServiceBase(string serviceName) : this()
        {
            this.ServiceName = serviceName;
            HttpContext = HttpContextInstance.Current;

            if (HttpContext != null)
            {
                Usuario = HttpContext?.UsuarioDoToken();
            }
        }

        public ServiceBase()
        {
            DateStarted = DateTime.Now;
            ServiceId = DateStarted.Value.ToString("yyyyMMddHHmmssfff");
        }

        public HttpContext HttpContext { get; private set; }
        public UsuarioDaSessao Usuario { get; private set; }

        public string ServiceId { get; protected set; }
        public string ServiceName { get; protected set; }
        protected DateTime? DateStarted { get; private set; }

        public void Dispose()
        {
            
        }

        /// <summary>
        /// Data uma pesquisa paginada de entrada, converte para outra pesquisa de saida
        /// </summary>
        /// <typeparam name="TEntrada">Tipo da classe de entrada</typeparam>
        /// <typeparam name="TSaida">Tipo da classe de saida</typeparam>
        /// <param name="pesquisaEntrada"></param>
        /// <returns></returns>
        public Pesquisa<TSaida> PesquisaResponsePaginada<TEntrada, TSaida>(Pesquisa<TEntrada> pesquisaEntrada)
		{
			var pesquisaSaida = new Pesquisa<TSaida>();

			pesquisaEntrada.Registros.ToList().ForEach(r => pesquisaSaida.Registros.Add(r.Map<TEntrada, TSaida>()));

			pesquisaSaida.TotalDeRegistros = pesquisaEntrada.TotalDeRegistros;

			return pesquisaSaida;
		}

        public override string ToString()
        {
            return $@"Servico {ServiceId} - {ServiceName} inicializado em {DateStarted}!";
        }


        public static string CriarHistoricoAlteracao(dynamic item, string contexto_origem, string historico_existente = null)
        {
            try
            {
                List<object> historicoExistente = new List<object>();
                if (historico_existente.IsJson())
                {
                    historicoExistente.AddRange(JSON.Carregar<List<object>>(historico_existente));
                }

                var historico = new Historico(historicoExistente.Count + 1, App.Session.User.Email, contexto_origem, JSON.Carregar<object>(((object)item).ToJson()));

                historicoExistente.Add(historico);

                return historicoExistente.ToJson();
            }
            catch (Exception ex)
            {
                ex.Logar();
            }
            return string.Empty;
        }
    }

    internal class Historico
    {
        public int Id { get; }
        public string Cd_usuario { get; }
        public DateTime Dt_criacao { get; }
        public string Cd_contexto { get; }
        public object Ds_dados { get; }

        public Historico()
        {
            Dt_criacao = DateTime.Now;
        }
        public Historico(int id, string cd_usuario, string cd_contexto, object ds_dados)
        {
            Id = id;
            Cd_usuario = cd_usuario;
            Dt_criacao = DateTime.Now;
            Cd_contexto = cd_contexto;
            Ds_dados = ds_dados;
        }

        public override bool Equals(object obj)
        {
            return obj is Historico other &&
                   Id == other.Id &&
                   Cd_usuario == other.Cd_usuario &&
                   Dt_criacao == other.Dt_criacao &&
                   Cd_contexto == other.Cd_contexto &&
                   EqualityComparer<object>.Default.Equals(Ds_dados, other.Ds_dados);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Cd_usuario, Dt_criacao, Cd_contexto, Ds_dados);
        }
    }
}
