using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChampionsLeagueApi.Models
{
    public class PartidosTorneo
    {
        public int id_partido_x_torneo { get; set; }
        
        public int estadio_id { get; set; }

        public int torneo_id { get; set; }

        public string equipo_a { get; set; }

        public string equipo_b { get; set; }

        public DateTime fecha_partido { get; set; }

        public string hora { get; set; }

        public int numero_partido { get; set; }

        public int resultado_a { get; set; }

        public int resultado_b { get; set; }

        public string estado_partido { get; set; }

    }
}
