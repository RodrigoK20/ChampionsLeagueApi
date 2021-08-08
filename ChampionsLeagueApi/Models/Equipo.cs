using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChampionsLeagueApi.Models
{
    public class Equipo
    {
        public int id_equipo { get; set; }

        public string nombre_equipo { get; set; }

        public string logo { get; set; }

        public int puntaje { get; set; }

        public string estado { get; set; }
    }
}
