using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedMessages.Events
{
    public record OrderFailed(Guid OrderId, string Reason);
}
