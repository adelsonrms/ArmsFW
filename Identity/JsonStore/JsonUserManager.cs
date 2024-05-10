using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArmsFW.Infra.Identity
{
    public class JsonUserManager<TUser> : UserManager<JsonIdentityUser> where TUser : IdentityUser
    {
        public JsonUserManager(
            IUserStore<JsonIdentityUser> store, 
            IOptions<IdentityOptions> optionsAccessor, 
            IPasswordHasher<JsonIdentityUser> passwordHasher, 
            IEnumerable<IUserValidator<JsonIdentityUser>> userValidators, 
            IEnumerable<IPasswordValidator<JsonIdentityUser>> passwordValidators, 
            ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, 
            IServiceProvider services, 
            ILogger<UserManager<JsonIdentityUser>> logger) : 
            base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {

            var user = JsonIdentityUser.Criar("Admin", "admin");
            user.Perfil = "admin";
            user.Descricao = "Conta Master para administração e suporte da aplicação";
            var result = base.CreateAsync(user, "Admin#0000");

            user = JsonIdentityUser.Criar("Conta Demo", "demo");
            user.Perfil = "usuario";
            user.Descricao = "Conta Demo para testes";
            result = base.CreateAsync(user, "demo");
        }

        public override Task<IdentityResult> CreateAsync(JsonIdentityUser user)
        {
            return base.Store.CreateAsync(user, base.CancellationToken);
        }
    }
}