using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ChampionsLeagueApi.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace ChampionsLeagueApi.Controllers
{
    // [Route("api/[controller]")]
    [ApiController]
    public class TorneoController : ControllerBase
    {
        public string error_message = "";

        //Read ConnectionString 
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        public TorneoController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }


        [HttpPost]
        [Route("api/Torneo/Add")]
        public JsonResult InsertTorneo([FromBody] Torneo torn)
        {
            //Evaluar modelo
            if (!Validate(torn))
            {
                string errorMessage = "Ingrese correctamente los campos";
                return new JsonResult(errorMessage);
            }

            if (ValidateValue(torn.nombre) == 1)
            {
                string errorMessage = "El nombre del torneo ya se encuentra registrado";
                return new JsonResult(errorMessage);

            }

            else
            {

                string query = @"INSERT INTO dbo.Torneo (nombre,estado,fecha_inicio,fecha_fin) VALUES (@nombre, @estado,@fecha_inicio, @fecha_fin)";

                DataTable table = new DataTable();
                string sqlDataSource = _configuration.GetConnectionString("ChampiosLeagueAppCon");
                SqlDataReader myReader;

                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@nombre", torn.nombre);
                        myCommand.Parameters.AddWithValue("@estado", 1);
                        myCommand.Parameters.AddWithValue("@fecha_inicio", torn.fecha_inicio);
                        myCommand.Parameters.AddWithValue("@fecha_fin", torn.fecha_fin);
                        myReader = myCommand.ExecuteReader();
                        table.Load(myReader);
                        myReader.Close();
                        myCon.Close();
                    }
                }
            }
            return new JsonResult("Agregado correctamente");
        }

        [HttpGet]
        [Route("api/Torneo/Get")]
        public JsonResult GetTorneos()
        {
            string query = @"SELECT id_torneo, nombre, CAST(fecha_inicio AS DATE) as fecha_inicio, fecha_fin, estado FROM dbo.Torneo WHERE estado='1'";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("ChampiosLeagueAppCon");
            SqlDataReader myReader;

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult(table);

        }

        [HttpPost]
        [Route("api/Torneo/AddEquipos")]
        public JsonResult InsertTorneoEquipos([FromBody] List<EquiposTorneo> myJson)
        {

            //Insertando a la tabla EQUIPOS_X_TORNEO
            string sqlDataSource = _configuration.GetConnectionString("ChampiosLeagueAppCon");
            SqlDataReader myReader;

           if( ValidateRankingTorneo(myJson) > 0)
            {
                string errorMessage = "El equipo ya se encuentra registrado en el torneo";
                return new JsonResult(errorMessage);
            }

            else
            {

            

            

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand("INSERT INTO equipos_x_torneo (id_equipo, id_torneo)  VALUES(@id_equipo, @id_torneo)", myCon))
                {
                    //Recorrer valores del arreglo
                    foreach (var items in myJson)
                    {
                        myCommand.Parameters.AddWithValue("@id_equipo", items.id_equipo);
                        myCommand.Parameters.AddWithValue("@id_torneo", items.id_torneo);
                        myReader = myCommand.ExecuteReader();
                        //Limpiar parametros
                        myCommand.Parameters.Clear();
                        myReader.Close();
                    }

                    myCon.Close();

                }

            }

            //INSERTANDO A LA TABLA RAKING_EQUIPO_TORNEO
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand2 = new SqlCommand("INSERT INTO dbo.ranking_equipo_torneo (torneo_id, equipo_id, partidos_ganados, partidos_perdidos, empates, puntaje) " +
                    "VALUES(@torneo_id, @equipo_id, @partidos_ganados, @partidos_perdidos, @empates, @puntaje)", myCon))
                {
                    //Recorrer valores del arreglo
                    foreach (var items in myJson)
                    {
                        myCommand2.Parameters.AddWithValue("@torneo_id", items.id_torneo);
                        myCommand2.Parameters.AddWithValue("@equipo_id", items.id_equipo);
                        myCommand2.Parameters.AddWithValue("@partidos_ganados", 0);
                        myCommand2.Parameters.AddWithValue("@partidos_perdidos", 0);
                        myCommand2.Parameters.AddWithValue("@empates", 0);
                        myCommand2.Parameters.AddWithValue("@puntaje", 0);
                        myReader = myCommand2.ExecuteReader();
                        //Limpiar parametros
                        myCommand2.Parameters.Clear();
                        myReader.Close();

                    }
                    myCon.Close();
                }
            }


            return new JsonResult("Agregado");
            }
        }


        //Validacion modelo Estadio
        [Route("api/Torneo/Validate")]
        public bool Validate(Torneo model)
        {
            if (model.nombre == "" || model.fecha_inicio.Equals("") || model.fecha_fin.Equals(""))
            {
                error_message = "Ingrese los campos obligatorios";
                return false;
            }

            return true;
        }
        [Route("api/Torneo/ValidateValue/{nombre}")]
        public int ValidateValue(string _nombre)
        {
            int cantidad;

            string query = @"SELECT nombre FROM torneo WHERE nombre = @nombre";

            string sqlDataSource = _configuration.GetConnectionString("ChampiosLeagueAppCon");
            SqlDataReader myReader;
            DataTable table = new DataTable();

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@nombre", _nombre);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    cantidad = table.Rows.Count;
                    myReader.Close();
                    myCon.Close();

                    if (cantidad > 0)
                    {
                        //return new JsonResult("DUPLICADO");
                        return 1;
                    }

                    else
                    {
                        // return new JsonResult("NO DUPLICADO");
                        return 0;
                    }



                }

            }
        }

        [Route("api/Torneo/ValidateRankingTorneo/")]
        public int ValidateRankingTorneo(List<EquiposTorneo> myObject)
        {
            int cantidad;

            //Insertando a la tabla EQUIPOS_X_TORNEO
            string sqlDataSource = _configuration.GetConnectionString("ChampiosLeagueAppCon");
            SqlDataReader myReader;
            DataTable table = new DataTable();

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand("SELECT id_equipo FROM dbo.equipos_x_torneo WHERE id_torneo=@id_torneo AND id_equipo = @id_equipo;", myCon))
                {
                    //Recorrer valores del arreglo
                    foreach (var items in myObject)
                    {
                        myCommand.Parameters.AddWithValue("@id_equipo", items.id_equipo);
                        myCommand.Parameters.AddWithValue("@id_torneo", items.id_torneo);
                        myReader = myCommand.ExecuteReader();
                        //Limpiar parametros
                        myCommand.Parameters.Clear();
                        table.Load(myReader);
                      //  cantidad = table.Rows.Count;
                        myReader.Close();



                    }
                    myCon.Close();

                    cantidad = table.Rows.Count;

                    if (cantidad > 0)
                    {
                        //return new JsonResult("DUPLICADO");
                        return 1;
                    }

                    else
                    {
                        // return new JsonResult("NO DUPLICADO");
                        return 0;
                    }

                    
                }

                
            }
        }


        [HttpGet]
        [Route("api/Torneo/PartidosTorneo/{id}")]
        public JsonResult PartidosTorneo(int id)
        {
            string query = @"SELECT * FROM partido_x_torneo pt JOIN torneo t ON t.id_torneo = pt.torneo_id JOIN estadio es ON es.id_estadio = pt.estadio_id WHERE t.id_torneo = @id_torneo";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("ChampiosLeagueAppCon");
            SqlDataReader myReader;

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@id_torneo", id);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult(table);

        }




    }

}
