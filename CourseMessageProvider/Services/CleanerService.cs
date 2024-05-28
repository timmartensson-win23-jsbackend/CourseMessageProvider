using CourseMessageProvider.Data.Contexts;
using CourseMessageProvider.Functions;
using Microsoft.Extensions.Logging;

namespace CourseMessageProvider.Services;

public class CleanerService(ILogger<CleanerService> logger, DataContext context)
{
    private readonly ILogger<CleanerService> _logger = logger;
    private readonly DataContext _context = context;

    public async Task RemoveRequestsAsync()
    {
        try
        {
            _context.JoinCourseRequests.RemoveRange(_context.JoinCourseRequests);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError($"ERROR : CleanerService.RemoveRequestsAsync() :: {ex.Message}");
        }
    }
}
