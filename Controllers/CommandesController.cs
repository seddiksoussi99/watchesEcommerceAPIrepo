using System.Drawing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WatchesEcommerce.Data;
using WatchesEcommerce.Models.Entities;
using WatchesEcommerce.Models.Views;

namespace WatchesEcommerce.Controllers
{
    [Route("api/Commandes")]
    [ApiController]
    public class CommandesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public CommandesController(AppDbContext context) {
            _context = context;
        }


        /* Get Commandes Json Format
        [
            {
            "id": 9,
            "cmd_date": "2025-12-05T00:21:30.183",
            "customer": {
                "id": 2,
                "first_name": "Med.Seddik",
                "last_name": "Soussi",
                "phone": "26451322",
                "city": "Tunis",
                "address": "Oued Ellil"
            },
            "commandeDetails": [
                {
                    "id": 3,
                    "watchId": 72,
                    "colorId": 33,
                    "quantity": 1
                }
            ]
            }
        ]
         */

        [HttpGet]
        public IActionResult Get([FromQuery] int customer_id) {
            try
            {
                var result = _context.Commandes.
                    Include(c => c.customer).Include(c => c.commandeDetails).AsQueryable();
                if(customer_id != 0)
                {
                    result = result.Where(c => c.customerId == customer_id).AsQueryable();
                }
                var cleanResult = result.Select(c => new
                {
                    c.id,
                    c.cmd_date,
                    c.customer,
                    commandeDetails = c.commandeDetails.Select(cd => new
                    {
                        cd.id,
                        cd.watchId,
                        cd.colorId,
                        cd.quantity

                    })

                }).AsQueryable();
                return Ok(cleanResult.ToArray());
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /* Post Commande Json Format
         {
            "customerId": 2,
            "commandeDetails": [
                {
                    "watchId": 72,
                    "colorId": 33,
                    "quantity": 1
                }
            ]
        }
         */
        [HttpPost]
        public IActionResult CreateCommande([FromBody] CommandeView data)
        {
            try
            {
                var cmd = new Commande()
                {
                    cmd_date = DateTime.Now,
                    customerId = data.customerId
                };
                _context.Commandes.Add(cmd);
                _context.SaveChanges();

                List<CommandeDetail> details = new();
                foreach(var cd in data.commandeDetails)
                {
                    details.Add(new CommandeDetail()
                    {
                        watchId = cd.watchId,
                        quantity = cd.quantity,
                        colorId = _context.Colors.Where(c => c.Name == cd.color).Select(c => c.Id).Single(),
                        commandeId = cmd.id
                    });
                }
                cmd.commandeDetails = details;
                _context.SaveChanges();

                return Ok(data);
            }
            catch
            {
                return BadRequest();
            }
        }


        /* Changing The Commande Status Json Format (Put)
          
        {
            "cmd_id" : 2,
            "terminated" : true
        }

         */
        [HttpPut]
        public IActionResult ChangeStatus(CmdStatusView cmdStatus)
        {
            try
            {
                var cmd = _context.Commandes.Single(c => c.id == cmdStatus.cmd_id);
                cmd.terminated = cmdStatus.terminated;
                _context.SaveChanges();
                return Ok(cmd);
            }
            catch
            {
                return BadRequest();
            }
        }

    }
}
