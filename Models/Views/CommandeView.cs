namespace WatchesEcommerce.Models.Views
{
    public class CommandeView
    {
        public int customerId { get; set; }
        public List<CommandeDetailView> commandeDetails { get; set; } = new List<CommandeDetailView>();

    }
}
