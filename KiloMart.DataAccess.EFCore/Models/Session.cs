using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class Session
{
    public long Id { get; set; }

    public string Token { get; set; } = null!;

    public int UserId { get; set; }

    public DateTime ExpireDate { get; set; }

    public DateTime CreationDate { get; set; }

    public string Code { get; set; } = null!;

    public virtual MembershipUser User { get; set; } = null!;
}
