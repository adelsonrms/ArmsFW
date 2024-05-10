using ArmsFW.Services.Logging;
using ArmsFW.Services.Shared;
using ArmsFW.Services.Shared.Pesquisa;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace ArmsFW.Infra.Data.Extensions
{
    public static class EFDataExtensions
	{
		public static IServiceCollection AddJsonDbContext<T>(this IServiceCollection services) where T : class
		{
			services.AddTransient<T>();

			return services;
		}

		public static ModelBuilder AplicarMapeamentoFluent(this ModelBuilder modelBuilder)
		{
			foreach (Type type in (from t in Assembly.GetExecutingAssembly().GetTypes()
				where t.GetInterfaces().Any((Type gi) => gi.IsGenericType && gi.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>))
				select t).ToList())
			{
				if (!type.ContainsGenericParameters)
				{
					dynamic configurationInstance = Activator.CreateInstance(type);
					modelBuilder.ApplyConfiguration(configurationInstance);
				}
			}
			return modelBuilder;
		}
		/// <summary>
		/// Data uma query de entidades, aplica regras de paginação e retorna em um objeto Pesquisa
		/// </summary>
		/// <typeparam name="T">Tipo de retorno</typeparam>
		/// <param name="query">Instancia da query</param>
		/// <param name="paginacao">Definições de paginação</param>
		/// <returns></returns>
		public static Pesquisa<T> ToListaPaginada<T>(this IQueryable<T> query, PaginacaoRequest paginacao)
		{
			List<T> registros = new List<T>();
			long total = 0;
			Result<object> retorno = null;
			paginacao = paginacao ?? new PaginacaoRequest();

			try
            {
				total = query.Count();

				//Se informado as regras de paginaçõ, aplica
				if (paginacao != null)
				{
					paginacao.Skip = (paginacao.ItemsPerPage * (paginacao.Pagina - 1));
					paginacao.AllItems = paginacao.Skip == 0 && paginacao.ItemsPerPage == 0;

					if (paginacao.AllItems)
					{
						registros = query.ToList();
					}
					else
					{
						if (paginacao.Skip < 0) paginacao.Skip = 0;
						if (paginacao.ItemsPerPage <= 0) paginacao.ItemsPerPage = 10;

						registros = query
							.Skip(paginacao.Skip)
							.Take(paginacao.ItemsPerPage)
							.ToList();
					}
				}
				else
				{
					//Se nao, traz tudo
					registros = query.ToList();
				}

				retorno = ResultBase.Sucesso("Consulta concluída com sucesso no DB");
			}
            catch (Exception ex)
            {
				ex.Logar();
				retorno = ResultBase.Erro($"Falha inesperada na consulta. Detalhe : {ex.Message}");
            }

            
            return new Pesquisa<T>
			{
				Registros = registros,
				TotalDeRegistros = total,
				QtdItemsDaPesquisa = registros.Count,
				Pagina = paginacao.Pagina,
				Retorno = retorno
			};
		}

		public static string PegarNomeDaTabela(this Type type)
        {
			TableAttribute att = null;

			if (type.GetCustomAttributes().Any()) att = type.GetCustomAttributes().ToList().Find(x => ((Type)x.TypeId).Name == "TableAttribute") as TableAttribute;

			if (att != null) return $"{att.Schema}.{att.Name}";

			return string.Empty;
		}
	}
}
