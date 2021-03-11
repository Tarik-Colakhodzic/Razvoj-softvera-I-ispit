using RS1_Ispit_asp.net_core.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RS1_Ispit_asp.net_core.ViewModels
{
    public class TakmicenjeDodajVM
    {
        public Skola Skola { get; set; }
        public Predmet Predmet { get; set; }
        public DateTime Datum { get; set; }
    }
}
