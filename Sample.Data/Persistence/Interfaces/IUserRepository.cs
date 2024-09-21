using Sample.Data.Entities;

namespace Sample.Data.Persistence.Interfaces
{
    public interface IUserRepository : ICrudRepository<User, long>
    {
        public Task<User> GetByUsernameAsync(string username);
    }
}