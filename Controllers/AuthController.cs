using FirstCoreWebApp.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Text;
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
            if (!Regex.IsMatch(reg.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                return BadRequest(AppMessages.InvalidEmail);
            }

            if (_context.Users.Any(u => u.Email == reg.Email))
            {
                return Conflict(AppMessages.AlreadyExists);
            }

            if (!IsValidaPassword(reg.Password))
            {
                return BadRequest(AppMessages.IncorrectFormat);
            }

            string hashPswd = BCrypt.Net.BCrypt.HashPassword(reg.Password);

            var userRole = _context.Roles.FirstOrDefault(r => r.RoleName == "User");

            var user = new User
            {
                Name = reg.Name,
                Email = reg.Email,
                PasswordHash = hashPswd,
                RoleId = userRole.Id  
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Created("",AppMessages.CreateSuccess);

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
                return BadRequest(AppMessages.InvalidEmail);
            }
            var user = _context.Users.Include(u => u.Role).FirstOrDefault(u=>u.Email == log.Email);
            if (user == null)
            {
                return Unauthorized(AppMessages.InvalidCred);
            }
            bool isvalid = BCrypt.Net.BCrypt.Verify(log.Password,user.PasswordHash );
            if(!isvalid) 
            {
                return Unauthorized(AppMessages.InvalidCred);
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.RoleName)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("my_super_secret_key_12345"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); //secure key by using hashing algorithum

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(2),
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);  //Convert token in string format

            var refreshToken = Guid.NewGuid().ToString();  //Random string generate

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(4);

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = AppMessages.LoginSuccess,
                token = jwt,
                refreshToken = refreshToken
            });
        }

        [Authorize]
        [HttpGet("GetProfile")]
        public IActionResult Profile()
        {
            var Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var mail = User.FindFirst(ClaimTypes.Email.ToString())?.Value;
            return Ok(new
            {
                message = AppMessages.ProfileFetched,
                userid = Id,
                email = mail
            });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(string refreshToken)
        {
            var user = _context.Users.FirstOrDefault(u => u.RefreshToken == refreshToken);

            if (user == null)
            {
                return Unauthorized(AppMessages.InvalidRefreshToken);
            }

            if (user.IsRefreshTokenRevoked)
            {
                return Unauthorized(AppMessages.RefreshTokenRevoked);
            }

            if (user.RefreshTokenExpiryTime < DateTime.Now)
            {
                return Unauthorized(AppMessages.RefreshTokenExpired);
            }
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("my_super_secret_key_12345"));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var newToken = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(newToken);

            return Ok(new
            {
                message = AppMessages.TokenGenerated,
                token = jwt
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Loggedout(string refreshToken)
        {
            var user = _context.Users.FirstOrDefault(u => u.RefreshToken == refreshToken);

            if (user == null)
            {
                return Unauthorized(AppMessages.InvalidRefreshToken);
            }

            user.RefreshToken = "";
            user.RefreshTokenExpiryTime = null;

            user.IsRefreshTokenRevoked = true;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = AppMessages.LogoutSuccess
            });
        }

        [Authorize(Roles="Admin")]
        [HttpGet("Admin")]
        public IActionResult AdminOnly()
        {
            return Ok(new
            {
                message = AppMessages.AdminWelcome
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("roles")]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            // 1. validation
            if (string.IsNullOrWhiteSpace(roleName))
            {
                return BadRequest(AppMessages.Required);
            }

            var exists = _context.Roles.Any(r => r.RoleName == roleName);

            if (exists)
            {
                return Conflict(AppMessages.RoleExists);
            }

            // 3. create role
            var role = new Role
            {
                RoleName = roleName
            };

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            // 4. return response
            return Created("", new
            {
                role.Id,
                role.RoleName,
                message = AppMessages.RoleCreated
            });
        }

        [Authorize(Roles ="Admin")]
        [HttpGet("roles")]
        public IActionResult GetAllRoles()
        {
            var roles = _context.Roles.Select(r => new
            {
                r.Id,
                r.RoleName
            }).ToList();
            return Ok(new
            {
                message = AppMessages.RolesFound,
                data = roles
            });
        }
    }
}
