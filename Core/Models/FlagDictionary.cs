using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Models
{
    [Table("fl__Flagi", Schema = "dbo")]
    public class FlagDictionary
    {
        public int flg_Id { get; set; }
        public int flg_Numer { get; set; }
        public string flg_Text { get; set; }
        public int flg_IdGrupy { get; set; }
        public bool flg_Domyslna { get; set; }
        public string flg_DomyslnyKomentarz { get; set; }
        public int? flg_IdUprNadaj { get; set; }
        public int? flg_IdUprZdejmij { get; set; }
    }
}
