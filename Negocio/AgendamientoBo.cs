using Datos;
using Microsoft.Extensions.Configuration;
using Modelo;
using System.Globalization;
using Utilidades;

namespace Negocio
{
    public class AgendamientoBo
    {
        UtilidadesApiss utils = new UtilidadesApiss();
        private readonly IConfiguration _config;

        public AgendamientoBo(IConfiguration config)
        {
            _config = config;


        }
        public async Task<List<ObtenerAgendamientoPacienteModel>?> ObtenerAgendamientoPorPaciente(ObtenerAgendamientoPorPacienteModel request)
        {
            request = utils.CleanObject(request);
            AgendamientoDal agendamientoDal = new AgendamientoDal(_config);
            LoginBo loginBo = new LoginBo(_config);
            UtilitarioDal utilitarioDal = new UtilitarioDal(_config);

            List<ObtenerAgendamientoPacienteModel> listaAgendamientos = await agendamientoDal.ObtenerAgendamientosFuturosPorPaciente(request.id_paciente);
            List<EspecialidadesModel> listEpecialidades = await utilitarioDal.ObtenerEspecialidades();
            foreach (var item in listaAgendamientos)
            {
                string fechaIso = item.fecha;
                DateTime fecha = DateTime.Parse(fechaIso); // O usa DateTime.ParseExact si quieres mayor control
                string fechaFormateada = fecha.ToString("dd/MM/yyyy");
                item.fecha = fechaFormateada;
                item.especialidad = listEpecialidades.Find(x => x.id_especialidad == item.id_especialidad)!.descripcion_especialidad;
            }
            List<ObtenerAgendamientoPacienteModel> citasOrdenadas = listaAgendamientos
            .OrderBy(c => DateTime.ParseExact($"{c.fecha} {c.hora}", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture))
            .ToList();

            return citasOrdenadas;
        }
        public async Task<List<ObtenerAgendamientoPacienteModel>?> ObtenerHistoricoAgendamientoPorPaciente(ObtenerAgendamientoPorPacienteModel request)
        {
            request = utils.CleanObject(request);
            AgendamientoDal agendamientoDal = new AgendamientoDal(_config);
            LoginBo loginBo = new LoginBo(_config);
            UtilitarioDal utilitarioDal = new UtilitarioDal(_config);

            List<ObtenerAgendamientoPacienteModel> listaAgendamientos = await agendamientoDal.ObtenerAgendamientoPorPaciente(request.id_paciente);
            List<EspecialidadesModel> listEpecialidades = await utilitarioDal.ObtenerEspecialidades();

            foreach (var item in listaAgendamientos)
            {
                bool result = await agendamientoDal.ObtenerPuntuacionPorPacienteProfesional(request.id_paciente, item.id_profesional);
                item.puntuacion_realizada = result;
                string fechaIso = item.fecha;
                DateTime fecha = DateTime.Parse(fechaIso); // O usa DateTime.ParseExact si quieres mayor control
                string fechaFormateada = fecha.ToString("dd/MM/yyyy");
                item.fecha = fechaFormateada;
                item.especialidad = listEpecialidades.Find(x => x.id_especialidad == item.id_especialidad)!.descripcion_especialidad;
            }
            List<ObtenerAgendamientoPacienteModel> citasOrdenadas = listaAgendamientos
            .OrderBy(c => DateTime.ParseExact($"{c.fecha} {c.hora}", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture))
            .ToList();

            return citasOrdenadas;
        }

