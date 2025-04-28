using Microsoft.Extensions.Configuration;
using Modelo;
using MySql.Data.MySqlClient;
using Utilidades;


namespace Datos
{
    public class UtilitarioDal
    {
        private readonly IConfiguration _config;
        MySqlConexion? mysql = null;
        UtilidadesApiss utils = new UtilidadesApiss();

        public UtilitarioDal(IConfiguration config)
        {
            _config = config;
            mysql = new MySqlConexion(_config);
        }

        public async Task<List<PreguntasInicialesModel>> ObtenerPreguntasIniciales()
        {
            using MySqlConnection conexion = await mysql!.getConexion("bd1");
            try
            {
                List<PreguntasInicialesModel> listPreguntas = new List<PreguntasInicialesModel>();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conexion;
                cmd.CommandText = $"select id_pregunta,texto,fecha_registro from pregunta order by id_pregunta;";
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    PreguntasInicialesModel pregunta = new PreguntasInicialesModel();
                    pregunta.id_pregunta = reader["id_pregunta"].ToString();
                    pregunta.texto = reader["texto"].ToString();
                    pregunta.fecha_registro = reader["fecha_registro"].ToString();
                    listPreguntas.Add(pregunta);


                }

                return listPreguntas;
            }
            catch (Exception ex)
            {

                utils.createlogFile(ex.Message); throw;
            }
            finally
            {
                await conexion.CloseAsync();
            }
        }
        public async Task<List<AlternativaPreguntasInicialesModel>> ObtenerAlternativasPreguntasIniciales()
        {
            using MySqlConnection conexion = await mysql!.getConexion("bd1");
            try
            {
                List<AlternativaPreguntasInicialesModel> listAlternativas = new List<AlternativaPreguntasInicialesModel>();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conexion;
                cmd.CommandText = $"select id_alternativa_pregunta,id_pregunta,id_alternativa_respuesta,texto,fecha_registro from alternativa_pregunta order by id_alternativa_pregunta;";
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    AlternativaPreguntasInicialesModel alternativa = new AlternativaPreguntasInicialesModel();
                    alternativa.id_pregunta = reader["id_pregunta"].ToString();
                    alternativa.id_alternativa_pregunta = reader["id_alternativa_pregunta"].ToString();
                    alternativa.id_alternativa_respuesta = reader["id_alternativa_respuesta"].ToString();
                    alternativa.texto = reader["texto"].ToString();
                    alternativa.fecha_registro = reader["fecha_registro"].ToString();
                    listAlternativas.Add(alternativa);

                }

                return listAlternativas;
            }
            catch (Exception ex)
            {

                utils.createlogFile(ex.Message); throw;
            }
            finally
            {
                await conexion.CloseAsync();
            }
        }
        public async Task<List<ProfesionalesModel>> ObtenerProfesionales()
        {
            using MySqlConnection conexion = await mysql!.getConexion("bd1");
            try
            {
                List<ProfesionalesModel> listProfesionales = new List<ProfesionalesModel>();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conexion;
                cmd.CommandText = $"select id_profesional,rut,dv,id_especialidad,nombres,apellido_paterno,apellido_materno from profesional p where id_perfil != 3 order by id_profesional;";
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    ProfesionalesModel alternativa = new ProfesionalesModel();
                    alternativa.id_profesional = reader["id_profesional"].ToString();
                    alternativa.rut = reader["rut"].ToString();
                    alternativa.dv = reader["dv"].ToString();
                    alternativa.nombres = reader["nombres"].ToString();
                    alternativa.apellido_paterno = reader["apellido_paterno"].ToString();
                    alternativa.apellido_materno = reader["apellido_materno"].ToString();
                    alternativa.id_especialidad = reader["id_especialidad"].ToString();

                    listProfesionales.Add(alternativa);

                }

                return listProfesionales;
            }
            catch (Exception ex)
            {

                utils.createlogFile(ex.Message); throw;
            }
            finally
            {
                await conexion.CloseAsync();
            }
        }
        public async Task<ProfesionalesPuntuacionesModel> ObtenerProfesionalesPuntuaciones(string profesional_id)
        {
            using MySqlConnection conexion = await mysql!.getConexion("bd1");
            try
            {
                ProfesionalesPuntuacionesModel puntuacionProfesional = new ProfesionalesPuntuacionesModel();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conexion;
                cmd.CommandText = $"select id_profesional, ROUND(AVG(puntaje_general), 0) AS puntaje_general, ROUND(AVG(recomendacion), 0) AS recomendacion,  ROUND(AVG(claridad), 0) AS claridad,ROUND(AVG(puntualidad), 0) AS puntualidad,ROUND(AVG(empatia), 0) AS empatia, ROUND(AVG(cordialidad), 0) AS cordialidad, ROUND(AVG(nivel_satisfaccion), 0) AS nivel_satisfaccion from puntuacion p where id_profesional = ?id_profesional  group by id_profesional;";
                cmd.Parameters.Add("?id_profesional", MySqlDbType.VarChar).Value = profesional_id;

                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {

                    puntuacionProfesional.id_profesional = reader["id_profesional"].ToString();
                    puntuacionProfesional.puntaje_general = reader["puntaje_general"].ToString();
                    puntuacionProfesional.recomendacion = reader["recomendacion"].ToString();
                    puntuacionProfesional.cordialidad = reader["cordialidad"].ToString();
                    puntuacionProfesional.claridad = reader["claridad"].ToString();
                    puntuacionProfesional.puntualidad = reader["puntualidad"].ToString();
                    puntuacionProfesional.empatia = reader["empatia"].ToString();
                    puntuacionProfesional.nivel_satisfaccion = reader["nivel_satisfaccion"].ToString();


                }

                return puntuacionProfesional;
            }
            catch (Exception ex)
            {

                utils.createlogFile(ex.Message); throw;
            }
            finally
            {
                await conexion.CloseAsync();
            }
        }
        public async Task<List<EspecialidadesModel>> ObtenerEspecialidades()
        {
            using MySqlConnection conexion = await mysql!.getConexion("bd1");
            try
            {
                List<EspecialidadesModel> listEspecialidades = new List<EspecialidadesModel>();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conexion;
                cmd.CommandText = $"select id_especialidad,descripcion_especialidad,fecha_registro from especialidad_profesional  order by id_especialidad;";
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    EspecialidadesModel especialidad = new EspecialidadesModel();
                    especialidad.id_especialidad = reader["id_especialidad"].ToString();
                    especialidad.descripcion_especialidad = reader["descripcion_especialidad"].ToString();
                    especialidad.fecha_registro = reader["fecha_registro"].ToString();


                    listEspecialidades.Add(especialidad);

                }

                return listEspecialidades;
            }
            catch (Exception ex)
            {

                utils.createlogFile(ex.Message); throw;
            }
            finally
            {
                await conexion.CloseAsync();
            }
        }

    }
}


