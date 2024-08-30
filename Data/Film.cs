using System.ComponentModel.DataAnnotations;

namespace mvcmvc.Data
{
    public class Film
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? Genre { get; set; }
        public string? Director { get; set; }
        public DateTime PublishDate { get; set; }

        public string? PosterPath { get; set; } // Resim URL'si
    }
}
