using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class Party
{
    public int Id { get; set; }

    public bool IsActive { get; set; }

    public string DisplayName { get; set; } = null!;

    public virtual Customer? Customer { get; set; }

    public virtual Delivery? Delivery { get; set; }

    public virtual ICollection<Location> Locations { get; set; } = new List<Location>();

    public virtual ICollection<MembershipUser> MembershipUsers { get; set; } = new List<MembershipUser>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<OrderActivity> OrderActivities { get; set; } = new List<OrderActivity>();

    public virtual ICollection<PhoneNumber> PhoneNumbers { get; set; } = new List<PhoneNumber>();

    public virtual Provider? Provider { get; set; }

    public virtual ICollection<SearchHistory> SearchHistories { get; set; } = new List<SearchHistory>();

    public virtual ICollection<Withdraw> Withdraws { get; set; } = new List<Withdraw>();
}
