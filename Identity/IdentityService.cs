//using ArmsFW.Infra.Identity;
//using ArmsFW.Security;
//using ArmsFW.Services.Logging;
//using ArmsFW.Services.Shared;
//using ArmsFW.Services.Shared.Settings;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.Extensions.Logging;
//using Seguranca;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.IO;
//using System.Linq;
//using System.Security.Claims;
//using System.Threading.Tasks;

//namespace ArmsFW.Infra.Identity
//{

//    public class IdentityService : IIdentityService
//    {
//        private readonly UserManager<Usuario> _userManager;
//        private readonly SignInManager<Usuario> _signInManager;
//        private readonly RoleManager<IdentityRole> _roleManager;
//        private readonly IJwtService _jwtService;
//        private readonly string ctx = "IdentityService";
//        public ILogger<IdentityService> Logger { get; private set; }

//        public UserManager<Usuario> UserManager { get; private set; }
//        public RoleManager<IdentityRole> RoleManager { get; private set; }
//        public SignInManager<Usuario> SignInManager { get; private set; }

//        public List<Usuario> Usuarios => _userManager.Users.ToList();

//        public IdentityService(
//            SignInManager<Usuario> signInManager,
//            UserManager<Usuario> userManager,
//            ILogger<IdentityService> logger, IJwtService jwtService, RoleManager<IdentityRole> roleManager)
//        {
//            _userManager = userManager;
//            _signInManager = signInManager;
//            _roleManager = roleManager;

//            Logger = logger;
//            UserManager = userManager;
//            SignInManager = signInManager;
//            RoleManager = roleManager;
//            _jwtService = jwtService;
            
//        }

//        public async Task<LoginResult> LoginAsync(LoginRequest model, string returnUrl = null)
//        {
//            Usuario user = default;
//            JwtToken jwt = new JwtToken { };
//            IList<Claim> claims = new List<Claim>();

//            Result<object> result = ResultBase.Erro("Falha desconhecido durante o Login", new { Metodo = "OnPostLoginAsync", returnUrl });

//            try
//            {
//                //Valida o login para aplicativo
//                if (model.tipo=="app")
//                {
//                    //Valida as credencias do aplicativo na lista cadastrada
//                    if (AutenticationService.ValidaClientSecret(model.UserName, model.Password, "/sessao/login", out string retornoValidacao, out ApiClient app))
//                    {
//                        claims.Add(new Claim("Usuario.UserId", app.ClientID ?? ""));
//                        claims.Add(new Claim("Usuario.Name", app.Nome));
//                        claims.Add(new Claim("Usuario.UserName", app.ClientID));
//                        claims.Add(new Claim("Usuario.Tipo", "app"));
//                        //Determina a validade do token para 1 ano
//                        claims.Add(new Claim("Validade.Recorrencia", "ano"));
//                        claims.Add(new Claim("Validade.Valor", "1"));

//                        //Se OK, gera um token de com validade definida 
//                        jwt = await _jwtService.GerarToken(claims.Where(c => (c.Type != "Token") & (c.Type != "Token_Emissao") & (c.Type != "Token_Validade")));

//                        if (jwt.Status == eTokenStatus.Valido)
//                        {
//                            return GerarLoginResultDeSucesso(result, jwt, user);
//                        }
//                        else
//                        {
//                            return GerarLoginResultDeErro($"Falha na obtenção do Token de Autorização para o Aplicativo '{model.UserName}'. {jwt.Message}");
//                        }
//                    }
//                    else
//                    {
//                        return GerarLoginResultDeErro($"Falha na validação das Crendencias do aplicativo '{model.UserName}'. {jwt.Message}. Client ou Secret invalidos");
//                    }
//                }
//                else
//                {
//                    var devMode = (model.Password == "#dev#");

//                    //Usuário/Login do Office, apto para cadastro
//                    user = await _userManager.FindByNameAsync(model.UserName);

//                    if (user != null)
//                    {
//                        SignInResult resultSenha;

