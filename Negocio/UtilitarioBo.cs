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
                     Puntaje = puntaje,
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
            if (puntuacion == null || respuestas == null) return 0;

            int score = 0;

            score += Convert.ToInt32(puntuacion.puntaje_general) * 2;

            // Pregunta 1 – Motivo principal de consulta
            var pregunta1Map = new Dictionary<int, Func<ProfesionalesPuntuacionesModel, int>>
    {
        { 1, p => Convert.ToInt32(p.puntualidad) + Convert.ToInt32(p.claridad) },                  // Prevención
        { 2, p => Convert.ToInt32(p.nivel_satisfaccion) + Convert.ToInt32(p.recomendacion) },      // Molestias
        { 3, p => Convert.ToInt32(p.empatia) + Convert.ToInt32(p.claridad) },                      // Sensibilidad
        { 4, p => Convert.ToInt32(p.cordialidad) + Convert.ToInt32(p.empatia) }                    // Estético
    };

            AgregarPuntajePorPregunta(respuestas, 1, pregunta1Map, puntuacion, ref score);

            // Pregunta 2 – Experiencias previas
            if (TieneRespuesta(respuestas, 2, 1))
                score += Convert.ToInt32(puntuacion.nivel_satisfaccion);

            // Pregunta 3 – Experiencias negativas
            if (TieneRespuesta(respuestas, 3, 1))
                score += Convert.ToInt32(puntuacion.empatia) * 2;

            // Pregunta 4 – Preocupaciones específicas
            var pregunta4Map = new Dictionary<int, Func<ProfesionalesPuntuacionesModel, int>>
    {
        { 1, p => Convert.ToInt32(p.claridad) },                                                         // Mal aliento
        { 2, p => Convert.ToInt32(p.puntualidad) },                                                      // Sarro
        { 3, p => Convert.ToInt32(p.nivel_satisfaccion) + Convert.ToInt32(p.recomendacion) },            // Caries
        { 4, p => Convert.ToInt32(p.cordialidad) + Convert.ToInt32(p.empatia) }                          // Mejorar sonrisa
    };

            AgregarPuntajePorPregunta(respuestas, 4, pregunta4Map, puntuacion, ref score);

            // Pregunta 5 – Tratamiento integral
            if (TieneRespuesta(respuestas, 5, 1))
                score += Convert.ToInt32(puntuacion.claridad) + Convert.ToInt32(puntuacion.recomendacion);

            return score;
        }

        private void AgregarPuntajePorPregunta(
            List<RespuestasInicialesModel> respuestas,
            int idPregunta,
            Dictionary<int, Func<ProfesionalesPuntuacionesModel, int>> mapa,
            ProfesionalesPuntuacionesModel puntuacion,
            ref int score)
        {
            var respuesta = respuestas.FirstOrDefault(r => r.id_pregunta == idPregunta);
            if (respuesta != null && mapa.TryGetValue(Convert.ToInt32(respuesta.respuesta), out var calculo))
            {
                score += calculo(puntuacion);
            }
        }

        private bool TieneRespuesta(List<RespuestasInicialesModel> respuestas, int idPregunta, int valorEsperado)
        {
            return respuestas.Any(r => r.id_pregunta == idPregunta && r.respuesta == valorEsperado);
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