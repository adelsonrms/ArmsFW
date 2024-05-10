//using ArmsFW.Infra.Identity;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;

//namespace app.web.Identity
//{

//    public static class IdentityConfigurations
//    {
//        public static string DefaultSchema => "Auth";
//    }


//    #region Classes de Mapeamento das Tabelas - FluentMapping
//    public class MappingBase<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : class
//    {
//        private readonly string nomeTabela;

//        public MappingBase(string tabela)
//        {
//            nomeTabela = tabela;
//        }

//        public void Configure(EntityTypeBuilder<TEntity> builder)
//        {
//            builder.ToTable(nomeTabela, IdentityConfigurations.DefaultSchema);
//        }
//    }

//    public class MappingTableToUser : MappingBase<Usuario>
//    {
//        public MappingTableToUser()
//            : base("AspNetUsers")
//        {

//        }
//    }

//    public class MappingTableToRole : MappingBase<IdentityRole>
//    {
//        public MappingTableToRole()
//            : base("AspNetRoles")
//        {
//        }
//    }

//    public class MappingTableToClaims : MappingBase<IdentityUserClaim<string>>
//    {
//        public MappingTableToClaims()
//            : base("AspNetUserClaims")
//        {
//        }
//    }

//    public class MappingTableToUserLogin : IEntityTypeConfiguration<IdentityUserLogin<string>>
//    {
//        public void Configure(EntityTypeBuilder<IdentityUserLogin<string>> builder)
//        {
//            builder.ToTable("AspNetUserLogins", IdentityConfigurations.DefaultSchema);
//            builder.HasKey((IdentityUserLogin<string> p) => new { p.UserId, p.LoginProvider, p.ProviderKey });
//        }
//    }

//    public class MappingTableToUserTokens : IEntityTypeConfiguration<IdentityUserToken<string>>
//    {
//        public void Configure(EntityTypeBuilder<IdentityUserToken<string>> builder)
//        {
//            builder.ToTable("AspNetUserTokens", IdentityConfigurations.DefaultSchema);
//            builder.HasKey((IdentityUserToken<string> p) => new { p.UserId, p.LoginProvider, p.Name });
//        }
//    }

//    public class MappingTableToUserRole : IEntityTypeConfiguration<IdentityUserRole<string>>
//    {
//        public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
//        {
//            builder.ToTable("AspNetUserRoles", IdentityConfigurations.DefaultSchema);
//            builder.HasKey((IdentityUserRole<string> p) => new { p.UserId, p.RoleId });
//        }
//    }

//    public class MappingTableToRoleClaims : IEntityTypeConfiguration<IdentityRoleClaim<string>>
//    {
//        public void Configure(EntityTypeBuilder<IdentityRoleClaim<string>> builder)
//        {
//            builder.ToTable("AspNetRoleClaims", IdentityConfigurations.DefaultSchema);
//            builder.HasKey((IdentityRoleClaim<string> p) => new { p.Id });
//        }
//    }
//    #endregion
//}
