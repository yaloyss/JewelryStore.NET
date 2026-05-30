using Catalog.API.Controllers;
using Catalog.BLL.DTOs.Product;
using Catalog.BLL.DTOs.Stone;
using Catalog.BLL.Exceptions;
using Catalog.BLL.Services.Interfaces;
using Catalog.DAL.Pagination;
using Catalog.Domain.Entities.Parameters;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Catalog.API.Tests
{
    public class ProductsControllerTests
    {
        private readonly Mock<IProductService> _mockService;
        private readonly Mock<ILogger<ProductsController>> _mockLogger;
        private readonly ProductsController _sut;

        public ProductsControllerTests()
        {
            _mockService = new Mock<IProductService>();
            _mockLogger  = new Mock<ILogger<ProductsController>>();
            _sut = new ProductsController(_mockService.Object, _mockLogger.Object);

            _sut.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        // GET /api/products  get all products

        [Fact]
        public async Task GetAllProducts_WithProducts_ReturnsOkWithPagedResponse()
        {
            var parameters = new ProductParameters { PageNumber = 1, PageSize = 10 };
            var items = new List<ProductDTO>
            {
                new() { ProductId = 1, Name = "Ring" },
                new() { ProductId = 2, Name = "Bracelet" }
            };
            var pagedResponse = new PagedResponse<ProductDTO>
            {
                Items      = items,
                TotalCount = 2,
                CurrentPage = 1,
                PageSize   = 10
            };

            _mockService
                .Setup(s => s.GetProductsPagedAsync(parameters, It.IsAny<CancellationToken>()))
                .ReturnsAsync(pagedResponse);

            var result = await _sut.GetAllProducts(parameters, CancellationToken.None);

            var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            ok.StatusCode.Should().Be(200);
            var response = ok.Value.Should().BeAssignableTo<PagedResponse<ProductDTO>>().Subject;
            response.Items.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAllProducts_EmptyCollection_ReturnsOkWithEmptyList()
        {
            var parameters = new ProductParameters { PageNumber = 1, PageSize = 10 };
            var pagedResponse = new PagedResponse<ProductDTO>
            {
                Items      = new List<ProductDTO>(),
                TotalCount = 0,
                CurrentPage = 1,
                PageSize   = 10
            };

            _mockService
                .Setup(s => s.GetProductsPagedAsync(parameters, It.IsAny<CancellationToken>()))
                .ReturnsAsync(pagedResponse);

            var result = await _sut.GetAllProducts(parameters, CancellationToken.None);

            var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            ok.StatusCode.Should().Be(200);
            var response = ok.Value.Should().BeAssignableTo<PagedResponse<ProductDTO>>().Subject;
            response.Items.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllProducts_ServiceThrowsValidationException_ExceptionPropagates()
        {
            var parameters = new ProductParameters { MinPrice = 500, MaxPrice = 100 };

            _mockService
                .Setup(s => s.GetProductsPagedAsync(parameters, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ValidationException("MinPrice cannot be greater than MaxPrice."));

            Func<Task> act = () => _sut.GetAllProducts(parameters, CancellationToken.None);

            await act.Should().ThrowAsync<ValidationException>();
        }

        // GET /api/products/{id} get by id

        [Fact]
        public async Task GetProductById_ExistingId_ReturnsOkWithProductDTO()
        {
            var expectedDto = new ProductDTO { ProductId = 1, Name = "Ring", Price = 500 };

            _mockService
                .Setup(s => s.GetProductByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            var result = await _sut.GetProductById(1, CancellationToken.None);

            result.Result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(expectedDto);
        }

        [Fact]
        public async Task GetProductById_NonExistingId_ExceptionPropagates()
        {
            // middleware обробляє NotFoundException 404, тому перевіряє що виняток виходить назовні
            _mockService
                .Setup(s => s.GetProductByIdAsync(999, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException("Product with ID 999 not found."));

            Func<Task> act = () => _sut.GetProductById(999, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task GetProductById_InvalidId_ExceptionPropagates()
        {
            _mockService
                .Setup(s => s.GetProductByIdAsync(0, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ValidationException("ProductId must be greater than 0."));

            Func<Task> act = () => _sut.GetProductById(0, CancellationToken.None);

            await act.Should().ThrowAsync<ValidationException>();
        }

        // GET /api/products/{id}/details  get product with details)

        [Fact]
        public async Task GetProductWithDetails_ExistingId_ReturnsOkWithDetailedDTO()
        {
            var expectedDto = new ProductDetailedInfoDTO { ProductId = 1, Name = "Ring" };

            _mockService
                .Setup(s => s.GetProductWithDetailsAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            var result = await _sut.GetProductWithDetails(1, CancellationToken.None);

            result.Result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(expectedDto);
        }

        [Fact]
        public async Task GetProductWithDetails_NonExistingId_ExceptionPropagates()
        {
            _mockService
                .Setup(s => s.GetProductWithDetailsAsync(999, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException("Product with ID 999 not found."));

            Func<Task> act = () => _sut.GetProductWithDetails(999, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        // POST /api/products  create product

        [Fact]
        public async Task CreateProduct_ValidDto_ReturnsCreatedAtActionWith201()
        {
            var createDto = new CreateProductDTO
            {
                Name       = "Ring",
                Price      = 500,
                Weight     = 5,
                CategoryId = 1
            };
            var createdDto = new ProductDTO { ProductId = 7, Name = "Ring", Price = 500 };

            _mockService
                .Setup(s => s.CreateProductAsync(createDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdDto);

            var result = await _sut.CreateProduct(createDto, CancellationToken.None);

            var created = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
            created.StatusCode.Should().Be(201);
            created.ActionName.Should().Be(nameof(ProductsController.GetProductById));
            created.RouteValues!["id"].Should().Be(createdDto.ProductId);
            created.Value.Should().BeEquivalentTo(createdDto);
        }

        [Fact]
        public async Task CreateProduct_ValidDto_CallsServiceOnce()
        {
            var createDto  = new CreateProductDTO { Name = "Ring", Price = 100, Weight = 5, CategoryId = 1 };
            var createdDto = new ProductDTO { ProductId = 1, Name = "Ring" };

            _mockService
                .Setup(s => s.CreateProductAsync(createDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdDto);

            await _sut.CreateProduct(createDto, CancellationToken.None);

            _mockService.Verify( s => s.CreateProductAsync(createDto, It.IsAny<CancellationToken>()), Times.Once);
        }
        

        [Fact]
        public async Task CreateProduct_CategoryNotFound_ExceptionPropagates()
        {
            var createDto = new CreateProductDTO { Name = "Ring", Price = 100, Weight = 5, CategoryId = 99 };

            _mockService
                .Setup(s => s.CreateProductAsync(createDto, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException("Category with ID 99 not found."));

            Func<Task> act = () => _sut.CreateProduct(createDto, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        // DELETE /api/products/{id}  delete product

        [Fact]
        public async Task DeleteProduct_ExistingId_ReturnsNoContent()
        {
            _mockService
                .Setup(s => s.DeleteProductAsync(1, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _sut.DeleteProduct(1, CancellationToken.None);

            result.Should().BeOfType<NoContentResult>()
                .Which.StatusCode.Should().Be(204);
        }

        [Fact]
        public async Task DeleteProduct_ExistingId_CallsServiceOnce()
        {
            _mockService
                .Setup(s => s.DeleteProductAsync(1, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await _sut.DeleteProduct(1, CancellationToken.None);

            _mockService.Verify(
                s => s.DeleteProductAsync(1, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task DeleteProduct_NonExistingId_ExceptionPropagates()
        {
            _mockService
                .Setup(s => s.DeleteProductAsync(999, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException("Product with ID 999 not found."));

            Func<Task> act = () => _sut.DeleteProduct(999, CancellationToken.None);

            // Assert — middleware перехопить NotFoundException 404
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task DeleteProduct_InvalidId_ExceptionPropagates()
        {
            _mockService
                .Setup(s => s.DeleteProductAsync(0, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ValidationException("ProductId must be greater than 0."));

            Func<Task> act = () => _sut.DeleteProduct(0, CancellationToken.None);

            await act.Should().ThrowAsync<ValidationException>();
        }

        // GET /api/products/by-stone-names  get products by stone names

        [Fact]
        public async Task GetProductsByStoneNames_ValidNames_ReturnsOkWithProducts()
        {
            // Arrange
            var stoneNames = new List<string> { "Ruby", "Sapphire" };
            var expectedDtos = new List<ProductDTO>
            {
                new() { ProductId = 1, Name = "Ring" },
                new() { ProductId = 2, Name = "Bracelet" }
            };

            _mockService
                .Setup(s => s.GetProductsByStoneNamesAsync(stoneNames, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDtos);

            // Act
            var result = await _sut.GetProductsByStoneNames(stoneNames, CancellationToken.None);

            // Assert
            var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            ok.StatusCode.Should().Be(200);
            ok.Value.Should().BeEquivalentTo(expectedDtos);
        }

        [Fact]
        public async Task GetProductsByStoneNames_StoneNotFound_ExceptionPropagates()
        {
            var stoneNames = new List<string> { "Unknown" };

            _mockService
                .Setup(s => s.GetProductsByStoneNamesAsync(stoneNames, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException("Stone with name 'Unknown' not found."));

            Func<Task> act = () => _sut.GetProductsByStoneNames(stoneNames, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("*Unknown*");
        }

        // GET /api/products/with-multiple-stones  get products with multiple stones

        [Fact]
        public async Task GetProductsWithMultipleStones_ReturnsOkWithDetailedDTOs()
        {
            var expectedDtos = new List<ProductDetailedInfoDTO>
            {
                new() { ProductId = 1, Name = "Ring" },
                new() { ProductId = 2, Name = "Necklace" }
            };

            _mockService
                .Setup(s => s.GetProductsWithMultipleStonesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDtos);

            var result = await _sut.GetProductsWithMultipleStones(CancellationToken.None);

            var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            ok.StatusCode.Should().Be(200);
            ok.Value.Should().BeEquivalentTo(expectedDtos);
        }

        [Fact]
        public async Task GetProductsWithMultipleStones_EmptyResult_ReturnsOkWithEmptyList()
        {
            _mockService
                .Setup(s => s.GetProductsWithMultipleStonesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ProductDetailedInfoDTO>());

            var result = await _sut.GetProductsWithMultipleStones(CancellationToken.None);

            var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            ok.StatusCode.Should().Be(200);
            ok.Value.Should().BeAssignableTo<IEnumerable<ProductDetailedInfoDTO>>()
                .Which.Should().BeEmpty();
        }

        // GET /api/products/{id}/stones-of-product  get product stones

        [Fact]
        public async Task GetProductStones_ExistingProduct_ReturnsOkWithStones()
        {
            // Arrange
            var expectedStones = new List<StoneDTO>
            {
                new() { StoneId = 1, Name = "Ruby" },
                new() { StoneId = 2, Name = "Sapphire" }
            };

            _mockService
                .Setup(s => s.GetProductStonesAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedStones);

            // Act
            var result = await _sut.GetProductStones(1, CancellationToken.None);

            // Assert
            var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            ok.StatusCode.Should().Be(200);
            ok.Value.Should().BeEquivalentTo(expectedStones);
        }

        [Fact]
        public async Task GetProductStones_ProductNotFound_ExceptionPropagates()
        {
            _mockService
                .Setup(s => s.GetProductStonesAsync(999, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException("Product with ID 999 not found."));

            Func<Task> act = () => _sut.GetProductStones(999, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task GetProductStones_InvalidId_ExceptionPropagates()
        {
            _mockService
                .Setup(s => s.GetProductStonesAsync(0, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ValidationException("ProductId must be greater than 0."));

            Func<Task> act = () => _sut.GetProductStones(0, CancellationToken.None);

            await act.Should().ThrowAsync<ValidationException>();
        }

        // POST /api/products/{productId}/stones/{stoneId}  add stone to product

        [Fact]
        public async Task AddStoneToProduct_ValidIds_ReturnsNoContent()
        {
            _mockService
                .Setup(s => s.AddStoneToProductAsync(1, 2, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _sut.AddStoneToProduct(1, 2, CancellationToken.None);

            result.Should().BeOfType<NoContentResult>()
                .Which.StatusCode.Should().Be(204);
        }

        [Fact]
        public async Task AddStoneToProduct_ValidIds_CallsServiceOnce()
        {
            _mockService
                .Setup(s => s.AddStoneToProductAsync(1, 2, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await _sut.AddStoneToProduct(1, 2, CancellationToken.None);

            _mockService.Verify( s => s.AddStoneToProductAsync(1, 2, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddStoneToProduct_ProductNotFound_ExceptionPropagates()
        {
            _mockService
                .Setup(s => s.AddStoneToProductAsync(999, 1, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException("Product with ID 999 not found."));

            Func<Task> act = () => _sut.AddStoneToProduct(999, 1, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task AddStoneToProduct_StoneNotFound_ExceptionPropagates()
        {
            _mockService
                .Setup(s => s.AddStoneToProductAsync(1, 999, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException("Stone with ID 999 not found."));

            Func<Task> act = () => _sut.AddStoneToProduct(1, 999, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task AddStoneToProduct_StoneAlreadyAdded_ExceptionPropagates()
        {
            // middleware обробить BusinessConflictException 409 Conflict
            _mockService
                .Setup(s => s.AddStoneToProductAsync(1, 2, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new BusinessConflictException("Stone 'Ruby' is already added to this product."));

            Func<Task> act = () => _sut.AddStoneToProduct(1, 2, CancellationToken.None);

            await act.Should().ThrowAsync<BusinessConflictException>();
        }

        [Fact]
        public async Task AddStoneToProduct_InvalidIds_ExceptionPropagates()
        {
            _mockService
                .Setup(s => s.AddStoneToProductAsync(0, 1, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ValidationException("ProductId must be greater than 0."));

            Func<Task> act = () => _sut.AddStoneToProduct(0, 1, CancellationToken.None);

            await act.Should().ThrowAsync<ValidationException>();
        }

        // DELETE /api/products/{productId}/stones/{stoneId} remove stone from product

        [Fact]
        public async Task RemoveStoneFromProduct_ExistingRelation_ReturnsNoContent()
        {
            _mockService
                .Setup(s => s.RemoveStoneFromProductAsync(1, 2, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _sut.RemoveStoneFromProduct(1, 2, CancellationToken.None);

            result.Should().BeOfType<NoContentResult>()
                .Which.StatusCode.Should().Be(204);
        }

        [Fact]
        public async Task RemoveStoneFromProduct_ExistingRelation_CallsServiceOnce()
        {
            _mockService
                .Setup(s => s.RemoveStoneFromProductAsync(1, 2, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await _sut.RemoveStoneFromProduct(1, 2, CancellationToken.None);

            _mockService.Verify(
                s => s.RemoveStoneFromProductAsync(1, 2, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task RemoveStoneFromProduct_ProductNotFound_ExceptionPropagates()
        {
            _mockService
                .Setup(s => s.RemoveStoneFromProductAsync(999, 2, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException("Product with ID 999 not found."));

            Func<Task> act = () => _sut.RemoveStoneFromProduct(999, 2, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task RemoveStoneFromProduct_StoneNotAssociated_ExceptionPropagates()
        {
            _mockService
                .Setup(s => s.RemoveStoneFromProductAsync(1, 99, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException("Stone with ID 99 is not associated with product ID 1."));

            Func<Task> act = () => _sut.RemoveStoneFromProduct(1, 99, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("*99*");
        }

        [Fact]
        public async Task RemoveStoneFromProduct_InvalidIds_ExceptionPropagates()
        {
            _mockService
                .Setup(s => s.RemoveStoneFromProductAsync(0, 1, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ValidationException("ProductId must be greater than 0."));

            Func<Task> act = () => _sut.RemoveStoneFromProduct(0, 1, CancellationToken.None);

            await act.Should().ThrowAsync<ValidationException>();
        }
    }
}