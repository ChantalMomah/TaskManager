using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Task_Manager.Data;
using Task_Manager.Models;
using Org.BouncyCastle.Crypto.Generators;
using BCrypt.Net;
using System.Linq;

namespace Task_Manager.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly TaskManagerContext _context;

        public AuthController(TaskManagerContext context)
        {
            _context = context;
        }

        // Login Endpoint
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            // Check if user exists by email
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserEmail == loginRequest.UserEmail);

            // If the user doesn't exist or the password is incorrect
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginRequest.UserPassword, user.UserPassword))
            {
                return Unauthorized("Invalid credentials");
            }

            // Check for outdated bcrypt hash format (e.g., $2a$ vs $2b$)
            if (user.UserPassword.StartsWith("$2a$"))
            {
                // Rehash the password with the latest bcrypt version
                var newHash = BCrypt.Net.BCrypt.HashPassword(loginRequest.UserPassword);
                user.UserPassword = newHash;  // Save the new hash to the database
                await _context.SaveChangesAsync();  // Make sure to save the updated hash
            }

            // Create claims for the user
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.Email, user.UserEmail),
        new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString())
    };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // Set authentication cookie
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true // Optional: makes the cookie persistent across sessions
            };

            // Sign in the user
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            return Ok(new { message = "Login successful" });
        }


        // Registration Endpoint
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            if (registerRequest == null)
            {
                return BadRequest("Request data is missing.");
            }

            // Validate that Name, Email, etc. are provided
            if (string.IsNullOrEmpty(registerRequest.UserName))
            {
                return BadRequest("Name is required.");
            }

            if (string.IsNullOrEmpty(registerRequest.UserEmail))
            {
                return BadRequest("Email is required.");
            }

            if (string.IsNullOrEmpty(registerRequest.UserPassword))
            {
                return BadRequest("Password is required.");
            }

            // ✅ HASH the password before saving it
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerRequest.UserPassword);

            // Create the user entity from the registration request
            var user = new User
            {
                Name = registerRequest.UserName,
                UserName = registerRequest.UserName,
                UserEmail = registerRequest.UserEmail,
                UserPassword = hashedPassword, // Store the hashed password
                DateCreated = DateTime.Now
            };

            // Add the new user to the database
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Registration successful" });
        }


        [HttpPost("update-passwords")]
        public async Task<IActionResult> UpdatePasswords()
        {
            try
            {
                // Fetch all users
                var users = await _context.Users.ToListAsync();

                // Loop through all users and hash their passwords if needed
                foreach (var user in users)
                {
                    if (user.UserPassword.Length != 60)  // BCrypt hashes are usually 60 characters long
                    {
                        user.UserPassword = BCrypt.Net.BCrypt.HashPassword(user.UserPassword); // Rehash password
                        _context.Users.Update(user);
                    }
                }

                await _context.SaveChangesAsync(); // Save changes to DB
                return Ok(new { message = "Passwords updated successfully." });
            }
            catch (Exception ex)
            {
                // Log the exception (this is just an example, replace with your logging framework)
                Console.WriteLine($"Error in UpdatePasswords: {ex.Message}");

                // Return a generic error response
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Registration Request DTO
        public class RegisterRequest
        {
            public string UserName { get; set; }
            public string UserEmail { get; set; }
            public string UserPassword { get; set; }
        }

        // Login Request DTO
        public class LoginRequestDTO
        {
            public string UserEmail { get; set; }
            public string UserPassword { get; set; }
        }
    }
}