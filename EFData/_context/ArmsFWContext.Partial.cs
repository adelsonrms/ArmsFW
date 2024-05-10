using ArmsFW.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ArmsFW.Infra.Data.Contexts
{
    public partial class ArmsFWContext
    {
        public DbSet<UsuarioLocal> Usuarios { get; set; }
	}
}
