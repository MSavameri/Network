using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NetworkInfrastructure.Web.Controllers;
using NetworkInfrastructure.Web.Data.Entities;
using NetworkInfrastructure.Web.Data.Services;
using NetworkInfrastructure.Web.Models;
using System.Security.Claims;

namespace NetworkInfrastructure.Tests
{
    [TestClass]
    public class NetworkControllerTests
    {
        private Mock<INetworkService> _mockNetworkService = null!;
        private Mock<IUserService> _mockUserService = null!;
        private Mock<IValidator<UserDto>> _mockUserValidator = null!;
        private Mock<IValidator<NetworkAssetDto>> _mockNetAssetValidator = null!;
        private Mock<IConfiguration> _mockConfiguration = null!;
        private Mock<IMapper> _mockMapper = null!;
        private Mock<ILogger<NetworkController>> _mockLogger = null!;
        private NetworkController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockNetworkService = new Mock<INetworkService>();
            _mockUserService = new Mock<IUserService>();
            _mockUserValidator = new Mock<IValidator<UserDto>>();
            _mockNetAssetValidator = new Mock<IValidator<NetworkAssetDto>>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<NetworkController>>();

            _controller = new NetworkController(
                _mockNetworkService.Object,
                _mockUserService.Object,
                _mockUserValidator.Object,
                _mockConfiguration.Object,
                _mockMapper.Object,
                _mockNetAssetValidator.Object,
                _mockLogger.Object
            );

            // Setup default valid validation result
            var validValidationResult = new ValidationResult();
            _mockNetAssetValidator.Setup(v => v.Validate(It.IsAny<NetworkAssetDto>()))
                                  .Returns(validValidationResult);

            // Setup default mapper behavior
            _mockMapper.Setup(m => m.Map<NetworkAsset>(It.IsAny<NetworkAssetDto>()))
                       .Returns((NetworkAssetDto dto) => new NetworkAsset { ServerName = dto.ServerName }); // Simple map for testing

            // Setup HttpContext and User claims
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "testuser")
            }, "mock"));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [TestMethod]
        public async Task Create_Post_WithValidModel_ReturnsRedirectToIndex()
        {
            // Arrange
            var networkAssetDto = new NetworkAssetDto { ServerName = "TestServer", Ip = "1.2.3.4" };
            var networkAsset = new NetworkAsset { Id = Guid.NewGuid(), ServerName = "TestServer" };

            _mockMapper.Setup(m => m.Map<NetworkAsset>(networkAssetDto))
                       .Returns(networkAsset);
            _mockNetworkService.Setup(s => s.AddAsync(networkAsset))
                               .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Create(networkAssetDto);

            // Assert
            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectResult.ActionName.Should().Be("Index");
            _mockNetworkService.Verify(s => s.AddAsync(networkAsset), Times.Once);
            // Verify logger messages if necessary (can be complex, optional for now)
        }

        [TestMethod]
        public async Task Create_Post_WithInvalidModel_ReturnsViewWithModelError()
        {
            // Arrange
            var networkAssetDto = new NetworkAssetDto { ServerName = "TestServer" }; // Assume IP is required for invalid state
            var validationResult = new ValidationResult(new[] { new ValidationFailure("Ip", "IP Address is required") });

            _mockNetAssetValidator.Setup(v => v.Validate(networkAssetDto))
                                  .Returns(validationResult);

            // Ensure ModelState is clear for this test if it's shared or modified by controller
            _controller.ModelState.Clear();

            // Act
            var result = await _controller.Create(networkAssetDto);

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.Model.Should().Be(networkAssetDto);
            _controller.ModelState.IsValid.Should().BeFalse();
            _controller.ModelState.Should().Contain(kv => kv.Key == "Ip" && kv.Value.Errors.Any(e => e.ErrorMessage == "IP Address is required"));
            _mockNetworkService.Verify(s => s.AddAsync(It.IsAny<NetworkAsset>()), Times.Never);
        }

        [TestMethod]
        public async Task Create_Post_WhenUserClaimIsMissing_ReturnsErrorView()
        {
            // Arrange
            var networkAssetDto = new NetworkAssetDto { ServerName = "TestServer", Ip = "1.2.3.4" };
            // Simulate User.FindFirstValue(ClaimTypes.Name) returning null
            var userWithoutNameClaim = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]{}, "mock")); // No claims
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userWithoutNameClaim }
            };

            // The controller will assign null to networkAssetDto.UserName
            // We then make the service throw ArgumentNullException if UserName on the mapped entity is null.
             _mockMapper.Setup(m => m.Map<NetworkAsset>(It.IsAny<NetworkAssetDto>()))
                       .Returns((NetworkAssetDto dto) => new NetworkAsset { ServerName = dto.ServerName, UserName = dto.UserName });

            // The controller should throw InvalidOperationException before the service is called when user claim is missing.
            // So, the mock setup for _mockNetworkService to throw an exception is not relevant for this specific test path.

            // Act
            var result = await _controller.Create(networkAssetDto);

            // Assert
            // Expect the general exception handler to catch the InvalidOperationException and return an Error view.
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.Should().NotBeNull(); // Subject can be null if the original result was null, BeOfType would fail first though.
            viewResult.ViewName.Should().Be("Error");
            viewResult.Model.Should().BeOfType<ErrorViewModel>();

            // The service's AddAsync should not be called if the controller throws due to missing user claim.
            _mockNetworkService.Verify(s => s.AddAsync(It.IsAny<NetworkAsset>()), Times.Never);
        }

        [TestMethod]
        public async Task Create_Post_WhenServiceThrowsGeneralException_ReturnsErrorView()
        {
            // Arrange
            var networkAssetDto = new NetworkAssetDto { ServerName = "TestServer", Ip = "1.2.3.4" };
            var networkAsset = new NetworkAsset { ServerName = "TestServer" }; // UserName will be "testuser" by default setup

            _mockMapper.Setup(m => m.Map<NetworkAsset>(networkAssetDto))
                       .Returns(networkAsset);
            _mockNetworkService.Setup(s => s.AddAsync(networkAsset))
                               .ThrowsAsync(new Exception("Database connection failed")); // Simulate a general exception

            // Act
            var result = await _controller.Create(networkAssetDto);

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.ViewName.Should().Be("Error");
            viewResult.Model.Should().BeOfType<ErrorViewModel>();
            _mockNetworkService.Verify(s => s.AddAsync(networkAsset), Times.Once);
        }
    }
}
