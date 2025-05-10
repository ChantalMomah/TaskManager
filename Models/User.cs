using System;
using System.Collections.Generic;

namespace Task_Manager.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string UserEmail { get; set; }
        public string UserPassword { get; set; } // In a real application, store hashed password
        public DateTime DateCreated { get; set; }
        public DateTime? LastLogin { get; set; }

        // Navigation properties
        public virtual ICollection<TaskCategory> Categories { get; set; } = new List<TaskCategory>();
        public virtual ICollection<TaskItem> TaskItems { get; set; } = new List<TaskItem>();
        public virtual ICollection<TaskTag> TaskTags { get; set; } = new List<TaskTag>(); // Changed from TaskComments to TaskTags
       
        
    }
}
