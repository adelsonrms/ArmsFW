using app.core.Domain.Request;
using ArmsFW.Domain.Entities;
using ArmsFW.Services.Shared.Pesquisa;

namespace ArmsFW.Infra.Data.Repositories
{
    public interface IUsuarioRepository : IRepository<UsuarioLocal>
    {
        /// <summary>
        /// Metodo de pesquisa com regras de filtro
        /// </summary>
        /// <param name="filtro">Regras de filtro</param>
        /// <returns></returns>
        Pesquisa<UsuarioLocal> Pesquisar(UsuarioRequest filtro);
    }
}
