using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChampionsLeagueApi.Models
{
    public class RankingEquipo
    {
        public int torneo_id { get; set; }

        public int equipo_id { get; set; }

        public int partidos_ganados { get; set; }
    }
}
