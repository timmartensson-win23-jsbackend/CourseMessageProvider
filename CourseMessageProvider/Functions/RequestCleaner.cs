using System;
using CourseMessageProvider.Data.Contexts;
using CourseMessageProvider.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CourseMessageProvider.Functions
{
    public class RequestCleaner(ILogger<RequestCleaner> logger, CleanerService cleanerService)
    {
        private readonly ILogger<RequestCleaner> _logger = logger;
        private readonly CleanerService _cleanerService = cleanerService;

        [Function("RequestCleaner")]
        public async Task Run([TimerTrigger("0 0 * * * *")] TimerInfo myTimer)
        {
            try
            {
                await _cleanerService.RemoveRequestsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"ERROR : RequestCleaner.Run() :: {ex.Message}");
            }
        }
    }
}
