using System;

namespace RootedWeb.Models
{
    public class TreePlanting
    {
        public int TreePlantingID { get; set; }
        public int UserID { get; set; }
        public DateTime DatePlanted { get; set; }
        public string? Region { get; set; }  // like amazon,india or something
    }
}
