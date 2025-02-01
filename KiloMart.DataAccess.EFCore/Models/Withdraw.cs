using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class Withdraw
{
    public long Id { get; set; }

    public int Party { get; set; }

    public string BankAccountNumber { get; set; } = null!;

    public string IbanNumber { get; set; } = null!;

    public DateTime Date { get; set; }

    public bool Done { get; set; }

    public bool Accepted { get; set; }

    public bool Rejected { get; set; }

    public virtual Party PartyNavigation { get; set; } = null!;
}
