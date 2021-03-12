using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Enums
{
    public enum DHLStatus
    {
        unknown = 0,
        pretransit = 1,
        transit = 2,
        delivered = 3,
        failure = 4,
        indelivery = 5
    }
}
