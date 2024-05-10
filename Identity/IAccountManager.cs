//using ArmsFW.Services.Shared;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.Extensions.Logging;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace ArmsFW.Infra.Identity
//{
//    public interface IAccountManager<TUser> where TUser : IdentityUser
//    {
//        Task<Result<TUser>> CriaUsuario(string userName, string email, string nome, string password = null, string id = null);
//        Task<Result<bool>> ExcluirUsuario(string userName);
//        Task<Result<TUser>> ObterContaIdentity(string userName);

//        UserManager<TUser> UserManager { get;  }
//        SignInManager<TUser> SignInManager { get; }
//        ILogger<AccountManager<TUser>> Logger { get; }

//        Task<TUser> ObterPorId(string id);

//        List<TUser> Usuarios { get; }
//    }
//}