using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Курсач_1.Models
{
    public class Event
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(300)]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Time { get; set; }

        public int UserId { get; set; }
        [JsonIgnore]
        public User? User { get; set; }
    }
}
