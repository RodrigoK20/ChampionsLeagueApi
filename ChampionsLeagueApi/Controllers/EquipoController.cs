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
    public class EquipoController : ControllerBase
    {
        public string error_message = "";

        //Read ConnectionString 
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        public EquipoController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        [HttpPost]
        [Route("api/Equipo/Add")]
        public JsonResult InsertEquipo([FromBody] Equipo eq)
        {
            //Evaluar modelo
            if (!Validate(eq))
            {
                string errorMessage = "Ingrese correctamente los campos";
                return new JsonResult(errorMessage);
            }

            if(ValidateValue(eq.nombre_equipo) == 1)
            {
                string errorMessage = "El nombre del equipo ya se encuentra registrado";
                return new JsonResult(errorMessage);

            }

            else
            {

                string query = @"INSERT INTO dbo.Equipo (nombre_equipo,logo,puntaje,estado) VALUES (@nombre_equipo, @logo,@puntaje, @estado)";

                DataTable table = new DataTable();
                string sqlDataSource = _configuration.GetConnectionString("ChampiosLeagueAppCon");
                SqlDataReader myReader;

                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@nombre_equipo", eq.nombre_equipo);
                        myCommand.Parameters.AddWithValue("@logo", eq.logo);
                        myCommand.Parameters.AddWithValue("@puntaje", 0);
                        myCommand.Parameters.AddWithValue("@estado", '1');
                        myReader = myCommand.ExecuteReader();
                        table.Load(myReader);
                        myReader.Close();
                        myCon.Close();
                    }
                }
            }
            return new JsonResult("Agregado correctamente");
        }


        [HttpPost]
        [Route("api/Equipo/SaveFile")]
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
                return new JsonResult("anonymous.png");
            }
        }


        //Validacion modelo Estadio
        [Route("api/Equipo/Validate")]
        public bool Validate(Equipo model)
        {
            if (model.nombre_equipo == "" || model.logo == "")
            {
                error_message = "Ingrese los campos obligatorios";
                return false;
            }

            return true;
        }

        [Route("api/Equipo/ValidateValue/{nombre}")]
        public int ValidateValue(string nombre)
        {
            int cantidad;

            string query = @"SELECT nombre_equipo FROM equipo WHERE nombre_equipo = @nombre_equipo";

            string sqlDataSource = _configuration.GetConnectionString("ChampiosLeagueAppCon");
            SqlDataReader myReader;
            DataTable table = new DataTable();

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@nombre_equipo", nombre);
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

        [HttpGet]
        [Route("api/Equipo/Get")]
        public JsonResult GetEquipos()
        {
            string query = @"SELECT id_equipo, nombre_equipo,logo,puntaje, estado FROM dbo.Equipo WHERE estado='1'";

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

        //SELECT EQUIPOS POR TORNEO - REGISTRAR EQUIPOS
        [HttpGet]
        [Route("api/Equipo/GetEquiposTorneo/{id}")]
        public JsonResult GetEquipos2(int id)
        {
            string query = @"select e.id_equipo, e.nombre_equipo FROM equipo e JOIN equipos_x_torneo ext ON ext.id_equipo = e.id_equipo WHERE ext.id_torneo = @id_torneo AND e.estado='1'";

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
