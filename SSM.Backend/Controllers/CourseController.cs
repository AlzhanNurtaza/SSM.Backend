using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSM.Backend.Models.Dto;
using SSM.Backend.Models;
using SSM.Backend.Repository.IRepository;

namespace SSM.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ICourseRepository _db;
        private readonly IMapper _mapper;
        public CourseController(ICourseRepository db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<Course>>> GetAllAsync(int _start, int _end, string? name_like = "")
        {
            try
            {
                string filter = name_like == string.Empty ? "" : $"Name={name_like}";
                List<Course> courses = await _db.GetAllAsync(_start: _start, _end: _end, filter);
                long total = await _db.GetCount();
                Response.Headers.Add("x-total-count", $"{total}");
                Response.Headers.Add("Access-Control-Expose-Headers", "x-total-count");
                return Ok(_mapper.Map<List<CourseDTO>>(courses));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        //[Authorize(Roles = "Admin,Editor")]
        public async Task<ActionResult<Course>> CreateAsync([FromBody] CourseCreateDTO courseDTO)
        {
            try
            {
                if (courseDTO == null)
                {
                    return BadRequest(new { message = "CourseDTO cannot be null" });
                }
                var course = await _db.CreateAsync(_mapper.Map<Course>(courseDTO));
                return CreatedAtAction("Get", new { id = course.Id }, course);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id:length(24)}", Name = "GetCourse")]
        public async Task<ActionResult<CourseDTO>> GetAsync(string id)
        {
            try
            {
                Course course = await _db.GetAsync(id);
                return Ok(_mapper.Map<CourseDTO>(course));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPatch("{id:length(24)}")]
        //[Authorize(Roles = "Admin,Editor")]
        public async Task<ActionResult<CourseDTO>> UpdateAsync(string id, [FromBody] CourseDTO courseDTO)
        {
            try
            {
                if (courseDTO == null || id != courseDTO.Id)
                {
                    return BadRequest(new { message = "courseDTO cannot be null or different Id provided" });
                }
                var course = await _db.UpdateAsync(id, _mapper.Map<Course>(courseDTO));
                return Ok(_mapper.Map<CourseDTO>(course));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:length(24)}")]
        //[Authorize(Roles = "Admin,Editor")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest(new { message = "id cannot be empty or null" });
                }
                await _db.RemoveAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
