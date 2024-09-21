using Sample.Data.Entities;

namespace Sample.Data.Persistence.Interfaces
{
    public interface IUserRepository
    {
        public Task<User> GetByUsernameAsync(string username);
    }
}