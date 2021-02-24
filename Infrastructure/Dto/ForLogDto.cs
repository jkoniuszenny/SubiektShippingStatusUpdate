using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Dto
{
    public class ForLogDto
    {
        public int DocumentId { get; set; }
        public int OldFlag { get; set; }
        public int NewFlag { get; set; }
        public string Operation { get; set; }

        public override string ToString()
        {
            return $"Dokument: {DocumentId}, stara flaga: {OldFlag}, nowa flaga: {NewFlag}, wykonana operacja: {Operation}  ------ {DateTime.Now}";
        }
    }
}
