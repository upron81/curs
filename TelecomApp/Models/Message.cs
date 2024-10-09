using System;
using System.Collections.Generic;

namespace TelecomApp.Models;

public partial class Message
{
    public int MessageId { get; set; }

    public int ContractId { get; set; }

    public DateTime MessageDate { get; set; }

    public bool IsMms { get; set; }

    public virtual Contract Contract { get; set; } = null!;
}
