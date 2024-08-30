namespace mvcmvc.Models
{
    public class Blog
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? Director { get; set; }
        public string? Genre { get; set; }

        public DateTime PublishDate { get; set; }


        public string? ImagePath { get; set; } // Resim URL'si
    }
}
