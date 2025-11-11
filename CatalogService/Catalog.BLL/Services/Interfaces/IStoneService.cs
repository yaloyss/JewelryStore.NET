using Catalog.BLL.DTOs.Stone;

namespace Catalog.BLL.Services.Interfaces
{
    public interface IStoneService
    {
        Task<IEnumerable<StoneDTO>> GetAllStonesAsync(CancellationToken cancellationToken = default);
        Task<StoneDTO> GetStoneByIdAsync(int stoneId, CancellationToken cancellationToken = default);
        Task<StoneDTO> GetStoneByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<StoneDTO> CreateStoneAsync(CreateStoneDTO dto, CancellationToken cancellationToken = default);
        Task DeleteStoneAsync(int stoneId, CancellationToken cancellationToken = default);
    }
}
