using Catalog.BLL.DTOs.Metal;

namespace Catalog.BLL.Services.Interfaces
{
	public interface IMetalService
	{
        Task<IEnumerable<MetalDTO>> GetAllMetalsAsync(CancellationToken cancellationToken = default);
        Task<MetalDTO> GetMetalByIdAsync(int metalId, CancellationToken cancellationToken = default);
        Task<MetalDTO> GetMetalByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<MetalDTO> CreateMetalAsync(CreateMetalDTO dto, CancellationToken cancellationToken = default);
        Task DeleteMetalAsync(int metalId, CancellationToken cancellationToken = default);
    }
}

