using AutoMapper;
using Microsoft.Extensions.Logging;
using Sample.Common.DTOs.Requests;
using Sample.Common.DTOs.Responses;
using Sample.Common.Exceptions;
using Sample.Core.Services.Interfaces;
using Sample.Data.Entities;
using Sample.Data.Persistence.Interfaces;

namespace Sample.Core.Services
{
    public class UserService : BaseService<UserService>, IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IValidatorService _validatorService;

        public UserService(
            IUserRepository userRepository,
            IMapper mapper,
            IValidatorService validatorService,
            ILogger<UserService> logger) : base(logger, mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _validatorService = validatorService ?? throw new ArgumentNullException(nameof(validatorService));
        }

        public async Task<UserResponse> LoginAsync(string username, string password)
        {
            var user = await _userRepository.GetByUsernameAsync(username);

            if (!IsValidPassword(password, user.Password))
                throw new DomainException("Wrong password");

            return _mapper.Map<UserResponse>(user);
        }

        public static bool IsValidPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

        public async Task<UserResponse> RegisterAsync(UserRequest request)
        {
            _validatorService.Validate(request);
            request.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var user = _mapper.Map<User>(request);
            await _userRepository.AddAsync(user);
            return await GetById(user.Id);
        }

        public async Task<UserResponse> GetById(long id)
        {
            var user = await _userRepository.GetByIdAsync(id, u => u.Role, u => u.Role.Permissions);
            return _mapper.Map<UserResponse>(user);
        }

        public async Task<PagedResult<UserResponse>> GetAllByFilters(UserFilter request)
        {
            var data = await _userRepository.GetPaged(request);
            return _mapper.Map<PagedResult<UserResponse>>(data);
        }
    }
}