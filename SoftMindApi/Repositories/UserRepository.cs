using SoftMindApi.Configuration;
using SoftMindApi.Repositories.Interface;

namespace SoftMindApi.Repositories
{
    // Current implementation validates against ApiCredentials from configuration.
    // Can be replaced later with a DB-backed user storage.
    public class UserRepository : IUserRepository
    {
        private readonly ApiCredentials _credentials;

        public UserRepository(ApiCredentials credentials)
        {
            _credentials = credentials;
        }

        public Task<bool> ValidateCredentialsAsync(string username, string password)
        {
            var valid = username == _credentials.Username && password == _credentials.Password;
            return Task.FromResult(valid);
        }
    }
}
