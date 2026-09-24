using Sample.Common.DTOs.Requests;
using Sample.Data.Entities;

namespace Sample.Data.Persistence.Interfaces
{
    public interface IUserRepository : ICrudRepository<User, long>, IFilterableRepository<User, UserFilter>
    {
        public Task<User> GetByUsernameAsync(string username);
    }
}