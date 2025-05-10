using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Task_Manager.Models
{
    public class TaskCategory
    {
        [Key]  // Define CategoryID as the primary key
        public int CategoryID { get; set; }    // Primary Key for the category

        [Required]  // Ensures that CategoryName is required
        [StringLength(100)]  // Optionally add length validation
        public string CategoryName { get; set; } // Name of the category

        [StringLength(500)]  // Optionally add length validation for Description
        public string Description { get; set; }  // Description of the category

        [StringLength(20)]  // Optionally add length validation for Color
        public string Color { get; set; }        // Color associated with the category

        public int UserID { get; set; }          // Foreign key to associate with a user

        [Required]  // Ensure CreatedDate is always populated
        public DateTime CreatedDate { get; set; }  // Date when the category was created

        // Navigation properties
        public virtual User User { get; set; }    // User associated with the category
        public virtual ICollection<TaskItem> TaskItems { get; set; }  // Tasks linked to this category
    }
}
