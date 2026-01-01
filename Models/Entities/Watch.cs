using System.ComponentModel.DataAnnotations.Schema;

namespace WatchesEcommerce.Models.Entities
{
    public class Watch
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public  string? Description { get; set; }
        public Decimal Price { get; set; }
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; } = new Category();
        public virtual ICollection<Color> Colors { get; set; } = new List<Color>();
        public virtual ICollection<Image> Images { get; set; } = new List<Image>();
        [NotMapped]
        public ICollection<IFormFile> ImagesForms { get; set; } = new List<IFormFile>();

    }
}
