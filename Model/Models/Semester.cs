using System;
using System.Collections.Generic;

namespace Model.Models;

public partial class Semester
{
    public int SemesterId { get; set; }

    public string SemesterName { get; set; } = null!;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();
}
