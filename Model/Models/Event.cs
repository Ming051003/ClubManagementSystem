using System;
using System.Collections.Generic;

namespace Model.Models;

public partial class Event
{
    public int EventId { get; set; }

    public string EventName { get; set; } = null!;

    public string? Description { get; set; }

    public DateOnly EventDate { get; set; }

    public string? Location { get; set; }

    public int ClubId { get; set; }

    public int? Capacity { get; set; }

    public string Status { get; set; } = null!;

    public virtual Club Club { get; set; } = null!;

    public virtual ICollection<EventParticipant> EventParticipants { get; set; } = new List<EventParticipant>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
