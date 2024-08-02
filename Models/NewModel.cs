using System.ComponentModel.DataAnnotations;

namespace portal_haber.Models
{
    public class New
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Summary { get; set; }
        [Required]
        public string Contents { get; set; }
        [Required]
        public string Slug { get; set; }
        public IFormFile Image { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
