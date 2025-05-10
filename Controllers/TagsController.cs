using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task_Manager.Data;
using Task_Manager.Models;
using Task_Manager.DTOs;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Task_Manager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TagsController : ControllerBase
    {
        private readonly TaskManagerContext _context;

        public TagsController(TaskManagerContext context)
        {
            _context = context;
        }

        // Get all tags for the current user
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskTagDTO>>> GetTags()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

            var userId = int.Parse(userIdClaim);

            var tags = await _context.TaskTags
                .Where(t => t.UserID == userId)
                .Select(t => new TaskTagDTO
                {
                    TagID = t.TagID,
                    TaskID = t.TaskID,
                    UserID = t.UserID,
                    UserName = t.User.Name, // Assuming the User model has a Name property
                    TagName = t.TagName,
                    CreatedDate = t.CreatedDate
                })
                .ToListAsync();

            return Ok(tags);
        }

        // Get a specific tag by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskTagDTO>> GetTag(int id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

            var userId = int.Parse(userIdClaim);

            var tag = await _context.TaskTags
                .FirstOrDefaultAsync(t => t.TagID == id && t.UserID == userId);

            if (tag == null) return NotFound();

            return Ok(new TaskTagDTO
            {
                TagID = tag.TagID,
                TaskID = tag.TaskID,
                UserID = tag.UserID,
                UserName = tag.User.Name, // Assuming the User model has a Name property
                TagName = tag.TagName,
                CreatedDate = tag.CreatedDate
            });
        }

        // Create a new tag for a task
        [HttpPost]
        public async Task<ActionResult<TaskTagDTO>> CreateTag(TaskTagDTO tagDto)
        {
            if (string.IsNullOrWhiteSpace(tagDto.TagName))
                return BadRequest("Tag name is required.");

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

            var userId = int.Parse(userIdClaim);

            var tag = new TaskTag
            {
                UserID = userId,
                TaskID = tagDto.TaskID,
                TagName = tagDto.TagName,
                CreatedDate = DateTime.Now
            };

            _context.TaskTags.Add(tag);
            await _context.SaveChangesAsync();

            tagDto.TagID = tag.TagID;
            return CreatedAtAction(nameof(GetTag), new { id = tag.TagID }, tagDto);
        }

        // Update an existing tag
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTag(int id, TaskTagDTO tagDto)
        {
            if (id != tagDto.TagID)
                return BadRequest("ID mismatch.");

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

            var userId = int.Parse(userIdClaim);

            var tag = await _context.TaskTags
                .FirstOrDefaultAsync(t => t.TagID == id && t.UserID == userId);

            if (tag == null) return NotFound();

            tag.TagName = tagDto.TagName;
            tag.TaskID = tagDto.TaskID;
            tag.CreatedDate = DateTime.Now;

            _context.Entry(tag).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Delete a tag
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTag(int id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

            var userId = int.Parse(userIdClaim);

            var tag = await _context.TaskTags
                .FirstOrDefaultAsync(t => t.TagID == id && t.UserID == userId);

            if (tag == null) return NotFound();

            _context.TaskTags.Remove(tag);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
