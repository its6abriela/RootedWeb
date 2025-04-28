using System.ComponentModel.DataAnnotations;

namespace RootedWeb.Models
{
    public class Streak
    {
        [Key]
        public int StreakID { get; set; }

        public string? LastCompletionDate { get; set; }

        public int CurrentStreak { get; set; }

        public int UserID { get; set; } 

        public User? User { get; set; } 

        public DateTime Date { get; set; }




    }
}
