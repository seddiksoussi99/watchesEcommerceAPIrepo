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
                var customerExists = _context.Customers.
                    Any(c => c.first_name == customer.first_name
                    && c.last_name == customer.last_name
                    && c.phone == customer.phone);
                if(customerExists) 
                {
                    var customerExist = _context.Customers.
                        Single(c => c.first_name == customer.first_name
                        && c.last_name == customer.last_name
                        && c.phone == customer.phone);
                    customerExist.city = customer.city;
                    customerExist.address = customer.address;
                    _context.Customers.Update(customerExist);
                    _context.SaveChanges();

                    return Ok(customerExist);
                }
                else
                {
                    _context.Customers.Add(customer);
                    _context.SaveChanges();

                    return Ok(customer);
                }
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
