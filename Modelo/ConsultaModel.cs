namespace Modelo
{
    public class CrearAgendamientoModel
    {
        public string? correo { get; set; }
        public string? id_profesional { get; set; }
        public string? id_paciente { get; set; }
        public string? nombres { get; set; }
        public string? apellido_paterno { get; set; }
        public string? apellido_materno { get; set; }
        public string? fecha { get; set; }
        public string? hora { get; set; }
    }
    public class CrearPuntuacionAtencionDoctorModel
    {
        public string id_agendamiento { get; set; }
        public string? id_paciente { get; set; }
        public string? id_profesional { get; set; }
        public string? puntaje_general { get; set; }
        public string? recomendacion { get; set; }
        public string? claridad { get; set; }
        public string? puntualidad { get; set; }
        public string? empatia { get; set; }
        public string? cordialidad { get; set; }
        public string? nivel_satisfaccion { get; set; }

    }
    public class CargarImagenExamenModel
    {
        public string id_agendamiento { get; set; }
        public string id_consulta_medica { get; set; }
        public byte[]? imagen { get; set; }

        public string? nombre_examen { get; set; }
        public string? mime_type { get; set; }
    }

    public class ObtenerImagenExamenModel
    {
        public string id_consulta_medica { get; set; }
        public string imagen_examen { get; set; }

        public string? nombre_examen { get; set; }
        public string? mime_type { get; set; }
        public string? fecha_registro { get; set; }
        public string? tamano { get; set; }
    }
    public class ObtenerImagenExamenConsultaModel
    {
        public string id_agendamiento { get; set; }

    }
    public class ObtenerAgendamientoPacienteModel
    {

        public Int64 id_agendamiento { get; set; }
        public string? fecha { get; set; }
        public string? hora { get; set; }
        public string? id_profesional { get; set; }
        public string? id_especialidad { get; set; }
        public string? especialidad { get; set; }
        public string? nombres { get; set; }
        public string? apellido_paterno { get; set; }
        public string? apellido_materno { get; set; }
        public string? consulta_realizada { get; set; }
        public bool puntuacion_realizada { get; set; }
        public string? estado { get; set; }

    }
    public class EliminarAgendamientoRequestModel
    {
        public string id_agendamiento { get; set; }

    }
    public class CrearEstadoAgendamientoModel
    {
        public string id_agendamiento { get; set; }

    }
    public class ObtenerAgendamientoPorPacienteModel
    {
        public string id_paciente { get; set; }
        public string rut { get; set; }
    }
    public class EliminarAgendamientoPacienteModel
    {
        public string id_paciente { get; set; }
        public string rut { get; set; }
        public string id_agendamiento { get; set; }

    }
    public class HorasAgendadasDoctorModel
    {
        public string? id_paciente { get; set; }
        public string? id_agendamiento { get; set; }
        public string? id_profesional { get; set; }
        public string? fecha { get; set; }
        public string? hora { get; set; }
        public string? consulta_realizada { get; set; }
        public string? nombre_paciente { get; set; }
        public string? apellido_paterno { get; set; }
        public string? apellido_materno { get; set; }
        public string? estado { get; set; }


    }
    public class ObtenerAgendamientoProfesionalModel
    {
        public Int64 id_agendamiento { get; set; }
        public DateTime? fecha { get; set; }
        public string? hora { get; set; }
        public string? id_profesional { get; set; }
        public string? id_paciente { get; set; }
        public string? nombres { get; set; }
        public string? apellido_paterno { get; set; }
        public string? apellido_materno { get; set; }

    }

    public class PaisesModelCiudades
    {
        public Int64 pais_id { get; set; }
        public string? nombre_pais { get; set; }
        public string? capital { get; set; }
        public string? region { get; set; }
        public string? poblacion { get; set; }

        public DateTime? fecha_registro { get; set; }
        public Int64 usuario_id { get; set; }
        public string? usuario { get; set; }
        public string? correo { get; set; }
        public string? nombre { get; set; }
        public List<CiudadesModel>? listCiudades { get; set; }

        public List<PaisesModelCiudades>? listPaises { get; set; }
        public string? listCiudadesSerialize { get; set; }
    }
    public class PaisesModelCiudadesOut
    {
        public Int64 usuario_id { get; set; }
        public string? correo { get; set; }
        public string? nombre { get; set; }
        public List<CiudadesModel>? listCiudades { get; set; }

        public List<PaisesModelCiudades>? listPaises { get; set; }
        public string? listPaisesSerialize { get; set; }
        public string? listCiudadesSerialize { get; set; }
    }
    public class CiudadesModel
    {
        public Int64 ciudad_id { get; set; }
        public Int64 pais_id { get; set; }
        public string? nombre_ciudad { get; set; }
        public string? region { get; set; }
        public string? poblacion { get; set; }
        public string? latitud { get; set; }
        public string? longitud { get; set; }
        public string? usuario { get; set; }
    }
    public class UsuarioRequest
    {
        public string? usuario { get; set; }
        public string? fecha_desde { get; set; }
        public string? fecha_hasta { get; set; }
        public Int64 pais_id { get; set; }
    }

    public class ExcelDataRequest
    {
        public string? base64string { get; set; }
        public string? usuario { get; set; }

        public long pais_id { get; set; }
    }
    public class UsuarioValidation
    {
        public string? usuario { get; set; }
    }
}
