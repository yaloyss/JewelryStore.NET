using Catalog.API.Controllers;
using Catalog.BLL.DTOs.Stone;
using Catalog.BLL.Exceptions;
using Catalog.BLL.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Catalog.API.Tests
{
    public class StonesControllerTests
    {
        private readonly Mock<IStoneService> _stoneServiceMock;
        private readonly Mock<ILogger<StonesController>> _loggerMock;
        private readonly StonesController _sut;

        public StonesControllerTests()
        {
            _stoneServiceMock = new Mock<IStoneService>();
            _loggerMock = new Mock<ILogger<StonesController>>();
            _sut = new StonesController(_stoneServiceMock.Object, _loggerMock.Object);
        }

        // GET

        [Fact]
        public async Task GetAllStones_HasStones_Returns200WithList()
        {
            // Arrange
            var stones = new List<StoneDTO>
            {
                new() { StoneId = 1, Name = "Diamond" },
                new() { StoneId = 2, Name = "Ruby" },
                new() { StoneId = 3, Name = "Emerald" }
            };
            _stoneServiceMock
                .Setup(s => s.GetAllStonesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(stones);

            var result = await _sut.GetAllStones(CancellationToken.None);

            var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            ok.StatusCode.Should().Be(200);
            var body = ok.Value.Should().BeAssignableTo<IEnumerable<StoneDTO>>().Subject;
            body.Should().HaveCount(3);
        }

        [Fact]
        public async Task GetAllStones_NoStones_Returns200WithEmptyList()
        {
            _stoneServiceMock
                .Setup(s => s.GetAllStonesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<StoneDTO>());

            var result = await _sut.GetAllStones(CancellationToken.None);

            var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeAssignableTo<IEnumerable<StoneDTO>>().Which.Should().BeEmpty();
        }

        // GET /api/stones/{id}

        [Fact]
        public async Task GetStoneById_ExistingId_Returns200WithStone()
        {
            var stone = new StoneDTO { StoneId = 1, Name = "Diamond" };
            _stoneServiceMock
                .Setup(s => s.GetStoneByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(stone);

            var result = await _sut.GetStoneById(1, CancellationToken.None);

            var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var body = ok.Value.Should().BeOfType<StoneDTO>().Subject;
            body.StoneId.Should().Be(1);
            body.Name.Should().Be("Diamond");
        }

        [Fact]
        public async Task GetStoneById_NonExistingId_ThrowsNotFoundException()
        {
            _stoneServiceMock
                .Setup(s => s.GetStoneByIdAsync(999, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException("Stone with ID 999 not found."));

            var act = async () => await _sut.GetStoneById(999, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task GetStoneById_InvalidId_ThrowsValidationException()
        {
            _stoneServiceMock
                .Setup(s => s.GetStoneByIdAsync(0, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ValidationException("StoneId must be greater than 0."));

            var act = async () => await _sut.GetStoneById(0, CancellationToken.None);

            await act.Should().ThrowAsync<ValidationException>();
        }

        // GET /api/stones/by-name/{name}

        [Fact]
        public async Task GetStoneByName_ExistingName_Returns200WithStone()
        {
            var stone = new StoneDTO { StoneId = 1, Name = "Diamond" };
            _stoneServiceMock
                .Setup(s => s.GetStoneByNameAsync("Diamond", It.IsAny<CancellationToken>()))
                .ReturnsAsync(stone);

            var result = await _sut.GetStoneByName("Diamond", CancellationToken.None);

            var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var body = ok.Value.Should().BeOfType<StoneDTO>().Subject;
            body.Name.Should().Be("Diamond");
        }

        [Fact]
        public async Task GetStoneByName_NonExistingName_ThrowsNotFoundException()
        {
            _stoneServiceMock
                .Setup(s => s.GetStoneByNameAsync("Kryptonite", It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException("Stone with name 'Kryptonite' not found."));

            var act = async () => await _sut.GetStoneByName("Kryptonite", CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task GetStoneByName_EmptyName_ThrowsValidationException()
        {
            _stoneServiceMock
                .Setup(s => s.GetStoneByNameAsync("   ", It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ValidationException("Stone name cannot be empty."));

            var act = async () => await _sut.GetStoneByName("   ", CancellationToken.None);

            await act.Should().ThrowAsync<ValidationException>();
        }

        // POST

        [Fact]
        public async Task CreateStone_ValidDto_Returns201WithCreatedStone()
        {
            // Arrange
            var dto = new CreateStoneDTO { Name = "Opal" };
            var created = new StoneDTO { StoneId = 7, Name = "Opal" };
            _stoneServiceMock
                .Setup(s => s.CreateStoneAsync(dto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(created);

            // Act
            var result = await _sut.CreateStone(dto, CancellationToken.None);

            // Assert
            var createdAt = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdAt.StatusCode.Should().Be(201);
            var body = createdAt.Value.Should().BeOfType<StoneDTO>().Subject;
            body.StoneId.Should().Be(7);
            body.Name.Should().Be("Opal");
        }

        [Fact]
        public async Task CreateStone_DuplicateName_ThrowsBusinessConflictException()
        {
            var dto = new CreateStoneDTO { Name = "Diamond" };
            _stoneServiceMock
                .Setup(s => s.CreateStoneAsync(dto, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new BusinessConflictException("Stone with name 'Diamond' already exists."));

            var act = async () => await _sut.CreateStone(dto, CancellationToken.None);

            await act.Should().ThrowAsync<BusinessConflictException>();
        }

        // DELETE

        [Fact]
        public async Task DeleteStone_ExistingUnusedStone_Returns204()
        {
            _stoneServiceMock
                .Setup(s => s.DeleteStoneAsync(1, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _sut.DeleteStone(1, CancellationToken.None);

            result.Should().BeOfType<NoContentResult>()
                .Which.StatusCode.Should().Be(204);
        }

        [Fact]
        public async Task DeleteStone_StoneUsedInProducts_ThrowsBusinessConflictException()
        {
            _stoneServiceMock
                .Setup(s => s.DeleteStoneAsync(1, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new BusinessConflictException("Cannot delete stone 'Diamond' because it is used in 2 product(s)."));

            var act = async () => await _sut.DeleteStone(1, CancellationToken.None);

            await act.Should().ThrowAsync<BusinessConflictException>();
        }

        [Fact]
        public async Task DeleteStone_NonExistingId_ThrowsNotFoundException()
        {
            _stoneServiceMock
                .Setup(s => s.DeleteStoneAsync(999, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException("Stone with ID 999 not found."));

            var act = async () => await _sut.DeleteStone(999, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}