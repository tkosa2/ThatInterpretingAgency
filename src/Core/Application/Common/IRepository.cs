using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ThatInterpretingAgency.Core.DTOs;

namespace ThatInterpretingAgency.Core.Application.Common;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
}

public interface IAgencyRepository : IRepository<ThatInterpretingAgency.Core.Domain.Aggregates.AgencyAggregate>
{
    Task<ThatInterpretingAgency.Core.Domain.Aggregates.AgencyAggregate?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}

public interface IInterpreterRepository : IRepository<ThatInterpretingAgency.Core.Domain.Entities.Interpreter>
{
    Task<IEnumerable<ThatInterpretingAgency.Core.Domain.Entities.Interpreter>> GetByAgencyIdAsync(Guid agencyId, CancellationToken cancellationToken = default);
    Task<ThatInterpretingAgency.Core.Domain.Entities.Interpreter?> GetByAgencyAndUserIdAsync(Guid agencyId, Guid userId, CancellationToken cancellationToken = default);
}

public interface IClientRepository : IRepository<ThatInterpretingAgency.Core.Domain.Entities.Client>
{
    Task<ThatInterpretingAgency.Core.Domain.Entities.Client?> GetByAgencyAndUserIdAsync(Guid agencyId, Guid userId, CancellationToken cancellationToken = default);
}

public interface IAppointmentRepository : IRepository<ThatInterpretingAgency.Core.Domain.Aggregates.AppointmentAggregate>
{
    Task<IEnumerable<ThatInterpretingAgency.Core.Domain.Aggregates.AppointmentAggregate>> GetByAgencyIdAsync(Guid agencyId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ThatInterpretingAgency.Core.Domain.Aggregates.AppointmentAggregate>> GetByInterpreterIdAsync(Guid interpreterId, CancellationToken cancellationToken = default);
    Task<bool> HasOverlappingAppointmentsAsync(Guid interpreterId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default);
}

public interface IInvoiceRepository : IRepository<ThatInterpretingAgency.Core.Domain.Aggregates.InvoiceAggregate>
{
    Task<ThatInterpretingAgency.Core.Domain.Aggregates.InvoiceAggregate?> GetByAppointmentIdAsync(Guid appointmentId, CancellationToken cancellationToken = default);
}

public interface IInterpreterRequestRepository : IRepository<ThatInterpretingAgency.Core.Domain.Entities.InterpreterRequest>
{
    Task<IEnumerable<ThatInterpretingAgency.Core.Domain.Entities.InterpreterRequest>> GetByAgencyIdAsync(Guid agencyId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ThatInterpretingAgency.Core.Domain.Entities.InterpreterRequest>> GetByRequestorIdAsync(Guid requestorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ThatInterpretingAgency.Core.Domain.Entities.InterpreterRequest>> GetByStatusAsync(string status, CancellationToken cancellationToken = default);
    Task<IEnumerable<ThatInterpretingAgency.Core.Domain.Entities.InterpreterRequest>> GetByLanguageAsync(string language, CancellationToken cancellationToken = default);
    Task<IEnumerable<ThatInterpretingAgency.Core.Domain.Entities.InterpreterRequest>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<ThatInterpretingAgency.Core.Domain.Entities.InterpreterRequest>> GetApprovedRequestsAsync(CancellationToken cancellationToken = default);
}

public interface IInterpreterRequestService
{
    Task<IEnumerable<InterpreterRequestData>> GetInterpreterRequestsAsync(
        Guid? agencyId = null, 
        string? status = null, 
        string? language = null,
        DateTime? fromDate = null,
        DateTime? toDate = null);
    
    Task<InterpreterRequestData?> GetInterpreterRequestAsync(Guid id);
    Task<IEnumerable<InterpreterRequestData>> GetClientRequestsAsync(Guid clientId);
    Task<IEnumerable<InterpreterRequestData>> GetApprovedRequestsAsync();
    Task<InterpreterRequestData> CreateInterpreterRequestAsync(CreateInterpreterRequestRequest request);
    Task<InterpreterRequestData> UpdateRequestStatusAsync(Guid id, UpdateInterpreterRequestStatusRequest request);
    Task<InterpreterRequestData> CancelRequestAsync(Guid id);
    Task<bool> DeleteInterpreterRequestAsync(Guid id);
}
