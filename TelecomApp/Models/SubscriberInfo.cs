using System;
using System.Collections.Generic;

namespace TelecomApp.Models;

public partial class SubscriberInfo
{
    public int SubscriberId { get; set; }

    public string SubscriberFullName { get; set; } = null!;

    public string? HomeAddress { get; set; }

    public string? PassportData { get; set; }

    public int ContractId { get; set; }

    public DateOnly ContractDate { get; set; }

    public DateOnly? ContractEndDate { get; set; }

    public string PhoneNumber { get; set; } = null!;

    public string TariffName { get; set; } = null!;
}
