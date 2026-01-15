using System.Linq;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WatchesEcommerce.Data;
using WatchesEcommerce.Models;
using WatchesEcommerce.Models.DTOs;
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
        public IActionResult GetWatches([FromQuery] int? categoryId, [FromQuery] string searchPattern = "", [FromQuery] int pageNb = 1)
        {
            const int pageSize = 2;
            
            try
            {
                var items = _context.Watches
                .Include(w => w.Category).Include(w => w.Colors)
                .Include(w => w.Images).AsQueryable();


                if (categoryId.HasValue && categoryId != 0)
                {
                    items = items.Where(w => w.CategoryId == categoryId);
                }

                if (searchPattern.Length > 2)
                {
                    items = items.Where(w => w.Name.Contains(searchPattern));
                }

                var numberOfPages = items.Count() / pageSize;

                items = items.Skip(pageSize * (pageNb-1)).Take(pageSize);

                var watches = items.Select(w => new WatchDto
                {
                    id = w.Id,
                    name = w.Name,
                    price = w.Price,
                    description = w.Description,
                    Category = w.Category,
                    colors = w.Colors.Select(c => c.Name).ToList(),
                    images = w.Images.Select(c => c.Name).ToList()
                }).ToList();
                foreach (var item in watches)
                {
                    item.colors.Sort();
                    item.images.Sort();
                }
                int next = pageNb+1;
                int previous = pageNb-1;
                var result = new WatchesListDto()
                {
                    watches = watches,
                    count = watches.Count,
                    currentPageNb = pageNb,
                    numberOfPages = numberOfPages,
                };

                if (next <= numberOfPages)
                    result.next = $"/Watches?pageNb={next}";

                if(previous >= 1)
                    result.previous = $"/Watches?pageNb={previous}";

                return Ok(result);
            }
            catch
            {
                return BadRequest();
            }
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

                var result = new WatchDto()
                {
                    id = watch.Id,
                    name = watch.Name,
                    price = watch.Price,
                    description = watch.Description,
                    Category = watch.Category,
                    colors = watch.Colors.Select(c => c.Name).ToList(),
                    images = watch.Images.Select(c => c.Name).ToList(),
                };
                result.colors.Sort();
                result.images.Sort();

                return Ok(result);
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
                    .Where(w => w.CategoryId == categoryId && w.Id != itemid)
                    .Select(w => new WatchDto(){
                        id = w.Id,
                        name = w.Name,
                        price = w.Price,
                        description = w.Description,
                        Category = w.Category,
                        colors = w.Colors.Select(c => c.Name).ToList(),
                        images = w.Images.Select(c => c.Name).ToList(),
                    });
                foreach(var item in relatedItems)
                {
                    item.colors.Sort();
                    item.images.Sort();
                }

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
            try
            {
                watch.Colors = _context.Colors.Where(c => watch.Colors.Select(c => c.Name).Contains(c.Name)).ToList();
                watch.Category = _context.Categories.Single(c => c.Name == watch.Category.Name);
                foreach (var img in watch.ImagesForms)
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
            catch
            {
                return BadRequest();
            }
            
        }

        [HttpPut]
        [Route("{Id:int}")]
        public IActionResult UpdateWatch(int Id, Watch newWatch) 
        {
            try
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
            }catch{
                return BadRequest();
            }
            

        }

        [HttpDelete]
        [Route("{Id:int}")]
        public IActionResult DeleteWatch(int Id)
        {
            try
            {
                var watch = _context.Watches.Find(Id);
                if (watch == null) return NotFound();
                _context.Watches.Remove(watch);
                _context.SaveChanges();

                var result = ToWatchView(watch);
                return Ok(result);
            }
            catch
            {
                return BadRequest();
            }
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
