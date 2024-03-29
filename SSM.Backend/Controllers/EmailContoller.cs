﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSM.Backend.Models;
using SSM.Backend.Repository.IRepository;

namespace SSM.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailContoller : ControllerBase
    {
        private readonly IMailService _mail;

        public EmailContoller(IMailService mail)
        {
            _mail = mail;
        }

        [HttpPost("sendmail")]
        [Authorize]
        public async Task<IActionResult> SendMailAsync(MailData mailData)
        {
            bool result = await _mail.SendAsync(mailData, new CancellationToken());

            if (result)
            {
                return StatusCode(StatusCodes.Status200OK, "Mail has successfully been sent.");
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occured. The Mail could not be sent.");
            }
        }
    }
}
