using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Infrastructure.Dto.GLS
{
    public class StatusBarDto
    {
        public string Status { get; set; }
        public string StatusText { get; set; }
        public string ImageStatus { get; set; }
        public string ImageText { get; set; }
    }
}
