﻿using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace LIN.Hangfire.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MailSenderController : ControllerBase
{

    [HttpPost]
    public IActionResult Send([FromQuery] string mail, [FromQuery] string subject, [FromBody] string content)
    {
        // Encolar job.
        BackgroundJob.Enqueue<Jobs.MailSenderJob>("mailing", t => t.Run(mail, subject, content));
        return Ok();
    }

}