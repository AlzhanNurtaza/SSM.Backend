using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSM.Backend.Models;
using SSM.Backend.Models.Dto;
using SSM.Backend.Repository.IRepository;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Org.BouncyCastle.Utilities;
using System;

namespace SSM.Backend.Controllers
{
    [Route("api/Schedule")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleRepository _db;
        private readonly IEnrollmentRepository _dbEnroll;
        private readonly IClassroomRepository _dbClassroom;
        private readonly IMapper _mapper;
        public ScheduleController(IScheduleRepository db, IMapper mapper, IEnrollmentRepository dbEnroll, IClassroomRepository dbClassroom)
        {
            _db = db;
            _mapper = mapper;
            _dbEnroll = dbEnroll;
            _dbClassroom = dbClassroom;
        }
        // GET: api/Schedule
        [HttpGet]
        public async Task<List<Schedule>> LoadData()
        {
            return await _db.GetAllScheduleAsync(null, null,null);
        }


        [HttpPost, HttpPut, HttpDelete]
        [Authorize]
        public async Task<IActionResult> Batch([FromBody] ScheduleParam param)
        {
           if (param.action == "insert" || (param.action == "batch" && param.added.Count > 0))
            {
                ScheduleCreateDTO eventData = (param.action == "insert") ? _mapper.Map<ScheduleCreateDTO>( param.value ) : param.added[0];
                Schedule insertData = _mapper.Map<Schedule>(eventData);

                await _db.CreateAsync(insertData);
            }
            if (param.action == "update" || (param.action == "batch" && param.changed.Count > 0))
            {
                Schedule eventData = (param.action == "update") ? param.value : param.changed[0];
                Schedule updateData = await _db.GetAsync(eventData.Id);
                if (updateData != null)
                {
                    await _db.UpdateAsync(eventData.Id, eventData);
                }
            }
            
            if (param.action == "remove" || (param.action == "batch" && param.deleted.Count > 0))
            {
                foreach (var events in param.deleted)
                {
                    await _db.RemoveAsync(events.Id);
                }
            }
            var result = await _db.GetAllScheduleAsync(null,null,param.where);

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = null
            };

            var json = JsonSerializer.Serialize(result, options);
            return Content(json, "application/json");
        }

        [HttpPost("createAuto")]
        [Authorize]
        public async Task<IActionResult> CreateAuto([FromBody] SingleObject singleObject)
        {
            if (singleObject == null)
            {
                return BadRequest(new { error = "Вы не передали Id" });

            }
            var enrollment = await _dbEnroll.GetAsync(singleObject.Id);
            if (enrollment == null)
            {
                return BadRequest(new { error = "Регистрация не может быть пустой" });
            }
            var classrooms = await _dbClassroom.GetAllAsync(0, 100);

            int studentsCount = 0;
            foreach(var group in enrollment.Groups)
            {
                studentsCount += group.Students.Count;
            }
            var filteredClassrooms = classrooms.Where(c=>c.Seats >= studentsCount).ToList();
            if (filteredClassrooms.Count <1)
            {
                return BadRequest(new { error = "Невозможно найти аудиторию где общее кол-во студентов=" + studentsCount });
            }
            bool enrollomentIsNew = await _db.isEnrollmentNewInSchedule(singleObject.Id);
            if (!enrollomentIsNew)
            {
                return BadRequest(new { error = "Невозможно создать расписание по данной регистрации. Уже имеются записи по ней" });
            }
            string groupsName = string.Empty;
            foreach (var group in enrollment.Groups)
            {
                groupsName += group.Name+"; ";
            }

            //Определить сколько повторений нужно по 50 мин
            if(enrollment.Course.CreditCount==0)
            {
                return BadRequest(new { error = "Ошибка. Кол-во, кредитов по предмету не может быть равно 0" });
            }
            int totalMin = enrollment.StudyCount;
            int totalAcamCount = Convert.ToInt32(totalMin / 50);
            int countByTwo = Convert.ToInt32(totalAcamCount / 2);

            //Выбор случайной даты
            List<string> days = new List<string> { "MO", "TU", "WE", "TH", "FR", "SA" };
            Random random = new Random();
            List<string> randomDays = days.OrderBy(x => random.Next()).Take(2).ToList();
            string reccurRule = $"FREQ=WEEKLY;BYDAY={string.Join(",",randomDays)};INTERVAL=1;COUNT={countByTwo};";

            //Выбор времени от 8 утра до 19 вечера
            int randomNumber = random.Next(7, 20); // Generates a random number between 7 (inclusive) and 20 (exclusive)

            // If the generated random number is 20, we subtract 1 to include 19
            if (randomNumber == 20)
            {
                randomNumber--;
            }
            DateTime startTime = enrollment.StartDate.AddHours(randomNumber);
            DateTime endTime = startTime.AddMinutes(50);

            //random classroom
            filteredClassrooms = filteredClassrooms.OrderBy(x => random.Next()).Take(1).ToList();

            ScheduleCreateDTO newDto = new ScheduleCreateDTO();
            newDto.EnrollmentId = enrollment.Id;
            newDto.StartTime = startTime;
            newDto.EndTime = endTime;
            newDto.ClassroomId = filteredClassrooms[0].Id; //
            newDto.Subject = enrollment.Name;
            newDto.RecurrenceRule = reccurRule;
            newDto.EnrollmentName = enrollment.Name;
            newDto.GroupsName = groupsName;
            newDto.StudentCount = studentsCount;
            newDto.CourseName = enrollment.Course.Name;
            newDto.SeatCount = filteredClassrooms[0].Seats;
            newDto.InstructorName = enrollment.Instructor.LastName + " " + enrollment.Instructor.FirstName;
            newDto.ClassroomName = filteredClassrooms[0].Name; //
            
            ScheduleParam param = new ScheduleParam();
            param.action = "batch";
            param.added = new List<ScheduleCreateDTO> { newDto };
            param.startDate = enrollment.StartDate;
            param.endDate = enrollment.EndDate;
            await this.Batch(param);

            var sch2 = newDto;
            sch2.StartTime = endTime.AddMinutes(10);
            sch2.EndTime = endTime.AddMinutes(60);

            var param2 = param;
            param2.added = new List<ScheduleCreateDTO> { sch2 };
            await this.Batch(param2);

            return Ok(new { message = $"Общее кол-во {enrollment.Course.CreditCount} кредитов ({totalAcamCount}) академ. часов раскидано" });
        }

        private static readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = null // Use default naming policy (preserve original case)
        };
    }   

}
