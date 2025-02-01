using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class Customer
{
    public int Party { get; set; }

    public virtual ICollection<Card> Cards { get; set; } = new List<Card>();

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual CustomerProfile? CustomerProfile { get; set; }

    public virtual ICollection<FavoriteProduct> FavoriteProducts { get; set; } = new List<FavoriteProduct>();

    public virtual ICollection<OrderCustomerInformation> OrderCustomerInformations { get; set; } = new List<OrderCustomerInformation>();

    public virtual Party PartyNavigation { get; set; } = null!;
}
