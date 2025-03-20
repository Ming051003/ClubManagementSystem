using System;
using System.Collections.Generic;

namespace TempScaffold.Models;

public partial class Report
{
    public int ReportId { get; set; }

    public int ClubId { get; set; }

    public int SemesterId { get; set; }

    public string? MemberChanges { get; set; }

    public string? EventSummary { get; set; }

    public string? ParticipationStats { get; set; }

    public string ReportStatus { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public virtual Club Club { get; set; } = null!;

    public virtual Semester Semester { get; set; } = null!;
}
