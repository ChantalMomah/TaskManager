using System;
using System.ComponentModel.DataAnnotations;

namespace Task_Manager.Models
{
    public class TaskTag
    {
        [Key]  // Mark TagID as the primary key
        public int TagID { get; set; }  // Primary Key for the tag

        public string TagName { get; set; }  // Name of the tag (label for categorization)

        public int TaskID { get; set; }  // Foreign key to associate with a TaskItem
        public int UserID { get; set; }  // Foreign key to associate with the user who created the tag
        public DateTime CreatedDate { get; set; }  // Date when the tag was created

        // Navigation properties
        public virtual TaskItem TaskItem { get; set; }  // TaskItem associated with this tag
        public virtual User User { get; set; }  // User who created the tag
    }
}
