using WatchesEcommerce.Models.Entities;

namespace WatchesEcommerce.Models.DTOs
{
    public class WatchDto
    {
        public int id {  get; set; }
        public string name { get; set; }
        public decimal price { get; set; }
        public string description { get; set; }
        public Category Category { get; set; }
        public List<string> colors { get; set; } = new List<string>();
        public List<string> images { get; set; } = new List<string>();

    }
}
