using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sample.Common.Exceptions;
using Sample.Data.Entities;
using Sample.Data.Persistence.Interfaces;
using Sample.Data.Persistence.MSSQL;

namespace Sample.Data.Persistence
{
    public class UserRepository : MSSQLRepository<User, long>, IUserRepository
    {
        public UserRepository(
            DataContext db,
            ILogger<UserRepository> logger
        ) : base(db, logger)
        {
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            return await GetAll().Include(u => u.Role)
                                    .ThenInclude(r => r.Permissions)
                                 .FirstOrDefaultAsync(u => u.Username == username)
                                 ?? throw new NotFoundException("User not found");
        }
    }
}