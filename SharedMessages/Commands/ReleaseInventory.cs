﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedMessages.Commands
{
    public record ReleaseInventory(Guid OrderId, List<OrderItem> Items);
}
