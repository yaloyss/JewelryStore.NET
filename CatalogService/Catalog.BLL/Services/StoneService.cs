using AutoMapper;
using Catalog.BLL.DTOs.Stone;
using Catalog.BLL.Exceptions;
using Catalog.BLL.Services.Interfaces;
using Catalog.DAL.UOW;
using Catalog.Domain.Entities;

namespace Catalog.BLL.Services
{
    public class StoneService : IStoneService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StoneService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<StoneDTO>> GetAllStonesAsync(CancellationToken cancellationToken = default)
        {
            var stones = await _unitOfWork.Stones.GetAllAsync(cancellationToken);
            return _mapper.Map<IEnumerable<StoneDTO>>(stones);
        }

        public async Task<StoneDTO> GetStoneByIdAsync(int stoneId, CancellationToken cancellationToken = default)
        {
            if (stoneId <= 0)
            {
                throw new ValidationException("StoneId must be greater than 0.");
            }

            var stone = await _unitOfWork.Stones.GetByIdAsync(stoneId, cancellationToken);
            if (stone == null)
            {
                throw new NotFoundException($"Stone with ID {stoneId} not found.");
            }
            return _mapper.Map<StoneDTO>(stone);
        }

        public async Task<StoneDTO> GetStoneByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ValidationException("Stone name cannot be empty.");
            }

            var stone = await _unitOfWork.Stones.GetStoneByNameAsync(name, cancellationToken);
            if (stone == null)
            {
                throw new NotFoundException($"Stone with name '{name}' not found.");
            }
            return _mapper.Map<StoneDTO>(stone);
        }

        public async Task<StoneDTO> CreateStoneAsync(CreateStoneDTO dto, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new ValidationException("Stone name cannot be empty.");
            }

            if (dto.Name.Length > 50)
            {
                throw new ValidationException("Stone name cannot exceed 50 characters.");
            }

            //if it has a duplicate
            var existingStone = await _unitOfWork.Stones.GetStoneByNameAsync(dto.Name, cancellationToken);
            if (existingStone != null)
            {
                throw new BusinessConflictException($"Stone with name '{dto.Name}' already exists.");
            }

            try
            {
                var stone = _mapper.Map<Stone>(dto);
                await _unitOfWork.Stones.AddAsync(stone, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return _mapper.Map<StoneDTO>(stone);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating the stone.", ex);
            }
        }

        public async Task DeleteStoneAsync(int stoneId, CancellationToken cancellationToken = default)
        {
            if (stoneId <= 0)
            {
                throw new ValidationException("StoneId must be greater than 0.");
            }

            var stone = await _unitOfWork.Stones.GetByIdAsync(stoneId, cancellationToken);
            if (stone == null)
            {
                throw new NotFoundException($"Stone with ID {stoneId} not found.");
            }

            var products = await _unitOfWork.ProductStones.GetProductsByStoneAsync(stoneId, cancellationToken);
            if (products.Any())
            {
                throw new BusinessConflictException($"Cannot delete stone '{stone.Name}' because it is used in {products.Count()} product(s).");
            }

            try
            {
                _unitOfWork.Stones.Delete(stone);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the stone.", ex);
            }
        }
    }
}

