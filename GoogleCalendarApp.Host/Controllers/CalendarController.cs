using System;
using System.IO;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebApplication1.Configs;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class CalendarController : ControllerBase
{
    private readonly IConfiguration _configuration;
    
    
    public CalendarController(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    
    [HttpPost]
    public IActionResult CreateEvent()
    {
        var googleCredentialsConfig = _configuration.GetSection("GoogleService").Get<GoogleServiceConfig>();
        
        if (System.IO.File.Exists(googleCredentialsConfig.CredentialsPath) == false)
        {
            throw new AggregateException(
                $"Transcribe Configuration is not correct. ConfigurationUrl file not found by path : {googleCredentialsConfig.CredentialsPath}");
        }
        
        GoogleCredential credential;
        using (var stream = new FileStream(googleCredentialsConfig.CredentialsPath, FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream)
                .CreateScoped(
                    CalendarService.Scope.Calendar,
                    CalendarService.Scope.CalendarEvents
                );
        }
        
        var calendarService = new CalendarService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "ILTEC",
        });
        
        
        var calendarEvent = new Event
        {
            Summary = "Test Event With Conference Data. With attendees",
            Start = new EventDateTime() { DateTime = DateTime.Now.AddHours(1) },
            End = new EventDateTime() { DateTime = DateTime.Now.AddHours(2) },
            
            // Attendees = new List<EventAttendee>
            // {
            //     new EventAttendee { Email = "vsanyinclude@gmail.com" },
            // },
            // ConferenceData = new ConferenceData
            // {
            //     CreateRequest = new CreateConferenceRequest
            //     {
            //         RequestId = Guid.NewGuid().ToString(),
            //     },
            // },
        };


        const string calendarId = "iltec.helper@gmail.com";


        var request = calendarService.Events.Insert(calendarEvent, calendarId);
        var createdEvent = request.Execute();

        Console.WriteLine("Event created: " + createdEvent.HtmlLink);
        
        return Ok(createdEvent);
    }
}