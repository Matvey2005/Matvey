using System.ComponentModel.DataAnnotations;

namespace Курсач_1.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(30)]
        public string UserName { get; set; }

        [Required]
        [StringLength(150)]
        public string Password { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public bool IsSendEmail { get; set; }

        public List<Event> Events { get; set; } = new List<Event>();
    }
}
