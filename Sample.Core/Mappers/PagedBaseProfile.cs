using AutoMapper;
using Sample.Common.DTOs.Responses;

namespace Sample.Core.Mappers
{
    public abstract class PagedBaseProfile<T, TResponse> : Profile
    {
        public PagedBaseProfile()
        {
            CreateMap<PagedResult<T>, PagedResult<TResponse>>();
        }
    }
}