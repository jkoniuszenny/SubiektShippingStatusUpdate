using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Dto.DHL
{
    public class DHLJsonDto
    {
        public IEnumerable<ShipmentsDto> Shipments { get; set; }
    }
}
