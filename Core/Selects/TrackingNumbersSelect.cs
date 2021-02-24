using Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Selects
{
    public class TrackingNumbersSelect
    {
        public int? dok_Id { get; set; }
        public int? dok_Type { get; set; }
        public string GLS { get; set; }
        public string  DHL { get; set; }
        public int? flw_IdGrupyFlag { get; set; }
        public int? flw_IdFlagi { get; set; }
        public GLSStatus GLSStatus { get; set; }
        //public string flg_Text { get; set; }

    }
}
