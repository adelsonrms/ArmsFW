
using ArmsFW.Services.Shared.Pesquisa;

namespace app.core.Domain.Request
{
    public class UsuarioRequest : PaginacaoRequest
	{
		public string Id { get; set; }
		public string Nome { get; set; }
	}
}