        public async Task<bool> EliminarAgendamientoPaciente(EliminarAgendamientoPacienteModel request)
        {
            request = utils.CleanObject(request);
            AgendamientoDal agendamientoDal = new AgendamientoDal(_config);
            LoginBo loginBo = new LoginBo(_config);

            await agendamientoDal.EliminarAgendamientoPaciente(request.id_agendamiento, request.id_paciente);

            return true;
        }
        public async Task<bool> ConfirmarAgendamientoPaciente(EliminarAgendamientoPacienteModel request)
        {
            request = utils.CleanObject(request);
            AgendamientoDal agendamientoDal = new AgendamientoDal(_config);
            LoginBo loginBo = new LoginBo(_config);
            UsuarioDal usuarioDal = new(_config);
            await agendamientoDal.ConfirmarAgendamientoPaciente(request.id_agendamiento);
            HorasAgendadasDoctorModel agendamiento = await agendamientoDal.obtenerAgendamientoPorId(request.id_agendamiento.ToString());
            PacienteModel paciente = await loginBo.ObtenerPacienteById(agendamiento.id_paciente.ToString());
            ProfesionalModel profesional = await usuarioDal.ObtenerDoctorById(agendamiento.id_profesional);
            EmailService emailService = new EmailService(_config);


            await emailService.SendEmailAsync(paciente.correo, "cita confirmada con éxito", $"<b>Cita confirmada con el Doctor {profesional.nombres} {profesional.apellido_paterno} {profesional.apellido_materno} , a las {agendamiento.hora} , el dia {agendamiento.fecha}</ b >");
            return true;
        }
        public async Task<List<HorasAgendadasDoctorModel>?> ObtenerHorasAgendadasPorDoctor(HorasAgendadasRequestModel request)
        {
            request = utils.CleanObject(request);
            AgendamientoDal agendamientoDal = new AgendamientoDal(_config);
            LoginBo loginBo = new LoginBo(_config);

            List<HorasAgendadasDoctorModel> listaAgendamientos = await agendamientoDal.obtenerHorasAgendadasPorDoctor(request);

            foreach (var item in listaAgendamientos)
            {
                if (request.tipoUsuario.Equals("Profesional"))
                {
                    PacienteModel paciente = await loginBo.ObtenerPacienteById(item.id_paciente);
                    item.apellido_materno = paciente.apellido_materno;
                    item.apellido_paterno = paciente.apellido_paterno;
                    item.nombre_paciente = paciente.nombres;
                }
                string fechaIso = item.fecha;
                DateTime fecha = DateTime.Parse(fechaIso); // O usa DateTime.ParseExact si quieres mayor control
                string fechaFormateada = fecha.ToString("dd/MM/yyyy");
                item.fecha = fechaFormateada;


            }
            List<HorasAgendadasDoctorModel> citasOrdenadas = listaAgendamientos
           .OrderBy(c => DateTime.ParseExact($"{c.fecha} {c.hora}", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture))
           .ToList();
            return listaAgendamientos;

        }
        public async Task<List<HorasAgendadasDoctorModel>?> ObtenerHistoricoHorasAgendadasPorDoctor(HorasAgendadasRequestModel request)
        {
            request = utils.CleanObject(request);
            AgendamientoDal agendamientoDal = new AgendamientoDal(_config);
            LoginBo loginBo = new LoginBo(_config);

            List<HorasAgendadasDoctorModel> listaAgendamientos = await agendamientoDal.obtenerHistoricoHorasAgendadasPorDoctor(request);
            if (request.tipoUsuario.Equals("Profesional"))
            {
                foreach (var item in listaAgendamientos)
                {
                    PacienteModel paciente = await loginBo.ObtenerPacienteById(item.id_paciente);
                    item.apellido_materno = paciente.apellido_materno;
                    string fechaIso = item.fecha;
                    DateTime fecha = DateTime.Parse(fechaIso); // O usa DateTime.ParseExact si quieres mayor control
                    string fechaFormateada = fecha.ToString("dd/MM/yyyy");
                    item.fecha = fechaFormateada;
                    item.apellido_paterno = paciente.apellido_paterno;
                    item.nombre_paciente = paciente.nombres;
                }
            }
            List<HorasAgendadasDoctorModel> citasOrdenadas = listaAgendamientos
           .OrderBy(c => DateTime.ParseExact($"{c.fecha} {c.hora}", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture))
           .ToList();
            return listaAgendamientos;

        }
        public async Task<List<ObtenerTratamientoConsultaPaciente>?> ObtenerTratamientoConsultaPaciente(ObtenerTratamientoConsultaPacienteModel request)
        {
            request = utils.CleanObject(request);
            AgendamientoDal agendamientoDal = new AgendamientoDal(_config);
            LoginBo loginBo = new LoginBo(_config);

            CrearEstadoAgendamientoModel estadoAgendamientoRequest = new CrearEstadoAgendamientoModel();
            estadoAgendamientoRequest.id_agendamiento = request.id_agendamiento.ToString();

            HorasAgendadasDoctorModel agendamiento = await agendamientoDal.obtenerAgendamientoPorId(estadoAgendamientoRequest.id_agendamiento.ToString());
            request.id_agendamiento = agendamiento.id_agendamiento;
            request.id_profesional = agendamiento.id_profesional;
            List<ObtenerTratamientoConsultaPaciente> listTratamientos = await agendamientoDal.ObtenerTratamientoConsultaPaciente(request);
            return listTratamientos;

        }
        public async Task<bool> CrearAgendamiento(CrearAgendamientoModel agendamientoRequest, string correo)
        {
            AgendamientoDal agendamientoDal = new AgendamientoDal(_config);
            UsuarioDal usuarioDal = new UsuarioDal(_config);
            LoginBo loginBo = new LoginBo(_config);
            EmailService emailService = new EmailService(_config);
            long idAgendamiento = await agendamientoDal.CrearAgendamiento(agendamientoRequest);
            if (idAgendamiento == 0)
            {
                return false;
            }
            CrearEstadoAgendamientoModel estadoAgendamientoRequest = new CrearEstadoAgendamientoModel();
            estadoAgendamientoRequest.id_agendamiento = idAgendamiento.ToString();
            await agendamientoDal.CrearEstadoAgendamiento(estadoAgendamientoRequest);
            HorasAgendadasDoctorModel agendamiento = await agendamientoDal.obtenerAgendamientoPorId(idAgendamiento.ToString());
            ProfesionalModel profesional = await usuarioDal.ObtenerDoctorById(agendamiento.id_profesional);


            await emailService.SendEmailAsync(correo, "cita realizada con éxito", $"<b>cita agendada con el Doctor {profesional.nombres} {profesional.apellido_paterno} {profesional.apellido_materno} , a las {agendamiento.hora} , el dia {agendamiento.fecha}</ b >");
            return true;


        }
        public async Task<bool> ModificarAgendamiento(ModificarAgendamientoModel agendamientoRequest, string correo)
        {
            AgendamientoDal agendamientoDal = new AgendamientoDal(_config);
            UsuarioDal usuarioDal = new UsuarioDal(_config);
            LoginBo loginBo = new LoginBo(_config);
            EmailService emailService = new EmailService(_config);
            HorasAgendadasDoctorModel agendamiento = await agendamientoDal.obtenerAgendamientoPorId(agendamientoRequest.id_agendamiento.ToString());
            ProfesionalModel profesional = await usuarioDal.ObtenerDoctorById(agendamiento.id_profesional);
            agendamientoRequest.id_profesional = profesional.id_profesional.ToString();
            long idAgendamiento = await agendamientoDal.ModificarAgendamiento(agendamientoRequest);
            if (idAgendamiento == 0)
            {
                return false;
            }

            await emailService.SendEmailAsync(correo, "cita modificada con éxito", $"<b>cita agendada con el Doctor {profesional.nombres} {profesional.apellido_paterno} {profesional.apellido_materno} , a las {agendamiento.hora} , el dia {agendamiento.fecha}</ b >");
            return true;


        }
        public async Task<bool> CrearPuntuacionAtencionDoctor(CrearPuntuacionAtencionDoctorModel crearPuntuacionRequest)
        {
            AgendamientoDal agendamientoDal = new AgendamientoDal(_config);
            LoginBo loginBo = new LoginBo(_config);

            HorasAgendadasDoctorModel agendamiento = await agendamientoDal.obtenerAgendamientoPorId(crearPuntuacionRequest.id_agendamiento);
            if (agendamiento == null)
            {
                return false;
            }
            crearPuntuacionRequest.id_profesional = agendamiento.id_profesional;
            crearPuntuacionRequest.id_paciente = agendamiento.id_paciente;
            crearPuntuacionRequest.id_agendamiento = agendamiento.id_agendamiento;
            if (crearPuntuacionRequest.claridad != null && crearPuntuacionRequest.cordialidad != null && crearPuntuacionRequest.empatia != null &&
                   crearPuntuacionRequest.nivel_satisfaccion != null && crearPuntuacionRequest.recomendacion != null && crearPuntuacionRequest.puntualidad != null)
            {
                var puntajeGeneral = new[] {
                    Convert.ToInt64(crearPuntuacionRequest.cordialidad),
                    Convert.ToInt64(crearPuntuacionRequest.claridad),
                    Convert.ToInt64(crearPuntuacionRequest.empatia),
                    Convert.ToInt64(crearPuntuacionRequest.nivel_satisfaccion),
                    Convert.ToInt64(crearPuntuacionRequest.recomendacion),
                    Convert.ToInt64(crearPuntuacionRequest.puntualidad)
                    }.Average();
                crearPuntuacionRequest.puntaje_general = Math.Round(puntajeGeneral, 0).ToString();
            }
            await agendamientoDal.CrearPuntuacionPorAtencionDoctor(crearPuntuacionRequest);

            return true;


        }
        public async Task<bool> CargarImagenesExamenes(List<CargarImagenExamenModel> cargarImagenExamenModel, string id_agendamiento)
        {
            AgendamientoDal agendamientoDal = new AgendamientoDal(_config);
            LoginBo loginBo = new LoginBo(_config);

            HorasAgendadasDoctorModel agendamiento = await agendamientoDal.obtenerAgendamientoPorId(id_agendamiento);
            if (agendamiento == null)
            {
                return false;
            }

            string idConsultaMedica = await agendamientoDal.ObtenerConsultaMedica(agendamiento.id_paciente, agendamiento.id_profesional, agendamiento.id_agendamiento);
            if (string.IsNullOrEmpty(idConsultaMedica))
            {
                return false;
            }
            await agendamientoDal.deleteImagenesConsulta(idConsultaMedica);
            foreach (var item in cargarImagenExamenModel)
            {
                item.id_consulta_medica = idConsultaMedica;
                await agendamientoDal.CargarImagenesExamen(item);

            }

            return true;


        }
        public async Task<List<ObtenerImagenExamenModel>> ObtenerImagenesExamenesCargadas(string id_agendamiento)
        {
            AgendamientoDal agendamientoDal = new AgendamientoDal(_config);
            LoginBo loginBo = new LoginBo(_config);

            HorasAgendadasDoctorModel agendamiento = await agendamientoDal.obtenerAgendamientoPorId(id_agendamiento);
            if (agendamiento == null)
            {
                return null;
            }

            string idConsultaMedica = await agendamientoDal.ObtenerConsultaMedica(agendamiento.id_paciente, agendamiento.id_profesional, agendamiento.id_agendamiento);
            if (string.IsNullOrEmpty(idConsultaMedica))
            {
                return null;
            }

            return await agendamientoDal.obtenerImagenesExamenesMedico(idConsultaMedica);

        }

