using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;

namespace ArmsFW.Infra.Identity
{
    /// <summary>
    /// UserStore customizada para carregar dados de um arquivo JSON
    /// </summary>
    public class JsonUserStore : 
        IUserStore<JsonIdentityUser>, 
        IUserPasswordStore<JsonIdentityUser>, 
        IQueryableUserStore<JsonIdentityUser>, 
        IUserEmailStore<JsonIdentityUser>
    {
        private readonly JsonIdentityDbContext _usersTable;

        public JsonUserStore(JsonIdentityDbContext usersTable)
        {
            _usersTable = usersTable;
        }

        public IQueryable<JsonIdentityUser> Users => _usersTable.ListarTodosOsUsuarios().AsQueryable();

        #region createuser
        public async Task<IdentityResult> CreateAsync(JsonIdentityUser user, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));

            return await _usersTable.CreateAsync(user);
        }
        #endregion

        public async Task<IdentityResult> DeleteAsync(JsonIdentityUser user, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));

            return await _usersTable.DeleteAsync(user);

        }

        public void Dispose()
        {
        }

        public async Task<JsonIdentityUser> FindByIdAsync(string userId, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (userId == null) throw new ArgumentNullException(nameof(userId));
            Guid idGuid;
            if(!Guid.TryParse(userId, out idGuid))
            {
                throw new ArgumentException("Not a valid Guid id", nameof(userId));
            }

            return await _usersTable.FindByIdAsync(idGuid);

        }

        public async Task<JsonIdentityUser> FindByNameAsync(string userName, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (userName == null) throw new ArgumentNullException(nameof(userName));
            return await _usersTable.FindByNameAsync(userName);
        }


        public Task<string> GetPasswordHashAsync(JsonIdentityUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.PasswordHash);
        }

        public Task<string> GetUserIdAsync(JsonIdentityUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(JsonIdentityUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.UserName);
        }

        public Task<bool> HasPasswordAsync(JsonIdentityUser user, CancellationToken cancellationToken)
        {
            
            throw new NotImplementedException();
        }

        public async Task<JsonIdentityUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken) => await _usersTable.FindByEmailAsync(normalizedEmail);
        public Task<string> GetEmailAsync(JsonIdentityUser user, CancellationToken cancellationToken) => Task.FromResult(user.Email);
        public Task<bool> GetEmailConfirmedAsync(JsonIdentityUser user, CancellationToken cancellationToken) => Task.FromResult(user.EmailConfirmed);


        public Task<string> GetNormalizedEmailAsync(JsonIdentityUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedEmail);
        }

        public Task<string> GetNormalizedUserNameAsync(JsonIdentityUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedUserName);
        }
        public Task SetEmailAsync(JsonIdentityUser user, string email, CancellationToken cancellationToken)
        {
            user.Email = email;
            return Task.CompletedTask;
        }

        public Task SetEmailConfirmedAsync(JsonIdentityUser user, bool confirmed, CancellationToken cancellationToken)
        {
            user.EmailConfirmed = confirmed;
            return Task.CompletedTask;
        }

        public Task SetNormalizedEmailAsync(JsonIdentityUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            user.NormalizedEmail = normalizedEmail;
            return Task.CompletedTask;
        }

        public Task SetNormalizedUserNameAsync(JsonIdentityUser user, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (normalizedName == null) throw new ArgumentNullException(nameof(normalizedName));

            user.NormalizedUserName = normalizedName;
            return Task.FromResult<object>(null);
        }

        public Task SetPasswordHashAsync(JsonIdentityUser user, string passwordHash, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (passwordHash == null) throw new ArgumentNullException(nameof(passwordHash));

            user.PasswordHash = passwordHash;
            return Task.FromResult<object>(null);

        }

        public Task SetUserNameAsync(JsonIdentityUser user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.CompletedTask;
        }

        public Task<IdentityResult> UpdateAsync(JsonIdentityUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));
            return _usersTable.UpdateAsync(user);
        }
    }
}
