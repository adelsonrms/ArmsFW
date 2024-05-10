//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore;
//using System;

///// <summary>
///// Funcionalidades do Identity
///// </summary>
//namespace ArmsFW.Infra.Identity
//{
//    /// <summary>
//    /// Contexto especifico parea
//    /// </summary>
//    public class IdentityContext : IdentityDbContext<Usuario>
//    {
//        public IdentityContext(DbContextOptions<IdentityContext> options)
//            : base(options)
//        {
//        }

//        public IdentityContext() : base() { }

//        public static IdentityContext Create() => new IdentityContext();

//        protected override void OnModelCreating(ModelBuilder modelBuilder)
//        {
//            modelBuilder.MapearTabelasIdentity();

//            //var hasher = new PasswordHasher<Usuario>();

//            //string id_usuario = Guid.NewGuid().ToString();
//            //string id_role = Guid.NewGuid().ToString();

//            ////Cria duas roles padrões : 1 - master, 2 - demo
//            //modelBuilder.Entity<IdentityRole>().HasData(new IdentityRole
//            //{
//            //    Id = id_role, // primary key
//            //    Name = "master"
//            //});

//            ////Cria dois usuarios iniciais, caso nao existam: ID 1 - Admin, 2 - usuario
//            //modelBuilder.Entity<Usuario>().HasData(
//            //    new Usuario
//            //    {
//            //        Id = id_usuario,
//            //        Name = "demo",
//            //        UserName = "demo",
//            //        NormalizedUserName = "DEMO",
//            //        PasswordHash = hasher.HashPassword(null, "demo#1234")
//            //    }
//            //);

//            ////Associa as roles com os usuarios
//            //modelBuilder.Entity<IdentityUserRole<string>>().HasData(
//            //new IdentityUserRole<string>
//            //{
//            //    RoleId = id_role,
//            //    UserId = id_usuario
//            //});
            
            
//            base.OnModelCreating(modelBuilder);
//        }

//        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//        {
//            if (!optionsBuilder.IsConfigured)
//            {
//                optionsBuilder.EnableSensitiveDataLogging(true);
//                optionsBuilder.EnableDetailedErrors(true);
//            }

//            base.OnConfiguring(optionsBuilder);
//        }
//    }
//}