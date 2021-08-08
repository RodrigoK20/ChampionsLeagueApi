using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChampionsLeagueApi.Models
{
    public class Estadio
    {
        public int id_estadio { get; set; }

        public string nombre_estadio { get; set; }

        public string pais { get; set; }

        public string ciudad { get; set; }

        public string estado { get; set; }

    }
}
