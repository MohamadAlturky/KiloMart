using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class Role
{
    public byte Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<MembershipUser> MembershipUsers { get; set; } = new List<MembershipUser>();
}
