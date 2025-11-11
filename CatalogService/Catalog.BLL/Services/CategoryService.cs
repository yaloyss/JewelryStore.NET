using AutoMapper;
using Catalog.BLL.DTOs.Category;
using Catalog.BLL.DTOs.Product;
using Catalog.BLL.Exceptions;
using Catalog.BLL.Services.Interfaces;
using Catalog.DAL.UOW;
using Catalog.Domain.Entities;

namespace Catalog.BLL.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync(CancellationToken cancellationToken = default)
        {
            var categories = await _unitOfWork.Categories.GetAllAsync(cancellationToken);
            return _mapper.Map<IEnumerable<CategoryDTO>>(categories);
        }

        public async Task<CategoryDTO> GetCategoryByIdAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            if (categoryId <= 0)
            {
                throw new ValidationException("CategoryId must be greater than 0.");
            }

            var category = await _unitOfWork.Categories.GetByIdAsync(categoryId, cancellationToken);
            if (category == null)
            {
                throw new NotFoundException($"Category with ID {categoryId} not found.");
            }

            return _mapper.Map<CategoryDTO>(category);
        }

        public async Task<CategoryWithInfoDTO> GetCategoryWithDetailsAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            if (categoryId <= 0)
            {
                throw new ValidationException("CategoryId must be greater than 0.");
            }

            var category = await _unitOfWork.Categories.GetCategoryByIdAsync(categoryId, cancellationToken);
            if (category == null)
            {
                throw new NotFoundException($"Category with ID {categoryId} not found.");
            }

            return _mapper.Map<CategoryWithInfoDTO>(category);
        }

        public async Task<CategoryStatisticsDTO> GetCategoryStatisticsAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            if (categoryId <= 0)
            {
                throw new ValidationException("CategoryId must be greater than 0.");
            }

            var category = await _unitOfWork.Categories.GetByIdAsync(categoryId, cancellationToken);
            if (category == null)
            {
                throw new NotFoundException($"Category with ID {categoryId} not found.");
            }

            var statistics = await _unitOfWork.Categories.GetCategoryStatisticsAsync(categoryId, cancellationToken);

            return new CategoryStatisticsDTO
            {
                CategoryId = categoryId,
                Name = category.Name,
                TotalProducts = statistics.GetValueOrDefault("TotalProducts", 0),
                GoldenProducts = statistics.GetValueOrDefault("GoldenProducts", 0),
                SilverProducts = statistics.GetValueOrDefault("SilverProducts", 0)
            };
        }

        public async Task<CategoryDTO> CreateCategoryAsync(CreateCategoryDTO dto, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new ValidationException("Category name cannot be empty.");
            }

            if (dto.Name.Length > 100)
            {
                throw new ValidationException("Category name cannot exceed 100 characters.");
            }

            //if it has a duplicate
            var existingCategories = await _unitOfWork.Categories.GetAllAsync(cancellationToken);
            if (existingCategories.Any(c => c.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new BusinessConflictException($"Category with name '{dto.Name}' already exists.");
            }

            try
            {
                var category = _mapper.Map<Category>(dto);
                await _unitOfWork.Categories.AddAsync(category, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return _mapper.Map<CategoryDTO>(category);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating the category.", ex);
            }
        }

        public async Task DeleteCategoryAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            if (categoryId <= 0)
            {
                throw new ValidationException("CategoryId must be greater than 0.");
            }

            var category = await _unitOfWork.Categories.GetCategoryByIdAsync(categoryId, cancellationToken);
            if (category == null)
            {
                throw new NotFoundException($"Category with ID {categoryId} not found.");
            }

            if (category.Products != null && category.Products.Any())
            {
                throw new BusinessConflictException($"Cannot delete category '{category.Name}' because it has {category.Products.Count} product(s). Remove products first.");
            }

            try
            {
                _unitOfWork.Categories.Delete(category);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the category.", ex);
            }
        }

        public async Task<IEnumerable<ProductDTO>> GetProductsForCategoryAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            if (categoryId <= 0)
            {
                throw new ValidationException("CategoryId must be greater than 0.");
            }

            var categoryExists = await _unitOfWork.Categories.GetByIdAsync(categoryId, cancellationToken);
            if (categoryExists == null)
            {
                throw new NotFoundException($"Category with ID {categoryId} not found.");
            }

            var products = await _unitOfWork.Categories.GetProductsForCategoryAsync(categoryId, cancellationToken);
            return _mapper.Map<IEnumerable<ProductDTO>>(products);
        }

        public async Task<int> GetProductCountByCategoryAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            if (categoryId <= 0)
            {
                throw new ValidationException("CategoryId must be greater than 0.");
            }

            var categoryExists = await _unitOfWork.Categories.GetByIdAsync(categoryId, cancellationToken);
            if (categoryExists == null)
            {
                throw new NotFoundException($"Category with ID {categoryId} not found.");
            }

            return await _unitOfWork.Categories.GetProductCountByCategoryAsync(categoryId, cancellationToken);
        }
    }
}

