using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace insta_scrape.Classes
{
    public class Phone: BaseModel
    {
        public int Id { get; set; }
        public int SearchId { get; set; }
        public string LinkToItem { get; set; }
        public string ImageLinks { get; set; }
        public long PrekesKodas { get; set; }
        public long Barkodas { get; set; }
        public string GamintojoKodas { get; set; }
        public int Garantija { get; set; }
        public float OldPrice { get; set; }
        public float CurrentPrice { get; set; }
        public string Gamintojas { get; set; }
        public string KorpusoMedziagos { get; set; }
        public float Svoris { get; set; }
        public string PapildomosFunkcijos { get; set; }
        public string Komplektacija { get; set; }
        public string SimJungtis { get; set; }
        public string EkranoIstrizaine { get; set; }
        public string EkranoRaiska { get; set; }
        public string GalineKamera { get; set; }
        public string PriekineKamera { get; set; }
        public string Autofokusavimas { get; set; }
        public int BaterijosTalpa { get; set; }
        public int PokalbioLaikas { get; set; }
        public string VidAtmintis { get; set; }
        public string Navigacija { get; set; }
        public string OS { get; set; }
        public string Branduoliai { get; set; }
        public string TaktinisDaznis { get; set; }
    }
}
