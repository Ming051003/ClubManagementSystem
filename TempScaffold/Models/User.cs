using System;
using System.Collections.Generic;

namespace TempScaffold.Models;

public partial class User
{
    public int UserId { get; set; }

    public string? StudentId { get; set; }

    public string? UserName { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Role { get; set; } = null!;

    public int? ClubId { get; set; }

    public DateOnly? JoinDate { get; set; }

    public bool? Status { get; set; }

    public virtual Club? Club { get; set; }

    public virtual ICollection<EventParticipant> EventParticipants { get; set; } = new List<EventParticipant>();

    public virtual ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();
}