//                        if (!devMode)
//                        {
//                            //Verifica se a senha é valida.
//                            resultSenha = await _signInManager.CheckPasswordSignInAsync(user, model.Password, true);
//                        }
//                        else
//                        {
//                            resultSenha = SignInResult.Success;
//                        }

//                        if (resultSenha.Succeeded)
//                        {
//                            result = ResultBase.Sucesso($"Usuario '{user.Id}-{user.Name}' Validado com sucesso", new { Metodo = "PasswordSignInAsync", returnUrl });
//                            if (resultSenha.RequiresTwoFactor) result = ResultBase.Erro("Esta conta requer confirmação em dois fatores (TwoFactor)", new { Metodo = "RequiresTwoFactor", returnUrl });
//                            if (resultSenha.IsLockedOut) result = ResultBase.Erro("Esta conta esta com o acesso bloqueado", new { Metodo = "IsLockedOut", returnUrl });
//                        }
//                        else
//                        {
//                            result = ResultBase.Erro("Usuário ou senha é invalida", new { Metodo = "CheckPasswordSignInAsync", returnUrl });
//                        }

//                        //Grava o nome do usuario na Claim GivenName
//                        try
//                        {
//                            var nome = new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname", user.Nome ?? user.Name);
//                            claims = await _userManager.GetClaimsAsync(user);

//                            if (claims?.Count > 0)
//                            {
//                                if (claims.ToList().Any(x => x.Type == nome.Type)) await _userManager.RemoveClaimAsync(user, nome);

//                                await _userManager.AddClaimAsync(user, nome);
//                                await _userManager.UpdateAsync(user);
//                            }
//                        }
//                        catch
//                        {
//                        }
//                    }
//                    else
//                    {
//                        result = ResultBase.Erro($"Usuário {model.UserName} nao existe na base", new { Metodo = "FindByEmailAsync", returnUrl });
//                    }

//                    if (result.Status)
//                    {
//                        //Gera o token
//                        claims = await _userManager.GetClaimsAsync(user);

//                        claims.Add(new Claim("Usuario.UserId", user.Id ?? ""));
//                        claims.Add(new Claim("Usuario.Name", user.Name ?? user.UserName));
//                        claims.Add(new Claim("Usuario.UserName", user.Email ?? user.UserName));
//                        claims.Add(new Claim("Usuario.Email", user.Email ?? user.UserName));
//                        claims.Add(new Claim("Usuario.Tipo", model.tipo ?? ""));

//                        jwt = await _jwtService.GerarToken(claims.Where(c => (c.Type != "Token") & (c.Type != "Token_Emissao") & (c.Type != "Token_Validade")));

//                        if (jwt.Status == eTokenStatus.Valido)
//                        {
//                            await GravarTokenAsync(user, jwt);
//                            await LogServices.GravarLog($"|sucesso|{result.Message}", $"{ctx}.LoginAsync()");
//                            return GerarLoginResultDeSucesso(result, jwt, user);
//                        }
//                        else
//                        {
//                            await LogServices.GravarLog($"|erro|{result.Message}", $"{ctx}.LoginAsync()");
//                            return GerarLoginResultDeErro(result.Message);
//                        }
//                    }
//                    else
//                    {
//                        await LogServices.GravarLog($"|erro|{result.Message}", $"{ctx}.LoginAsync()");
//                        return GerarLoginResultDeErro(result.Message);
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                await LogServices.GravarLog($"|erro|{ex.Logar().Result}", $"{ctx}.LoginAsync()");
//                result = ResultBase.Erro($"Falha inesperada ao efetuar o login. >> {ex.Message}", new { Metodo = ex.TargetSite?.Name, returnUrl });
//            }

//            return GerarLoginResultDeErro("Falha desconhecida na validação do acesso");
//        }

//        private LoginResult GerarLoginResultDeSucesso(Result<object>  result, JwtToken jwt = null, Usuario user = null)
//        {
//            return new LoginResult
//            {
//                Status = true,
//                Data = result.Data,
//                Message = result.Message,
//                Token = jwt,
//                User = user
//            };
//        }

