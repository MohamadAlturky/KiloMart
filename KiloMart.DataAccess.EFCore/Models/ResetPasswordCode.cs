using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class ResetPasswordCode
{
    public long Id { get; set; }

    public string Code { get; set; } = null!;

    public int MembershipUser { get; set; }

    public DateTime Date { get; set; }

    public virtual MembershipUser MembershipUserNavigation { get; set; } = null!;
}
