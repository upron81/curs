using System;
using System.Collections.Generic;

namespace TelecomApp.Models;

public partial class TariffPlan
{
    public int TariffPlanId { get; set; }

    public string TariffName { get; set; } = null!;

    public decimal SubscriptionFee { get; set; }

    public decimal LocalCallRate { get; set; }

    public decimal LongDistanceCallRate { get; set; }

    public decimal InternationalCallRate { get; set; }

    public bool IsPerSecond { get; set; }

    public decimal SmsRate { get; set; }

    public decimal MmsRate { get; set; }

    public decimal DataRatePerMb { get; set; }

    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();
}
