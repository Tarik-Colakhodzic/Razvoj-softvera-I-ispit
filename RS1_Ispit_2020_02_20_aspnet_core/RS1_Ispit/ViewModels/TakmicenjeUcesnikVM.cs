using Microsoft.AspNetCore.Mvc.Rendering;
using RS1_Ispit_asp.net_core.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RS1_Ispit_asp.net_core.ViewModels
{
    public class TakmicenjeUcesnikVM
    {
        public int UcesnikId { get; set; }
        public List<SelectListItem> Ucesnici { get; set; }
        public string Ucesnik { get; set; }
        public int? Bodovi { get; set; }
        public bool Uredi { get; set; } = false;
        public int TakmicenjeId { get; set; }
    }
}
