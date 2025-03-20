using System;
using System.Collections.Generic;

namespace Model.Models;

public partial class UserView
{
    public int UserId { get; set; }

    public string? StudentId { get; set; }

    public string UserName { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Role { get; set; } = null!;

    public int? ClubId { get; set; }
    public string ClubName { get; set; } = null!;
    public DateOnly JoinDate { get; set; }

    public bool Status { get; set; }


 
}
