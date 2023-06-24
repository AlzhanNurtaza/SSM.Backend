using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSM.Backend.Models;
using SSM.Backend.Models.Dto;
using SSM.Backend.Repository.IRepository;

namespace SSM.Backend.Controllers
{
    [Route("api/Speciality")]
    [ApiController]
    public class SpecialityController : ControllerBase
    {
        private readonly ISpecialityRepository _db;
        private readonly IMapper _mapper;
        public SpecialityController(ISpecialityRepository db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Speciality>> CreateAsync([FromBody] SpecialityCreateDTO createDTO)
        {
            try
            {
                if(createDTO == null)
                {
                    return BadRequest(new { message = "createDTO cannot be null" });
                }
                var newEntity = await _db.CreateAsync(_mapper.Map<Speciality>(createDTO));
                return CreatedAtAction("Get", new { id = newEntity.Id }, newEntity);
            }
            catch(Exception ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }
        [HttpGet("{id:length(24)}")]
        [Authorize]
        public async Task<ActionResult<SpecialityDTO>> GetAsync(string id)
        {
            try
            {
                var data= await _db.GetAsync(id);
                return Ok(_mapper.Map<SpecialityDTO>(data));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<SpecialityDTO>>> GetAllAsync(int _start, int _end, string? name_like = "",string? title_like="")
        {
            try
            {
                string nameFilter = name_like == string.Empty ? "" : $"Name={name_like}";
                string titleFilter = title_like == string.Empty ? "" : $"Name={title_like}";
                var entities = await _db.GetAllAsync(_start:_start,_end:_end, nameFilter, titleFilter);
                long total = await _db.GetCount();
                Response.Headers.Add("x-total-count", $"{total}");
                Response.Headers.Add("Access-Control-Expose-Headers", "x-total-count");
                return Ok(_mapper.Map<List<SpecialityDTO>>(entities));
            }
            catch(Exception ex) 
            {
                return BadRequest(new {message = ex.Message });
            }
        }

        [HttpPatch("{id:length(24)}")]
        [Authorize]
        public async Task<ActionResult<SpecialityDTO>> UpdateAsync(string id, [FromBody] SpecialityDTO entityDTO)
        {
            try
            {
                if(entityDTO == null || id != entityDTO.Id)
                {
                    return BadRequest(new { message = "entityDTO cannot be null or different Id provided" });
                }
                var data = await _db.UpdateAsync(id, _mapper.Map<Speciality>(entityDTO));
                return Ok(_mapper.Map<SpecialityDTO>(data));
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
