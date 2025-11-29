using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Codexly.Models;

public partial class User : IdentityUser<Guid>
{
    public Guid UserId
    {
        get => Id;
        set => Id = value;
    }

    public string Name { get; set; } = null!;

    public string Email
    {
        get => base.Email;
        set => base.Email = value;
    }

    // Alias for PasswordHash
    public string PasswordHash
    {
        get => base.PasswordHash;
        set => base.PasswordHash = value;
    }

    public string? ProfileImageUrl { get; set; }

    public Guid? LevelId { get; set; }

    public int? CurrentPoints { get; set; }

    public DateOnly? LastLoginDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Aichat> Aichats { get; set; } = new List<Aichat>();

    public virtual ICollection<CodeSession> CodeSessions { get; set; } = new List<CodeSession>();

    public virtual ICollection<Email> Emails { get; set; } = new List<Email>();

    public virtual Level? Level { get; set; }

    public virtual ICollection<Note> Notes { get; set; } = new List<Note>();

    public virtual ICollection<Todo> Todos { get; set; } = new List<Todo>();

    public virtual ICollection<UserActivity> UserActivities { get; set; } = new List<UserActivity>();
}
