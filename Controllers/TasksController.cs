using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task_Manager.Data;
using Task_Manager.Models;
using Task_Manager.DTOs;
using System.Security.Claims;

namespace Task_Manager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly TaskManagerContext _context;

        public TasksController(TaskManagerContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskDTO>>> GetTasks([FromQuery] string status = null)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var query = _context.TaskItems
                .Where(t => t.UserID == int.Parse(userId))
                .Include(t => t.Category);

            if (!string.IsNullOrEmpty(status))
            {
                query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<TaskItem, TaskCategory?>)query.Where(t => t.Status == status); // Fixed type casting here
            }

            var tasks = await query
                .Select(t => new TaskDTO
                {
                    TaskID = t.TaskID,
                    CategoryID = t.CategoryID,
                    CategoryName = t.Category != null ? t.Category.CategoryName : null,
                    Title = t.Title,
                    Description = t.Description,
                    DueDate = t.DueDate,
                    Priority = t.Priority,
                    Status = t.Status,
                    CreatedDate = t.CreatedDate,
                    ModifiedDate = t.ModifiedDate
                })
                .ToListAsync();

            return Ok(tasks);
        }

        [HttpPost]
        public async Task<ActionResult<TaskDTO>> CreateTask(TaskDTO taskDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var taskItem = new TaskItem
            {
                UserID = int.Parse(userId),
                CategoryID = taskDto.CategoryID,
                Title = taskDto.Title,
                Description = taskDto.Description,
                DueDate = taskDto.DueDate,
                Priority = taskDto.Priority,
                Status = "Pending",
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            };

            _context.TaskItems.Add(taskItem);
            await _context.SaveChangesAsync();

            taskDto.TaskID = taskItem.TaskID;
            taskDto.CreatedDate = taskItem.CreatedDate;
            taskDto.ModifiedDate = taskItem.ModifiedDate;
            taskDto.Status = taskItem.Status;

            return CreatedAtAction(nameof(GetTask), new { id = taskItem.TaskID }, taskDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskDTO>> GetTask(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var taskItem = await _context.TaskItems
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.TaskID == id && t.UserID == int.Parse(userId));

            if (taskItem == null)
                return NotFound();

            return Ok(new TaskDTO
            {
                TaskID = taskItem.TaskID,
                CategoryID = taskItem.CategoryID,
                CategoryName = taskItem.Category?.CategoryName,
                Title = taskItem.Title,
                Description = taskItem.Description,
                DueDate = taskItem.DueDate,
                Priority = taskItem.Priority,
                Status = taskItem.Status,
                CreatedDate = taskItem.CreatedDate,
                ModifiedDate = taskItem.ModifiedDate
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, TaskDTO taskDto)
        {
            if (id != taskDto.TaskID)
                return BadRequest();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var taskItem = await _context.TaskItems
                .FirstOrDefaultAsync(t => t.TaskID == id && t.UserID == int.Parse(userId));

            if (taskItem == null)
                return NotFound();

            taskItem.CategoryID = taskDto.CategoryID;
            taskItem.Title = taskDto.Title;
            taskItem.Description = taskDto.Description;
            taskItem.DueDate = taskDto.DueDate;
            taskItem.Priority = taskDto.Priority;
            taskItem.Status = taskDto.Status;
            taskItem.ModifiedDate = DateTime.Now;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateTaskStatus(int id, [FromBody] string status)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var taskItem = await _context.TaskItems
                .FirstOrDefaultAsync(t => t.TaskID == id && t.UserID == int.Parse(userId));

            if (taskItem == null)
                return NotFound();

            taskItem.Status = status;
            taskItem.ModifiedDate = DateTime.Now;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var taskItem = await _context.TaskItems
                .FirstOrDefaultAsync(t => t.TaskID == id && t.UserID == int.Parse(userId));

            if (taskItem == null)
                return NotFound();

            _context.TaskItems.Remove(taskItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
