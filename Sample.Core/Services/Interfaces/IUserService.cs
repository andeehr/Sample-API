using Sample.Common.DTOs.Requests;
using Sample.Common.DTOs.Responses;

namespace Sample.Core.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserResponse> LoginAsync(string user, string password);

        Task RegisterAsync(UserRequest request);
    }
}