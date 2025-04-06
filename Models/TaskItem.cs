using System.ComponentModel.DataAnnotations;

namespace RootedWeb.Models
{
    public class TaskItem
    {
        [Key]
        public int TaskID { get; set; }

        [Required]
        public string Title { get; set; }

        public string Mood { get; set; }


        public bool IsComplete { get; set; } = false;


        public int UserID { get; set; } // foreign key

        public User? User { get; set; } // navigation property 





    }
}
