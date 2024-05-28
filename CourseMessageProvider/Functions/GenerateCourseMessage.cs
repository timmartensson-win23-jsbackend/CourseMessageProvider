using Azure.Messaging.ServiceBus;
using CourseMessageProvider.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;



namespace CourseMessageProvider.Functions;

public class GenerateCourseMessage(ILogger<GenerateCourseMessage> logger, CourseMessageService courseMessageService)
{
    private readonly ILogger<GenerateCourseMessage> _logger = logger;
    private readonly CourseMessageService _courseMessageService = courseMessageService;


    [Function(nameof(GenerateCourseMessage))]
    [ServiceBusOutput("email_request", Connection = "ServiceBusConnection")]
    public async Task<string> Run([ServiceBusTrigger("coursemessage_request", Connection = "ServiceBusConnection")] ServiceBusReceivedMessage message, ServiceBusMessageActions messageActions)
    {
        try
        {
            var joinCourseRequest = _courseMessageService.UnpackJoinRequest(message);
            if (joinCourseRequest != null)
            {
                var result = await _courseMessageService.saveJoinRequest(joinCourseRequest);
                if (result == true)
                {
                    var emailRequest = _courseMessageService.GenerateEmailRequest(joinCourseRequest);
                    if (emailRequest != null)
                    {
                        var payload = _courseMessageService.GenerateServiceBusEmailRequest(emailRequest);
                        if (!string.IsNullOrEmpty(payload))
                        {
                            await messageActions.CompleteMessageAsync(message);
                            return payload;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"ERROR : GenerateCourseMessage.Run() :: {ex.Message}");
        }
        return null!;
    }




}

