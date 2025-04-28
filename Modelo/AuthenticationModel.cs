

namespace Modelo
{
    public class AuthenticationModel
    {
        public class WithoutLoginRequest
        {

            public required string rut { get; set; }
        }
        public class ObtenerPacienteRequest
        {

            public string rut { get; set; }
        }
        public class ModificarPerfilRequest
        {

            public string rut { get; set; }
            public string perfil { get; set; }
        }
        public class LoginRequest
        {

            public required string rut { get; set; }
            public required string password { get; set; }

        }
        public class EmptyRequest
        {



        }
        public class LoginResponse
        {

            public string? access_Token { get; set; }
            public string? correo { get; set; }
            public bool? auth { get; set; }
            public Int64? id { get; set; }
        }
        public class OkResponse
        {
            public bool? auth { get; set; }
        }
    }
    public class PacienteModel
    {

        public Int64 id_paciente { get; set; }
        public string? rut { get; set; }
        public string? id_perfil { get; set; }
        public string? dv { get; set; }
        public string? nombres { get; set; }
        public string? apellido_paterno { get; set; }
        public string? apellido_materno { get; set; }
        public string? fecha_nacimiento { get; set; }
        public string? telefono { get; set; }
        public string? correo { get; set; }
        public string? direccion { get; set; }
        public string? fecha_registro { get; set; }
        public string? contrasena { get; set; }

    }
    public class RespuestasPersonalesModel
    {

        public Int64 id_respuesta_paciente { get; set; }
        public string? id_pregunta { get; set; }
        public string? id_paciente { get; set; }
        public string? respuesta { get; set; }
        public string? fecha_registro { get; set; }

    }
    public class HorasAgendadasRequestModel
    {
        public string? id_profesional { get; set; }
        public string? fechaDesde { get; set; }
        public string? fechaHasta { get; set; }
        public string? tipoUsuario { get; set; }
    }
    public class GuardarConsultaMedicaModel
    {
        public required string id_agendamiento { get; set; }
        public string? id_paciente { get; set; }
        public string? id_profesional { get; set; }
        public required string motivo_consulta { get; set; }
        public required string observaciones { get; set; }
        public required List<TratamientoModel> tratamientos { get; set; }
    }
    public class ObtenerImagenesExamenesConsultaModel
    {
        public required string id_agendamiento { get; set; }

    }
    public class TratamientoModel
    {
        public string? nombre_tratamiento { get; set; }
        public string? descripcion { get; set; }
        public string? valor { get; set; }
        public string? id_presupuesto_consulta { get; set; }
    }
    public class PresupuestoTotalModel
    {
        public string? id_consulta_medica { get; set; }
        public string? total { get; set; }
    }
    public class ProfesionalModel
    {

        public Int64 id_profesional { get; set; }
        public string? especialidad { get; set; }
        public string? rut { get; set; }
        public string? dv { get; set; }
        public string? nombres { get; set; }
        public Int64 id_especialidad { get; set; }
        public string? apellido_paterno { get; set; }
        public string? apellido_materno { get; set; }
        public string? fecha_nacimiento { get; set; }
        public string? telefono { get; set; }
        public string? correo { get; set; }
        public string? direccion { get; set; }
        public string? fecha_registro { get; set; }
        public string? contrasena { get; set; }
        public string? id_perfil { get; set; }

    }
    public class PasswordPacienteModel
    {
        public string? contrasena { get; set; }
        public string? correo { get; set; }
        public Int64 id_contrasena { get; set; }
        public Int64 id_paciente { get; set; }
        public string? cod_recover_password { get; set; }
        public string? fecha_registro { get; set; }
    }
    public class PasswordProfesionalModel
    {
        public string? contrasena { get; set; }
        public string? correo { get; set; }
        public Int64 id_contrasena_profesional { get; set; }
        public Int64 id_profesional { get; set; }
        public string? cod_recover_password { get; set; }
        public string? fecha_registro { get; set; }
    }
}
