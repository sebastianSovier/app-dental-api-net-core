using Datos;
using Microsoft.Extensions.Configuration;
using Modelo;
using Utilidades;
namespace Negocio
{
    public class UtilitarioBo
    {
        private readonly IConfiguration _config;
        private UtilidadesApiss util = new UtilidadesApiss();
        public UtilitarioBo(IConfiguration config)
        {
            _config = config;

        }


        public UtilitarioBo() { }


        public async Task<List<PreguntasInicialesModel>> ObtenerPreguntasIniciales()
        {
            UtilitarioDal utilitarioDal = new UtilitarioDal(_config);
            return await utilitarioDal.ObtenerPreguntasIniciales();

        }
        public async Task<List<AlternativaPreguntasInicialesModel>> ObtenerAlternativaPreguntasIniciales()
        {
            UtilitarioDal utilitarioDal = new UtilitarioDal(_config);
            return await utilitarioDal.ObtenerAlternativasPreguntasIniciales();

        }
        public async Task<List<ProfesionalesModel>> ObtenerProfesionales()
        {
            UtilitarioDal utilitarioDal = new UtilitarioDal(_config);
            List<ProfesionalesModel> listProfesionales = await utilitarioDal.ObtenerProfesionales();
            List<EspecialidadesModel> listEpecialidades = await utilitarioDal.ObtenerEspecialidades();
            foreach (var item in listProfesionales)
            {
                item.especialidad = listEpecialidades.Find(x => x.id_especialidad == item.id_especialidad)!.descripcion_especialidad;
            }
            return listProfesionales;

        }
        public async Task<List<ProfesionalesModel>> ObtenerRecomendacionesProfesionales(string rut)
        {
            UtilitarioDal utilitarioDal = new UtilitarioDal(_config);
            UsuarioDal UsuarioDal = new UsuarioDal(_config);
            List<ProfesionalesModel> listProfesionales = await utilitarioDal.ObtenerProfesionales();
            List<EspecialidadesModel> listEpecialidades = await utilitarioDal.ObtenerEspecialidades();
            PacienteModel paciente = await UsuarioDal.ObtenerPaciente(util.getRutWithoutDv(rut));
            List<RespuestasInicialesModel> respuestasPaciente = await UsuarioDal.ObtenerRespuestasPersonalesPaciente(paciente.id_paciente.ToString());
            foreach (var item in listProfesionales)
            {
                item.especialidad = listEpecialidades.Find(x => x.id_especialidad == item.id_especialidad)!.descripcion_especialidad;
            }

            var tareas = listProfesionales
            .Select(async p =>
             {
                 var puntuaciones = await utilitarioDal.ObtenerProfesionalesPuntuaciones(p.id_profesional);
                 var puntaje = CalcularPuntajeRelevancia(p, respuestasPaciente, puntuaciones);
                 return new
                 {
                     Profesional = p,
                     Puntaje = puntaje
                 };
             });

            var resultados = await Task.WhenAll(tareas);

            List<ProfesionalesModel> recomendados = resultados
            .OrderByDescending(x => x.Puntaje)
             .Select(x =>
             {
                 x.Profesional.puntaje = x.Puntaje.ToString();
                 return x.Profesional;
             })
             .ToList();

            //return recomendados;

            return recomendados;


        }
        private int CalcularPuntajeRelevancia(ProfesionalesModel p, List<RespuestasInicialesModel> respuestas, ProfesionalesPuntuacionesModel puntuacion)
        {
            int score = 0;

            score += Convert.ToInt32(puntuacion.puntaje_general) * 2;

            // Pregunta 1 – Motivo principal de consulta
            if (respuestas.Any(r => r.id_pregunta == 1 && r.respuesta == 1)) // Prevención
                score += Convert.ToInt32(puntuacion.puntualidad) + Convert.ToInt32(puntuacion.claridad);

            if (respuestas.Any(r => r.id_pregunta == 1 && r.respuesta == 2)) // Molestias
                score += Convert.ToInt32(puntuacion.nivel_satisfaccion) + Convert.ToInt32(puntuacion.recomendacion);

            if (respuestas.Any(r => r.id_pregunta == 1 && r.respuesta == 3)) // Sensibilidad
                score += Convert.ToInt32(puntuacion.empatia) + Convert.ToInt32(puntuacion.claridad);

            if (respuestas.Any(r => r.id_pregunta == 1 && r.respuesta == 4)) // Estético
                score += Convert.ToInt32(puntuacion.cordialidad) + Convert.ToInt32(puntuacion.empatia);

            // Pregunta 2 – ¿Experiencias dentales previas?
            if (respuestas.Any(r => r.id_pregunta == 2 && r.respuesta == 1)) // Sí
                score += Convert.ToInt32(puntuacion.nivel_satisfaccion);

            // Pregunta 3 – ¿Experiencias negativas?
            if (respuestas.Any(r => r.id_pregunta == 3 && r.respuesta == 1)) // Sí
                score += Convert.ToInt32(puntuacion.empatia) * 2;

            // Pregunta 4 – Preocupaciones específicas
            if (respuestas.Any(r => r.id_pregunta == 4 && r.respuesta == 1)) // Mal aliento
                score += Convert.ToInt32(puntuacion.claridad);

            if (respuestas.Any(r => r.id_pregunta == 4 && r.respuesta == 2)) // Sarro
                score += Convert.ToInt32(puntuacion.puntualidad);

            if (respuestas.Any(r => r.id_pregunta == 4 && r.respuesta == 3)) // Caries
                score += Convert.ToInt32(puntuacion.nivel_satisfaccion) + Convert.ToInt32(puntuacion.recomendacion);

            if (respuestas.Any(r => r.id_pregunta == 4 && r.respuesta == 4)) // Mejorar la sonrisa
                score += Convert.ToInt32(puntuacion.cordialidad) + Convert.ToInt32(puntuacion.empatia);

            // Pregunta 5 – ¿Dispuesto a seguir tratamiento integral?
            if (respuestas.Any(r => r.id_pregunta == 5 && r.respuesta == 1)) // Sí
                score += Convert.ToInt32(puntuacion.claridad) + Convert.ToInt32(puntuacion.recomendacion);

            return score;
        }

