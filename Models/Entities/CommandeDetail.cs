using System.ComponentModel.DataAnnotations.Schema;

namespace WatchesEcommerce.Models.Entities
{
    public class CommandeDetail
    {
        public int id { get; set; }
        [Column("cmd_id")]
        public int commandeId { get; set; }
        [Column("prod_id")]
        public int watchId { get; set; }
        [Column("color_id")]
        public int colorId { get; set; }
        public int quantity { get; set; }
    }
}
