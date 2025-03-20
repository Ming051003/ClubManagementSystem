using System;
using System.Collections.Generic;

namespace TempScaffold.Models;

public partial class EventParticipant
{
    public int EventParticipantId { get; set; }

    public int EventId { get; set; }

    public int UserId { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? RegistrationDate { get; set; }

    public virtual Event Event { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
