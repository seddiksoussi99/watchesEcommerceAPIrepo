using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WatchesEcommerce.Data;
using WatchesEcommerce.Models.Entities;

namespace WatchesEcommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly AppDbContext _context;
        public CustomersController(AppDbContext context) 
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var result = _context.Customers;
                return Ok(result);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("{id:int}")]
        public IActionResult Get(int id) 
        {
            try
            {
                var result = _context.Customers.Find(id);
                return Ok(result);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public IActionResult CreateCustomer(Customer customer) 
        {
            try
            {
                _context.Customers.Add(customer);
                _context.SaveChanges();
                return Ok(customer);
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