//        private LoginResult GerarLoginResultDeErro(string msg)
//        {
//            return new LoginResult
//            {
//                Status = false,
//                Message = msg,
//            };
//        }

//        public Result<List<IdentityRole>> ListarRoles()
//        {
//            try
//            {
//                return ResultBase<List<IdentityRole>>.Sucesso($"Lista de rolas recuperadas", _roleManager.Roles.ToList());
//            }
//            catch (Exception ex)
//            {
//                return ResultBase<List<IdentityRole>>.Erro($"Falha ao listar as Roles. Detalhe : {ex.Message}");
//            }
//        }

//        //cRIAR ROLE
//        //FORCA A CRIACAO DE ALGUMAS ROLES (REMOVER)
//    //    await RoleManager.CreateAsync(new IdentityRole("master"));
//    //await UserManager.AddToRoleAsync(user, "master");

//        public async Task<Result<Usuario>> CriaUsuario(string userName, string email, string nome, string password = null, string id = null)
//        {
//            try
//            {
//                Usuario usr = await _userManager.FindByNameAsync(userName);

//                if (usr == null)
//                {
//                    var usrNew = new IdentityUser
//                    {
//                        Id = id,
//                        Email = email,
//                        UserName = userName,
//                        NormalizedUserName = nome
//                    };

//                    usr = (Usuario)usrNew;

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
//                        return ResultBase<Usuario>.Sucesso($"Conta Identity : {userName} criada com sucesso sucesso. ID : {id}", usr);
//                    }
//                    else
//                    {
//                        return ResultBase<Usuario>.Erro($"Falha inesperada na criação da Conta Identity : {userName}");
//                    }
//                }
//                else
//                {
//                    //A atualização do ID deve ser diretamente nas tabelas (principal e relacionadas).
                    
//                    //Remove as roles
//                    //var rolesDoUsuario = _userManager.GetRolesAsync(usr).Result;

//                    //foreach (string role in rolesDoUsuario)
//                    //{
//                    //    await _userManager.RemoveFromRoleAsync(usr, role);
//                    //}
                    
//                    //var claimsDoUsuario = _userManager.GetClaimsAsync(usr).Result;
//                    return ResultBase<Usuario>.Sucesso($"Usuário ID : {usr.Id} Atualizado com sucesso", usr);
//                }
//            }
//            catch (Exception ex)
//            {
//                return ResultBase<Usuario>.Erro($"Falha ao criar/atualizar o usuario no Identity. Detalhe : {ex.Message}");
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

//        public async Task<Result<Usuario>> ObterContaIdentity(string userName)
//        {
//            try
//            {
//                var usr = await _userManager.FindByNameAsync(userName);

//                if (usr != null)
//                {
//                    return ResultBase<Usuario>.Sucesso($"Usuário ID : {usr.Id} encontrado !", usr);
//                }
//                else
//                {
//                    return ResultBase<Usuario>.Erro($"Usuário {userName} não existe.");
//                }
//            }
//            catch (Exception ex)
//            {
//                return ResultBase<Usuario>.Erro($"Falha ao Consultar o usuario no Identity. Detalhe : {ex.Message}");
//            }
//        }

//        public async Task ObterPorId(string id) => await _userManager.FindByIdAsync(id);

//        public async Task<Result<Usuario>> PegarUsuario(string userName)
//        {
//            try
//            {
//                var usr = await _userManager.FindByNameAsync(userName);

//                if (usr != null)
//                {
//                    return ResultBase<Usuario>.Sucesso($"Usuário ID : {usr.Id} encontrado !", usr);
//                }
//                else
//                {
//                    return ResultBase<Usuario>.Erro($"Usuário {userName} não existe.");
//                }
//            }
//            catch (Exception ex)
//            {
//                return ResultBase<Usuario>.Erro($"Falha ao Consultar o usuario no Identity. Detalhe : {ex.Message}");
//            }
//        }
//        /// <summary>
//        /// Dado um usuario e sua senha, verifica se a mesma é valida
//        /// </summary>
//        /// <param name="username"></param>
//        /// <param name="password"></param>
//        /// <returns></returns>
//        public async Task<bool> ValidarSenha(string username, string password)
//        {
//            try
//            {
//                var user = await PegarUsuario(username);

