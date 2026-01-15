namespace WatchesEcommerce.Models.DTOs
{
    public class WatchesListDto
    {
        public List<WatchDto> watches { get; set; }
        public string? next {  get; set; }
        public string? previous { get; set; }
        public int count { get; set; }
        public int currentPageNb { get; set; }
        public int numberOfPages { get; set; }
    }
}
