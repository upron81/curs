namespace TelecomWeb.Models;

public partial class ContractInternetUsage
{
    public int ContractId { get; set; }

    public string PhoneNumber { get; set; } = null!;

    public int UsageId { get; set; }

    public DateTime UsageDate { get; set; }

    public decimal DataSentMb { get; set; }

    public decimal DataReceivedMb { get; set; }
}
