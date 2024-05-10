using app.core.Domain.Request;
using app.core.Domain.Response;
using ArmsFW.Domain.Entities;
using ArmsFW.Services.Shared.Pesquisa;

namespace app.core.Domain.Interfaces
{

    public interface IUsuarioService : ICrudService<UsuarioLocal, int>
    {
        Pesquisa<UsuarioResponse> Pesquisar(UsuarioRequest pesquisaRequest);
        UsuarioLocal GetUsuario(object id);
    }
}