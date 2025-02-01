using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class Notification
{
    public long Id { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public DateTime Date { get; set; }

    public int ForParty { get; set; }

    public string JsonPayLoad { get; set; } = null!;

    public bool IsRead { get; set; }

    public virtual Party ForPartyNavigation { get; set; } = null!;
}
