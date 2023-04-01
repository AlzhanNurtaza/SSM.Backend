using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using SSM.Backend.Models;
using SSM.Backend.Models.Dto;
using SSM.Backend.Repositoty.IRepository;
using System.Text.Json;

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
        public async Task<ActionResult<Department>> CreateAsync([FromBody] DepartmentCreateDTO departmentCreateDTO)
        {
            try
            {
                if(departmentCreateDTO == null)
                {
                    return BadRequest(new { message = "departmentCreateDTO cannot be null" });
                }
                Department newDepartment = await _db.CreateAsync(_mapper.Map<Department>(departmentCreateDTO));
                return CreatedAtAction("Get", new { id = newDepartment.Id }, newDepartment);
            }
            catch(Exception ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }
        [HttpGet("{id:length(24)}",Name = "GetDepartment")]
        public async Task<ActionResult<DepartmentDTO>> GetAsync(string id)
        {
            try
            {
                Department department= await _db.GetAsync(id);
                return Ok(_mapper.Map<DepartmentDTO>(department));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<DepartmentDTO>>> GetAllAsync(int _start, int _end)
        {
            try
            {
                List<Department> departments = await _db.GetAllAsync(_start:_start,_end:_end);
                long total = await _db.GetCount();
                Response.Headers.Add("x-total-count", $"{total}");
                Response.Headers.Add("Access-Control-Expose-Headers", "x-total-count");
                return Ok(_mapper.Map<List<DepartmentDTO>>(departments));
            }
            catch(Exception ex) 
            {
                return BadRequest(new {message = ex.Message });
            }
        }

        [HttpPatch("{id:length(24)}")]
        public async Task<ActionResult<DepartmentDTO>> UpdateAsync(string id, [FromBody] DepartmentDTO departmentDTO)
        {
            try
            {
                if(departmentDTO == null || id != departmentDTO.Id)
                {
                    return BadRequest(new { message = "departmentDTO cannot be null or different Id provided" });
                }
                var department = await _db.UpdateAsync(id, _mapper.Map<Department>(departmentDTO));
                return Ok(_mapper.Map<DepartmentDTO>(department));
            }
            catch(Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:length(24)}")]
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
