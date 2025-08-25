using BlazorFrontend.Data;

namespace BlazorFrontend.Services;

public interface IBffDashboardService
{
    Task<BffDashboardDTO?> GetDashboardAsync();
    Task<BffDashboardDTO?> GetAgencyDashboardAsync(Guid agencyId);
}

public class BffDashboardService : IBffDashboardService
{
    private readonly IBffService _bffService;
    private readonly ILogger<BffDashboardService> _logger;

    public BffDashboardService(IBffService bffService, ILogger<BffDashboardService> logger)
    {
        _bffService = bffService;
        _logger = logger;
    }

    public async Task<BffDashboardDTO?> GetDashboardAsync()
    {
        try
        {
            return await _bffService.GetAsync<BffDashboardDTO>("api/dashboard");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dashboard data from BFF");
            return null;
        }
    }

    public async Task<BffDashboardDTO?> GetAgencyDashboardAsync(Guid agencyId)
    {
        try
        {
            return await _bffService.GetAsync<BffDashboardDTO>($"api/dashboard/agency/{agencyId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting agency dashboard data for {AgencyId} from BFF", agencyId);
            return null;
        }
    }
}

