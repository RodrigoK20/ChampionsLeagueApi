using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChampionsLeagueApi.Models
{
    public class Jugador
    {
        public int id_jugador { get; set; }

        public string nombre { get; set; }

        public DateTime fecha_nac { get; set; }

        public string fotografia { get; set; }

        public int puntos { get; set; }

        public int dorsal { get; set; }

        public int equipo_id { get; set; }

        public string posicion { get; set; }
    }
}
