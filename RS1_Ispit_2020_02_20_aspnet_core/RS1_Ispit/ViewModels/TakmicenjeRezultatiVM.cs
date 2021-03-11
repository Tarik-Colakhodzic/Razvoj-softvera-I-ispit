using RS1_Ispit_asp.net_core.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RS1_Ispit_asp.net_core.ViewModels
{
    public class TakmicenjeRezultatiVM
    {
        public Takmicenje Takmicenje { get; set; }
        public List<TakmicenjeUcesnik> Ucesnici { get; set; }
    }
}
