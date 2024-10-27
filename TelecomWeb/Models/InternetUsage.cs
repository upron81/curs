using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TelecomWeb.Models;

public partial class InternetUsage
{
    public int UsageId { get; set; }

    public int ContractId { get; set; }

    public DateTime UsageDate { get; set; }

    [Range(0.0, double.MaxValue, ErrorMessage = "Please enter a valid data sent value.")]
    public decimal DataSentMb { get; set; }

    [Range(0.0, double.MaxValue, ErrorMessage = "Please enter a valid data receive value.")]
    public decimal DataReceivedMb { get; set; }

    public virtual Contract? Contract { get; set; } = null!;
}
