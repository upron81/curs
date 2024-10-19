using System;
using System.Collections.Generic;

namespace TelecomWeb.Models;

public partial class ContractMessage
{
    public int ContractId { get; set; }

    public string PhoneNumber { get; set; } = null!;

    public int MessageId { get; set; }

    public DateTime MessageDate { get; set; }

    public bool IsMms { get; set; }
}
