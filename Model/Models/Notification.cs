using System;
using System.Collections.Generic;

namespace Model.Models;

public partial class Notification
{
    public int NotificationId { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public int? EventId { get; set; }

    public int? ClubId { get; set; }

    public DateTime CreatedDate { get; set; }

    public bool IsRead { get; set; }

    public int UserId { get; set; }

    public virtual Club? Club { get; set; }

    public virtual Event? Event { get; set; }

    public virtual User User { get; set; } = null!;
}
