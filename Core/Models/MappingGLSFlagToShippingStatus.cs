using Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class MappingGLSFlagToShippingStatus
    {
        public int FlagId { get; set; }
        public int FlagGroup { get; set; }
        public GLSStatus GLSStatus { get; set; }
    }
}
