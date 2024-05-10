//using ArmsFW.Security;
//using ArmsFW.Services.Logging;
//using ArmsFW.Services.Shared;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace ArmsFW.Infra.Identity
//{
//    public class AccountManager : AccountManager<Usuario>
//    {
//        public AccountManager(SignInManager<Usuario> signInManager, UserManager<Usuario> userManager, ILogger<AccountManager> logger) : base(signInManager, userManager, logger) { }
//    }

//    public class AccountManager<TUser> : IAccountManager<TUser> where TUser : IdentityUser
//    {
//        private readonly UserManager<TUser> _userManager;
//        private readonly SignInManager<TUser> _signInManager;

//        public ILogger<AccountManager<TUser>> Logger { get; private set; }

//        public UserManager<TUser> UserManager { get; private set; }
//        public SignInManager<TUser> SignInManager { get; private set; }

//        public List<TUser> Usuarios => _userManager.Users.ToList();

//        public AccountManager(SignInManager<TUser> signInManager, 
//            UserManager<TUser> userManager,
//            ILogger<AccountManager<TUser>> logger)
//        {
//            _userManager = userManager;
//            _signInManager = signInManager;

//            Logger = logger;
//            UserManager = _userManager;
//            SignInManager = signInManager;
//        }

//        public async Task<Result<TUser>> CriaUsuario(string userName, string email, string nome, string password = null, string id = null)
//        {
//            try
//            {
//                TUser usr = await _userManager.FindByNameAsync(userName);

//                if (usr == null)
//                {
//                    var usrNew = new IdentityUser
//                    {
//                        Id = id,
//                        Email = email,
//                        UserName = userName,
//                        NormalizedUserName = nome
//                    };

//                    usr = (TUser)usrNew;

//                    IdentityResult resultCriar;

//                    if (string.IsNullOrEmpty(password))
//                    {
//                        //Cria a conta sem informar a password
//                        resultCriar = await _userManager.CreateAsync(usr);
//                    }
//                    else
//                    {
//                        //Cria a conta com a password
//                        resultCriar = await _userManager.CreateAsync(usr, password);
//                    }

//                    if (resultCriar.Succeeded)
//                    {
//                        return ResultBase<TUser>.Sucesso($"Conta Identity : {userName} criada com sucesso sucesso. ID : {id}", usr);
//                    }
//                    else
//                    {
//                        return ResultBase<TUser>.Erro($"Falha inesperada na criação da Conta Identity : {userName}");
//                    }
//                }
//                else
//                {
//                    //A atualização do ID deve ser diretamente nas tabelas (principal e relacionadas).
                    
//                    //Remove as roles
//                    var rolesDoUsuario = _userManager.GetRolesAsync(usr).Result;

//                    foreach (string role in rolesDoUsuario)
//                    {
//                        await _userManager.RemoveFromRoleAsync(usr, role);
//                    }
                    
//                    var claimsDoUsuario = _userManager.GetClaimsAsync(usr).Result;
//                    return ResultBase<TUser>.Sucesso($"Usuário ID : {usr.Id} Atualizado com sucesso", usr);
//                }
//            }
//            catch (Exception ex)
//            {
//                return ResultBase<TUser>.Erro($"Falha ao criar/atualizar o usuario no Identity. Detalhe : {ex.Message}");
//            }
//        }

//        public async Task<Result<bool>> ExcluirUsuario(string userName)
//        {
//            try
//            {
//                var usr = await _userManager.FindByNameAsync(userName);

//                if (usr != null)
//                {
//                    var resultDelete = await _userManager.DeleteAsync(usr);

//                    if (resultDelete.Succeeded)
//                    {
//                        return ResultBase<bool>.Sucesso($"Usuário ID : {usr.Id} foi removido com sucesso da base", true);
//                    }
//                    else
//                    {
//                        return ResultBase<bool>.Erro($"Erro ao excluir o usuario : {userName}");
//                    }
//                }
//                else
//                {
//                    return ResultBase<bool>.Erro($"Usuário {userName} não existe. Nao pode ser excluido");
//                }
//            }
//            catch (Exception ex)
//            {
//                return ResultBase<bool>.Erro($"Falha ao criar o usuario no Identity. Detalhe : {ex.Message}");
//            }
//        }

//        public async Task<Result<TUser>> ObterContaIdentity(string userName)
//        {
//            try
//            {
//                var usr = await _userManager.FindByNameAsync(userName);

