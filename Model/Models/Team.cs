using System;
using System.Collections.Generic;

namespace Model.Models;

public partial class Team
{
    public int TeamId { get; set; }

    public string TeamName { get; set; } = null!;

    public string? Description { get; set; }

    public int ClubId { get; set; }

    public int? TeamLeaderId { get; set; }

    public virtual Club Club { get; set; } = null!;

    public virtual User? TeamLeader { get; set; }

    public virtual ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();
}
