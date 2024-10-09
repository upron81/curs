using System;
using System.Collections.Generic;

namespace TelecomApp.Models;

public partial class ContractCall
{
    public int ContractId { get; set; }

    public string PhoneNumber { get; set; } = null!;

    public int CallId { get; set; }

    public DateTime CallDate { get; set; }

    public int CallDuration { get; set; }
}
