using System;
using System.Collections.Generic;

namespace Model.Models;

public partial class EventParticipant
{
    public int ParticipantId { get; set; }

    public int EventId { get; set; }

    public int UserId { get; set; }

    public DateOnly RegistrationDate { get; set; }

    public string Status { get; set; } = null!;

    public virtual Event Event { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
