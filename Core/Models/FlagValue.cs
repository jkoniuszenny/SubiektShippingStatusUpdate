using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Models
{
    [Table("fl_Wartosc", Schema = "dbo")]
    public class FlagValue
    {
 
        public int flw_IdGrupyFlag { get; set; }
        public int flw_TypObiektu { get; set; }
        public int flw_IdObiektu { get; set; }
        public int? flw_IdFlagi { get; set; }
        public string flw_Komentarz { get; set; }
        public int flw_IdUzytkownika { get; set; }
        public DateTime flw_CzasOstatniejZmiany { get; set; }
    }
}
