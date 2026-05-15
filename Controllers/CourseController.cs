using FirstCoreWebApp.Model;
using FirstCoreWebApp.Model.Course;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FirstCoreWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CourseController(AppDbContext context)
        {
            _context = context;
        }


        [Authorize(Roles = "Instructor,Teacher")]
        [HttpPost("AddCourse")]
        public async Task<IActionResult> Course(CreateCourseDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!Enum.IsDefined(typeof(CourseStatus), dto.Status))
            {
                return BadRequest(AppMessages.InvalidStatus);
            }

            if (dto.PrerequisiteCourseIds != null && dto.PrerequisiteCourseIds.Any())
            {
                var existingIds = await _context.Courses
                    .Where(c => dto.PrerequisiteCourseIds.Contains(c.Id)) // PrerequisiteCourseIds that sends in json that are matches with Course Id those who are matches it retrns
                    .Select(c => c.Id)
                    .ToListAsync();

                var invalidIds = dto.PrerequisiteCourseIds
                    .Except(existingIds)
                    .ToList();

                if (invalidIds.Any())
                {
                    return BadRequest(new
                    {
                        Message = string.Format(AppMessages.PrerequisiteCourseIds, string.Join(", ", invalidIds))
                    });
                }
            }

            var course = new Course
            {
                Title = dto.Title,
                Description = dto.Description,
                Status = dto.Status
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            if (dto.PrerequisiteCourseIds != null && dto.PrerequisiteCourseIds.Any())
            {
                foreach (var preCId in dto.PrerequisiteCourseIds)
                {
                    _context.CoursePrerequisites.Add(new CoursePrerequisite
                    {
                        CourseId = course.Id,
                        PrerequisiteCourseId = preCId
                    });
                }

                await _context.SaveChangesAsync();
            }

            return Ok(new
            {
                CourseId = course.Id,
                Title = course.Title,
                Status = course.Status,
                Message = AppMessages.CreateSuccess
            });
        }

        [Authorize(Roles = "Instructor,Teacher")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse(int id, CreateCourseDto dto)
        {
            var existingCourse = await _context.Courses
                .Include(c => c.Prerequisites)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (existingCourse == null)
            {
                return BadRequest(AppMessages.NotFound);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!Enum.IsDefined(typeof(CourseStatus), dto.Status))
            {
                return BadRequest(AppMessages.InvalidStatus);
            }

            if (dto.PrerequisiteCourseIds != null && dto.PrerequisiteCourseIds.Any())
            {
                if (dto.PrerequisiteCourseIds.Contains(id))
                {
                    return BadRequest(new
                    {
                        Message = "Course cannot be its own prerequisite"
                    });
                }

                dto.PrerequisiteCourseIds = dto.PrerequisiteCourseIds.Distinct().ToList();

                var existingIds = await _context.Courses
                    .Where(c => dto.PrerequisiteCourseIds.Contains(c.Id))
                    .Select(c => c.Id)
                    .ToListAsync();

                var invalidIds = dto.PrerequisiteCourseIds
                    .Except(existingIds)
                    .ToList();
                if (invalidIds.Any())
                {
                    return BadRequest(new
                    {
                        Message = string.Format(AppMessages.PrerequisiteCourseIds, string.Join(", ", invalidIds))
                    });
                }
            }


            existingCourse.Title = dto.Title;
            existingCourse.Description = dto.Description;
            existingCourse.Status = dto.Status;

            var oldPrerequisites = _context.CoursePrerequisites
              .Where(cp => cp.CourseId == id)
              .ToList();

            if (oldPrerequisites.Any())
            {
                _context.CoursePrerequisites.RemoveRange(oldPrerequisites);
            }

            if (dto.PrerequisiteCourseIds != null && dto.PrerequisiteCourseIds.Any())
            {
                var newPrerequisites = dto.PrerequisiteCourseIds
                    .Select(preId => new CoursePrerequisite
                    {
                        CourseId = id,
                        PrerequisiteCourseId = preId
                    }).ToList();

                await _context.CoursePrerequisites
                    .AddRangeAsync(newPrerequisites);
            }


            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = AppMessages.CourseUpdate,

                Course = new
                {
                    Id = existingCourse.Id,
                    Title = existingCourse.Title,
                    Description = existingCourse.Description,
                    Status = existingCourse.Status,

                    PrerequisiteCourseIds = dto.PrerequisiteCourseIds
                }
            });
        }

        [Authorize(Roles = "Instructor,Teacher")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourseById(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Prerequisites)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
            {
                return NotFound(new
                {
                    Message = AppMessages.NotFound
                });
            }

            return Ok(new
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                Status = course.Status,

                PrerequisiteCourseIds = course.Prerequisites
                    .Select(p => p.PrerequisiteCourseId)
                    .ToList()
            });
        }

        [Authorize(Roles = "Instructor,Teacher")]
        [HttpPost("CreateCourseModule")]
        public async Task<IActionResult> AddCourseModule(CreateCourseModuleDto CMod)
        {
            var claimValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(claimValue))
            {
                return Unauthorized();
            }

            int userid = int.Parse(claimValue);
            if (!ModelState.IsValid)
            {
                return BadRequest(AppMessages.Required);
            }

            var course = new Course
            {
                Title = CMod.Title,
                Description = CMod.Description,
                Category = CMod.Category,
                Status = CourseStatus.Draft,
                InstructorId = userid
            };
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            return Ok(new 
            {
                Message = AppMessages.CreateSuccess
            });
        }


        [Authorize(Roles = "Instructor,Teacher")]
        [HttpPut("UpdateCourse/{courseId}")]
        public async Task<IActionResult> UpdateCourse(int courseId, UpdateCourseDto dto)
        {
            var claimValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(claimValue))
            {
                return Unauthorized();
            }

            int userid = int.Parse(claimValue);
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
            {
                return BadRequest(AppMessages.NotFound);
            }
            if (course.InstructorId != userid)
            {
                return StatusCode(403, AppMessages.EditNotAllowed);
            }
            course.Title = dto.Title;
            course.Description = dto.Description;
            await _context.SaveChangesAsync();

            return Ok(course);
        }

        [Authorize(Roles = "Instructor,Teacher")]
        [HttpPatch("{courseId}/publish")]
        public async Task<IActionResult> PublishCourse(int courseId)
        {
            var claimValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(claimValue))
            {
                return Unauthorized();
            }

            int userid = int.Parse(claimValue);

            var course = await _context.Courses.FindAsync(courseId);

            if (course == null)
            {
                return BadRequest(AppMessages.NotFound);
            }

            if (course.InstructorId != userid)
            {
                return StatusCode(403,
                    AppMessages.PublishNotAllowed);
            }
            bool hasModule = await _context.Modules.AnyAsync(m => m.CourseId == courseId);
            if (!hasModule)
            {
                return BadRequest(AppMessages.CheckModuleContainCourseId);
            }
            course.Status = CourseStatus.Published;

            await _context.SaveChangesAsync();

            return Ok(AppMessages.PublishSuccess);
        }

        [Authorize(Roles = "Student")]
        [HttpGet("StudentCourses")]
        public async Task<IActionResult> GetCourseForStudent()
        {
            var course = await _context.Courses.Where(c => c.Status == CourseStatus.Published).Select(c => new
            {
                c.Id,
                c.Title,
                c.Description,
                c.Category,
                c.Status
            }).ToListAsync();

            return Ok(course);
        }

        [Authorize(Roles = "Teacher,Instructor")]
        [HttpGet("Mine")]
        public async Task<IActionResult> GetCourse()
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (user == null)
            {
                return Unauthorized();
            }
            int userId = int.Parse(user);
            var course = await _context.Courses.Where(c => c.InstructorId == userId).ToListAsync();
            return Ok(course);
        }

        [Authorize(Roles = "Instructor,Teacher")]
        [HttpDelete("{courseid}")]
        public async Task<IActionResult> DeleteCourse(int courseid)
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (claim == null)
            {
                return Unauthorized();
            }
            int userid = int.Parse(claim);
            var course = await _context.Courses.FindAsync(courseid);
            if (course == null)
            {
                return BadRequest(AppMessages.NotFound);
            }
            if (course.InstructorId != userid)
            {
                return StatusCode(403, AppMessages.DeleteNotAllowed);
            }
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                message = AppMessages.CoureseDeleted
            });
        }

       
    }
}