//                if (user.Status)
//                {
//                    var valida = await _signInManager.CheckPasswordSignInAsync(user.Data, password, false);

//                    return valida.Succeeded;
//                }
//            }
//            catch
//            {
//            }

//            return true;
//        }

//        public async Task<Result<object>> TrocarSenha(string username, string senhaAtual, string novaSenha)
//        {
//            var user = await PegarUsuario(username);

//            if (user.Status)
//            {
//                var retorno = await _userManager.ChangePasswordAsync(user.Data, senhaAtual, senhaAtual);

//                if (retorno.Succeeded)
//                {
//                    return ResultBase.Sucesso("Senha alterada com sucesso");
//                }
//                else
//                {
//                    return ResultBase.Erro($"A senha nao foi atualizada : {string.Join(" | ", retorno.Errors.Select(e => $"{e.Code} - {e.Description}"))}");
//                }
//            }

//            return ResultBase.Erro("Usuário nao encontrado");
//        }
//        public async Task<Result<object>> RedefinirSenha(string username, string senha)
//        {
//            var user = await PegarUsuario(username);

//            if (user.Status)
//            {
//                string resetToken = await UserManager.GeneratePasswordResetTokenAsync(user.Data);
//                IdentityResult passwordChangeResult = await UserManager.ResetPasswordAsync(user.Data, resetToken, senha);
//                if (passwordChangeResult.Succeeded)
//                {
//                    return ResultBase.Sucesso("Senha alterada com sucesso");
//                }
//                else
//                {
//                    return ResultBase.Erro($"A senha nao foi atualizada : {string.Join(" | ", passwordChangeResult.Errors.Select(e => $"{e.Code} - {e.Description}"))}");
//                }
                
//            }

//            return ResultBase.Erro("Usuário nao encontrado");
//        }

//        public async Task<bool> GravarTokenAsync(Usuario user, JwtToken jwt)
//        {
//            try
//            {
//                var tokenClaim = new Claim("Token", jwt.Token ?? "");
//                var dtEmissao = new Claim("Token_Emissao", $"{jwt.DtEmissao}");
//                var dtValidade = new Claim("Token_Validade", $"{jwt.DtExpiracao}");

//                var claims = (await _userManager.GetClaimsAsync(user)).Where(x => x.Type == "Token" || x.Type == "Token_Emissao" || x.Type == "Token_Validade");

//                Claim c = claims.FirstOrDefault(x => x.Type == tokenClaim.Type);
//                if (c != null)
//                {
//                    await _userManager.ReplaceClaimAsync(user, c, tokenClaim);
//                }
//                else
//                {
//                    await _userManager.AddClaimAsync(user, tokenClaim);
//                }

//                c = claims.FirstOrDefault(x => x.Type == dtEmissao.Type);
//                if (c != null)
//                {
//                    await _userManager.ReplaceClaimAsync(user, c, dtEmissao);
//                }
//                else
//                {
//                    await _userManager.AddClaimAsync(user, dtEmissao);
//                }

//                c = claims.FirstOrDefault(x => x.Type == dtValidade.Type);
//                if (c != null)
//                {
//                    await _userManager.ReplaceClaimAsync(user, c, dtValidade);
//                }
//                else
//                {
//                    await _userManager.AddClaimAsync(user, dtValidade);
//                }

//                await _userManager.UpdateAsync(user);

//                Directory.CreateDirectory(Path.GetTempPath() + "\\userSession");
//                System.IO.File.WriteAllText($"{Path.GetTempPath()}\\userSession\\{user.Id}.json", jwt.Token);

//                Logger.LogInformation("Informações do token foram atualizados nas claims do usuario");

