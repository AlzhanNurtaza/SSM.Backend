using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SSM.Backend.Models;
using SSM.Backend.Models.Dto;
using SSM.Backend.Repository.IRepository;

namespace SSM.Backend.Controllers
{
    [Route("api/Enrollment")]
    [ApiController]
    public class EnrollmentController : ControllerBase
    {
        private readonly IEnrollmentRepository _db;
        private readonly IMapper _mapper;
        public EnrollmentController(IEnrollmentRepository db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<EnrollmentDTO>> CreateAsync([FromBody] EnrollmentCreateDTO createDTO)
        {
            try
            {
                if(createDTO == null)
                {
                    return BadRequest(new { message = "createDTO cannot be null" });
                }
                bool isUnique = await _db.IsUnique("Name", createDTO.Name);
                if (!isUnique)
                {
                    return BadRequest(new { message = "Name already exists" });
                }
                var newData = await _db.CreateAsync(_mapper.Map<Enrollment>(createDTO));
                return CreatedAtAction("Get", new { id = newData.Id }, newData);
            }
            catch(Exception ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }
        [HttpGet("{id:length(24)}")]
        [Authorize]
        public async Task<ActionResult<EnrollmentDTO>> GetAsync(string id)
        {
            try
            {
                var data= await _db.GetAsync(id);
                return Ok(_mapper.Map<EnrollmentDTO>(data));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<EnrollmentDTO>>> GetAllAsync(int _start, int _end, string? name_like = "",string? title_like="")
        {
            try
            {
                string nameFilter = name_like == string.Empty ? "" : $"Name={name_like}";
                string titleFilter = title_like == string.Empty ? "" : $"Name={title_like}";
                var datas = await _db.GetAllAsync(_start:_start,_end:_end, nameFilter, titleFilter);
                long total = await _db.GetCount();
                Response.Headers.Add("x-total-count", $"{total}");
                Response.Headers.Add("Access-Control-Expose-Headers", "x-total-count");
                return Ok(_mapper.Map<List<EnrollmentDTO>>(datas));
            }
            catch(Exception ex) 
            {
                return BadRequest(new {message = ex.Message });
            }
        }

        [HttpPatch("{id:length(24)}")]
        [Authorize]
        public async Task<ActionResult<GroupDTO>> UpdateAsync(string id, [FromBody] EnrollmentDTO dataDTO)
        {
            try
            {
                if(dataDTO == null || id != dataDTO.Id)
                {
                    return BadRequest(new { message = "dataDTO cannot be null or different Id provided" });
                }
                bool isUnique = await _db.IsUnique("Name", dataDTO.Name,id);
                if (!isUnique)
                {
                    return BadRequest(new { message = "Name already exists" });
                }
                var data = await _db.UpdateAsync(id, _mapper.Map<Enrollment>(dataDTO));
                return Ok(_mapper.Map<EnrollmentDTO>(data));
            }
            catch(Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:length(24)}")]
        [Authorize]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            try
            {
                if(string.IsNullOrEmpty(id))
                {
                    return BadRequest(new { message = "id cannot be empty or null" });
                }
                await _db.RemoveAsync(id);
                return NoContent();
            }
            catch(Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
