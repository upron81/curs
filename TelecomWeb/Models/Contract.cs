using System;
using System.Collections.Generic;

namespace TelecomWeb.Models;

public partial class Contract
{
    public int ContractId { get; set; }

    public int SubscriberId { get; set; }

    public int TariffPlanId { get; set; }

    public DateOnly ContractDate { get; set; }

    public DateOnly? ContractEndDate { get; set; }

    public string PhoneNumber { get; set; } = null!;

    public int StaffId { get; set; }

    public virtual ICollection<Call> Calls { get; set; } = new List<Call>();

    public virtual ICollection<InternetUsage> InternetUsages { get; set; } = new List<InternetUsage>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual Staff Staff { get; set; } = null!;

    public virtual Subscriber Subscriber { get; set; } = null!;

    public virtual TariffPlan TariffPlan { get; set; } = null!;
}
