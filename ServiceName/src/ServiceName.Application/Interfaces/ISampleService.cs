using ServiceName.Application.DTO;

namespace ServiceName.Application.Interfaces;

public interface ISampleService
{
    Task<IReadOnlyList<SampleDTO>> GetAllAsync();
    Task<SampleDTO?> GetAsync(Guid id);
    Task AddAsync(SampleDTO dto);
    Task UpdateAsync(SampleDTO dto);
    Task DeleteAsync(Guid id);
}
