using FirstCoreWebApp.Model;
using FirstCoreWebApp.Model.Lesson;
using FirstCoreWebApp.Model.Material;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FirstCoreWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialController : ControllerBase
    {

        private readonly AppDbContext _context;

        public MaterialController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Instructor,Teacher")]
        [HttpPost("{courseId}/materials")]
        public async Task<IActionResult> UploadMaterial(int courseId, [FromForm] CreateMaterialDto dto)
        {
            if (dto.File == null || dto.File.Length == 0)
            {
                return BadRequest("File required");
            }
            // Allowed file formats
            var allowedExtensions = new[] { ".pdf", ".txt", ".mp4" };

            var extension = Path
                .GetExtension(dto.File.FileName)
                .ToLower();

            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest(AppMessages.CheckFormat);
            }

            if (dto.File.Length > 5 * 1024 * 1024)
            {
                return BadRequest(AppMessages.CheckFileSize);
            }

            var uploadFolder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Upload");

            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            var fileName = Guid.NewGuid() + extension;

            var filePath = Path.Combine(uploadFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.File.CopyToAsync(stream);
            }


            var material = new CourseMaterial
            {
                CourseId = courseId,
                FileName = dto.File.FileName,
                FilePath = filePath,
                ContentType = dto.File.ContentType
            };

            _context.CourseMaterials.Add(material);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                MaterialId = material.Id
            });
        }

        [Authorize(Roles = "Instructor,Teacher")]
        [HttpGet("{courseId}/materials")]
        public async Task<IActionResult> GetMaterials(int courseId)
        {
            var materials = await _context.CourseMaterials
                .Where(m => m.CourseId == courseId)
                .Select(m => new
                {
                    m.Id,
                    m.FileName,
                    m.ContentType
                })
                .ToListAsync();

            return Ok(materials);
        }

        [Authorize(Roles = "Instructor,Teacher")]
        [HttpPut("{courseId}/materials/{materialId}")]
        public async Task<IActionResult> UpdateMaterial(int courseId, int materialId, [FromForm] UpdateMaterialDto dto)
        {
            var material = await _context.CourseMaterials
                .FirstOrDefaultAsync(x => x.Id == materialId && x.CourseId == courseId);

            if (material == null)
            {
                return NotFound(new
                {
                    Message = "Material not found"
                });
            }

            string? oldFilePath = material.FilePath;

            if (dto.File != null && dto.File.Length > 0)
            {
                var allowedExtensions = new[] { ".pdf", ".txt", ".mp4" };

                var extension = Path.GetExtension(dto.File.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    return BadRequest(AppMessages.CheckFormat);
                }

                if (dto.File.Length > 5 * 1024 * 1024)
                {
                    return BadRequest(AppMessages.CheckFileSize);
                }

                var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "Upload");

                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                var newFileName = Guid.NewGuid() + extension;
                var newFilePath = Path.Combine(uploadFolder, newFileName);

                using (var stream = new FileStream(newFilePath, FileMode.Create))
                {
                    await dto.File.CopyToAsync(stream);
                }

                material.FileName = dto.File.FileName;
                material.FilePath = newFilePath;
                material.ContentType = dto.File.ContentType;
            }

            await _context.SaveChangesAsync();

            if (!string.IsNullOrEmpty(oldFilePath) &&
                System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);
            }

            return Ok(new
            {
                MaterialId = material.Id,
                Message = AppMessages.MaterialUpdated
            });
        }
        [Authorize(Roles = "Instructor,Teacher")]
        [HttpDelete("{courseId}/materials/{materialId}")]
        public async Task<IActionResult> DeleteMaterial(int courseId, int materialId)
        {
            var material = await _context.CourseMaterials
                .FirstOrDefaultAsync(x => x.Id == materialId && x.CourseId == courseId);

            if (material == null)
            {
                return NotFound(new
                {
                    Message = "Material not found"
                });
            }

            var filePath = material.FilePath;

            _context.CourseMaterials.Remove(material);

            await _context.SaveChangesAsync();

            if (!string.IsNullOrEmpty(filePath) &&
                System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
            return Ok(new
            {
                Message = AppMessages.CoureseDeleted
            });
        }


        [Authorize(Roles = "Instructor,Teacher")]
        [HttpPost("{courseId}/lessons")]
        public async Task<IActionResult> CreateLesson(int courseId, CreateLessonDto dto)
        {
            // 1. Validation
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // 2. Check Course Exists
            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null)
            {
                return NotFound("Course not found");
            }

            if (dto.CourseMaterialIds != null && dto.CourseMaterialIds.Any())
            {
                var existingMaterialIds = await _context.CourseMaterials
                    .Where(cm => dto.CourseMaterialIds.Contains(cm.Id))
                    .Select(cm => cm.Id)
                    .ToListAsync();

                var invalidIds = dto.CourseMaterialIds
                    .Except(existingMaterialIds)
                    .ToList();

                if (invalidIds.Any())
                {
                    return BadRequest(new
                    {
                        Message = string.Format(AppMessages.CourseMaterialIds, string.Join(", ", invalidIds))
                    });
                }
            }

            var lesson = new Lesson
            {
                Title = dto.Title,
                Content = dto.Content,
                CourseId = courseId
            };

            _context.Lessons.Add(lesson);

            await _context.SaveChangesAsync();

            if (dto.CourseMaterialIds != null && dto.CourseMaterialIds.Any())
            {
                foreach (var materialId in dto.CourseMaterialIds)
                {
                    var lessonMaterial = new LessonMaterial
                    {
                        LessonId = lesson.Id,
                        CourseMaterialId = materialId
                    };

                    _context.LessonMaterials.Add(lessonMaterial);
                }

                await _context.SaveChangesAsync();
            }

            // 6. Response
            return Ok(new
            {
                Message = "Lesson created successfully",
                LessonId = lesson.Id,
                lesson.Title,
                lesson.Content,
                lesson.CourseId
            });
        }
    }
}
