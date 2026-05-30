using Catalog.API.Controllers;
using Catalog.BLL.DTOs.Metal;
using Catalog.BLL.Exceptions;
using Catalog.BLL.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Catalog.API.Tests
{
    public class MetalsControllerTests
    {
        private readonly Mock<IMetalService> _metalServiceMock;
        private readonly Mock<ILogger<MetalsController>> _loggerMock;
        private readonly MetalsController _sut;

        public MetalsControllerTests()
        {
            _metalServiceMock = new Mock<IMetalService>();
            _loggerMock = new Mock<ILogger<MetalsController>>();
            _sut = new MetalsController(_metalServiceMock.Object, _loggerMock.Object);
        }

        // GET /api/metals

        [Fact]
        public async Task GetAllMetals_HasMetals_Returns200WithList()
        {
            // Arrange
            var metals = new List<MetalDTO>
            {
                new() { MetalId = 1, Name = "Gold", Color = "Yellow" },
                new() { MetalId = 2, Name = "Silver", Color = "Silver" }
            };
            _metalServiceMock
                .Setup(s => s.GetAllMetalsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(metals);

            // Act
            var result = await _sut.GetAllMetals(CancellationToken.None);

            // Assert
            var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            ok.StatusCode.Should().Be(200);
            var body = ok.Value.Should().BeAssignableTo<IEnumerable<MetalDTO>>().Subject;
            body.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAllMetals_NoMetals_Returns200WithEmptyList()
        {
            _metalServiceMock
                .Setup(s => s.GetAllMetalsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<MetalDTO>());

            var result = await _sut.GetAllMetals(CancellationToken.None);

            var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeAssignableTo<IEnumerable<MetalDTO>>().Which.Should().BeEmpty();
        }

        // GET /api/metals/{id}

        [Fact]
        public async Task GetMetalById_ExistingId_Returns200WithMetal()
        {
            // Arrange
            var metal = new MetalDTO { MetalId = 1, Name = "Gold", Color = "Yellow" };
            _metalServiceMock
                .Setup(s => s.GetMetalByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(metal);

            // Act
            var result = await _sut.GetMetalById(1, CancellationToken.None);

            // Assert
            var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var body = ok.Value.Should().BeOfType<MetalDTO>().Subject;
            body.MetalId.Should().Be(1);
            body.Name.Should().Be("Gold");
        }

        [Fact]
        public async Task GetMetalById_NonExistingId_ThrowsNotFoundException()
        {
            _metalServiceMock
                .Setup(s => s.GetMetalByIdAsync(999, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException("Metal with ID 999 not found."));

            var act = async () => await _sut.GetMetalById(999, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task GetMetalById_InvalidId_ThrowsValidationException()
        {
            _metalServiceMock
                .Setup(s => s.GetMetalByIdAsync(-1, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ValidationException("MetalId must be greater than 0."));

            var act = async () => await _sut.GetMetalById(-1, CancellationToken.None);

            await act.Should().ThrowAsync<ValidationException>();
        }

        // GET /api/metals/by-name/{name}

        [Fact]
        public async Task GetMetalByName_ExistingName_Returns200WithMetal()
        {
            var metal = new MetalDTO { MetalId = 1, Name = "Gold", Color = "Yellow" };
            _metalServiceMock
                .Setup(s => s.GetMetalByNameAsync("Gold", It.IsAny<CancellationToken>()))
                .ReturnsAsync(metal);

            var result = await _sut.GetMetalByName("Gold", CancellationToken.None);

            var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var body = ok.Value.Should().BeOfType<MetalDTO>().Subject;
            body.Name.Should().Be("Gold");
        }

        [Fact]
        public async Task GetMetalByName_NonExistingName_ThrowsNotFoundException()
        {
            _metalServiceMock
                .Setup(s => s.GetMetalByNameAsync("Adamantium", It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException("Metal with name 'Adamantium' not found."));

            var act = async () => await _sut.GetMetalByName("Adamantium", CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task GetMetalByName_EmptyName_ThrowsValidationException()
        {
            _metalServiceMock
                .Setup(s => s.GetMetalByNameAsync("", It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ValidationException("Metal name cannot be empty."));

            var act = async () => await _sut.GetMetalByName("", CancellationToken.None);

            await act.Should().ThrowAsync<ValidationException>();
        }

        // POST /api/metals

        [Fact]
        public async Task CreateMetal_ValidDto_Returns201WithCreatedMetal()
        {
            var dto = new CreateMetalDTO { Name = "Platinum", Color = "White" };
            var created = new MetalDTO { MetalId = 5, Name = "Platinum", Color = "White" };
            _metalServiceMock
                .Setup(s => s.CreateMetalAsync(dto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(created);

            var result = await _sut.CreateMetal(dto, CancellationToken.None);

            var createdAt = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdAt.StatusCode.Should().Be(201);
            var body = createdAt.Value.Should().BeOfType<MetalDTO>().Subject;
            body.MetalId.Should().Be(5);
            body.Name.Should().Be("Platinum");
        }

        [Fact]
        public async Task CreateMetal_DuplicateName_ThrowsBusinessConflictException()
        {
            var dto = new CreateMetalDTO { Name = "Gold", Color = "Yellow" };
            _metalServiceMock
                .Setup(s => s.CreateMetalAsync(dto, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new BusinessConflictException("Metal with name 'Gold' already exists."));

            var act = async () => await _sut.CreateMetal(dto, CancellationToken.None);

            await act.Should().ThrowAsync<BusinessConflictException>();
        }

        // DELETE /api/metals/{id}

        [Fact]
        public async Task DeleteMetal_ExistingUnusedMetal_Returns204()
        {
            _metalServiceMock
                .Setup(s => s.DeleteMetalAsync(1, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _sut.DeleteMetal(1, CancellationToken.None);

            result.Should().BeOfType<NoContentResult>()
                .Which.StatusCode.Should().Be(204);
        }

        [Fact]
        public async Task DeleteMetal_MetalUsedInProducts_ThrowsBusinessConflictException()
        {
            _metalServiceMock
                .Setup(s => s.DeleteMetalAsync(1, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new BusinessConflictException("Cannot delete metal 'Gold' because it is used in 3 product(s)."));

            var act = async () => await _sut.DeleteMetal(1, CancellationToken.None);

            await act.Should().ThrowAsync<BusinessConflictException>();
        }

        [Fact]
        public async Task DeleteMetal_NonExistingId_ThrowsNotFoundException()
        {
            _metalServiceMock
                .Setup(s => s.DeleteMetalAsync(999, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException("Metal with ID 999 not found."));

            var act = async () => await _sut.DeleteMetal(999, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}