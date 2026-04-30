using FirstCoreWebApp.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;

namespace FirstCoreWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("registers")]
        public async Task<IActionResult> Registers(Register reg)
        {
            //Email Validatio
            if(!Regex.IsMatch(reg.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                return ApiResponse.BadRequest("Invalid Email Format");
            }

            //Duplicate Email
            if(_context.Users.Any(u=>u.Email == reg.Email))
            {
                return ApiResponse.Conflict("Email Already Exists in the system");
            }

            if(!(IsValidaPassword(reg.Password)))
            {
                return ApiResponse.BadRequest("Password must be at least 8 characters and include uppercase, lowercase, number, and special character");
            }

            string hashPswd = BCrypt.Net.BCrypt.HashPassword(reg.Password);

            var user = new User
            {
                Name = reg.Name,
                Email = reg.Email,
                PasswordHash = hashPswd
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return ApiResponse.Created( "User Registered Successfully");

        }

        private bool IsValidaPassword(string password)
        {
            if(password.Length < 8)
            {
                return false;
            }
            if(!Regex.IsMatch(password, @"[A-Z]"))
            {
                return false;
            }
            if (!Regex.IsMatch(password, @"[a-z]"))
            {
                return false;
            }
            if (!Regex.IsMatch(password, @"[0-9]"))
            {
                return false;
            }
            if (!Regex.IsMatch(password, @"[\W_]"))
                return false;

            return true;
        }

        [HttpPost("loging")]
        public async Task<IActionResult> Logins(Login log)
        {
            if (!Regex.IsMatch(log.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                return ApiResponse.BadRequest("Invalid Email Format");
            }
            var user = _context.Users.FirstOrDefault(u=>u.Email == log.Email);
            if (user == null)
            {
                return ApiResponse.Unauthorized("Invalid Credential");
            }
            bool isvalid = BCrypt.Net.BCrypt.Verify(log.Password,user.PasswordHash );
            if(!isvalid) 
            {
                return ApiResponse.Unauthorized("Invalid Credential");
            }
            return ApiResponse.Success("Login Successfully");
        }
    }
}
