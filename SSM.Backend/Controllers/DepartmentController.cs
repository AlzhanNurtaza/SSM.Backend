using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSM.Backend.Models;
using SSM.Backend.Models.Dto;
using SSM.Backend.Repository.IRepository;

namespace SSM.Backend.Controllers
{
    [Route("api/Department")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentRepository _db;
        private readonly IMapper _mapper;
        public DepartmentController(IDepartmentRepository db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Department>> CreateAsync([FromBody] DepartmentCreateDTO createDTO)
        {
            try
            {
                if(createDTO == null)
                {
                    return BadRequest(new { message = "createDTO cannot be null" });
                }
                Department newData = await _db.CreateAsync(_mapper.Map<Department>(createDTO));
                return CreatedAtAction("Get", new { id = newData.Id }, newData);
            }
            catch(Exception ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }
        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<DepartmentDTO>> GetAsync(string id)
        {
            try
            {
                var data= await _db.GetAsync(id);
                return Ok(_mapper.Map<DepartmentDTO>(data));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<DepartmentDTO>>> GetAllAsync(int _start, int _end, string? name_like = "",string? title_like="")
        {
            try
            {
                string nameFilter = name_like == string.Empty ? "" : $"Name={name_like}";
                string titleFilter = title_like == string.Empty ? "" : $"Name={title_like}";
                var datas = await _db.GetAllAsync(_start:_start,_end:_end, nameFilter, titleFilter);
                long total = await _db.GetCount();
                Response.Headers.Add("x-total-count", $"{total}");
                Response.Headers.Add("Access-Control-Expose-Headers", "x-total-count");
                return Ok(_mapper.Map<List<DepartmentDTO>>(datas));
            }
            catch(Exception ex) 
            {
                return BadRequest(new {message = ex.Message });
            }
        }

        [HttpPatch("{id:length(24)}")]
        [Authorize]
        public async Task<ActionResult<DepartmentDTO>> UpdateAsync(string id, [FromBody] DepartmentDTO dataDTO)
        {
            try
            {
                if(dataDTO == null || id != dataDTO.Id)
                {
                    return BadRequest(new { message = "dataDTO cannot be null or different Id provided" });
                }
                var data = await _db.UpdateAsync(id, _mapper.Map<Department>(dataDTO));
                return Ok(_mapper.Map<DepartmentDTO>(data));
            }
            catch(Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:length(24)}")]
        [Authorize]
        public async Task<IActionResult> DeleteDepartmentAsync(string id)
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
