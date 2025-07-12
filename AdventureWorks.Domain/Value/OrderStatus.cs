using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureWorks.Domain.Value
{
    public enum OrderStatus : byte
    {
        InProcess = 1,
        Approved = 2,
        Backordered = 3,
        Rejected = 4,
        Shipped = 5,
        Cancelled = 6
    }
}
