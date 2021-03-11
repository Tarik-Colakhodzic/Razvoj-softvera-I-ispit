using Microsoft.AspNetCore.Mvc.Rendering;
using RS1_Ispit_asp.net_core.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RS1_Ispit_asp.net_core.ViewModels
{
    public class TakmicenjeIndexVM
    {
        public int SkolaFilterId { get; set; }
        public int PredmetFilterId { get; set; }
        public List<SelectListItem> Skole { get; set; }
        public List<SelectListItem> Predmeti { get; set; }
        public List<Zapis> Zapisi { get; set; }
        public class Zapis
        {
            public Takmicenje Takmicenje { get; set; }
            public string NajboljiUcesnik { get; set; }
        }
    }
}
