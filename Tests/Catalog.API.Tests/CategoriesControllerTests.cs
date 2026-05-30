using Catalog.API.Controllers;
using Catalog.BLL.DTOs.Category;
using Catalog.BLL.DTOs.Product;
using Catalog.BLL.Exceptions;
using Catalog.BLL.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Catalog.API.Tests
{
    public class CategoriesControllerTests
    {
        private readonly Mock<ICategoryService> _categoryServiceMock;
        private readonly Mock<ILogger<CategoriesController>> _loggerMock;
        private readonly CategoriesController _sut;

        public CategoriesControllerTests()
        {
            _categoryServiceMock = new Mock<ICategoryService>();
            _loggerMock = new Mock<ILogger<CategoriesController>>();
            _sut = new CategoriesController(_categoryServiceMock.Object, _loggerMock.Object);
        }

        // GET /api/categories

        [Fact]
        public async Task GetAllCategories_HasCategories_Returns200WithList()
        {
            // Arrange
            var categories = new List<CategoryDTO>
            {
                new() { CategoryId = 1, Name = "Rings" },
                new() { CategoryId = 2, Name = "Earrings" }
            };
            _categoryServiceMock
                .Setup(s => s.GetAllCategoriesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(categories);

            // Act
            var result = await _sut.GetAllCategories(CancellationToken.None);

            // Assert
            var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            ok.StatusCode.Should().Be(200);
            var body = ok.Value.Should().BeAssignableTo<IEnumerable<CategoryDTO>>().Subject;
            body.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAllCategories_NoCategories_Returns200WithEmptyList()
        {
            // Arrange
            _categoryServiceMock
                .Setup(s => s.GetAllCategoriesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<CategoryDTO>());

            // Act
            var result = await _sut.GetAllCategories(CancellationToken.None);

            // Assert
            var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var body = ok.Value.Should().BeAssignableTo<IEnumerable<CategoryDTO>>().Subject;
            body.Should().BeEmpty();
        }

        // GET /api/categories/{id}

        [Fact]
        public async Task GetCategoryById_ExistingId_Returns200WithCategory()
        {
            var category = new CategoryDTO { CategoryId = 1, Name = "Rings" };
            _categoryServiceMock
                .Setup(s => s.GetCategoryByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);

            var result = await _sut.GetCategoryById(1, CancellationToken.None);

            var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var body = ok.Value.Should().BeOfType<CategoryDTO>().Subject;
            body.CategoryId.Should().Be(1);
            body.Name.Should().Be("Rings");
        }

        [Fact]
        public async Task GetCategoryById_NonExistingId_ThrowsNotFoundException()
        {
            _categoryServiceMock
                .Setup(s => s.GetCategoryByIdAsync(999, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException("Category with ID 999 not found."));

            var act = async () => await _sut.GetCategoryById(999, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task GetCategoryById_InvalidId_ThrowsValidationException()
        {
            _categoryServiceMock
                .Setup(s => s.GetCategoryByIdAsync(-1, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ValidationException("CategoryId must be greater than 0."));

            var act = async () => await _sut.GetCategoryById(-1, CancellationToken.None);

            await act.Should().ThrowAsync<ValidationException>();
        }

        // GET /api/categories/{id}/details

        [Fact]
        public async Task GetCategoryWithDetails_ExistingId_Returns200WithProducts()
        {
            // Arrange
            var dto = new CategoryWithInfoDTO
            {
                CategoryId = 1,
                Name = "Rings",
                ProductCount = 2,
                Products = new List<ProductDTO>
                {
                    new() { ProductId = 1, Name = "Ring A" },
                    new() { ProductId = 2, Name = "Ring B" }
                }
            };
            _categoryServiceMock
                .Setup(s => s.GetCategoryWithDetailsAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dto);

            // Act
            var result = await _sut.GetCategoryWithDetails(1, CancellationToken.None);

            // Assert
            var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var body = ok.Value.Should().BeOfType<CategoryWithInfoDTO>().Subject;
            body.Products.Should().HaveCount(2);
        }

        // GET /api/categories/{id}/statistics

        [Fact]
        public async Task GetCategoryStatistics_ExistingId_Returns200WithStats()
        {
            var stats = new CategoryStatisticsDTO
            {
                CategoryId = 1,
                Name = "Rings",
                TotalProducts = 4,
                GoldenProducts = 2,
                SilverProducts = 1
            };
            _categoryServiceMock
                .Setup(s => s.GetCategoryStatisticsAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(stats);

            var result = await _sut.GetCategoryStatistics(1, CancellationToken.None);

            var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var body = ok.Value.Should().BeOfType<CategoryStatisticsDTO>().Subject;
            body.TotalProducts.Should().Be(4);
            body.GoldenProducts.Should().Be(2);
            body.SilverProducts.Should().Be(1);
        }

        // GET /api/categories/{id}/products

        [Fact]
        public async Task GetProductsForCategory_CategoryWithProducts_Returns200WithProducts()
        {
            // Arrange
            var products = new List<ProductDTO>
            {
                new() { ProductId = 1, Name = "Ring A" },
                new() { ProductId = 2, Name = "Ring B" }
            };
            _categoryServiceMock
                .Setup(s => s.GetProductsForCategoryAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(products);

            // Act
            var result = await _sut.GetProductsForCategory(1, CancellationToken.None);

            // Assert
            var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var body = ok.Value.Should().BeAssignableTo<IEnumerable<ProductDTO>>().Subject;
            body.Should().HaveCount(2);
        }

        // GET /api/categories/{id}/products/count

        [Fact]
        public async Task GetProductCount_ExistingCategory_Returns200WithCount()
        {
            _categoryServiceMock
                .Setup(s => s.GetProductCountByCategoryAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(5);

            var result = await _sut.GetProductCount(1, CancellationToken.None);

            var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().Be(5);
        }

        // POST /api/categories

        [Fact]
        public async Task CreateCategory_ValidDto_Returns201WithCreatedCategory()
        {
            // Arrange
            var dto = new CreateCategoryDTO { Name = "Bracelets" };
            var created = new CategoryDTO { CategoryId = 6, Name = "Bracelets" };
            _categoryServiceMock
                .Setup(s => s.CreateCategoryAsync(dto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(created);

            // Act
            var result = await _sut.CreateCategory(dto, CancellationToken.None);

            // Assert
            var createdAt = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdAt.StatusCode.Should().Be(201);
            var body = createdAt.Value.Should().BeOfType<CategoryDTO>().Subject;
            body.CategoryId.Should().Be(6);
            body.Name.Should().Be("Bracelets");
        }

        [Fact]
        public async Task CreateCategory_DuplicateName_ThrowsBusinessConflictException()
        {
            var dto = new CreateCategoryDTO { Name = "Rings" };
            _categoryServiceMock
                .Setup(s => s.CreateCategoryAsync(dto, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new BusinessConflictException("Category with name 'Rings' already exists."));

            var act = async () => await _sut.CreateCategory(dto, CancellationToken.None);

            await act.Should().ThrowAsync<BusinessConflictException>();
        }

        // DELETE /api/categories/{id}

        [Fact]
        public async Task DeleteCategory_ExistingEmptyCategory_Returns204()
        {
            _categoryServiceMock
                .Setup(s => s.DeleteCategoryAsync(1, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _sut.DeleteCategory(1, CancellationToken.None);

            result.Should().BeOfType<NoContentResult>()
                .Which.StatusCode.Should().Be(204);
        }

        [Fact]
        public async Task DeleteCategory_CategoryWithProducts_ThrowsBusinessConflictException()
        {
            _categoryServiceMock
                .Setup(s => s.DeleteCategoryAsync(1, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new BusinessConflictException("Cannot delete category because it has products."));

            var act = async () => await _sut.DeleteCategory(1, CancellationToken.None);

            await act.Should().ThrowAsync<BusinessConflictException>();
        }

        [Fact]
        public async Task DeleteCategory_NonExistingId_ThrowsNotFoundException()
        {
            _categoryServiceMock
                .Setup(s => s.DeleteCategoryAsync(999, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException("Category with ID 999 not found."));

            var act = async () => await _sut.DeleteCategory(999, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}