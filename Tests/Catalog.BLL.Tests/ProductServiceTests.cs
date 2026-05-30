using AutoMapper;
using Catalog.BLL.DTOs.Product;
using Catalog.BLL.DTOs.Stone;
using Catalog.BLL.Exceptions;
using Catalog.BLL.Services;
using Catalog.DAL.Pagination;
using Catalog.DAL.Repositories.Interfaces;
using Catalog.DAL.UOW;
using Catalog.Domain.Entities;
using Catalog.Domain.Entities.Parameters;
using FluentAssertions;
using Moq;

namespace Catalog.BLL.Tests
{
    public class ProductServiceTests
    {
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<IMapper>     _mapperMock;
        private readonly ProductService    _sut;

        // repository mocks
        private readonly Mock<IProductRepository>  _productRepoMock;
        private readonly Mock<ICategoryRepository> _categoryRepoMock;
        private readonly Mock<IMetalRepository> _metalRepoMock;
        private readonly Mock<IStoneRepository> _stoneRepoMock;
        private readonly Mock<IProductStoneRepository> _productStoneRepoMock;

        public ProductServiceTests()
        {
            _uowMock    = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();

            _productRepoMock      = new Mock<IProductRepository>();
            _categoryRepoMock     = new Mock<ICategoryRepository>();
            _metalRepoMock        = new Mock<IMetalRepository>();
            _stoneRepoMock        = new Mock<IStoneRepository>();
            _productStoneRepoMock = new Mock<IProductStoneRepository>();

            _uowMock.Setup(u => u.Products).Returns(_productRepoMock.Object);
            _uowMock.Setup(u => u.Categories).Returns(_categoryRepoMock.Object);
            _uowMock.Setup(u => u.Metals).Returns(_metalRepoMock.Object);
            _uowMock.Setup(u => u.Stones).Returns(_stoneRepoMock.Object);
            _uowMock.Setup(u => u.ProductStones).Returns(_productStoneRepoMock.Object);
            _sut = new ProductService(_uowMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetProductByIdAsync_ExistingId_ReturnsProductDTO()
        {
            var product    = CreateProduct(1);
            var productDto = new ProductDTO { ProductId = 1, Name = "Ring" };

            _productRepoMock.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(product);
            _mapperMock.Setup(m => m.Map<ProductDTO>(product)).Returns(productDto);

            var result = await _sut.GetProductByIdAsync(1);

            result.Should().NotBeNull();
            result.ProductId.Should().Be(1);
        }

        [Fact]
        public async Task GetProductByIdAsync_NonExistingId_ThrowsNotFoundException()
        {
            _productRepoMock.Setup(r => r.GetByIdAsync(999, default)).ReturnsAsync((Product?)null);

            var act = async () => await _sut.GetProductByIdAsync(999);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public async Task GetProductByIdAsync_InvalidId_ThrowsValidationException(int invalidId)
        {
            var act = async () => await _sut.GetProductByIdAsync(invalidId);

            await act.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task GetProductWithDetailsAsync_ExistingId_ReturnsDetailedDTO()
        {
            // Arrange
            var product = CreateProductWithDetails(1);
            var dto     = new ProductDetailedInfoDTO { ProductId = 1, Name = "Ring" };

            _productRepoMock.Setup(r => r.GetProductWithDetailsAsync(1, default)).ReturnsAsync(product);
            _mapperMock.Setup(m => m.Map<ProductDetailedInfoDTO>(product)).Returns(dto);

            // Act
            var result = await _sut.GetProductWithDetailsAsync(1);

            // Assert
            result.Should().NotBeNull();
            result.ProductId.Should().Be(1);
        }

        [Fact]
        public async Task GetProductWithDetailsAsync_NonExistingId_ThrowsNotFoundException()
        {
            _productRepoMock.Setup(r => r.GetProductWithDetailsAsync(999, default)).ReturnsAsync((Product?)null);

            var act = async () => await _sut.GetProductWithDetailsAsync(999);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public async Task GetProductWithDetailsAsync_InvalidId_ThrowsValidationException(int invalidId)
        {
            var act = async () => await _sut.GetProductWithDetailsAsync(invalidId);

            await act.Should().ThrowAsync<ValidationException>();
        }



        [Fact]
        public async Task GetProductsPagedAsync_ValidParameters_ReturnsPagedResponse()
        {
            var parameters = new ProductParameters { PageNumber = 1, PageSize = 10 };
            var products   = new List<Product> { CreateProduct(1), CreateProduct(2) };
            var pagedList  = CreatePagedList(products, parameters);
            var dtos       = new List<ProductDTO>
            {
                new() { ProductId = 1 },
                new() { ProductId = 2 }
            };

            _productRepoMock
                .Setup(r => r.GetProductsPagedAsync(parameters, null, default))
                .ReturnsAsync(pagedList);
            _mapperMock.Setup(m => m.Map<List<ProductDTO>>(It.IsAny<List<Product>>())).Returns(dtos);

            var result = await _sut.GetProductsPagedAsync(parameters);

            result.Should().NotBeNull();
            result.Items.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetProductsPagedAsync_MinPriceGreaterThanMaxPrice_ThrowsValidationException()
        {
            var parameters = new ProductParameters
            {
                PageNumber = 1,
                PageSize   = 10,
                MinPrice   = 500,
                MaxPrice   = 100
            };

            var act = async () => await _sut.GetProductsPagedAsync(parameters);

            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("*MinPrice*");
        }

        
        [Fact]
        public async Task CreateProductAsync_ValidDto_ReturnsCreatedProductDTO()
        {
            // Arrange
            var dto = new CreateProductDTO
            {
                Name       = "Ring",
                Price      = 100,
                Weight     = 5,
                CategoryId = 1,
                StoneIds   = new List<int>()
            };
            var product    = CreateProduct(1);
            var productDto = new ProductDTO { ProductId = 1, Name = "Ring" };

            _categoryRepoMock.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(new Category { CategoryId = 1 });
            _mapperMock.Setup(m => m.Map<Product>(dto)).Returns(product);
            _mapperMock.Setup(m => m.Map<ProductDTO>(product)).Returns(productDto);
            _productRepoMock.Setup(r => r.AddAsync(product, default)).ReturnsAsync(product);
            _uowMock.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1);
            _uowMock.Setup(u => u.BeginTransactionAsync(default)).Returns(Task.CompletedTask);
            _uowMock.Setup(u => u.CommitTransactionAsync(default)).Returns(Task.CompletedTask);

            // Act
            var result = await _sut.CreateProductAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result.ProductId.Should().Be(1);
            result.Name.Should().Be("Ring");
        }

        [Fact]
        public async Task CreateProductAsync_CategoryNotFound_ThrowsNotFoundException()
        {
            var dto = new CreateProductDTO
            {
                Name       = "Ring",
                Price      = 100,
                Weight     = 5,
                CategoryId = 99
            };

            _categoryRepoMock.Setup(r => r.GetByIdAsync(99, default)).ReturnsAsync((Category?)null);

            var act = async () => await _sut.CreateProductAsync(dto);

            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("*Category*99*");
        }

        [Fact]
        public async Task CreateProductAsync_MetalNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var dto = new CreateProductDTO
            {
                Name       = "Ring",
                Price      = 100,
                Weight     = 5,
                CategoryId = 1,
                MetalId    = 55
            };

            _categoryRepoMock.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(new Category { CategoryId = 1 });
            _metalRepoMock.Setup(r => r.GetByIdAsync(55, default)).ReturnsAsync((Metal?)null);

            // Act
            var act = async () => await _sut.CreateProductAsync(dto);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("*Metal*55*");
        }

        [Fact]
        public async Task CreateProductAsync_StoneNotFound_ThrowsNotFoundException()
        {
            var dto = new CreateProductDTO
            {
                Name       = "Ring",
                Price      = 100,
                Weight     = 5,
                CategoryId = 1,
                StoneIds   = new List<int> { 10 }
            };
            _categoryRepoMock.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(new Category { CategoryId = 1 });
            _stoneRepoMock.Setup(r => r.GetByIdAsync(10, default)).ReturnsAsync((Stone?)null);

            var act = async () => await _sut.CreateProductAsync(dto);

            await act.Should().ThrowAsync<NotFoundException>() .WithMessage("*Stone*10*");
        }

        [Fact]
        public async Task CreateProductAsync_WithStones_AddsStones()
        {
            // Arrange
            var dto = new CreateProductDTO
            {
                Name       = "Ring",
                Price      = 100,
                Weight     = 5,
                CategoryId = 1,
                StoneIds   = new List<int> { 1, 2 }
            };
            var product    = CreateProduct(1);
            var productDto = new ProductDTO { ProductId = 1 };

            _categoryRepoMock.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(new Category { CategoryId = 1 });
            _stoneRepoMock.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(new Stone { StoneId = 1 });
            _stoneRepoMock.Setup(r => r.GetByIdAsync(2, default)).ReturnsAsync(new Stone { StoneId = 2 });
            _mapperMock.Setup(m => m.Map<Product>(dto)).Returns(product);
            _mapperMock.Setup(m => m.Map<ProductDTO>(product)).Returns(productDto);
            _productRepoMock.Setup(r => r.AddAsync(product, default)).ReturnsAsync(product);
            _uowMock.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1);
            _uowMock.Setup(u => u.BeginTransactionAsync(default)).Returns(Task.CompletedTask);
            _uowMock.Setup(u => u.CommitTransactionAsync(default)).Returns(Task.CompletedTask);
            _productStoneRepoMock
                .Setup(r => r.AddStoneToProductAsync(It.IsAny<int>(), It.IsAny<int>(), default))
                .ReturnsAsync(true);

            // Act
            var result = await _sut.CreateProductAsync(dto);

            // Assert
            _productStoneRepoMock.Verify(
                r => r.AddStoneToProductAsync(product.ProductId, It.IsAny<int>(), default),
                Times.Exactly(2));
        }


        // [Fact]
        // public async Task DeleteProductAsync_ExistingProduct_DeletesSuccessfully()
        // {
        //     // Arrange
        //     var product = CreateProduct(1);
        //     var stones  = new List<ProductStone>
        //     {
        //         new() { ProductId = 1, StoneId = 10 }
        //     };
        //
        //     _productRepoMock.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(product);
        //     _productStoneRepoMock.Setup(r => r.GetProductStonesAsync(1, default)).ReturnsAsync(stones);
        //     _productStoneRepoMock
        //         .Setup(r => r.RemoveStoneFromProductAsync(1, 10, default))
        //         .ReturnsAsync(true);
        //     _uowMock.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1);
        //     _uowMock.Setup(u => u.BeginTransactionAsync(default)).Returns(Task.CompletedTask);
        //     _uowMock.Setup(u => u.CommitTransactionAsync(default)).Returns(Task.CompletedTask);
        //
        //     // Act
        //     var act = async () => await _sut.DeleteProductAsync(1);
        //
        //     // Assert
        //     await act.Should().NotThrowAsync();
        //     _productRepoMock.Verify(r => r.Delete(product), Times.Once);
        // }

        [Fact]
        public async Task DeleteProductAsync_NonExistingProduct_ThrowsNotFoundException()
        {
            _productRepoMock.Setup(r => r.GetByIdAsync(999, default)).ReturnsAsync((Product?)null);

            var act = async () => await _sut.DeleteProductAsync(999);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task DeleteProductAsync_InvalidId_ThrowsValidationException(int invalidId)
        {
            var act = async () => await _sut.DeleteProductAsync(invalidId);

            await act.Should().ThrowAsync<ValidationException>();
        }

 

        [Fact]
        public async Task AddStoneToProductAsync_ValidIds_AddsSuccessfully()
        {
            var product = CreateProduct(1);
            var stone   = new Stone { StoneId = 2, Name = "Ruby" };

            _productRepoMock.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(product);
            _stoneRepoMock.Setup(r => r.GetByIdAsync(2, default)).ReturnsAsync(stone);
            _productStoneRepoMock
                .Setup(r => r.AddStoneToProductAsync(1, 2, default))
                .ReturnsAsync(true);
            _uowMock.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1);

            var act = async () => await _sut.AddStoneToProductAsync(1, 2);

            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task AddStoneToProductAsync_DuplicateStone_ThrowsBusinessConflictException()
        {
            // Arrange
            var product = CreateProduct(1);
            var stone   = new Stone { StoneId = 2, Name = "Ruby" };

            _productRepoMock.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(product);
            _stoneRepoMock.Setup(r => r.GetByIdAsync(2, default)).ReturnsAsync(stone);
            _productStoneRepoMock
                .Setup(r => r.AddStoneToProductAsync(1, 2, default))
                .ReturnsAsync(false); // вже існує

            // Act
            var act = async () => await _sut.AddStoneToProductAsync(1, 2);

            // Assert
            await act.Should().ThrowAsync<BusinessConflictException>();
        }

        [Fact]
        public async Task AddStoneToProductAsync_ProductNotFound_ThrowsNotFoundException()
        {
            _productRepoMock.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync((Product?)null);

            var act = async () => await _sut.AddStoneToProductAsync(1, 2);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(1, 0)]
        [InlineData(-1, -1)]
        public async Task AddStoneToProductAsync_InvalidIds_ThrowsValidationException(int productId, int stoneId)
        {
            var act = async () => await _sut.AddStoneToProductAsync(productId, stoneId);

            await act.Should().ThrowAsync<ValidationException>();
        }



        [Fact]
        public async Task RemoveStoneFromProductAsync_ExistingRelation_RemovesSuccessfully()
        {
            // Arrange
            var product = CreateProduct(1);

            _productRepoMock.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(product);
            _productStoneRepoMock
                .Setup(r => r.RemoveStoneFromProductAsync(1, 2, default))
                .ReturnsAsync(true);
            _uowMock.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var act = async () => await _sut.RemoveStoneFromProductAsync(1, 2);

            // Assert
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task RemoveStoneFromProductAsync_NonExistingRelation_ThrowsNotFoundException()
        {
            // Arrange
            var product = CreateProduct(1);

            _productRepoMock.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(product);
            _productStoneRepoMock
                .Setup(r => r.RemoveStoneFromProductAsync(1, 99, default))
                .ReturnsAsync(false);

            // Act
            var act = async () => await _sut.RemoveStoneFromProductAsync(1, 99);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task RemoveStoneFromProductAsync_ProductNotFound_ThrowsNotFoundException()
        {
            // Arrange
            _productRepoMock.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync((Product?)null);

            // Act
            var act = async () => await _sut.RemoveStoneFromProductAsync(1, 2);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }


        [Fact]
        public async Task GetProductStonesAsync_ExistingProduct_ReturnsStoneDTOs()
        {
            // Arrange
            var product = CreateProduct(1);
            var stones  = new List<Stone>
            {
                new() { StoneId = 1, Name = "Ruby" },
                new() { StoneId = 2, Name = "Sapphire" }
            };
            var stoneDtos = new List<StoneDTO>
            {
                new() { StoneId = 1, Name = "Ruby" },
                new() { StoneId = 2, Name = "Sapphire" }
            };

            _productRepoMock.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(product);
            _productStoneRepoMock.Setup(r => r.GetProductStonesAsync(1, default)).ReturnsAsync(stones);
            _mapperMock.Setup(m => m.Map<IEnumerable<StoneDTO>>(stones)).Returns(stoneDtos);

            // Act
            var result = await _sut.GetProductStonesAsync(1);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetProductStonesAsync_ProductNotFound_ThrowsNotFoundException()
        {
            // Arrange
            _productRepoMock.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync((Product?)null);

            // Act
            var act = async () => await _sut.GetProductStonesAsync(1);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-3)]
        public async Task GetProductStonesAsync_InvalidId_ThrowsValidationException(int invalidId)
        {
            // Act
            var act = async () => await _sut.GetProductStonesAsync(invalidId);

            // Assert
            await act.Should().ThrowAsync<ValidationException>();
        }


        [Fact]
        public async Task GetProductsByStoneNamesAsync_AllStonesExist_ReturnsProducts()
        {
            // Arrange
            var stoneNames = new List<string> { "Ruby" };
            var products   = new List<Product> { CreateProduct(1) };
            var productDtos = new List<ProductDTO> { new() { ProductId = 1 } };

            _stoneRepoMock
                .Setup(r => r.GetStoneByNameAsync("Ruby", default))
                .ReturnsAsync(new Stone { StoneId = 1, Name = "Ruby" });
            _productStoneRepoMock
                .Setup(r => r.GetProductsByStoneNamesAsync(stoneNames, default))
                .ReturnsAsync(products);
            _mapperMock
                .Setup(m => m.Map<IEnumerable<ProductDTO>>(products))
                .Returns(productDtos);

            var result = await _sut.GetProductsByStoneNamesAsync(stoneNames);

            result.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetProductsByStoneNamesAsync_StoneNameNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var stoneNames = new List<string> { "Unknown" };

            _stoneRepoMock
                .Setup(r => r.GetStoneByNameAsync("Unknown", default))
                .ReturnsAsync((Stone?)null);

            // Act
            var act = async () => await _sut.GetProductsByStoneNamesAsync(stoneNames);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("*Unknown*");
        }

        
        // helpers
        private static Product CreateProduct(int id) => new()
        {
            ProductId    = id,
            Name         = $"Product {id}",
            Price        = 100,
            Weight       = 5,
            CategoryId   = 1,
            ProductStones = new List<ProductStone>()
        };

        private static Product CreateProductWithDetails(int id) => new()
        {
            ProductId  = id,
            Name       = $"Product {id}",
            Price      = 100,
            Weight     = 5,
            CategoryId = 1,
            Metal      = new Metal { MetalId = 1, Name = "Gold", Color = "Yellow" },
            Category   = new Category { CategoryId = 1, Name = "Rings" },
            ProductStones = new List<ProductStone>
            {
                new() { ProductId = id, StoneId = 1, Stone = new Stone { StoneId = 1, Name = "Ruby" } }
            }
        };

        // фабрика PagedList для тестів
        private static PagedList<Product> CreatePagedList(List<Product> items, ProductParameters p)
        {
            return new PagedList<Product>(items, items.Count, p.PageNumber, p.PageSize);
        }
    }
}