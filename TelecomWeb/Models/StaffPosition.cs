﻿using System;
using System.Collections.Generic;

namespace TelecomWeb.Models;

public partial class StaffPosition
{
    public int PositionId { get; set; }

    public string PositionName { get; set; } = null!;

    public virtual ICollection<Staff> Staff { get; set; } = new List<Staff>();
}
