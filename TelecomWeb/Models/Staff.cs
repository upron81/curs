using System;
using System.Collections.Generic;

namespace TelecomWeb.Models;

public partial class Staff
{
    public int StaffId { get; set; }

    public string FullName { get; set; } = null!;

    public int PositionId { get; set; }

    public string? Education { get; set; }

    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();

    public virtual StaffPosition Position { get; set; } = null!;
}
