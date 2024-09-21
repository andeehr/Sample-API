using Sample.Common.DTOs.Requests;
using Sample.Common.DTOs.Responses;

namespace Sample.Core.Services.Interfaces
{
    public interface IUserService
    {
        Task<PagedResult<UserResponse>> GetAllByFilters(UserFilter request);

        Task<UserResponse> LoginAsync(string user, string password);

        Task RegisterAsync(UserRequest request);
    }
}