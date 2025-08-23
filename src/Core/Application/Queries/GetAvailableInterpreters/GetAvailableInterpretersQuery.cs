using System;
using System.Collections.Generic;
using MediatR;

namespace ThatInterpretingAgency.Core.Application.Queries.GetAvailableInterpreters;

public record GetAvailableInterpretersQuery : IRequest<GetAvailableInterpretersResponse>
{
    public Guid AgencyId { get; init; }
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }
    public List<string>? RequiredSkills { get; init; }
    public string? Location { get; init; }
    public string? Language { get; init; }
}

public record GetAvailableInterpretersResponse
{
    public List<AvailableInterpreterDto> Interpreters { get; init; } = new();
}

public record AvailableInterpreterDto
{
    public Guid Id { get; init; }
    public string FullName { get; init; } = string.Empty;
    public List<string> Skills { get; init; } = new();
    public List<AvailabilitySlotDto> AvailableSlots { get; init; } = new();
    public decimal? HourlyRate { get; init; }
}

public record AvailabilitySlotDto
{
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }
    public string TimeZone { get; init; } = string.Empty;
}
