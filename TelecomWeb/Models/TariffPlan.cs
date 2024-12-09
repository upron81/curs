using System.ComponentModel.DataAnnotations;

namespace TelecomWeb.Models;

public partial class TariffPlan
{
    public int TariffPlanId { get; set; }

    public string TariffName { get; set; } = null!;

    [Range(0.0, double.MaxValue, ErrorMessage = "Please enter a valid subscription fee.")]
    public decimal SubscriptionFee { get; set; }

    [Range(0.0, double.MaxValue, ErrorMessage = "Please enter a valid local call rate.")]
    public decimal LocalCallRate { get; set; }

    [Range(0.0, double.MaxValue, ErrorMessage = "Please enter a valid long distance call rate.")]
    public decimal LongDistanceCallRate { get; set; }

    [Range(0.0, double.MaxValue, ErrorMessage = "Please enter a valid international call rate.")]
    public decimal InternationalCallRate { get; set; }

    public bool IsPerSecond { get; set; }

    [Range(0.0, double.MaxValue, ErrorMessage = "Please enter a valid SMS rate.")]
    public decimal SmsRate { get; set; }

    [Range(0.0, double.MaxValue, ErrorMessage = "Please enter a valid MMS rate.")]
    public decimal MmsRate { get; set; }

    [Range(0.0, double.MaxValue, ErrorMessage = "Please enter a valid data rate per MB.")]
    public decimal DataRatePerMb { get; set; }

    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();
}
