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

        public List<Event> Events { get; set; } = new List<Event>();
    }
}
