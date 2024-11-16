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

    public virtual Party PartyNavigation { get; set; } = null!;

    public virtual Role RoleNavigation { get; set; } = null!;

    public virtual ICollection<VerificationToken> VerificationTokens { get; set; } = new List<VerificationToken>();
}
