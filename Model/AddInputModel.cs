namespace RentalSite.Pages.Model
{
    public class AddInputModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<IFormFile> ProductPhoto { get; set; } // multiple file uploads
        public string Inclusion { get; set; }
        public string Price { get; set; }
    }
}
