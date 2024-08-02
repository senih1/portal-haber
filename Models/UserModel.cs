using System.ComponentModel.DataAnnotations;

namespace portal_haber.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
