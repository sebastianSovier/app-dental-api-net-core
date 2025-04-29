using Microsoft.Extensions.Configuration;
using Modelo;
using MySql.Data.MySqlClient;
using System.Globalization;
using Utilidades;

namespace Datos
{
    public class AgendamientoDal
    {
        private readonly IConfiguration _config;
        MySqlConexion? mysql = null;
        UtilidadesApiss utils = new UtilidadesApiss();

        public AgendamientoDal(IConfiguration config)
        {
            _config = config;
            mysql = new MySqlConexion(_config);
        }

        public async Task<List<ObtenerAgendamientoPacienteModel>> ObtenerAgendamientoPorPaciente(string paciente_id)
        {
            using MySqlConnection conexion = await mysql!.getConexion("bd1");
            try
            {
                List<ObtenerAgendamientoPacienteModel> listAgendamientos = new List<ObtenerAgendamientoPacienteModel>();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conexion;
                cmd.CommandText = $"select a.id_agendamiento,a.fecha,e.consulta_realizada,a.hora,p.id_profesional,p.nombres,p.apellido_paterno,p.apellido_materno,p.id_especialidad from agendamiento a join profesional p  on a.id_profesional = p.id_profesional join estado_agendamiento e  on e.id_agendamiento = a.id_agendamiento  where a.id_paciente  = ?paciente_id and TIMESTAMP(fecha, STR_TO_DATE(hora, '%H:%i')) < NOW() order by id_paciente;";
                cmd.Parameters.Add("?paciente_id", MySqlDbType.VarChar).Value = paciente_id;
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    ObtenerAgendamientoPacienteModel agendamiento = new ObtenerAgendamientoPacienteModel();
                    agendamiento.id_agendamiento = Convert.ToInt64(reader["id_agendamiento"]);
                    agendamiento.id_especialidad = reader["id_especialidad"].ToString();
                    agendamiento.consulta_realizada = reader["consulta_realizada"].ToString();
                    agendamiento.fecha = reader["fecha"].ToString();
                    TimeSpan horaSpan = TimeSpan.Parse(reader["hora"].ToString());
                    agendamiento.hora = horaSpan.ToString(@"hh\:mm");
                    agendamiento.id_profesional = reader["id_profesional"].ToString();
                    agendamiento.nombres = reader["nombres"].ToString();
                    agendamiento.apellido_materno = reader["apellido_materno"].ToString();
                    agendamiento.apellido_paterno = reader["apellido_paterno"].ToString();
                    listAgendamientos.Add(agendamiento);

                }
                return listAgendamientos;
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
        public async Task<List<ObtenerAgendamientoPacienteModel>> ObtenerAgendamientosFuturosPorPaciente(string paciente_id)
        {
            using MySqlConnection conexion = await mysql!.getConexion("bd1");
            try
            {
                List<ObtenerAgendamientoPacienteModel> listAgendamientos = new List<ObtenerAgendamientoPacienteModel>();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conexion;
                cmd.CommandText = $"select a.id_agendamiento,a.fecha,e.consulta_realizada,a.hora,p.id_profesional,p.nombres,p.apellido_paterno,p.apellido_materno,p.id_especialidad,e.estado from agendamiento a join profesional p  on a.id_profesional = p.id_profesional join estado_agendamiento e  on e.id_agendamiento = a.id_agendamiento  where a.id_paciente  = ?paciente_id and e.consulta_realizada = '0' and TIMESTAMP(fecha, STR_TO_DATE(hora, '%H:%i')) >= NOW() order by id_paciente;";
                cmd.Parameters.Add("?paciente_id", MySqlDbType.VarChar).Value = paciente_id;
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    ObtenerAgendamientoPacienteModel agendamiento = new ObtenerAgendamientoPacienteModel();
                    agendamiento.id_agendamiento = Convert.ToInt64(reader["id_agendamiento"]);
                    agendamiento.id_especialidad = reader["id_especialidad"].ToString();
                    agendamiento.consulta_realizada = reader["consulta_realizada"].ToString();
                    agendamiento.fecha = reader["fecha"].ToString();
                    TimeSpan horaSpan = TimeSpan.Parse(reader["hora"].ToString());
                    agendamiento.hora = horaSpan.ToString(@"hh\:mm");
                    agendamiento.id_profesional = reader["id_profesional"].ToString();
                    agendamiento.nombres = reader["nombres"].ToString();
                    agendamiento.apellido_materno = reader["apellido_materno"].ToString();
                    agendamiento.apellido_paterno = reader["apellido_paterno"].ToString();
                    agendamiento.estado = reader["estado"].ToString();

                    listAgendamientos.Add(agendamiento);

                }
                return listAgendamientos;
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
        public async Task<bool> ObtenerPuntuacionPorPacienteProfesional(string paciente_id, string profesional_id)
        {
            using MySqlConnection conexion = await mysql!.getConexion("bd1");
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conexion;
                cmd.CommandText = $"select p.id_puntuacion from puntuacion p  where p.id_paciente  = ?paciente_id and p.id_profesional = ?profesional_id order by id_puntuacion;";
                cmd.Parameters.Add("?paciente_id", MySqlDbType.VarChar).Value = paciente_id;
                cmd.Parameters.Add("?profesional_id", MySqlDbType.VarChar).Value = profesional_id;
                using var reader = await cmd.ExecuteReaderAsync();

                bool result = false;
                while (await reader.ReadAsync())
                {
                    result = true;

                }
                return result;
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
        public async Task<List<ObtenerAgendamientoPacienteModel>> ObtenerHistoricoAgendamientoPorPaciente(string paciente_id)
        {
            using MySqlConnection conexion = await mysql!.getConexion("bd1");
            try
            {
                List<ObtenerAgendamientoPacienteModel> listAgendamientos = new List<ObtenerAgendamientoPacienteModel>();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conexion;
                cmd.CommandText = $"select a.id_agendamiento,a.fecha,e.consulta_realizada,a.hora,p.id_profesional,p.nombres,p.apellido_paterno,p.apellido_materno,p.id_especialidad from agendamiento a join profesional p  on a.id_profesional = p.id_profesional join estado_agendamiento e  on e.id_agendamiento = a.id_agendamiento  where a.id_paciente  = ?paciente_id and e.consulta_realizada = '1' and fecha < NOW() order by id_paciente;";
                cmd.Parameters.Add("?paciente_id", MySqlDbType.VarChar).Value = paciente_id;
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    ObtenerAgendamientoPacienteModel agendamiento = new ObtenerAgendamientoPacienteModel();
                    agendamiento.id_agendamiento = Convert.ToInt64(reader["id_agendamiento"]);
                    agendamiento.id_especialidad = reader["id_especialidad"].ToString();
                    agendamiento.consulta_realizada = reader["consulta_realizada"].ToString();
                    agendamiento.fecha = reader["fecha"].ToString();
                    TimeSpan horaSpan = TimeSpan.Parse(reader["hora"].ToString());
                    agendamiento.hora = horaSpan.ToString(@"hh\:mm");
                    agendamiento.id_profesional = reader["id_profesional"].ToString();
                    agendamiento.nombres = reader["nombres"].ToString();
                    agendamiento.apellido_materno = reader["apellido_materno"].ToString();
                    agendamiento.apellido_paterno = reader["apellido_paterno"].ToString();
                    listAgendamientos.Add(agendamiento);

                }
                return listAgendamientos;
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
        public async Task<List<ObtenerTratamientoConsultaPaciente>> ObtenerTratamientoConsultaPaciente(ObtenerTratamientoConsultaPacienteModel request)
        {
            using MySqlConnection conexion = await mysql!.getConexion("bd1");
            try
            {
                List<ObtenerTratamientoConsultaPaciente> listTratamientos = new List<ObtenerTratamientoConsultaPaciente>();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conexion;
                cmd.CommandText = $"select c.motivo_consulta, c.observaciones, p.total, t.nombre, t.descripcion, t.valor_unitario from consulta_medica c join presupuesto_consulta p  on c.id_consulta_medica = p.id_consulta_medica join tratamiento t  on t.id_presupuesto_consulta = p.id_presupuesto_consulta  where c.id_paciente  = ?id_paciente and c.id_profesional = ?id_profesional and c.id_agendamiento = ?id_agendamiento order by t.id_tratamiento;";
                cmd.Parameters.Add("?id_paciente", MySqlDbType.VarChar).Value = request.id_paciente;
                cmd.Parameters.Add("?id_profesional", MySqlDbType.VarChar).Value = request.id_profesional;
                cmd.Parameters.Add("?id_agendamiento", MySqlDbType.VarChar).Value = request.id_agendamiento;

                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    ObtenerTratamientoConsultaPaciente tratamiento = new ObtenerTratamientoConsultaPaciente();
                    tratamiento.nombre = reader["nombre"].ToString();
                    tratamiento.observaciones = reader["observaciones"].ToString();
                    tratamiento.descripcion = reader["descripcion"].ToString();
                    tratamiento.valor_unitario = reader["valor_unitario"].ToString();
                    tratamiento.total = reader["total"].ToString();
                    tratamiento.motivo_consulta = reader["motivo_consulta"].ToString();



                    listTratamientos.Add(tratamiento);

                }
                return listTratamientos;
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

        public async Task<bool> EliminarAgendamientoPaciente(string agendamiento_id, string paciente_id)
        {
            using MySqlConnection conexion = await mysql!.getConexion("bd1");
            try
            {
                List<ObtenerAgendamientoPacienteModel> listAgendamientos = new List<ObtenerAgendamientoPacienteModel>();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conexion;
                cmd.CommandText = $"delete from agendamiento where id_paciente = ?id_paciente and id_agendamiento = ?id_agendamiento;";
                cmd.Parameters.Add("?id_paciente", MySqlDbType.VarChar).Value = paciente_id;
                cmd.Parameters.Add("?id_agendamiento", MySqlDbType.VarChar).Value = agendamiento_id;

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
        public async Task<bool> ConfirmarAgendamientoPaciente(string agendamiento_id)
        {
            using MySqlConnection conexion = await mysql!.getConexion("bd1");
            try
            {
                List<ObtenerAgendamientoPacienteModel> listAgendamientos = new List<ObtenerAgendamientoPacienteModel>();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conexion;
                cmd.CommandText = $"update estado_agendamiento set estado = 'CONFIRMADO' where id_agendamiento = ?id_agendamiento;";
                cmd.Parameters.Add("?id_agendamiento", MySqlDbType.VarChar).Value = agendamiento_id;

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
        public async Task<bool> ConsultaRealizada(string agendamiento_id)
        {
            using MySqlConnection conexion = await mysql!.getConexion("bd1");
            try
            {
                List<ObtenerAgendamientoPacienteModel> listAgendamientos = new List<ObtenerAgendamientoPacienteModel>();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conexion;
                cmd.CommandText = $"update estado_agendamiento set consulta_realizada = '1' where id_agendamiento = ?id_agendamiento;";
                cmd.Parameters.Add("?id_agendamiento", MySqlDbType.VarChar).Value = agendamiento_id;

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
        public async Task<List<ObtenerAgendamientoProfesionalModel>> ObtenerAgendamientoPorProfesional(Int64 profesional_id)
        {
            using MySqlConnection conexion = await mysql!.getConexion("bd1");
            try
            {
                List<ObtenerAgendamientoProfesionalModel> listAgendamientos = new List<ObtenerAgendamientoProfesionalModel>();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conexion;
                cmd.CommandText = $"select a.id_agendamiento,a.fecha,a.hora,a.id_profesional,p.nombres,p.apellido_paterno,p.apellido_materno from agendamiento a , paciente p join on a.id_paciente = p.id_paciente where a.profesional_id = ?profesional_id order by profesional_id;";
                cmd.Parameters.Add("?profesional_id", MySqlDbType.VarChar).Value = profesional_id;
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    ObtenerAgendamientoProfesionalModel agendamiento = new ObtenerAgendamientoProfesionalModel();
                    agendamiento.id_agendamiento = Convert.ToInt64(reader["id_agendamiento"]);
                    agendamiento.fecha = (DateTime?)reader["fecha"];
                    TimeSpan horaSpan = TimeSpan.Parse(reader["hora"].ToString());
                    agendamiento.hora = horaSpan.ToString(@"hh\:mm");
                    agendamiento.id_paciente = reader["id_paciente"].ToString();
                    agendamiento.nombres = reader["nombres"].ToString();
                    agendamiento.apellido_materno = reader["apellido_materno"].ToString();
                    agendamiento.apellido_paterno = reader["apellido_paterno"].ToString();
                    listAgendamientos.Add(agendamiento);

                }
                return listAgendamientos;
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

        public async Task<long> CrearAgendamiento(CrearAgendamientoModel agendamientoRequest)
        {

            using MySqlConnection conexion = await mysql!.getConexion("bd1");
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conexion;
                cmd.CommandText = "INSERT INTO `sistemaodontologico`.`agendamiento` (`id_paciente`, `id_profesional`, `fecha`, `hora`) VALUES (?id_paciente, ?id_profesional, ?fecha, ?hora);";

                cmd.Parameters.Add("?id_paciente", MySqlDbType.VarChar).Value = agendamientoRequest.id_paciente;
                cmd.Parameters.Add("?id_profesional", MySqlDbType.VarChar).Value = agendamientoRequest.id_profesional;
                cmd.Parameters.Add("?fecha", MySqlDbType.VarChar).Value = agendamientoRequest.fecha;
                cmd.Parameters.Add("?hora", MySqlDbType.VarChar).Value = agendamientoRequest.hora;
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
        public async Task<bool> ModificarAgendamiento(ModificarAgendamientoModel agendamientoRequest)
        {

            using MySqlConnection conexion = await mysql!.getConexion("bd1");
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conexion;
                cmd.CommandText = "UPDATE `sistemaodontologico`.`agendamiento` SET fecha = ?fecha , hora = ?hora , id_profesional = ?id_profesional where id_agendamiento =  ?id_agendamiento and id_paciente = ?id_paciente;";

                cmd.Parameters.Add("?id_agendamiento", MySqlDbType.VarChar).Value = agendamientoRequest.id_agendamiento;
                cmd.Parameters.Add("?id_paciente", MySqlDbType.VarChar).Value = agendamientoRequest.id_paciente;
                cmd.Parameters.Add("?id_profesional", MySqlDbType.VarChar).Value = agendamientoRequest.id_profesional;
                cmd.Parameters.Add("?fecha", MySqlDbType.VarChar).Value = agendamientoRequest.fecha;
                cmd.Parameters.Add("?hora", MySqlDbType.VarChar).Value = agendamientoRequest.hora;
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
        public async Task<bool> CrearPuntuacionPorAtencionDoctor(CrearPuntuacionAtencionDoctorModel crearPuntuacionRequest)
        {

            using MySqlConnection conexion = await mysql!.getConexion("bd1");
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conexion;
                cmd.CommandText = "INSERT INTO `sistemaodontologico`.`puntuacion` (`id_paciente`, `id_profesional`, `puntaje_general`, `recomendacion`, `empatia`, `claridad`, `puntualidad`, `cordialidad`, `nivel_satisfaccion`) VALUES (?id_paciente, ?id_profesional, ?puntaje_general, ?recomendacion,?claridad, ?puntualidad, ?empatia, ?cordialidad, ?nivel_satisfaccion);";

                cmd.Parameters.Add("?id_paciente", MySqlDbType.VarChar).Value = crearPuntuacionRequest.id_paciente;
                cmd.Parameters.Add("?id_profesional", MySqlDbType.VarChar).Value = crearPuntuacionRequest.id_profesional;
                cmd.Parameters.Add("?puntaje_general", MySqlDbType.VarChar).Value = crearPuntuacionRequest.puntaje_general;
                cmd.Parameters.Add("?recomendacion", MySqlDbType.VarChar).Value = crearPuntuacionRequest.recomendacion;
                cmd.Parameters.Add("?claridad", MySqlDbType.VarChar).Value = crearPuntuacionRequest.claridad;
                cmd.Parameters.Add("?puntualidad", MySqlDbType.VarChar).Value = crearPuntuacionRequest.puntualidad;
                cmd.Parameters.Add("?empatia", MySqlDbType.VarChar).Value = crearPuntuacionRequest.empatia;
                cmd.Parameters.Add("?cordialidad", MySqlDbType.VarChar).Value = crearPuntuacionRequest.cordialidad;
                cmd.Parameters.Add("?nivel_satisfaccion", MySqlDbType.VarChar).Value = crearPuntuacionRequest.nivel_satisfaccion;

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
        public async Task<bool> CargarImagenesExamen(CargarImagenExamenModel cargarImagenRequest)
        {

            using MySqlConnection conexion = await mysql!.getConexion("bd1");
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conexion;
                cmd.CommandText = "INSERT INTO `sistemaodontologico`.`examen_medico` (`id_consulta_medica`, `imagen_examen`, `nombre_examen`, `mime_type`) VALUES (?id_consulta_medica, ?imagen_examen,?nombre_examen, ?mime_type);";

                cmd.Parameters.Add("?id_consulta_medica", MySqlDbType.VarChar).Value = cargarImagenRequest.id_consulta_medica;
                cmd.Parameters.Add("?imagen_examen", MySqlDbType.LongBlob).Value = cargarImagenRequest.imagen;
                cmd.Parameters.Add("?nombre_examen", MySqlDbType.VarChar).Value = cargarImagenRequest.nombre_examen;
                cmd.Parameters.Add("?mime_type", MySqlDbType.VarChar).Value = cargarImagenRequest.mime_type;

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
        public async Task<bool> deleteImagenesConsulta(string id_consulta_medica)
        {

            using MySqlConnection conexion = await mysql!.getConexion("bd1");
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conexion;
                cmd.CommandText = "delete from examen_medico where id_consulta_medica = ?id_consulta_medica";

                cmd.Parameters.Add("?id_consulta_medica", MySqlDbType.VarChar).Value = id_consulta_medica;

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
        public async Task<string> ObtenerConsultaMedica(string id_paciente, string id_profesional, string id_agendamiento)
        {

            using MySqlConnection conexion = await mysql!.getConexion("bd1");
            try
            {
                List<ObtenerAgendamientoProfesionalModel> listAgendamientos = new List<ObtenerAgendamientoProfesionalModel>();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conexion;
                cmd.CommandText = $"select c.id_consulta_medica from consulta_medica c where id_paciente = ?id_paciente and id_profesional = ?id_profesional and id_agendamiento = ?id_agendamiento;";
                cmd.Parameters.Add("?id_paciente", MySqlDbType.VarChar).Value = id_paciente;
                cmd.Parameters.Add("?id_profesional", MySqlDbType.VarChar).Value = id_profesional;
                cmd.Parameters.Add("?id_agendamiento", MySqlDbType.VarChar).Value = id_agendamiento;

                using var reader = await cmd.ExecuteReaderAsync();
                string idConsultaMedica = "";
                while (await reader.ReadAsync())
                {
                    idConsultaMedica = reader["id_consulta_medica"].ToString();

                }
                return idConsultaMedica;
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
        public async Task<bool> CrearEstadoAgendamiento(CrearEstadoAgendamientoModel agendamientoRequest)
        {

            using MySqlConnection conexion = await mysql!.getConexion("bd1");
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conexion;
                cmd.CommandText = "INSERT INTO `sistemaodontologico`.`estado_agendamiento` (`id_agendamiento`, `estado`, `notificado`,`consulta_realizada`) VALUES (?id_agendamiento, 'PENDIENTE', '0','0');";

                cmd.Parameters.Add("?id_agendamiento", MySqlDbType.VarChar).Value = agendamientoRequest.id_agendamiento;

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
        public async Task<List<HorasAgendadasDoctorModel>> obtenerHorasAgendadasPorDoctor(HorasAgendadasRequestModel request)
        {

            using MySqlConnection conexion = await mysql!.getConexion("bd1");
            try
            {
                List<HorasAgendadasDoctorModel> listAgendamientos = new List<HorasAgendadasDoctorModel>();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conexion;
                cmd.CommandText = $"select e.estado,a.id_agendamiento,e.consulta_realizada,a.id_paciente,a.fecha,a.hora,a.id_profesional from agendamiento a join estado_agendamiento e  on e.id_agendamiento = a.id_agendamiento   where a.id_profesional = ?id_profesional and fecha >= ?fechaDesde and fecha <= ?fechaHasta order by a.id_agendamiento;";
                cmd.Parameters.Add("?id_profesional", MySqlDbType.VarChar).Value = request.id_profesional;
                DateTime fechaDesde = DateTime.ParseExact(request.fechaDesde, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                cmd.Parameters.Add("?fechaDesde", MySqlDbType.Date).Value = fechaDesde;
                DateTime fechaHasta = DateTime.ParseExact(request.fechaHasta, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                cmd.Parameters.Add("?fechaHasta", MySqlDbType.Date).Value = fechaHasta;
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    HorasAgendadasDoctorModel agendamiento = new HorasAgendadasDoctorModel();
                    agendamiento.id_profesional = reader["id_profesional"].ToString();
                    agendamiento.consulta_realizada = reader["consulta_realizada"].ToString();
                    agendamiento.id_paciente = reader["id_paciente"].ToString();
                    agendamiento.fecha = reader["fecha"].ToString();
                    TimeSpan horaSpan = TimeSpan.Parse(reader["hora"].ToString());
                    agendamiento.hora = horaSpan.ToString(@"hh\:mm");
                    agendamiento.id_agendamiento = reader["id_agendamiento"].ToString();
                    agendamiento.estado = reader["estado"].ToString();

                    listAgendamientos.Add(agendamiento);

                }
                return listAgendamientos;
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
        public async Task<List<HorasAgendadasDoctorModel>> obtenerHistoricoHorasAgendadasPorDoctor(HorasAgendadasRequestModel request)
        {

            using MySqlConnection conexion = await mysql!.getConexion("bd1");
            try
            {
                List<HorasAgendadasDoctorModel> listAgendamientos = new List<HorasAgendadasDoctorModel>();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conexion;
                cmd.CommandText = $"select a.id_agendamiento,e.consulta_realizada,a.id_paciente,a.fecha,a.hora,a.id_profesional from agendamiento a join estado_agendamiento e  on e.id_agendamiento = a.id_agendamiento   where a.id_profesional = ?id_profesional and fecha < NOW() and e.consulta_realizada = '1' order by a.id_agendamiento;";
                cmd.Parameters.Add("?id_profesional", MySqlDbType.VarChar).Value = request.id_profesional;
                DateTime fechaDesde = DateTime.ParseExact(request.fechaDesde, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                cmd.Parameters.Add("?fechaDesde", MySqlDbType.Date).Value = fechaDesde;
                DateTime fechaHasta = DateTime.ParseExact(request.fechaHasta, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                cmd.Parameters.Add("?fechaHasta", MySqlDbType.Date).Value = fechaHasta;
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    HorasAgendadasDoctorModel agendamiento = new HorasAgendadasDoctorModel();
                    agendamiento.id_profesional = reader["id_profesional"].ToString();
                    agendamiento.consulta_realizada = reader["consulta_realizada"].ToString();
                    agendamiento.id_paciente = reader["id_paciente"].ToString();
                    agendamiento.fecha = reader["fecha"].ToString();
                    TimeSpan horaSpan = TimeSpan.Parse(reader["hora"].ToString());
                    agendamiento.hora = horaSpan.ToString(@"hh\:mm");
                    agendamiento.id_agendamiento = reader["id_agendamiento"].ToString();

                    listAgendamientos.Add(agendamiento);

                }
                return listAgendamientos;
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
        public async Task<HorasAgendadasDoctorModel> obtenerAgendamientoPorId(string id_agendamiento)
        {

            using MySqlConnection conexion = await mysql!.getConexion("bd1");
            try
            {
                HorasAgendadasDoctorModel agendamiento = new HorasAgendadasDoctorModel();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conexion;
                cmd.CommandText = $"select a.id_agendamiento,a.id_paciente,a.fecha,a.hora,a.id_profesional from agendamiento a  where a.id_agendamiento = ?id_agendamiento order by id_agendamiento;";
                cmd.Parameters.Add("?id_agendamiento", MySqlDbType.VarChar).Value = id_agendamiento;

                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    agendamiento.id_profesional = reader["id_profesional"].ToString();
                    agendamiento.id_paciente = reader["id_paciente"].ToString();
                    agendamiento.fecha = reader["fecha"].ToString();
                    TimeSpan horaSpan = TimeSpan.Parse(reader["hora"].ToString());
                    agendamiento.hora = horaSpan.ToString(@"hh\:mm");
                    agendamiento.id_agendamiento = reader["id_agendamiento"].ToString();


                }
                return agendamiento;
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
        public async Task<List<ObtenerImagenExamenModel>> obtenerImagenesExamenesMedico(string id_consulta_medica)
        {

            using MySqlConnection conexion = await mysql!.getConexion("bd1");
            try
            {
                List<ObtenerImagenExamenModel> listImagenesExamenes = new List<ObtenerImagenExamenModel>();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conexion;
                cmd.CommandText = $"select e.id_examen,e.id_consulta_medica,e.imagen_examen,e.nombre_examen,e.mime_type,e.fecha_registro from examen_medico e  where e.id_consulta_medica = ?id_consulta_medica order by id_examen;";
                cmd.Parameters.Add("?id_consulta_medica", MySqlDbType.VarChar).Value = id_consulta_medica;

                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    ObtenerImagenExamenModel imagenExamen = new();
                    imagenExamen.fecha_registro = reader["fecha_registro"].ToString();
                    byte[] imagen = (byte[])reader["imagen_examen"];
                    imagenExamen.imagen_examen = $"data:{reader["mime_type"]};base64,{Convert.ToBase64String(imagen)}";
                    imagenExamen.tamano = Math.Round(imagen.Length / 1024.0, 0).ToString();
                    byte[] imagenBytes = (byte[])reader["imagen_examen"];
                    imagenExamen.mime_type = reader["mime_type"].ToString();

                    imagenExamen.nombre_examen = reader["nombre_examen"].ToString();
                    listImagenesExamenes.Add(imagenExamen);

                }
                return listImagenesExamenes;
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
        public async Task<long> GuardarConsultaMedica(GuardarConsultaMedicaModel request)
        {

            using MySqlConnection conexion = await mysql!.getConexion("bd1");
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conexion;
                cmd.CommandText = "INSERT INTO `sistemaodontologico`.`consulta_medica` (`id_paciente`, `id_profesional`, `motivo_consulta`,`observaciones`, `id_agendamiento`) VALUES (?id_paciente, ?id_profesional,?motivo_consulta,?observaciones,?id_agendamiento);";

                cmd.Parameters.Add("?id_paciente", MySqlDbType.VarChar).Value = request.id_paciente;
                cmd.Parameters.Add("?id_profesional", MySqlDbType.VarChar).Value = request.id_profesional;
                cmd.Parameters.Add("?motivo_consulta", MySqlDbType.VarChar).Value = request.motivo_consulta;
                cmd.Parameters.Add("?observaciones", MySqlDbType.VarChar).Value = request.observaciones;
                cmd.Parameters.Add("?id_agendamiento", MySqlDbType.VarChar).Value = request.id_agendamiento;

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
        public async Task<bool> GuardarTratamiento(TratamientoModel request)
        {

            using MySqlConnection conexion = await mysql!.getConexion("bd1");
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conexion;
                cmd.CommandText = "INSERT INTO `sistemaodontologico`.`tratamiento` (`nombre`, `descripcion`, `valor_unitario`,`id_presupuesto_consulta`) VALUES (?nombre, ?descripcion,?valor_unitario,?id_presupuesto_consulta);";

                cmd.Parameters.Add("?nombre", MySqlDbType.VarChar).Value = request.nombre_tratamiento;
                cmd.Parameters.Add("?descripcion", MySqlDbType.VarChar).Value = request.descripcion;
                cmd.Parameters.Add("?valor_unitario", MySqlDbType.VarChar).Value = request.valor;
                cmd.Parameters.Add("?id_presupuesto_consulta", MySqlDbType.VarChar).Value = request.id_presupuesto_consulta;


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
            return true;

        }
        public async Task<long> GuardarPresupuestoConsulta(PresupuestoTotalModel request)
        {

            using MySqlConnection conexion = await mysql!.getConexion("bd1");
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conexion;
                cmd.CommandText = "INSERT INTO `sistemaodontologico`.`presupuesto_consulta` (`id_consulta_medica`, `total`) VALUES (?id_consulta_medica, ?total);";

                cmd.Parameters.Add("?id_consulta_medica", MySqlDbType.VarChar).Value = request.id_consulta_medica;
                cmd.Parameters.Add("?total", MySqlDbType.VarChar).Value = request.total;
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
