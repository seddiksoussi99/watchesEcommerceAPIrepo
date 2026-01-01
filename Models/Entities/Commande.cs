using System.ComponentModel.DataAnnotations.Schema;

namespace WatchesEcommerce.Models.Entities
{
    public class Commande
    {
        public int id { get; set; }
        public DateTime cmd_date { get; set; }
        [Column("customer_id")]
        public int customerId { get; set; }
        public bool terminated { get; set; } = false;
        public virtual Customer customer { get; set; }
        public virtual ICollection<CommandeDetail> commandeDetails { get; set; } = new List<CommandeDetail>();

    }
}
