using Sample.Data.Entities;
using Sample.Data.Persistence.Interfaces;

namespace Sample.Data.Persistence
{
    public class UserRepository : IUserRepository
    {
        public Task<User> GetByUsernameAsync(string username)
        {
            throw new NotImplementedException();
        }
    }
}