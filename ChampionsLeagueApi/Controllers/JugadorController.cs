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
    //[Route("api/[controller]")]
    [ApiController]
    public class JugadorController : ControllerBase
    {
        public string error_message = "";

        //Read ConnectionString 
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        public JugadorController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        [HttpPost]
        [Route("api/Jugador/Add")]
        public JsonResult InsertJugador([FromBody] Jugador player)
        {
            //Evaluar modelo
            if (!Validate(player))
            {
                string errorMessage = "Ingrese correctamente los campos";
                return new JsonResult(errorMessage);
            }

            else
            {

                string query = @"INSERT INTO dbo.Jugador (nombre, fecha_nac, fotografia, posicion, puntos,dorsal, equipo_id) VALUES (@nombre, @fecha_nac,@fotografia, @posicion, @puntos,@dorsal, @equipo_id)";

                DataTable table = new DataTable();
                string sqlDataSource = _configuration.GetConnectionString("ChampiosLeagueAppCon");
                SqlDataReader myReader;

                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@nombre", player.nombre);
                        myCommand.Parameters.AddWithValue("@fecha_nac", player.fecha_nac);
                        myCommand.Parameters.AddWithValue("@fotografia", player.fotografia);
                        myCommand.Parameters.AddWithValue("@posicion", player.posicion);
                        myCommand.Parameters.AddWithValue("@puntos", 0);
                        myCommand.Parameters.AddWithValue("@dorsal", player.dorsal);
                        myCommand.Parameters.AddWithValue("@equipo_id", player.equipo_id);

                        myReader = myCommand.ExecuteReader();
                        table.Load(myReader);
                        myReader.Close();
                        myCon.Close();
                    }
                }
            }
            return new JsonResult("Agregado correctamente");
        }


        //Validacion modelo Jugador
        [Route("api/Jugador/Validate")]
        public bool Validate(Jugador model)
        {
            if (model.nombre == "" || model.fecha_nac.Equals("") || model.fotografia == "" || model.posicion == "" || model.dorsal < 0 || model.dorsal == 0)
            {
                error_message = "Ingrese los campos obligatorios";
                return false;
            }

            return true;
        }

        [HttpGet]
        [Route("api/Jugador/Get")]
        public JsonResult GetJugadores()
        {
            string query = @"SELECT id_jugador, nombre, posicion, dorsal, puntos, fotografia, eq.nombre_equipo FROM dbo.Jugador jg INNER JOIN dbo.Equipo eq ON eq.id_equipo = jg.equipo_id ";

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

        [HttpGet]
        [Route("api/Jugador/Player/{id}")]
        public JsonResult GetJugador(int id)
        {
            string query = @"SELECT id_jugador, nombre, posicion, dorsal, puntos, fotografia, eq.nombre_equipo, fecha_nac FROM dbo.Jugador jg INNER JOIN dbo.Equipo eq ON eq.id_equipo = jg.equipo_id WHERE jg.id_jugador = @id_jugador ";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("ChampiosLeagueAppCon");
            SqlDataReader myReader;

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@id_jugador", id);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult(table);

        }


        [HttpPost]
        [Route("api/Jugador/SaveFile")]
        public JsonResult SaveFile()
        {
            try
            {
                var httpRequest = Request.Form;
                var postedFile = httpRequest.Files[0];
                string filename = postedFile.FileName;
                var physicalPath = _env.ContentRootPath + "/Photos/" + filename;

                using (var stream = new FileStream(physicalPath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }
                return new JsonResult(filename);
            }
            catch (Exception)
            {
                return new JsonResult("car.png");
            }
        }

        [HttpGet]
        [Route("api/Jugador/EquipoId/{id}")]
        public JsonResult JugadorEquipo(int id)
        {

            string query = @"SELECT id_jugador, nombre, fotografia, posicion, dorsal, puntos FROM dbo.Jugador WHERE equipo_id = @equipo_id ORDER BY puntos DESC";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("ChampiosLeagueAppCon");
            SqlDataReader myReader;

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@equipo_id", id);

                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }


                return new JsonResult(table);
            }
        }
    }


}
