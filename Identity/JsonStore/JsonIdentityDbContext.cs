using ArmsFW.Core;
using ArmsFW.Lib.Web.Json;
using ArmsFW.Services.Shared.Settings;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArmsFW.Infra.Identity
{
    public class JsonIdentityDbContext
    {
        private readonly List<JsonIdentityUser> _users;
        private string FileDB => $@"{Aplicacao.Diretorio}\identity_users.json";

        public JsonIdentityDbContext()
        {
            _users = JSON.Carregar<List<JsonIdentityUser>>(this.FileDB);
        }

        #region createuser

        internal async Task<IdentityResult> UpdateAsync(JsonIdentityUser user)
        {

            try
            {
                var usr = _users.FirstOrDefault(x => x.Id.Equals(user.Id));

                if (usr!=null)
                {
                    usr = user;
                }

                await SaveDBStore();

                return await Task.FromResult(IdentityResult.Success);
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError { Description = $"Falha inesperada ao Atualizar o usuario ! {user.Email}. Detalhe : {ex.Message}" });
            }
        }

        public List<JsonIdentityUser> ListarTodosOsUsuarios(string status = "") => _users.Where(x => (x.Status == (status) || string.IsNullOrEmpty(status))).ToList();

        public List<JsonIdentityUser> ListarSomenteAtivos() => _users.Where(x => x.IsActive).ToList();

        public List<JsonIdentityUser> ListarSomenteExcluidos() => _users.Where(x => x.IsExcluido).ToList();

        public async Task<IdentityResult> CreateAsync(JsonIdentityUser user)
        {
            try
            {
                //Validações basicas de integridade
                if (_users.Any(x => x.Email.Equals(user.Email))) return IdentityResult.Failed(new IdentityError { Description = $"Cadastro com esse email ja existe ! {user.Email}." });

                if (!_users.Any(x => x.UserName.Equals(user.UserName)))
                {
                    _users.Add(user);
                }

                await SaveDBStore();
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError { Description = $"Falha inesperada ao Registrar o novo usuario ! {user.Email}. Detalhe : {ex.Message}" });
            }
        }
#endregion

        public async Task<IdentityResult> DeleteAsync(JsonIdentityUser user)
        {
            try
            {
                if (_users.Any(usr => usr.Id.Equals(user.Id)))
                {
                    if (_users.RemoveAll(usr => usr.Id.Equals(user.Id))>0)
                    {
                        App.GravarLog($"Usuario '{user}' foi removido com sucesso !");

                        if (await SaveDBStore())
                        {
                            return IdentityResult.Success;
                        }
                        else
                        {
                            return IdentityResult.Failed(new IdentityError { Description = $"Falha inespera ao atualizar o banco de dados (fileStore)" });
                        }
                    }
                    else
                    {
                        return IdentityResult.Failed(new IdentityError { Description = $"Nenhum usuario com ID {user.Id} foi excluido" });
                    }
                }
                else
                {
                    return IdentityResult.Failed(new IdentityError { Description = $"Nenhum usuario encontrado com ID {user.Id}. Exclusao cancelada" });
                }
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError { Description = $"Falha inesperada ao Registrar o novo usuario ! {user.Email}. Detalhe : {ex.Message}" });
            }
        }

        private async Task<bool> SaveDBStore()
        {
            try
            {
                await JSON.SaveJSonObjectToFile<List<JsonIdentityUser>>(_users, this.FileDB, false);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<JsonIdentityUser> FindByIdAsync(Guid userId)
        {
            try
            {
                if (_users.Any(usr => usr.Id.Equals(userId.ToString())))
                {
                    return _users.FirstOrDefault(usr => usr.Id.Equals(userId.ToString()));
                }
                else
                {
                    App.GravarLog($"Usuario com ID ! {userId} nao encontrado.");
                }
            }
            catch (Exception ex)
            {
                App.GravarLog($"Falha inesperada localizar os usuario ! {userId}. Detalhe : {ex.Message}");
            }
            return await Task.FromResult(new JsonIdentityUser() { Id = string.Empty });
        }


        public async Task<JsonIdentityUser> FindByNameAsync(string userName)
        {
            if (string.IsNullOrEmpty(userName)) return new JsonIdentityUser() { Id = string.Empty };

            try
            {
                if (_users.Any(usr => usr.UserName.ToUpper().Equals(userName.ToUpper())))
                {
                    return _users.FirstOrDefault(usr => usr.UserName.ToUpper().Equals(userName.ToUpper()));
                }
                else
                {
                    App.GravarLog($"Usuario ! {userName} nao encontrado.");
                }
            }
            catch (Exception ex)
            {
                App.GravarLog($"Falha inesperada localizar os usuario ! {userName}. Detalhe : {ex.Message}");
            }

            return await Task.FromResult(new JsonIdentityUser() { Id = string.Empty });
        }

        public async Task<JsonIdentityUser> FindByEmailAsync(string userName)
        {
            try
            {
                if (_users.Any(usr => usr.Email.ToUpper().Equals(userName.ToUpper())))
                {
                    return _users.FirstOrDefault(usr => usr.Email.ToUpper().Equals(userName.ToUpper()));
                }
                else
                {
                    App.GravarLog($"Usuario com email ! {userName} nao encontrado.");
                }
            }
            catch (Exception ex)
            {
                App.GravarLog($"Falha inesperada localizar os usuario ! {userName}. Detalhe : {ex.Message}");
            }

            return await Task.FromResult(new JsonIdentityUser() { Id = string.Empty });
        }


    }
}