        public async Task<List<ProfesionalesPuntuacionesModel>> ObtenerProfesionalesPuntuacion()
        {
            UtilitarioDal utilitarioDal = new UtilitarioDal(_config);
            List<ProfesionalesModel> listProfesionales = await utilitarioDal.ObtenerProfesionales();
            List<EspecialidadesModel> listEpecialidades = await utilitarioDal.ObtenerEspecialidades();
            List<ProfesionalesPuntuacionesModel> listPuntuaciones = new List<ProfesionalesPuntuacionesModel>();
            foreach (var item in listProfesionales)
            {
                ProfesionalesPuntuacionesModel PuntuacionesPorProfesional = await utilitarioDal.ObtenerProfesionalesPuntuaciones(item.id_profesional);

                ProfesionalesPuntuacionesModel PuntuacionProfesional = new();
                PuntuacionProfesional.id_profesional = item.id_profesional;

                PuntuacionProfesional.puntaje_general = PuntuacionesPorProfesional.puntaje_general;


                PuntuacionProfesional.cordialidad = PuntuacionesPorProfesional.cordialidad;
                PuntuacionProfesional.claridad = PuntuacionesPorProfesional.claridad;
                PuntuacionProfesional.empatia = PuntuacionesPorProfesional.empatia;
                PuntuacionProfesional.nivel_satisfaccion = PuntuacionesPorProfesional.nivel_satisfaccion;
                PuntuacionProfesional.recomendacion = PuntuacionesPorProfesional.recomendacion;
                PuntuacionProfesional.puntualidad = PuntuacionesPorProfesional.puntualidad;

                PuntuacionProfesional.nombres = item.nombres;
                PuntuacionProfesional.apellido_paterno = item.apellido_paterno;
                PuntuacionProfesional.apellido_materno = item.apellido_materno;
                var especialidad = listEpecialidades.Find(x => x.id_especialidad == item.id_especialidad);
                var descripcion = especialidad != null ? especialidad.descripcion_especialidad : "Sin especialidad";

                PuntuacionProfesional.especialidad = descripcion;

                listPuntuaciones.Add(PuntuacionProfesional);
            }
            return listPuntuaciones;

        }


    }
}