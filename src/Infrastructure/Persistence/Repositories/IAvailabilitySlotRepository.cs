using ThatInterpretingAgency.Core.Domain.Entities;

namespace ThatInterpretingAgency.Infrastructure.Persistence.Repositories;

public interface IAvailabilitySlotRepository
{
    Task<AvailabilitySlot?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<AvailabilitySlot>> GetByInterpreterIdAsync(Guid interpreterId, CancellationToken cancellationToken = default);
    Task<IEnumerable<AvailabilitySlot>> GetAvailableSlotsAsync(Guid interpreterId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default);
    Task<AvailabilitySlot> AddAsync(AvailabilitySlot entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(AvailabilitySlot entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(AvailabilitySlot entity, CancellationToken cancellationToken = default);
    Task<bool> HasOverlappingSlotsAsync(Guid interpreterId, DateTime startTime, DateTime endTime, Guid? excludeSlotId = null, CancellationToken cancellationToken = default);
}
