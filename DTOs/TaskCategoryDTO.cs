using System.ComponentModel.DataAnnotations;

namespace Task_Manager.DTOs
{
    public class TaskCategoryDTO
    {
        public int CategoryID { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string CategoryName { get; set; }

        [StringLength(500)]  // Optional description, limited to 500 characters
        public string Description { get; set; }

        [Required]
        [StringLength(7, MinimumLength = 7)]  // Color code must be in the format "#RRGGBB"
        public string Color { get; set; }
    }
}
