using Sample.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Data.Persistence
{
    public interface IUserRepository
    {
        public Task<User> GetByUsernameAsync(string username);
    }

    public class UserRepository : IUserRepository
    {
        public Task<User> GetByUsernameAsync(string username)
        {
            throw new NotImplementedException();
        }
    }
}