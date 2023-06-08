﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSM.Backend.Models;
using SSM.Backend.Models.Dto;
using SSM.Backend.Repository.IRepository;

namespace SSM.Backend.Controllers
{
    [Route("api/Group")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly IGroupRepository _db;
        private readonly IMapper _mapper;
        public GroupController(IGroupRepository db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpPost]
        //[Authorize(Roles = "Admin,Editor")]
        public async Task<ActionResult<GroupDTO>> CreateAsync([FromBody] GroupCreateDTO createDTO)
        {
            try
            {
                if(createDTO == null)
                {
                    return BadRequest(new { message = "createDTO cannot be null" });
                }
                var newData = await _db.CreateAsync(_mapper.Map<Group>(createDTO));
                return CreatedAtAction("Get", new { id = newData.Id }, newData);
            }
            catch(Exception ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }
        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<GroupDTO>> GetAsync(string id)
        {
            try
            {
                var data= await _db.GetAsync(id);
                return Ok(_mapper.Map<GroupDTO>(data));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<GroupDTO>>> GetAllAsync(int _start, int _end, string? name_like = "",string? title_like="")
        {
            try
            {
                string nameFilter = name_like == string.Empty ? "" : $"Name={name_like}";
                string titleFilter = title_like == string.Empty ? "" : $"Name={title_like}";
                var datas = await _db.GetAllAsync(_start:_start,_end:_end, nameFilter, titleFilter);
                long total = await _db.GetCount();
                Response.Headers.Add("x-total-count", $"{total}");
                Response.Headers.Add("Access-Control-Expose-Headers", "x-total-count");
                return Ok(_mapper.Map<List<GroupDTO>>(datas));
            }
            catch(Exception ex) 
            {
                return BadRequest(new {message = ex.Message });
            }
        }

        [HttpPatch("{id:length(24)}")]
        //[Authorize(Roles = "Admin,Editor")]
        public async Task<ActionResult<GroupDTO>> UpdateAsync(string id, [FromBody] GroupDTO dataDTO)
        {
            try
            {
                if(dataDTO == null || id != dataDTO.Id)
                {
                    return BadRequest(new { message = "dataDTO cannot be null or different Id provided" });
                }
                var data = await _db.UpdateAsync(id, _mapper.Map<Group>(dataDTO));
                return Ok(_mapper.Map<GroupDTO>(data));
            }
            catch(Exception ex)
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