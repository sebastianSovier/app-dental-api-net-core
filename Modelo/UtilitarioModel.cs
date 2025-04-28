namespace Modelo
{
    public class PreguntasInicialesModel
    {
        public string? id_pregunta { get; set; }
        public string? texto { get; set; }
        public string? fecha_registro { get; set; }

    }
    public class AlternativaPreguntasInicialesModel
    {
        public string? id_alternativa_pregunta { get; set; }
        public string? id_pregunta { get; set; }
        public string? id_alternativa_respuesta { get; set; }
        public string? texto { get; set; }
        public string? fecha_registro { get; set; }

    }
    public class ProfesionalesModel
    {
        public string? id_profesional { get; set; }
        public string? rut { get; set; }
        public string? dv { get; set; }
        public string? id_especialidad { get; set; }
        public string? especialidad { get; set; }
        public string? nombres { get; set; }

        public string? apellido_paterno { get; set; }

        public string? apellido_materno { get; set; }
        public string? puntaje { get; set; }
    }
    public class ProfesionalesPuntuacionesModel
    {
        public string? id_profesional { get; set; }

        public string? rut { get; set; }
        public string? dv { get; set; }
        public string? id_especialidad { get; set; }
        public string? especialidad { get; set; }
        public string? nombres { get; set; }

        public string? apellido_paterno { get; set; }

        public string? apellido_materno { get; set; }

        public string? puntaje_general { get; set; }
        public string? recomendacion { get; set; }
        public string? claridad { get; set; }
        public string? puntualidad { get; set; }
        public string? empatia { get; set; }
        public string? cordialidad { get; set; }

        public string? nivel_satisfaccion { get; set; }

    }
    public class EspecialidadesModel
    {
        public string? id_especialidad { get; set; }
        public string? descripcion_especialidad { get; set; }
        public string? fecha_registro { get; set; }

    }
    public class RespuestasInicialesModel
    {
        public Int64 id_respuesta_paciente { get; set; }
        public Int64 id_paciente { get; set; }
        public Int64? id_pregunta { get; set; }
        public Int64? respuesta { get; set; }
        public string? rut { get; set; }

    }

}
