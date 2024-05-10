using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using app.core.Domain.Entities;
using ArmsFW.Domain.Entities;

namespace app.core.Infra.Data.Mapping
{
	public class UsuarioMapping : IEntityTypeConfiguration<UsuarioLocal>
	{
		public void Configure(EntityTypeBuilder<UsuarioLocal> builder) => builder.ToTable("Usuario", "dbo");
	}
}
