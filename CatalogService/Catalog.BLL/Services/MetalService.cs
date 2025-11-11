using AutoMapper;
using Catalog.BLL.DTOs.Metal;
using Catalog.BLL.Exceptions;
using Catalog.BLL.Services.Interfaces;
using Catalog.DAL.UOW;
using Catalog.Domain.Entities;

namespace Catalog.BLL.Services
{
    public class MetalService : IMetalService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MetalService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MetalDTO>> GetAllMetalsAsync(CancellationToken cancellationToken = default)
        {
            var metals = await _unitOfWork.Metals.GetAllAsync(cancellationToken);
            return _mapper.Map<IEnumerable<MetalDTO>>(metals);
        }

        public async Task<MetalDTO> GetMetalByIdAsync(int metalId, CancellationToken cancellationToken = default)
        {
            if (metalId <= 0)
            {
                throw new ValidationException("MetalId must be greater than 0.");
            }

            var metal = await _unitOfWork.Metals.GetByIdAsync(metalId, cancellationToken);
            if (metal == null)
            {
                throw new NotFoundException($"Metal with ID {metalId} not found.");
            }
            return _mapper.Map<MetalDTO>(metal);
        }

        public async Task<MetalDTO> GetMetalByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ValidationException("Metal name cannot be empty.");
            }

            var metal = await _unitOfWork.Metals.GetMetalByNameAsync(name, cancellationToken);
            if (metal == null)
            {
                throw new NotFoundException($"Metal with name '{name}' not found.");
            }
            return _mapper.Map<MetalDTO>(metal);
        }

        public async Task<MetalDTO> CreateMetalAsync(CreateMetalDTO dto, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new ValidationException("Metal name cannot be empty.");
            }

            if (dto.Name.Length > 50)
            {
                throw new ValidationException("Metal name cannot exceed 50 characters.");
            }

            if (dto.Color != null && dto.Color.Length > 30)
            {
                throw new ValidationException("Metal color cannot exceed 30 characters.");
            }

            //if it has a duplicate
            var existingMetal = await _unitOfWork.Metals.GetMetalByNameAsync(dto.Name, cancellationToken);
            if (existingMetal != null)
            {
                throw new BusinessConflictException($"Metal with name '{dto.Name}' already exists.");
            }

            try
            {
                var metal = _mapper.Map<Metal>(dto);
                await _unitOfWork.Metals.AddAsync(metal, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return _mapper.Map<MetalDTO>(metal);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating the metal.", ex);
            }
        }

        public async Task DeleteMetalAsync(int metalId, CancellationToken cancellationToken = default)
        {
            if (metalId <= 0)
            {
                throw new ValidationException("MetalId must be greater than 0.");
            }

            var metal = await _unitOfWork.Metals.GetByIdAsync(metalId, cancellationToken);
            if (metal == null)
            {
                throw new NotFoundException($"Metal with ID {metalId} not found.");
            }

            var products = await _unitOfWork.Products.GetProductsByMetalAsync(metalId, cancellationToken);
            if (products.Any())
            {
                throw new BusinessConflictException($"Cannot delete metal '{metal.Name}' because it is used in {products.Count()} product(s).");
            }

            try
            {
                _unitOfWork.Metals.Delete(metal);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the metal.", ex);
            }
        }
    }
}

