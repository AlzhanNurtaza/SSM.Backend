using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSM.Backend.Models;
using SSM.Backend.Models.Dto;
using SSM.Backend.Repository.IRepository;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Diagnostics;

namespace SSM.Backend.Controllers
{
    [Route("api/Schedule")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleRepository _db;
        private readonly IMapper _mapper;
        public ScheduleController(IScheduleRepository db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        // GET: api/Schedule
        [HttpGet]
        public async Task<List<Schedule>> LoadData()
        {
            return await _db.GetAllScheduleAsync(null, null);
        }


        [HttpPost, HttpPut, HttpDelete]
        public async Task<IActionResult> Batch([FromBody] ScheduleParam param)
        {
            if (param.action == "insert" || (param.action == "batch" && param.added.Count > 0))
            {
                ScheduleCreateDTO eventData = (param.action == "insert") ? _mapper.Map<ScheduleCreateDTO>( param.value ) : param.added[0];
                Schedule insertData = new Schedule();
                insertData.StartTime = Convert.ToDateTime(eventData.StartTime).ToLocalTime();
                insertData.EndTime = Convert.ToDateTime(eventData.EndTime).ToLocalTime();
                insertData.Subject = eventData.Subject;
                insertData.IsAllDay = eventData.IsAllDay;
                //insertData.Location = eventData.Location;
                insertData.Description = eventData.Description;
                insertData.RecurrenceRule = eventData.RecurrenceRule;
                //insertData.RecurrenceID = eventData.RecurrenceID;
                //insertData.RecurrenceException = eventData.RecurrenceException;
                insertData.ClassroomId = eventData.ClassroomId;         
                insertData.EnrollmentId=eventData.EnrollmentId;

                await _db.CreateAsync(insertData);
            }
            if (param.action == "update" || (param.action == "batch" && param.changed.Count > 0))
            {
                Schedule eventData = (param.action == "update") ? param.value : param.changed[0];
                Schedule updateData = await _db.GetAsync(eventData.Id);
                if (updateData != null)
                {
                    updateData.StartTime = Convert.ToDateTime(eventData.StartTime).ToLocalTime();
                    updateData.EndTime = Convert.ToDateTime(eventData.EndTime).ToLocalTime();
                    updateData.Subject = eventData.Subject;
                    updateData.IsAllDay = eventData.IsAllDay;
                    //updateData.Location = eventData.Location;
                    updateData.Description = eventData.Description;
                    updateData.RecurrenceRule = eventData.RecurrenceRule;
                    //updateData.RecurrenceID = eventData.RecurrenceID;
                    //updateData.RecurrenceException = eventData.RecurrenceException;
                    updateData.ClassroomId = eventData.ClassroomId;
                    updateData.EnrollmentId = eventData.EnrollmentId;
                    await _db.UpdateAsync(eventData.Id,updateData);
                }
            }
            
            if (param.action == "remove" || (param.action == "batch" && param.deleted.Count > 0))
            {
                foreach (var events in param.deleted)
                {
                    await _db.RemoveAsync(events.Id);
                }
            }

            var result = await _db.GetAllScheduleAsync(null,null);

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = null
            };

            var json = JsonSerializer.Serialize(result, options);
            return Content(json, "application/json");
        }
        private static readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = null // Use default naming policy (preserve original case)
        };
    }   

}
