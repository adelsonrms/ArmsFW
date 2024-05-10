using ArmsFW.Core;
using ArmsFW.Domain.Entities;
using ArmsFW.Lib.Web.Json;
using ArmsFW.Services.Extensions;
using ArmsFW.Services.Logging;
using ArmsFW.Services.Shared.Settings;
using System;
using System.Collections.Generic;
using System.IO;

namespace ArmsFW.Infra.Data.JsonStore.Old
{
    public class JsonDbContext
    {
		private readonly List<UsuarioLocal> _usuario;
		

		private readonly Dictionary<string, dynamic> _DbSets;

		public JsonDbContext()
        {
			//Dicionario para manter os DBSets na memoria.
			_DbSets = new Dictionary<string, dynamic>();

			//Carrega os dados do Store
			_usuario = CarregarTabela<UsuarioLocal>();
			


			//Mantem no DbSet
			_DbSets.Add(PegarNomeDaTabela<UsuarioLocal>(), _usuario);
			
		}

        public List<UsuarioLocal> Usuarios => _usuario;
		


		public List<T> CarregarTabela<T>() => CarregarTabela<T>(PegarNomeDaTabela<T>());

		private List<T> CarregarTabela<T>(string arquivo_tabela)
		{
			List<T> tabela = new List<T>();
			string arquivo = $@"{Aplicacao.DiretorioStore}\{arquivo_tabela}";

			//Se o arquivo inda nao existe, cria-o
            if (!File.Exists(arquivo)) GravarStore<T>(tabela);

			try
			{
				tabela = JSON.LoadFromFile<List<T>>(arquivo).Result;
			}
			catch (Exception ex)
			{
				App.GravarLog($"Falha inesperada (Exception) ao recuperar a lista de termos no fileStore.json : \n{ex.Message}\nStack : {ex.StackTrace}");
			}

			return tabela;
		}

        public static JsonDbContext Create()
        {
			return new JsonDbContext();
        }

        public void SaveChangesAsync()
        {
			

		}

		public List<T> Set<T>()
        {
            if (_DbSets.TryGetValue(PegarNomeDaTabela<T>(), out dynamic tabela))
            {
				return (tabela as List<T>);
            }

			return null;
        }

		public string PegarNomeDaTabela<T>()
        {
			var tabela = typeof(T).GetAtributo("JsonTable");

			//Se nao encontrar o atributo, utiliza o proprio nome da classe
			if (string.IsNullOrEmpty(tabela)) tabela = $"{typeof(T).Name}.json";

			return tabela;
		}
		public void GravarStore<T>(List<T> linhas)
		{
			var arquivo = $@"{Aplicacao.DiretorioStore}\{PegarNomeDaTabela<T>()}";

			if (JSON.Gravar(linhas, arquivo, false))
            {
				LogServices.Debug($"Arquivo Atualizado : {arquivo}");
            }
            else
            {
				LogServices.Debug($"Falha na gravação do arquivo : {arquivo}");
			}
		}

	}

	public class DbSetCustom<T> : List<T>
    {

    }
}
