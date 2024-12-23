﻿namespace TelecomWeb.Models;

public partial class Subscriber
{
    public int SubscriberId { get; set; }

    public string FullName { get; set; } = null!;

    public string? HomeAddress { get; set; }

    public string? PassportData { get; set; }

    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();
}
