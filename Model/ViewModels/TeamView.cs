using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModels
{
    public class TeamView
    {
        public int TeamId { get; set; }
        public string TeamName { get; set; }
        public string Description { get; set; }
        public string ClubName { get; set; }
        public int ClubId { get; set; }
        public int LeaderId { get; set; }
        public string Leader { get; set; }
    }
}
