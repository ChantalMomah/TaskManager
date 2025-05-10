using System.ComponentModel.DataAnnotations;

namespace Task_Manager.Models
{
    public class RegisterRequest
    {
        [Required] // Ensures Name must be provided
        public string Name { get; set; }

        [Required] // Ensures UserName must be provided
        public string UserName { get; set; }

        [Required] // Ensures Email must be provided
        public string UserEmail { get; set; }

        [Required] // Ensures Password must be provided
        public string UserPassword { get; set; }
    }
}
