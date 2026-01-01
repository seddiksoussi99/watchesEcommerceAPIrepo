using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WatchesEcommerce.Data;

namespace WatchesEcommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public CategoriesController(AppDbContext context) 
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Get() 
        {
            return Ok(_context.Categories);
        }
    }
}
