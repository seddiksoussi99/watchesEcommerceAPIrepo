using System.ComponentModel.DataAnnotations.Schema;

namespace WatchesEcommerce.Models.Entities
{
    public class Customer
    {
        public int id { get; set; }
        public String first_name { get; set; }
        public String last_name { get; set; }
        public String phone { get; set; }
        public String city { get; set; }
        public String address { get; set; }

    }
}
