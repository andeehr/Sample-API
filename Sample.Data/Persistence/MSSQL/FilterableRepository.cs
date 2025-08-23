using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sample.Common.DTOs.Requests;
using Sample.Common.DTOs.Responses;
using Sample.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Sample.Core.Config;
using Sample.Data.Persistence.MSSQL.Extensions;

namespace Sample.Data.Persistence.MSSQL
{
    public abstract class FilterableRepository<T, TKey, TFilter> : MSSQLRepository<T, TKey>
        where T : BaseEntity<TKey>
        where TFilter : Filter
    {
        private readonly FilterQueryOptions _filterOptions;

        protected FilterableRepository(
            DataContext db,
            ILogger logger,
            IOptionsSnapshot<FilterQueryOptions> filterOptions) : base(db, logger)
        {
            _filterOptions = filterOptions?.Value ?? throw new ArgumentNullException(nameof(filterOptions));
        }

        protected abstract IQueryable<T> FilterQuery(IQueryable<T> query, TFilter filter);

        protected abstract Dictionary<string, Expression<Func<T, object>>> Sorting { get; }

        public async Task<PagedResult<T>> GetPaged(TFilter filter)
        {
            var query = FilterQuery(GetAll(), filter);
            var rows = query.Count();
            (var sortedQuery, var inMemory) = query.Take(_filterOptions.LimitRows).Sort(filter, Sorting);
            return await GetPaged(sortedQuery, filter.PageNumber, filter.PageSize, inMemory, rows);
        }

        private static async Task<PagedResult<T>> GetPaged(IQueryable<T> query, int pageNumber, int pageSize, bool inMemory, int realRows)
        {
            var skip = (pageNumber - 1) * pageSize;

            if (inMemory)
            {
                var results = await query.ToListAsync();

                return new PagedResult<T>(
                    results.Skip(skip).Take(pageSize),
                    realRows,
                    results.Count
                );
            }

            var result = await query.Skip(skip).Take(pageSize).ToListAsync();
            return new PagedResult<T>(result, realRows, query.Count());
        }
    }
}