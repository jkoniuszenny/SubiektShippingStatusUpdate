using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Infrastructure.Dto.GLS
{
    public class ProgressBarDto
    {
        public int Level { get; set; }
        public string StatusInfo { get; set; }
        public IEnumerable<StatusBarDto> StatusBar { get; set; }
        public string StatusText { get; set; }
        public int ColourIndex { get; set; }
    }
}
