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
    public class PartidosTorneoController : ControllerBase
    {

        public string error_message = "";

        //Read ConnectionString 
        private readonly IConfiguration _configuration;

        public PartidosTorneoController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        [HttpPost]
        [Route("api/PartidosTorneo/Add")]
        public JsonResult InsertPartidosTorneo([FromBody] List<PartidosTorneo> myJson)
        {

            //Insertando a la tabla Partido_x_Torneo
            string sqlDataSource = _configuration.GetConnectionString("ChampiosLeagueAppCon");
            SqlDataReader myReader;

            //if (!Validate(myJson))
            //{
            //    string errorMessage = "El equipo ya se encuentra registrado en el torneo";
            //    return new JsonResult(errorMessage);
            //}
   
             using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand("INSERT INTO partido_x_torneo (estadio_id, torneo_id, equipo_a, equipo_b, resultado_a, resultado_b, fecha_partido, hora, numero_partido, estado_partido)  VALUES(@estadio_id, @torneo_id, @equipo_a, @equipo_b, @resultado_a, @resultado_b, @fecha_partido, @hora, @numero_partido, @estado_partido)", myCon))

                    {
                        //Recorrer valores del arreglo
                        foreach (var items in myJson)
                        {
                            myCommand.Parameters.AddWithValue("@estadio_id", items.estadio_id);
                            myCommand.Parameters.AddWithValue("@torneo_id", items.torneo_id);
                            myCommand.Parameters.AddWithValue("@equipo_a", items.equipo_a);
                            myCommand.Parameters.AddWithValue("@equipo_b", items.equipo_b);
                            myCommand.Parameters.AddWithValue("@resultado_a", 0);
                            myCommand.Parameters.AddWithValue("@resultado_b", 0);
                            myCommand.Parameters.AddWithValue("@fecha_partido", items.fecha_partido);
                            myCommand.Parameters.AddWithValue("@hora", items.hora);
                            myCommand.Parameters.AddWithValue("@numero_partido", items.numero_partido);
                            myCommand.Parameters.AddWithValue("@estado_partido", "PROCESO");
                            myReader = myCommand.ExecuteReader();
                            //Limpiar parametros
                            myCommand.Parameters.Clear();
                            myReader.Close();
                        }

                        myCon.Close();

                    }

                }

              

                return new JsonResult("Agregado");
            }

        [HttpGet]
        [Route("api/PartidosTorneo/GetPartido/{id}")]
        public JsonResult GetPartidoById(int id)
        {
            string query = @"SELECT id_partido_x_torneo, equipo_a, equipo_b, torneo_id FROM partido_x_torneo WHERE id_partido_x_torneo = @id_partido_x_torneo ";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("ChampiosLeagueAppCon");
            SqlDataReader myReader;

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@id_partido_x_torneo", id);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult(table);

        }

        //Actualizar resultado partido
        [Route("api/PartidosTorneo/Update/{id}")]
        [HttpPut]
        public JsonResult ActualizarPartido(PartidosTorneo pt)
        {

            if (!Validate(pt))
            {
                string errorMessage = "Ingrese correctamente los campos";
                return new JsonResult(errorMessage);
            }

            else
            {

            string query = @"UPDATE dbo.partido_x_torneo SET resultado_a = @resultado_a, resultado_b= @resultado_B, estado_partido = 'FINALIZADO' WHERE id_partido_x_torneo = @id_partido_x_torneo";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("ChampiosLeagueAppCon");
            SqlDataReader myReader;

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@id_partido_x_torneo", pt.id_partido_x_torneo);
                    myCommand.Parameters.AddWithValue("@resultado_a", pt.resultado_a);
                    myCommand.Parameters.AddWithValue("@resultado_b", pt.resultado_b);                  
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

           }
            return new JsonResult("Actualizado correctamente");

        }

        //Validacion modelo
        [Route("api/PartidosTorneo/Validate")]
        public bool Validate(PartidosTorneo model)
        {
            if (model.resultado_a < 0 || model.resultado_a == 0 || model.resultado_b < 0 || model.resultado_b == 0)
            {
                error_message = "Ingrese los campos obligatorios";
                return false;
            }

            return true;
        }

    }

}

////Validacion modelo PartidosTorneo
//[Route("api/PartidosTorneo/Validate")]
//public bool Validate(PartidosTorneo model)
//{
//    if (model.equipo_a == "" || model.equipo_b == "" || model.fecha_partido.Equals("") || model.hora == "" || model.estadio_id < 0 || model.numero_partido < 0 || model.hora =="" || model.torneo_id < 0)
//    {
//        error_message = "Ingrese los campos obligatorios";
//        return false;
//    }

//    return true;
//}




