using FirstCoreWebApp.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FirstCoreWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModuleController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ModuleController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("AddModule")]
        public async Task<IActionResult> AddModule(CreateModuleDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(AppMessages.Required);
                }

                var module = new Module
                {
                    CourseId = dto.CourseId,
                    Title = dto.Title
                };

                _context.Modules.Add(module);

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Message = AppMessages.CreateSuccess
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
