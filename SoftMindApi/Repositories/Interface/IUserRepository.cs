namespace SoftMindApi.Repositories.Interface
{
    public interface IUserRepository
    {
        Task<bool> ValidateCredentialsAsync(string username, string password);
    }
}
