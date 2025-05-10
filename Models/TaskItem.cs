using System;
using System.ComponentModel.DataAnnotations;
using Task_Manager.Models;

public class TaskItem
{
    [Key] // ✅ Explicitly mark TaskID as the primary key
    public int TaskID { get; set; }

    public int UserID { get; set; }

    public int? CategoryID { get; set; }

    public User User { get; set; }

    public string TaskName { get; set; }

    public TaskCategory? Category { get; set; }

    [Required]
    [StringLength(100)]
    public string Title { get; set; }

    [StringLength(500)]
    public string Description { get; set; }

    public DateTime DueDate { get; set; }

    [StringLength(20)]
    public string Priority { get; set; }

    [StringLength(20)]
    public string Status { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime ModifiedDate { get; set; }

    // Navigation property for TaskTags
    public virtual ICollection<TaskTag> TaskTags { get; set; }  // Correct navigation property for TaskTags
}
