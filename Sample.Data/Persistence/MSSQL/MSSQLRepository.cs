using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sample.Common.Exceptions;
using Sample.Data.Entities;
using Sample.Data.Persistence.Interfaces;
using System.Linq.Expressions;

namespace Sample.Data.Persistence.MSSQL
{
    public abstract class MSSQLRepository<T, TKey> : ICrudRepository<T, TKey> where T : BaseEntity<TKey>
    {
        private readonly DataContext _db;
        private readonly ILogger _logger;

        public MSSQLRepository(DataContext db, ILogger logger)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected DbSet<T> DbSet => _db.Set<T>();

        protected async Task SaveChanges(bool save)
        {
            if (save)
            {
                try
                {
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException is SqlException innerException && innerException.Number == 2601)
                    {
                        var errorMessage = innerException.Message;
                        var start = errorMessage.IndexOf('(');
                        var end = errorMessage.IndexOf(')');
                        var duplicateValue = "unknown value";

                        if (start != -1 && end != -1)
                            duplicateValue = errorMessage.Substring(start + 1, end - start - 1);

                        throw new DuplicateKeyException($"An error occurred while saving the record: The value '{duplicateValue}' already exists and cannot be repeated");
                    }

                    throw new Exception("Unhandled duplicate key error", ex);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while saving on database");
                    throw;
                }
            }
        }

        public async Task AddAsync(T entity, bool saveChanges = true)
        {
            await DbSet.AddAsync(entity);
            await SaveChanges(saveChanges);
        }

        public async Task UpdateAsync(T entity, bool saveChanges = true)
        {
            if (_db.Entry(entity).State == EntityState.Detached)
                DbSet.Attach(entity);

            entity.Update();
            DbSet.Update(entity);
            await SaveChanges(saveChanges);
        }

        public virtual async Task DeleteAsync(T entity, bool saveChanges = true)
        {
            entity.Delete();
            await UpdateAsync(entity, saveChanges);
        }

        public virtual async Task<List<T>> FindByAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            var query = DbSet.Where(predicate);
            return await includes.Aggregate(query, (current, includeProperty) => current.Include(includeProperty)).ToListAsync();
        }

        public async Task<T> GetByIdAsync(TKey key, params Expression<Func<T, object>>[] includes)
        {
            var entity = await includes.Aggregate(GetAll(), (c, i) => c.Include(i))
                                       .FirstOrDefaultAsync(x => x.Id.Equals(key));

            if (entity == null)
            {
                _logger.LogError("{entity} with {key} not found", typeof(T).Name, key);
                throw new NotFoundException();
            }
            return entity;
        }

        protected IQueryable<T> GetAll()
        {
            return DbSet.Where(x => x.DeletedAt == null).AsQueryable();
        }
    }
}