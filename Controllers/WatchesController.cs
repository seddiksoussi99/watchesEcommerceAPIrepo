using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WatchesEcommerce.Data;
using WatchesEcommerce.Models;
using WatchesEcommerce.Models.Entities;

namespace WatchesEcommerce.Controllers
{
    [Route("api/Watches")]
    [ApiController]
    public class WatchesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public WatchesController(AppDbContext context) 
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetWatches([FromQuery] int? categoryId, [FromQuery] string searchPattern = "")
        {
            var result = _context.Watches
                .Include(w => w.Category).Include(w => w.Colors)
                .Include(w => w.Images).AsQueryable();


            if (categoryId.HasValue && categoryId != 0)
            {
                result = result.Where(w => w.CategoryId == categoryId);
            }

            if(searchPattern.Length > 2)
            {
                result = result.Where(w => w.Name.Contains(searchPattern));
            }
            return Ok(result.Select(w => ToWatchView(w)).ToList());
        }

        [HttpGet]
        [Route("{id:int}")]
        public IActionResult GetWatchById(int id) 
        {
            try
            {
                var watch = _context.Watches
                .Include(w => w.Category).Include(w => w.Colors)
                .Include(w => w.Images)
                .Single(w => w.Id == id);
                if (watch is null) return NotFound();

                return Ok(ToWatchView(watch));
            }
            catch
            {
                return BadRequest();
            }
            
        }

        [HttpGet]
        [Route("relateditems/{itemid:int}")]
        public IActionResult GetRelatedItems(int itemid)
        {
            try
            {
                var categoryId = _context.Watches.Single(w => w.Id == itemid).CategoryId;

                var relatedItems = _context.Watches.Include(w => w.Images)
                    .Where(w => w.CategoryId == categoryId && w.Id != itemid).Select(w => ToWatchView(w));

                return Ok(relatedItems);
            }
            catch
            {
                return BadRequest();
            }

        }

        [HttpPost]
        public IActionResult CreateWatch(Watch watch)
        {
            watch.Colors = _context.Colors.Where(c => watch.Colors.Select(c => c.Name).Contains(c.Name)).ToList();
            watch.Category = _context.Categories.Single(c => c.Name == watch.Category.Name);
            foreach(var img in watch.ImagesForms)
            {
                using (Stream stream = new FileStream(Path.Combine(@"uploads", img.FileName), FileMode.Create))
                {
                    img.CopyTo(stream);
                }
            }
            
            _context.Watches.Add(watch);
            _context.SaveChanges();

            watch.Images = watch.ImagesForms.Select(i => new Image
            {
                Name = i.FileName,
                WatchId = watch.Id

            }).ToList();
            _context.SaveChanges();
            var result = ToWatchView(watch);
            return Ok(result);
        }

        [HttpPut]
        [Route("{Id:int}")]
        public IActionResult UpdateWatch(int Id, Watch newWatch) 
        {
            var watch = _context.Watches.Find(Id);
            if (watch == null) return NotFound();

            if (newWatch.Name != null) 
                watch.Name = newWatch.Name;
            if (newWatch.Description != null) 
                watch.Description = newWatch.Description;
            if (newWatch.Colors != null) 
                watch.Colors = _context.Colors.Where(c => newWatch.Colors.Select(c => c.Name).Contains(c.Name)).ToList();
            if (newWatch.Category != null) 
                watch.Category = _context.Categories.Single(c => c.Name == newWatch.Category.Name);

            _context.SaveChanges();
            var result = ToWatchView(watch);
            return Ok(result);

        }

        [HttpDelete]
        [Route("{Id:int}")]
        public IActionResult DeleteWatch(int Id)
        {
            var watch = _context.Watches.Find(Id);
            if (watch == null) return NotFound();
            _context.Watches.Remove(watch);
            _context.SaveChanges();

            var result = ToWatchView(watch);
            return Ok(result);
        }

        private static object ToWatchView(Watch watch)
        {
            var result = new
            {
                id = watch.Id,
                name = watch.Name,
                price = watch.Price,
                description = watch.Description,
                category = watch.Category,
                colors = watch.Colors.Select(c => c.Name).ToList(),
                images = watch.Images.Select(i => i.Name).ToList()
            };

            result.colors.Sort();
            result.images.Sort();
            return result;
        }
    }
}
