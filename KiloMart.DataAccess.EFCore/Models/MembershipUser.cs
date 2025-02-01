using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class MembershipUser
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public bool EmailConfirmed { get; set; }

    public string PasswordHash { get; set; } = null!;

    public byte Role { get; set; }

    public int Party { get; set; }

    public bool IsActive { get; set; }

    public byte Language { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual Party PartyNavigation { get; set; } = null!;

    public virtual ICollection<ResetPasswordCode> ResetPasswordCodes { get; set; } = new List<ResetPasswordCode>();

    public virtual Role RoleNavigation { get; set; } = null!;

    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();

    public virtual ICollection<VerificationToken> VerificationTokens { get; set; } = new List<VerificationToken>();
}
