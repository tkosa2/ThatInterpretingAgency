using ThatInterpretingAgency.Core.Application.Common;
using ThatInterpretingAgency.Core.Domain.Entities;
using ThatInterpretingAgency.Core.DTOs;

namespace ThatInterpretingAgency.Core.Application.Services;

public class InterpreterRequestService : IInterpreterRequestService
{
    private readonly IInterpreterRequestRepository _repository;

    public InterpreterRequestService(IInterpreterRequestRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<InterpreterRequestData>> GetInterpreterRequestsAsync(
        Guid? agencyId = null, 
        string? status = null, 
        string? language = null,
        DateTime? fromDate = null,
        DateTime? toDate = null)
    {
        try
        {
            IEnumerable<InterpreterRequest> requests;

            if (agencyId.HasValue)
            {
                requests = await _repository.GetByAgencyIdAsync(agencyId.Value);
            }
            else
            {
                requests = await _repository.GetAllAsync();
            }

            // Apply filters
            if (!string.IsNullOrEmpty(status))
            {
                requests = requests.Where(r => r.Status == status);
            }

            if (!string.IsNullOrEmpty(language))
            {
                requests = requests.Where(r => r.Language.Contains(language, StringComparison.OrdinalIgnoreCase));
            }

            if (fromDate.HasValue)
            {
                requests = requests.Where(r => r.RequestedDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                requests = requests.Where(r => r.RequestedDate <= toDate.Value);
            }

            return requests.Select(MapToDto);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<InterpreterRequestData?> GetInterpreterRequestAsync(Guid id)
    {
        try
        {
            var request = await _repository.GetByIdAsync(id);
            return request != null ? MapToDto(request) : null;
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<IEnumerable<InterpreterRequestData>> GetClientRequestsAsync(Guid clientId)
    {
        try
        {
            var requests = await _repository.GetByRequestorIdAsync(clientId);
            return requests.Select(MapToDto);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<IEnumerable<InterpreterRequestData>> GetApprovedRequestsAsync()
    {
        try
        {
            var requests = await _repository.GetApprovedRequestsAsync();
            return requests.Select(MapToDto);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<InterpreterRequestData> CreateInterpreterRequestAsync(CreateInterpreterRequestRequest request)
    {
        try
        {
            var interpreterRequest = new InterpreterRequest(
                agencyId: Guid.Parse(request.AgencyId),
                requestorId: Guid.Parse(request.RequestorId),
                appointmentType: request.AppointmentType,
                requestedDate: request.RequestedDate,
                startTime: request.StartTime,
                endTime: request.EndTime,
                language: request.Language,
                description: request.Description,
                mode: request.Mode,
                location: request.Location,
                virtualMeetingLink: request.VirtualMeetingLink,
                specialInstructions: request.SpecialInstructions,
                division: request.Division,
                program: request.Program,
                lniContact: request.LniContact,
                dayOfEventContact: request.DayOfEventContact,
                dayOfEventContactPhone: request.DayOfEventContactPhone,
                costCode: request.CostCode,
                invoiceApprover: request.InvoiceApprover,
                supportingMaterials: request.SupportingMaterials
            );

            var createdRequest = await _repository.AddAsync(interpreterRequest);
            return MapToDto(createdRequest);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<InterpreterRequestData> UpdateRequestStatusAsync(Guid id, UpdateInterpreterRequestStatusRequest request)
    {
        try
        {
            var interpreterRequest = await _repository.GetByIdAsync(id);
            if (interpreterRequest == null)
            {
                throw new InvalidOperationException($"Interpreter request with ID {id} not found");
            }

            interpreterRequest.UpdateStatus(request.Status, 
                !string.IsNullOrEmpty(request.AppointmentId) ? Guid.Parse(request.AppointmentId) : null);

            await _repository.UpdateAsync(interpreterRequest);
            return MapToDto(interpreterRequest);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<InterpreterRequestData> CancelRequestAsync(Guid id)
    {
        try
        {
            var interpreterRequest = await _repository.GetByIdAsync(id);
            if (interpreterRequest == null)
            {
                throw new InvalidOperationException($"Interpreter request with ID {id} not found");
            }

            interpreterRequest.Cancel();
            await _repository.UpdateAsync(interpreterRequest);
            return MapToDto(interpreterRequest);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<bool> DeleteInterpreterRequestAsync(Guid id)
    {
        try
        {
            var interpreterRequest = await _repository.GetByIdAsync(id);
            if (interpreterRequest == null)
            {
                return false;
            }

            await _repository.DeleteAsync(interpreterRequest);
            return true;
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    private static InterpreterRequestData MapToDto(InterpreterRequest request)
    {
        return new InterpreterRequestData
        {
            Id = request.Id.ToString(),
            AgencyId = request.AgencyId.ToString(),
            RequestorId = request.RequestorId.ToString(),
            AppointmentType = request.AppointmentType,
            VirtualMeetingLink = request.VirtualMeetingLink,
            Location = request.Location,
            Mode = request.Mode,
            Description = request.Description,
            RequestedDate = request.RequestedDate,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            Language = request.Language,
            SpecialInstructions = request.SpecialInstructions,
            Status = request.Status,
            Division = request.Division,
            Program = request.Program,
            LniContact = request.LniContact,
            DayOfEventContact = request.DayOfEventContact,
            DayOfEventContactPhone = request.DayOfEventContactPhone,
            CostCode = request.CostCode,
            InvoiceApprover = request.InvoiceApprover,
            SupportingMaterials = request.SupportingMaterials,
            CreatedAt = request.CreatedAt,
            RequestorName = string.Empty, // This will come from UserProfile when we have users
            OrganizationName = request.Requestor?.OrganizationName ?? string.Empty
        };
    }
}
