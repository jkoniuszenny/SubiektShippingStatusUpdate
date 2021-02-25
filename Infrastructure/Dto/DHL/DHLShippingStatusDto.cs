using Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Dto.DHL
{
    public class DHLShippingStatusDto
    {
        public string TrackingNumber { get; set; }
        public DHLStatus ActualStatus { get; set; }
    }
}
