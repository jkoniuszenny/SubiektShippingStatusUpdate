using Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Dto.GLS
{
    public class GLSShippingStatusDto
    {
        public string TrackingNumber { get; set; }
        public GLSStatus ActualStatus { get; set; }
    }
}
