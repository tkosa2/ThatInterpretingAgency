using System;
using MediatR;

namespace ThatInterpretingAgency.Core.Application.Commands.BookAppointment;

public record BookAppointmentCommand : IRequest<BookAppointmentResponse>
{
    public Guid AgencyId { get; init; }
    public string InterpreterId { get; init; } = string.Empty; // Changed to string to match UserId
    public string ClientId { get; init; } = string.Empty; // Changed to string to match UserId
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }
    public string TimeZone { get; init; } = string.Empty;
    public string? Location { get; init; }
    public string? Language { get; init; }
    public decimal? Rate { get; init; }
    public string? Notes { get; init; }
}

public record BookAppointmentResponse
{
    public Guid AppointmentId { get; init; }
    public Guid AgencyId { get; init; }
    public string InterpreterId { get; init; } = string.Empty; // Changed to string to match UserId
    public string ClientId { get; init; } = string.Empty; // Changed to string to match UserId
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }
    public string TimeZone { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}
