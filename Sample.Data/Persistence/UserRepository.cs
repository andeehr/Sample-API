using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sample.Common.DTOs.Requests;
using Sample.Common.Exceptions;
using Sample.Core.Config;
using Sample.Data.Entities;
using Sample.Data.Persistence.Interfaces;
using Sample.Data.Persistence.MSSQL;
using System.Linq.Expressions;

namespace Sample.Data.Persistence
{
    public class UserRepository : FilterableRepository<User, long, UserFilter>, IUserRepository
    {
        public UserRepository(
            DataContext db,
            ILogger<UserRepository> logger,
            IOptionsSnapshot<FilterQueryOptions> filterOptions) : base(db, logger, filterOptions)
        {
        }

        protected override Dictionary<string, Expression<Func<User, object>>> Sorting
            => new()
            {
                { "username", p => p.Username },
                { "firstname", p => p.FirstName },
                { "lastname", p => p.LastName },
                { "role", p => p.RoleId },
                { "fullname", p => $"{p.LastName}, {p.FirstName}" }
            };

        public async Task<User> GetByUsernameAsync(string username)
        {
            return await GetAll().Include(u => u.Role)
                                    .ThenInclude(r => r.Permissions)
                                 .FirstOrDefaultAsync(u => u.Username == username)
                                 ?? throw new NotFoundException("User not found");
        }

        protected override IQueryable<User> FilterQuery(IQueryable<User> query, UserFilter filter)
        {
            var result = (from item in query
                          where item.Username.Contains(filter.Username ?? string.Empty) &&
                                item.FirstName.Contains(filter.FirstName ?? string.Empty) &&
                                item.LastName.Contains(filter.LastName ?? string.Empty) &&
                                (!filter.RoleId.HasValue || (filter.RoleId.Value == item.RoleId))
                          select item).Include(x => x.Role)
                                          .ThenInclude(x => x.Permissions);

            return result;
        }
    }
}