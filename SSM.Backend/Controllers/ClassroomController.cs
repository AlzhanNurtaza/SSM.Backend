﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSM.Backend.Models;
using SSM.Backend.Models.Dto;
using SSM.Backend.Repository.IRepository;

namespace SSM.Backend.Controllers
{
    [Route("api/Classroom")]
    [ApiController]
    public class ClassroomController : ControllerBase
    {
        private readonly IClassroomRepository _db;
        private readonly IMapper _mapper;
        public ClassroomController(IClassroomRepository db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Classroom>> CreateAsync([FromBody] ClassroomCreateDTO createDTO)
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
                var newData = await _db.CreateAsync(_mapper.Map<Classroom>(createDTO));
                return CreatedAtAction("Get", new { id = newData.Id }, newData);
            }
            catch(Exception ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }
        [HttpGet("{id:length(24)}")]
        [Authorize]
        public async Task<ActionResult<ClassroomDTO>> GetAsync(string id)
        {
            try
            {
                var data= await _db.GetAsync(id);
                return Ok(_mapper.Map<ClassroomDTO>(data));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<ClassroomDTO>>> GetAllAsync(int _start, int _end, [FromQuery(Name = "$filter")] string? filter,string? undefined="")
        {
            try
            {
                string filterName = undefined != string.Empty ? "Name=" + undefined : string.Empty;
                var datas = await _db.GetAllClassroomsAsync(_start:_start,_end:_end, filter, filterMain:filterName);
                long total = await _db.GetCount();
                Response.Headers.Add("x-total-count", $"{total}");
                Response.Headers.Add("Access-Control-Expose-Headers", "x-total-count");
                return Ok(_mapper.Map<List<ClassroomDTO>>(datas));
            }
            catch(Exception ex) 
            {
                return BadRequest(new {message = ex.Message });
            }
        }

        [HttpPatch("{id:length(24)}")]
        [Authorize]
        public async Task<ActionResult<ClassroomDTO>> UpdateAsync(string id, [FromBody] ClassroomDTO dataDTO)
        {
            try
            {
                if(dataDTO == null || id != dataDTO.Id)
                {
                    return BadRequest(new { message = "dataDTO cannot be null or different Id provided" });
                }
                bool isUnique = await _db.IsUnique("Name", dataDTO.Name, id);
                if (!isUnique)
                {
                    return BadRequest(new { message = "Name already exists" });
                }
                var data = await _db.UpdateAsync(id, _mapper.Map<Classroom>(dataDTO));
                return Ok(_mapper.Map<ClassroomDTO>(data));
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