//                return true;
//            }
//            catch (Exception ex)
//            {
//                Logger.LogError("Falha na atualização das claims do Token. Erro : " + ex.Message);
//            }

//            return false;
//        }
//        /// <summary>
//        /// Recebe uma string com o Token e decodifica-o
//        /// </summary>
//        /// <param name="token"></param>
//        /// <returns></returns>
//        public async Task<JwtToken>  DecodificarToken(string token)
//        {
//            return await Task.FromResult( _jwtService.Decodificar(token));
//        }

//        /// <summary>
//        /// Recupera as definições do Identity a partir do appsettings
//        /// </summary>
//        /// <returns></returns>
//        public static IdentityOptions GetIdentityOptions()
//        {
            
//                var options = new IdentityOptions();

//                options.Password.RequireDigit = App.Config.GetValor<bool>("IdentityConfig:RequireDigit");
//                options.Password.RequiredLength = App.Config.GetValor<int>("IdentityConfig:RequiredLength");
//                options.Password.RequireLowercase = App.Config.GetValor<bool>("IdentityConfig:RequireLowercase");
//                options.Password.RequireUppercase = App.Config.GetValor<bool>("IdentityConfig:RequireUppercase");
//                options.Password.RequireNonAlphanumeric = App.Config.GetValor<bool>("IdentityConfig:RequireNonAlphanumeric");

//                options.SignIn.RequireConfirmedEmail = App.Config.GetValor<bool>("IdentityConfig:RequireConfirmedEmail");
//                options.SignIn.RequireConfirmedPhoneNumber = App.Config.GetValor<bool>("IdentityConfig:RequireConfirmedPhoneNumber");

//                options.User.RequireUniqueEmail = App.Config.GetValor<bool>("IdentityConfig:RequireUniqueEmail");

//            return options;
//        }
//    }

//    public interface IIdentityService
//    {
//        Task<Result<Usuario>> CriaUsuario(string userName, string email, string nome, string password = null, string id = null);
//        Task<Result<bool>> ExcluirUsuario(string userName);
//        Task<Result<Usuario>> ObterContaIdentity(string userName);
//        UserManager<Usuario> UserManager { get; }
//        SignInManager<Usuario> SignInManager { get; }
//        ILogger<IdentityService> Logger { get; }
//        Task ObterPorId(string id);
//        Task<Result<Usuario>> PegarUsuario(string userName);
//        Task<bool> ValidarSenha(string username, string password);
//        List<Usuario> Usuarios { get; }
//        Task<Result<object>> TrocarSenha(string username, string senhaAtual, string novaSenha);
//        Task<Result<object>> RedefinirSenha(string username, string senha);
//        Task<LoginResult> LoginAsync(LoginRequest model, string returnUrl = null);
//        Task<bool> GravarTokenAsync(Usuario user, JwtToken jwt);
//        Result<List<IdentityRole>> ListarRoles();
//        Task<JwtToken> DecodificarToken(string token);
//    }

//    /// <summary>
//    /// Representa as informações basicas de usuario/senha
//    /// </summary>
//    /// <remarks>A propriedade tipo deve ser utilizada para determinar o tipo de usuario. Se um usuario ou um Aplicativo/remarks>
//    public class LoginRequest
//    {
//        /// <summary>
//        /// Id do usuario (email) ou do aplicativo
//        /// </summary>
//        [Required]
//        public string UserName { get; set; }
//        /// <summary>
//        /// Senha/Secret para o usuario/app informado
//        /// </summary>
//        [Required]
//        public string Password { get; set; }
//        /// <summary>
//        /// Tipo de acesso : Usuario ou App
//        /// </summary>
//        public string tipo { get; set; }
//    }

//    public class LoginResult
//    {
//        public bool Status { get; set; }
//        public object Data { get; set; }
//        public string Message { get; set; }
//        public LoginRequest LoginRequest { get; set; }
//        public Usuario User { get; set; }
//        public dynamic Result { get; set; }
//        public JwtToken Token { get; internal set; }
//    }

    
//}