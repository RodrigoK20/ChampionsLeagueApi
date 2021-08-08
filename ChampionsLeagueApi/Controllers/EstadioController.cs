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

namespace ChampionsLeagueApi.Controllers
{
   // [Route("api/[controller]")]
    [ApiController]
    public class EstadioController : ControllerBase
    {
        public string error_message = "";

        //Read ConnectionString 
        private readonly IConfiguration _configuration;

        public EstadioController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

    
        [HttpGet]
        [Route("api/Estadio/Get")]
        public JsonResult GetEstadios()
        {
            string query = @"SELECT id_estadio, nombre_estadio,pais,ciudad FROM dbo.Estadio WHERE estado='1'";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("ChampiosLeagueAppCon");
            SqlDataReader myReader;

            using(SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using(SqlCommand myCommand = new SqlCommand(query, myCon))
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
        [Route("api/Estadio/GetEstadio/{id}")]
        public JsonResult GetEstadioById(int id)
        {
            string query = @"SELECT id_estadio,nombre_estadio,pais,ciudad FROM dbo.Estadio WHERE id_estadio=@id_estadio";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("ChampiosLeagueAppCon");
            SqlDataReader myReader;

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@id_estadio", id);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult(table);

        }


        [HttpPost]
        [Route("api/Estadio/Add")]
        public JsonResult InsertEstadio([FromBody]Estadio est)
        {
       
            //Evaluar modelo
            if (!Validate(est))
            {
                string errorMessage = "Ingrese correctamente los campos";
                return new JsonResult(errorMessage);
            }

            ////Validar si ya se encuentra registrado
            //if (ValueExist(est.nombre_estadio) == false)
            //{
            //    string errorMessage = "El nombre del estadio ya se encuentra registrado!";
            //    return new JsonResult(errorMessage);
                
            //}
      
            else
            {

            string query = @"INSERT INTO dbo.Estadio (nombre_estadio,pais,ciudad,estado) VALUES (@nombre_estadio, @pais,@ciudad, @estado)";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("ChampiosLeagueAppCon");
            SqlDataReader myReader;

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@nombre_estadio", est.nombre_estadio);
                    myCommand.Parameters.AddWithValue("@pais", est.pais);
                    myCommand.Parameters.AddWithValue("@ciudad", est.ciudad);
                    myCommand.Parameters.AddWithValue("@estado", '1');
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

                return new JsonResult("Agregado correctamente");
            }

        }

        [Route("api/Estadio/Update/{id}")]
        [HttpPut]
        public JsonResult ActualizarEstadio(Estadio est)
        {


            string query = @"UPDATE dbo.Estadio SET nombre_estadio = @nombre_estadio, pais= @pais, ciudad = @ciudad WHERE id_estadio = @id_estadio";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("ChampiosLeagueAppCon");
            SqlDataReader myReader;

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@id_estadio", est.id_estadio);
                    myCommand.Parameters.AddWithValue("@nombre_estadio", est.nombre_estadio);
                    myCommand.Parameters.AddWithValue("@pais", est.pais);
                    myCommand.Parameters.AddWithValue("@ciudad", est.ciudad);

                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult("Actualizado correctamente");

        }

        [Route("api/Estadio/Delete/{id}")]
        [HttpPost]
        public JsonResult ActualizarEstadioDelete(int id)
        {

            string query = @"UPDATE dbo.Estadio SET estado = '0' WHERE id_estadio = @id_estadio";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("ChampiosLeagueAppCon");
            SqlDataReader myReader;

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@id_estadio", id);
                 
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult("Actualizado correctamente");

        }

        //Validacion modelo Estadio
        [Route("api/Estadio/Validate")]
        public bool Validate(Estadio model)
        {
            if(model.nombre_estadio == "" || model.pais == "" || model.ciudad == "")
            {
                error_message = "Ingrese los campos obligatorios";
                return false;
            }

            return true;
        }


        [Route("api/Estadio/Value")]
        public int ValueExist(string _nombre)
        {
            string query = @"SELECT nombre_estadio FROM dbo.Estadio WHERE nombre_estadio=@nombre_estadio";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("ChampiosLeagueAppCon");
            SqlDataReader myReader;

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@nombre_estadio", _nombre);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                   
                }
            }

            if (myReader.HasRows)
            {
                return 1;
            }
            return 0;

        }

    }
}
