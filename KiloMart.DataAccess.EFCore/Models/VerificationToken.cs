using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class VerificationToken
{
    public int Id { get; set; }

    public int MembershipUser { get; set; }

    public string Value { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual MembershipUser MembershipUserNavigation { get; set; } = null!;
}
