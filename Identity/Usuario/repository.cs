using app.core.Domain.Interfaces;
using app.core.Domain.Request;
using ArmsFW.Domain.Entities;
using ArmsFW.Services.Shared.Pesquisa;
using System.Linq;

namespace ArmsFW.Infra.Data.Repositories
{
    public class UsuarioRepository : Repository<UsuarioLocal>, IUsuarioRepository
    {
        public UsuarioRepository()
        {

        }

        /// <summary>
        /// Aplica as regras de filtro na entidade do EF
        /// </summary>
        /// <param name="filtro">Regras de filtro</param>
        /// <returns>Um objeto Queryble de uma entidade DbSet com ou sem filtros</returns>
        protected IQueryable<UsuarioLocal> FiltrarQuery(UsuarioRequest filtro)
        {
            var query = base.DbSet.AsQueryable();

            //Se nao houver filtros, a pesquisa retornará tudo
            if (filtro != null)
            {
            }

            return query;
        }

        /// <summary>
        /// Retorna o resultado da pesquisa filtrando pelos parametros configurados no metodo FiltrarQuery
        /// </summary>
        /// <param name="filtro">Definições dos filtros</param>
        /// <returns>UM objeto Pesquisa conta os resultado</returns>
        public Pesquisa<UsuarioLocal> Pesquisar(UsuarioRequest filtro) => base.PesquisarPaginada(FiltrarQuery(filtro), filtro);

        
    }
}