//                if (usr != null)
//                {
//                    return ResultBase<TUser>.Sucesso($"Usuário ID : {usr.Id} encontrado !", usr);
//                }
//                else
//                {
//                    return ResultBase<TUser>.Erro($"Usuário {userName} não existe.");
//                }
//            }
//            catch (Exception ex)
//            {
//                return ResultBase<TUser>.Erro($"Falha ao Consultar o usuario no Identity. Detalhe : {ex.Message}");
//            }
//        }

//        public async Task<TUser> ObterPorId(string id) => await _userManager.FindByIdAsync(id);
//    }

//    public class LoginService : ILoginService
//    {
//        private readonly UserManager<Usuario> _userManager;
//        private readonly SignInManager<Usuario> _signInManager;
//        public readonly ILogger<LoginService> _logger;

//        public LoginService(SignInManager<Usuario> signInManager, UserManager<Usuario> userManager, ILogger<LoginService> logger) {
//            _userManager = userManager;
//            _signInManager = signInManager;
//            _logger = logger;
//        }
     

//        public async Task<LoginResult> LoginAsync(LoginRequest model, string returnUrl = null)
//        {
//            Usuario user = new Usuario();

//            Result<object> result = ResultBase.Erro("Falha desconhecido durante o Login", new { Metodo = "OnPostLoginAsync", returnUrl });

//            try
//            {
//                var devMode = (model.UserName == "#dev#");

//                //Se nao mandar o dominio no email, complemento com o padrão
//                if (!model.UserName.Contains("@") && !model.UserName.ToLower().Equals("admin")) model.UserName += "@tecnun.com.br";

//                //Usuário/Login do Office, apto para cadastro
//                user = _userManager.FindByEmailAsync(model.UserName).Result;

//                if (user != null)
//                {
                    
//                    //if (model.Password.Equals(user.SenhaTemporaria))
//                    //{
//                    //    result = ResultBase.Erro("A senha informado é temporaria, deve ser redefinida", new { Metodo = "FindByEmailAsync", returnUrl });
//                    //}

//                    //Verifica se a senha é valida.
//                    var resultSenha = await _signInManager.CheckPasswordSignInAsync(user, model.Password, true);

//                    if (resultSenha.Succeeded)
//                    {
//                        result = ResultBase.Sucesso($"Usuário/Senha valido no sistema", new { Metodo = "PasswordSignInAsync", returnUrl });

//                        //Verifica se a conta ta bloqueada
//                        //if (user.IsBloqueado)
//                        //{
//                        //    result = ResultBase.Erro("A conta esta com acesso bloqueado. Contate o Administrador do sistema", new { Metodo = "IsBloqueado", returnUrl });
//                        //    result.Validation.Adicionar("Conta com acesso bloqueado !");
//                        //}

//                        if (resultSenha.RequiresTwoFactor)
//                        {
//                            result = ResultBase.Erro("Esta conta requer confirmação em dois fatores (TwoFactor)", new { Metodo = "RequiresTwoFactor", returnUrl });
//                        }

//                        if (resultSenha.IsLockedOut)
//                        {
//                            result = ResultBase.Erro("Esta conta esta com o acesso bloqueado", new { Metodo = "IsLockedOut", returnUrl });
//                        }


//                        if (user.IsExcluido)
//                        {
//                            result = ResultBase.Erro("Conta excluida. Nao existe mais na base de dados !", new { Metodo = "IsExcluido", returnUrl });
//                            result.Validation.Adicionar("AccountIsDeleted - Conta excluída !");
//                        }
                       
//                    }
//                    else
//                    {
//                        result = ResultBase.Erro("A senha é invalida", new { Metodo = "CheckPasswordSignInAsync", returnUrl });
//                    }

//                }
//                else
//                {
//                    result = ResultBase.Erro("Usuário nao existe", new { Metodo = "FindByEmailAsync", returnUrl });
//                }
//            }
//            catch (Exception ex)
//            {
//                ex.Logar();
//                result = ResultBase.Erro($"Falha (Exception) inesperada ao efetuar o login. Detalhe : {ex.Message}", new { Metodo = ex.TargetSite?.Name, returnUrl });
//            }

//            return new LoginResult
//            {
//                Status = result.Status,
//                Data = result.Data,
//                Message = result.Message,
//                RedirectTo = returnUrl,
//                User = user
//            } ;
//        }
//    }
//}