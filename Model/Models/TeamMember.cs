using System;
using System.Collections.Generic;

namespace Model.Models;

public partial class TeamMember
{
    public int TeamMemberId { get; set; }

    public int TeamId { get; set; }

    public int UserId { get; set; }

    public DateOnly? JoinDate { get; set; }

    public virtual Team Team { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