        public async Task<bool> CrearConsultaMedica(GuardarConsultaMedicaModel consultaMedicaRequest, string rut_profesional)
        {
            AgendamientoDal agendamientoDal = new AgendamientoDal(_config);
            LoginBo loginBo = new LoginBo(_config);
            EmailService emailService = new EmailService(_config);

            HorasAgendadasDoctorModel agendamiento = await agendamientoDal.obtenerAgendamientoPorId(consultaMedicaRequest.id_agendamiento);
            if (agendamiento == null)
            {
                return false;
            }
            CrearEstadoAgendamientoModel estadoAgendamientoRequest = new CrearEstadoAgendamientoModel();
            consultaMedicaRequest.id_profesional = agendamiento.id_profesional;
            consultaMedicaRequest.id_paciente = agendamiento.id_paciente;
            long idConsultaMedica = await agendamientoDal.GuardarConsultaMedica(consultaMedicaRequest);
            PacienteModel paciente = await loginBo.ObtenerPacienteById(agendamiento.id_paciente);
            ProfesionalModel profesional = await loginBo.ObtenerProfesional(rut_profesional);
            long sumaTotalTratamiento = 0;
            foreach (var item in consultaMedicaRequest.tratamientos)
            {
                sumaTotalTratamiento = sumaTotalTratamiento + Convert.ToInt64(item.valor);

            }
            PresupuestoTotalModel totalPresupusto = new();
            totalPresupusto.id_consulta_medica = idConsultaMedica.ToString();
            totalPresupusto.total = sumaTotalTratamiento.ToString();
            long idPresupuesto = await agendamientoDal.GuardarPresupuestoConsulta(totalPresupusto);
            foreach (var item in consultaMedicaRequest.tratamientos)
            {
                item.id_presupuesto_consulta = idPresupuesto.ToString();
                await agendamientoDal.GuardarTratamiento(item);
            }
            await agendamientoDal.ConsultaRealizada(agendamiento.id_agendamiento);
            var pdfBytes = utils.GeneratePdf(profesional, paciente, consultaMedicaRequest);

            await File.WriteAllBytesAsync("orden_medica.pdf", pdfBytes);
            using var pdf = new MemoryStream(pdfBytes);
            var attachments = new List<(Stream, string, string)>
            {
             (pdf, "documento.pdf", "application/pdf")
            };
            await emailService.SendEmailAsync(paciente.correo, "Se envia Orden medica ", $"<b>Sr {paciente.nombres} {paciente.apellido_paterno} {paciente.apellido_materno}, se adjunta orden medica , según consulta del dia {DateTime.Now.ToString("dd/MM/yyyy")}</ b >", attachments);

            return true;


        }
    }
}
