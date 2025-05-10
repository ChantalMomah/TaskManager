using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task_Manager.Data;
using Task_Manager.Models;
using Task_Manager.DTOs;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task_Manager.Models;
using Microsoft.IdentityModel.Tokens;

namespace Task_Manager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly TaskManagerContext _context;


        public CategoriesController(TaskManagerContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategories()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

            var userId = int.Parse(userIdClaim);

            var categories = await _context.TaskCategories
                .Where(c => c.UserID == userId)
                .Select(c => new CategoryDTO
                {
                    CategoryID = c.CategoryID,
                    CategoryName = c.CategoryName,
                    Description = c.Description,
                    Color = c.Color
                })
                .ToListAsync();

            return Ok(categories);
        
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDTO>> GetCategory(int id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

            var userId = int.Parse(userIdClaim);

            var category = await _context.TaskCategories
                .FirstOrDefaultAsync(c => c.CategoryID == id && c.UserID == userId);

            if (category == null) return NotFound();

            return Ok(new CategoryDTO
            {
                CategoryID = category.CategoryID,
                CategoryName = category.CategoryName,
                Description = category.Description,
                Color = category.Color
            });
        }

        [HttpPost]
        public async Task<ActionResult<CategoryDTO>> CreateCategory(CategoryDTO categoryDto)
        {
            if (string.IsNullOrWhiteSpace((string?)categoryDto.CategoryName))
                return BadRequest("Category name is required.");

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

            var userId = int.Parse(userIdClaim);

            var category = new TaskCategory
            {
                UserID = userId,
                CategoryName = (string)categoryDto.CategoryName,
                Description = (string)categoryDto.Description,
                Color = (string)(string.IsNullOrEmpty((string?)categoryDto.Color) ? "#808080" : categoryDto.Color)
            };

            _context.TaskCategories.Add(category);
            await _context.SaveChangesAsync();

            categoryDto.CategoryID = category.CategoryID;
            return CreatedAtAction(nameof(GetCategory), new { id = category.CategoryID }, categoryDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, CategoryDTO categoryDto)
        {
            if (id != (int?)categoryDto.CategoryID)
                return BadRequest("ID mismatch.");

            if (string.IsNullOrWhiteSpace((string?)categoryDto.CategoryName))
                return BadRequest("Category name is required.");

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

            var userId = int.Parse(userIdClaim);

            var category = await _context.TaskCategories
                .FirstOrDefaultAsync(c => c.CategoryID == id && c.UserID == userId);

            if (category == null) return NotFound();

            // Here we reference categoryDto correctly
            category.CategoryName = (string)categoryDto.CategoryName;
            category.Description = (string)categoryDto.Description;
            category.Color = (string)categoryDto.Color ?? category.Color;

            _context.Entry(category).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

            var userId = int.Parse(userIdClaim);

            var category = await _context.TaskCategories
                .FirstOrDefaultAsync(c => c.CategoryID == id && c.UserID == userId);

            if (category == null) return NotFound();

            _context.TaskCategories.Remove(category);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool CategoryExists(int id)
        {
            return _context.TaskCategories.Any(c => c.CategoryID == id);
        }
    }
}