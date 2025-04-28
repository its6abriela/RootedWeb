using System.ComponentModel.DataAnnotations;

namespace RootedWeb.Models
{
    public class MoodLog
    {
        [Key]
        public int LogID { get; set; }

        [Required]
        public string? Mood { get; set; }

        public int EnergyLevel { get; set; }

        public DateTime EntryDate { get; set; }

        public int UserID { get; set; }

        public User? User { get; set; }
    }
}
