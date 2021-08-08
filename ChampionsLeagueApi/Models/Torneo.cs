using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChampionsLeagueApi.Models
{
    public class Torneo
    {
        public int id_torneo { get; set; }

        public string nombre { get; set; }

        public DateTime fecha_inicio { get; set; }

        public DateTime fecha_fin { get; set; }
    }
}
