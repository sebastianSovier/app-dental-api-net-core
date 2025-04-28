using Microsoft.Extensions.Configuration;
using Modelo;
using MySql.Data.MySqlClient;
using System.Globalization;
using Utilidades;


namespace Datos
{
    public class UsuarioDal
    {
        private readonly IConfiguration _config;
        MySqlConexion? mysql = null;
        UtilidadesApiss utils = new UtilidadesApiss();

        public UsuarioDal(IConfiguration config)
        {
            _config = config;
            mysql = new MySqlConexion(_config);
        }

        public async Task<List<PacienteModel>> ObtenerPacientes()
        {
            using MySqlConnection conexion = await mysql!.getConexion("bd1");
            try
            {
                List<PacienteModel> listPacientes = new List<PacienteModel>();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conexion;
                cmd.CommandText = $"select id_perfil,id_paciente,dv,rut,nombres,apellido_paterno,apellido_materno,fecha_nacimiento,correo,direccion,fecha_registro from paciente where id_perfil = '1' order by id_paciente;";
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    PacienteModel paciente = new PacienteModel();
                    paciente.id_paciente = Convert.ToInt64(reader["id_paciente"]);
                    paciente.nombres = reader["nombres"].ToString();
                    paciente.apellido_paterno = reader["apellido_paterno"].ToString();
                    paciente.correo = reader["correo"].ToString();
                    paciente.direccion = reader["direccion"].ToString();
                    paciente.fecha_nacimiento = reader["fecha_nacimiento"].ToString();
                    paciente.rut = reader["rut"].ToString();
                    paciente.id_perfil = reader["id_perfil"].ToString();
                    paciente.dv = reader["dv"].ToString();
                    paciente.apellido_materno = reader["apellido_materno"].ToString();
                    DateTime fecha = DateTime.ParseExact(reader["fecha_registro"].ToString(), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    string fechaFormateada = fecha.ToString("dd/MM/yyyy");
                    paciente.fecha_registro = fechaFormateada; listPacientes.Add(paciente);


                }

                return listPacientes;
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
        public async Task<List<PacienteModel>> ObtenerPacientesAdministrador()
        {
            using MySqlConnection conexion = await mysql!.getConexion("bd1");
            try
            {
                List<PacienteModel> listPacientes = new List<PacienteModel>();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conexion;
                cmd.CommandText = $"select id_perfil,id_paciente,dv,rut,nombres,apellido_paterno,apellido_materno,fecha_nacimiento,correo,direccion,fecha_registro from paciente  order by id_paciente;";
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    PacienteModel paciente = new PacienteModel();
                    paciente.id_paciente = Convert.ToInt64(reader["id_paciente"]);
                    paciente.nombres = reader["nombres"].ToString();
                    paciente.apellido_paterno = reader["apellido_paterno"].ToString();
                    paciente.correo = reader["correo"].ToString();
                    paciente.direccion = reader["direccion"].ToString();
                    paciente.fecha_nacimiento = reader["fecha_nacimiento"].ToString();
                    paciente.rut = reader["rut"].ToString();
                    paciente.id_perfil = reader["id_perfil"].ToString();
                    paciente.dv = reader["dv"].ToString();
                    paciente.apellido_materno = reader["apellido_materno"].ToString();
                    DateTime fecha = DateTime.ParseExact(reader["fecha_registro"].ToString(), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    string fechaFormateada = fecha.ToString("dd/MM/yyyy");
                    paciente.fecha_registro = fechaFormateada; listPacientes.Add(paciente);


                }

                return listPacientes;
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
        public async Task<List<ProfesionalModel>> ObtenerProfesionales()
        {
            using MySqlConnection conexion = await mysql!.getConexion("bd1");
            try
            {
                List<ProfesionalModel> listProfesionales = new List<ProfesionalModel>();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conexion;
                cmd.CommandText = $"select id_perfil,id_profesional,id_especialidad,rut,dv,nombres,apellido_paterno,apellido_materno,fecha_nacimiento,correo,direccion,fecha_registro from profesional where id_perfil = '2' order by id_profesional;";
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    ProfesionalModel profesional = new ProfesionalModel();
                    profesional.id_profesional = Convert.ToInt64(reader["id_profesional"]);
                    profesional.id_perfil = reader["id_perfil"].ToString();
                    profesional.id_especialidad = Convert.ToInt64(reader["id_especialidad"]);
                    profesional.nombres = reader["nombres"].ToString();
                    profesional.apellido_paterno = reader["apellido_paterno"].ToString();
                    profesional.correo = reader["correo"].ToString();
                    profesional.direccion = reader["direccion"].ToString();
                    profesional.fecha_nacimiento = reader["fecha_nacimiento"].ToString();
                    profesional.rut = reader["rut"].ToString();
                    profesional.dv = reader["dv"].ToString();
                    profesional.apellido_materno = reader["apellido_materno"].ToString();
                    DateTime fecha = DateTime.ParseExact(reader["fecha_registro"].ToString(), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    string fechaFormateada = fecha.ToString("dd/MM/yyyy");
                    profesional.fecha_registro = fechaFormateada;

                    listProfesionales.Add(profesional);


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
        public async Task<List<ProfesionalModel>> ObtenerProfesionalesAdministrador()
        {
            using MySqlConnection conexion = await mysql!.getConexion("bd1");
            try
            {
                List<ProfesionalModel> listProfesionales = new List<ProfesionalModel>();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conexion;
                cmd.CommandText = $"select id_perfil,id_profesional,id_especialidad,rut,dv,nombres,apellido_paterno,apellido_materno,fecha_nacimiento,correo,direccion,fecha_registro from profesional order by id_profesional;";
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    ProfesionalModel profesional = new ProfesionalModel();
                    profesional.id_profesional = Convert.ToInt64(reader["id_profesional"]);
                    profesional.id_perfil = reader["id_perfil"].ToString();
                    profesional.id_especialidad = Convert.ToInt64(reader["id_especialidad"]);
                    profesional.nombres = reader["nombres"].ToString();
                    profesional.apellido_paterno = reader["apellido_paterno"].ToString();
                    profesional.correo = reader["correo"].ToString();
                    profesional.direccion = reader["direccion"].ToString();
                    profesional.fecha_nacimiento = reader["fecha_nacimiento"].ToString();
                    profesional.rut = reader["rut"].ToString();
                    profesional.dv = reader["dv"].ToString();
                    profesional.apellido_materno = reader["apellido_materno"].ToString();
                    DateTime fecha = DateTime.ParseExact(reader["fecha_registro"].ToString(), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    string fechaFormateada = fecha.ToString("dd/MM/yyyy");
                    profesional.fecha_registro = fechaFormateada;

                    listProfesionales.Add(profesional);


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
        public async Task<PacienteModel> ObtenerPaciente(string usuarioRequest)
        {
            using MySqlConnection conexion = await mysql!.getConexion("bd1");
            try
            {
                PacienteModel paciente = new PacienteModel();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conexion;
                cmd.CommandText = $"select id_paciente,rut,nombres,apellido_paterno,telefono,apellido_materno,fecha_nacimiento,correo,direccion,fecha_registro,dv from paciente where rut = ?rut and id_perfil = '1' order by id_paciente;";
                cmd.Parameters.Add("?rut", MySqlDbType.VarChar).Value = usuarioRequest;
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    paciente.id_paciente = Convert.ToInt64(reader["id_paciente"]);
                    paciente.nombres = reader["nombres"].ToString();
                    paciente.apellido_paterno = reader["apellido_paterno"].ToString();
                    paciente.telefono = reader["telefono"].ToString();
                    paciente.correo = reader["correo"].ToString();
                    paciente.direccion = reader["direccion"].ToString();
                    DateTime fecha = DateTime.ParseExact(reader["fecha_nacimiento"].ToString(), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    string fechaFormateada = fecha.ToString("dd/MM/yyyy");
                    paciente.fecha_nacimiento = fechaFormateada;
                    paciente.rut = reader["rut"].ToString();
                    paciente.dv = reader["dv"].ToString();
                    paciente.apellido_materno = reader["apellido_materno"].ToString();

                }

                return paciente;
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
        public async Task<PacienteModel> ObtenerPacienteById(string id_paciente)
        {
            using MySqlConnection conexion = await mysql!.getConexion("bd1");
            try
            {
                PacienteModel paciente = new PacienteModel();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conexion;
                cmd.CommandText = $"select id_paciente,rut,nombres,apellido_paterno,apellido_materno,fecha_nacimiento,telefono,correo,direccion,fecha_registro,dv from paciente where id_paciente = ?id_paciente and id_perfil = '1' order by id_paciente;";
                cmd.Parameters.Add("?id_paciente", MySqlDbType.VarChar).Value = id_paciente;
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    paciente.id_paciente = Convert.ToInt64(reader["id_paciente"]);
                    paciente.nombres = reader["nombres"].ToString();
                    paciente.telefono = reader["telefono"].ToString();
                    paciente.apellido_paterno = reader["apellido_paterno"].ToString();
                    paciente.correo = reader["correo"].ToString();
                    paciente.direccion = reader["direccion"].ToString();
                    DateTime fecha = DateTime.ParseExact(reader["fecha_nacimiento"].ToString(), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    string fechaFormateada = fecha.ToString("dd/MM/yyyy");
                    paciente.fecha_nacimiento = fechaFormateada;
                    paciente.rut = reader["rut"].ToString();
                    paciente.dv = reader["dv"].ToString();
                    paciente.apellido_materno = reader["apellido_materno"].ToString();

                }

                return paciente;
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
        public async Task<ProfesionalModel> ObtenerDoctorById(string id_profesional)
        {
            using MySqlConnection conexion = await mysql!.getConexion("bd1");
            try
            {
                ProfesionalModel paciente = new ProfesionalModel();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conexion;
                cmd.CommandText = $"select id_profesional,rut,nombres,apellido_paterno,apellido_materno,fecha_nacimiento,telefono,correo,direccion,fecha_registro,dv from profesional where id_profesional = ?id_profesional and id_perfil = '2' order by id_profesional;";
                cmd.Parameters.Add("?id_profesional", MySqlDbType.VarChar).Value = id_profesional;
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    paciente.id_profesional = Convert.ToInt64(reader["id_profesional"]);
                    paciente.nombres = reader["nombres"].ToString();
                    paciente.telefono = reader["telefono"].ToString();
                    paciente.apellido_paterno = reader["apellido_paterno"].ToString();
                    paciente.correo = reader["correo"].ToString();
                    paciente.direccion = reader["direccion"].ToString();
                    DateTime fecha = DateTime.ParseExact(reader["fecha_nacimiento"].ToString(), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    string fechaFormateada = fecha.ToString("dd/MM/yyyy");
                    paciente.fecha_nacimiento = fechaFormateada;
                    paciente.rut = reader["rut"].ToString();
                    paciente.dv = reader["dv"].ToString();
                    paciente.apellido_materno = reader["apellido_materno"].ToString();

                }

                return paciente;
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
        public async Task<bool> EliminarPaciente(string id_paciente)
        {
            using MySqlConnection conexion = await mysql!.getConexion("bd1");
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conexion;
                cmd.CommandText = "delete from paciente where id_paciente = ?id_paciente";

                cmd.Parameters.Add("?id_paciente", MySqlDbType.VarChar).Value = id_paciente;
                await cmd.ExecuteNonQueryAsync();
                return true;
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
        public async Task<bool> EliminarProfesional(string id_profesional)
        {
            using MySqlConnection conexion = await mysql!.getConexion("bd1");
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conexion;
                cmd.CommandText = "delete from profesional where id_profesional = ?id_profesional";

                cmd.Parameters.Add("?id_profesional", MySqlDbType.VarChar).Value = id_profesional;
                await cmd.ExecuteNonQueryAsync();
                return true;
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
        public async Task<bool> ModificarPerfilProfesional(string id, string perfil)
        {
            using MySqlConnection conexion = await mysql!.getConexion("bd1");
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conexion;
                cmd.CommandText = "update profesional set id_perfil = ?id_perfil where id_profesional = ?id_profesional";

                cmd.Parameters.Add("?id_profesional", MySqlDbType.VarChar).Value = id;
                cmd.Parameters.Add("?id_perfil", MySqlDbType.VarChar).Value = perfil;
                await cmd.ExecuteNonQueryAsync();
                return true;
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
        public async Task<bool> ModificarPerfilPaciente(string id, string perfil)
        {
            using MySqlConnection conexion = await mysql!.getConexion("bd1");
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conexion;
                cmd.CommandText = "update paciente set id_perfil = ?id_perfil where id_paciente = ?id_paciente";

                cmd.Parameters.Add("?id_paciente", MySqlDbType.VarChar).Value = id;
                cmd.Parameters.Add("?id_perfil", MySqlDbType.VarChar).Value = perfil;
                await cmd.ExecuteNonQueryAsync();
                return true;
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
        public async void GuardarRespuestasPersonales(RespuestasInicialesModel RespuestasInicialesModel)
        {
            using MySqlConnection conexion = await mysql!.getConexion("bd1");
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conexion;
                cmd.CommandText = "INSERT INTO `sistemaodontologico`.`respuesta_paciente` (`id_paciente`, `id_pregunta`, `respuesta`) VALUES (?id_paciente, ?id_pregunta,?respuesta);";

                cmd.Parameters.Add("?id_paciente", MySqlDbType.VarChar).Value = RespuestasInicialesModel.id_paciente;
                cmd.Parameters.Add("?id_pregunta", MySqlDbType.VarChar).Value = RespuestasInicialesModel.id_pregunta;
                cmd.Parameters.Add("?respuesta", MySqlDbType.VarChar).Value = RespuestasInicialesModel.respuesta;

                await cmd.ExecuteNonQueryAsync();

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
        public async Task<List<RespuestasInicialesModel>> ObtenerRespuestasPersonalesPaciente(string id_paciente)
        {
            using MySqlConnection conexion = await mysql!.getConexion("bd1");
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conexion;
                cmd.CommandText = $"select id_respuesta_paciente,id_paciente,id_pregunta,respuesta from respuesta_paciente where id_paciente = ?id_paciente order by id_respuesta_paciente;";
                cmd.Parameters.Add("?id_paciente", MySqlDbType.VarChar).Value = id_paciente;
                using var reader = await cmd.ExecuteReaderAsync();
                List<RespuestasInicialesModel> respuestas = new List<RespuestasInicialesModel>();
                while (await reader.ReadAsync())
                {
                    RespuestasInicialesModel respuesta = new();
                    respuesta.id_respuesta_paciente = Convert.ToInt64(reader["id_respuesta_paciente"]);
                    respuesta.id_paciente = Convert.ToInt64(reader["id_paciente"]);
                    respuesta.id_pregunta = Convert.ToInt64(reader["id_pregunta"]);
                    respuesta.respuesta = Convert.ToInt64(reader["respuesta"]);
                    respuestas.Add(respuesta);
                }

                return respuestas;
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
        public async Task<ProfesionalModel> ObtenerProfesional(string rut)
        {
            using MySqlConnection conexion = await mysql!.getConexion("bd1");
            try
            {
                ProfesionalModel profesional = new ProfesionalModel();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conexion;
                cmd.CommandText = $"select id_perfil,id_profesional,rut,nombres,apellido_paterno,apellido_materno,fecha_nacimiento,correo,direccion,fecha_registro,dv from profesional where rut = ?rut and id_perfil = '2' order by id_profesional;";
                cmd.Parameters.Add("?rut", MySqlDbType.VarChar).Value = rut;
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    profesional.id_perfil = reader["id_perfil"].ToString();

                    profesional.id_profesional = Convert.ToInt64(reader["id_profesional"]);
                    profesional.nombres = reader["nombres"].ToString();
                    profesional.apellido_paterno = reader["apellido_paterno"].ToString();
                    profesional.correo = reader["correo"].ToString();
                    profesional.direccion = reader["direccion"].ToString();
                    profesional.fecha_nacimiento = reader["fecha_nacimiento"].ToString();
                    profesional.rut = reader["rut"].ToString();
                    profesional.dv = reader["dv"].ToString();
                    profesional.apellido_materno = reader["apellido_materno"].ToString();

                }

                return profesional;
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

        public async Task<bool> CrearPaciente(PacienteModel usuarioRequest)
        {

            using MySqlConnection conexion = await mysql!.getConexion("bd1");
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conexion;
                cmd.CommandText = "INSERT INTO `sistemaodontologico`.`paciente` (`nombres`, `apellido_paterno`, `apellido_materno`,`telefono`, `direccion`, `correo`,`fecha_nacimiento`, `rut`, `dv`, `id_perfil`) VALUES (?nombres, ?apellido_paterno, ?apellido_materno,?telefono, ?direccion, ?correo,?fecha_nacimiento, ?rut, ?dv,1);";

                cmd.Parameters.Add("?nombres", MySqlDbType.VarChar).Value = usuarioRequest.nombres;
                cmd.Parameters.Add("?apellido_paterno", MySqlDbType.VarChar).Value = usuarioRequest.apellido_paterno;
                cmd.Parameters.Add("?apellido_materno", MySqlDbType.VarChar).Value = usuarioRequest.apellido_materno;
                cmd.Parameters.Add("?telefono", MySqlDbType.VarChar).Value = usuarioRequest.telefono;
                cmd.Parameters.Add("?direccion", MySqlDbType.VarChar).Value = usuarioRequest.direccion;
                cmd.Parameters.Add("?correo", MySqlDbType.VarChar).Value = usuarioRequest.correo;
                DateTime fecha = DateTime.ParseExact(usuarioRequest.fecha_nacimiento, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                cmd.Parameters.Add("?fecha_nacimiento", MySqlDbType.Date).Value = fecha;
                cmd.Parameters.Add("?rut", MySqlDbType.VarChar).Value = usuarioRequest.rut;
                cmd.Parameters.Add("?dv", MySqlDbType.VarChar).Value = usuarioRequest.dv;

                await cmd.ExecuteNonQueryAsync();
                return true;
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
        public async Task<long> CrearProfesional(ProfesionalModel usuarioRequest)
        {

            using MySqlConnection conexion = await mysql!.getConexion("bd1");
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conexion;
                cmd.CommandText = "INSERT INTO `sistemaodontologico`.`profesional` (`nombres`, `apellido_paterno`, `id_especialidad`, `apellido_materno`,`telefono`, `direccion`, `correo`,`fecha_nacimiento`, `rut`, `dv`,`id_perfil`) VALUES (?nombres, ?apellido_paterno,?id_especialidad, ?apellido_materno,?telefono, ?direccion, ?correo,?fecha_nacimiento, ?rut, ?dv,2);";

                cmd.Parameters.Add("?nombres", MySqlDbType.VarChar).Value = usuarioRequest.nombres;
                cmd.Parameters.Add("?apellido_paterno", MySqlDbType.VarChar).Value = usuarioRequest.apellido_paterno;
                cmd.Parameters.Add("?id_especialidad", MySqlDbType.VarChar).Value = usuarioRequest.id_especialidad;
                cmd.Parameters.Add("?apellido_materno", MySqlDbType.VarChar).Value = usuarioRequest.apellido_materno;
                cmd.Parameters.Add("?telefono", MySqlDbType.VarChar).Value = usuarioRequest.telefono;
                cmd.Parameters.Add("?direccion", MySqlDbType.VarChar).Value = usuarioRequest.direccion;
                cmd.Parameters.Add("?correo", MySqlDbType.VarChar).Value = usuarioRequest.correo;
                cmd.Parameters.Add("?fecha_nacimiento", MySqlDbType.Date).Value = usuarioRequest.fecha_nacimiento;
                cmd.Parameters.Add("?rut", MySqlDbType.VarChar).Value = usuarioRequest.rut;
                cmd.Parameters.Add("?dv", MySqlDbType.VarChar).Value = usuarioRequest.dv;

                await cmd.ExecuteNonQueryAsync();
                long idGenerado = cmd.LastInsertedId;
                return idGenerado;
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


