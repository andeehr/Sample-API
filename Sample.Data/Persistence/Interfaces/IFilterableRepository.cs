using Sample.Common.DTOs.Responses;

namespace Sample.Data.Persistence.Interfaces
{
    public interface IFilterableRepository<T, TFilter>
    {
        Task<PagedResult<T>> GetPaged(TFilter filter);
    }
}