using AutoFixture.Xunit2;
using AutoMapper;
using Moq;
using Sample.Common.DTOs.Requests;
using Sample.Core.Services;
using Sample.Core.Services.Interfaces;
using Sample.Data.Entities;
using Sample.Data.Persistence.Interfaces;
using Sample.Test.Utils;
using Xunit;

namespace Sample.Test.Services
{
    public class UserServiceTests
    {
        [Theory()]
        [DefaultData()]
        public async Task AddAsync_HappyCase_ShouldBeOk(
            UserRequest request,
            [Frozen] Mock<IMapper> mockMapper,
            [Frozen] Mock<IUserRepository> mockUserRepository,
            [Frozen] Mock<IValidatorService> mockValidatorService,
            UserService sut)
        {
            // Act
            await sut.RegisterAsync(request);

            // Assert
            mockMapper.Verify(r => r.Map<User>(request), Times.Once);
            mockValidatorService.Verify(r => r.Validate(request), Times.Once);
            mockUserRepository.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<bool>()), Times.Once);
        }
    }
}