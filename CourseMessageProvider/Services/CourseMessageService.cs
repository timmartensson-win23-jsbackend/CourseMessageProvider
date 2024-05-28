using Azure.Messaging.ServiceBus;
using CourseMessageProvider.Data.Contexts;
using CourseMessageProvider.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CourseMessageProvider.Services;

public class CourseMessageService(ILogger<CourseMessageService> logger, IServiceProvider serviceProvider)
{
    private readonly ILogger<CourseMessageService> _logger = logger;
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public JoinCourseRequestModel UnpackJoinRequest(ServiceBusReceivedMessage message)
    {
        try
        {
            var joinCourseRequest = JsonConvert.DeserializeObject<JoinCourseRequestModel>(message.Body.ToString());
            if (joinCourseRequest != null && !string.IsNullOrEmpty(joinCourseRequest.Email) && !string.IsNullOrEmpty(joinCourseRequest.CourseName))
            {
                return joinCourseRequest;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"ERROR : CourseMessageService.UnpackJoinRequest() :: {ex.Message}");
        }
        return null!;
    }

    public async Task<bool> saveJoinRequest(JoinCourseRequestModel joinCourseRequest)
    {
        try
        {
            using var context = _serviceProvider.GetRequiredService<DataContext>();

            var existingRequest = await context.JoinCourseRequests.FirstOrDefaultAsync(x => x.Email == joinCourseRequest.Email);
            if (existingRequest != null)
            {
                existingRequest.CourseName = joinCourseRequest.CourseName;
                context.Entry(existingRequest).State = EntityState.Modified;
            }
            else
            {
                context.JoinCourseRequests.Add(new Data.Entities.JoinCourseRequest() { Email = joinCourseRequest.Email, CourseName = joinCourseRequest.CourseName });
            }
            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"ERROR : CourseMessageService.saveJoinRequest() :: {ex.Message}");
        }
        return false;
    }

    public EmailRequest GenerateEmailRequest(JoinCourseRequestModel joinCourseRequest)
    {
        try
        {
            if (!string.IsNullOrEmpty(joinCourseRequest.Email) && !string.IsNullOrEmpty(joinCourseRequest.CourseName))
            {
                var emailRequest = new EmailRequest
                {
                    Recipient = joinCourseRequest.Email,
                    Subject = "Course Registration",
                    HtmlContent = $@"
                        <!DOCTYPE html>
                        <html lang='en'>
                            <head>
                                <meta charset='UTF-8'>
                                <meta name='viewport' content='idth=device-width, initial-scale=1.0'>
                                <title>Course Registration</title>
                            </head>
                            <body>
                                <div style='color: #191919; max-width: 500px;'>
                                    <div style='background-color: #21aeff; color: white; text-align: center; padding: 20px 0;'>
                                        <h1 style='font-weight: 400;'>Course Registration</h1>
                                    </div>
                                    <div style='background-color: #f4f4f4; padding: 2rem 2rem;'>
                                        <p>Hello {joinCourseRequest.Email},</p>
                                        <p>We are sending you this message to remind you that you just registered for this course:</p>
                                        <p style='font-weight: 600; text-align: center; font-size: 25px; letter-spacing: 5px; padding: 2rem 0rem;'>
                                            {joinCourseRequest.CourseName}
                                        </p>
                                        <div style='color: #191919; font-size: 12px;'>
                                            <p>This email was sent from a notification-only address that cannot accept incoming email.</p>
                                            <p>If you did not join this course, we will still take your money. Try suing us.</p>
                                        </div>
                                    </div>
                                    <div style='color: #191919; text-align: center; font-size: 11px;'>
                                        <p>© Silicon Sweden. All rights reserved.</p>
                                    </div>
                                </div>
                            </body>
                        </html>
                    ",
                    PlainTextContent = $"Thank you for registering for the course {joinCourseRequest.CourseName}"
                };
                return emailRequest;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"ERROR : CourseMessageService.GenerateEmailRequest() :: {ex.Message}");
        }
        return null!;
    }

    public string GenerateServiceBusEmailRequest(EmailRequest emailRequest)
    {
        try
        {
            var payload = JsonConvert.SerializeObject(emailRequest);
            if (!string.IsNullOrEmpty(payload))
            {
                return payload;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"ERROR : CourseMessageService.GenerateServiceBusEmailRequest() :: {ex.Message}");
        }
        return null!;
    }
}
