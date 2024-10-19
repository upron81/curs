using System;
using System.Collections.Generic;

namespace TelecomWeb.Models;

public partial class InternetUsage
{
    public int UsageId { get; set; }

    public int ContractId { get; set; }

    public DateTime UsageDate { get; set; }

    public decimal DataSentMb { get; set; }

    public decimal DataReceivedMb { get; set; }

    public virtual Contract Contract { get; set; } = null!;
}
