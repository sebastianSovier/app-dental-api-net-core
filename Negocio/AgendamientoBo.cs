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
                DateTime fecha = DateTime.Parse(fechaIso);
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
                bool result = await agendamientoDal.ObtenerPuntuacionPorPacienteProfesional(request.id_paciente, item.id_profesional, item.id_agendamiento.ToString());
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
            EmailService emailService = new EmailService(_config);
            UsuarioDal usuarioDal = new(_config);
            HorasAgendadasDoctorModel agendamiento = await agendamientoDal.obtenerAgendamientoPorId(request.id_agendamiento.ToString());
            PacienteModel paciente = await loginBo.ObtenerPacienteById(agendamiento.id_paciente.ToString());
            ProfesionalModel profesional = await usuarioDal.ObtenerDoctorById(agendamiento.id_profesional);
            await agendamientoDal.EliminarAgendamientoPaciente(request.id_agendamiento, agendamiento.id_paciente);
            DateTime fecha = DateTime.ParseExact(agendamiento.fecha, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

            string fechaFormateada = fecha.ToString("dd/MM/yyyy");
            await emailService.SendEmailAsync(paciente.correo, "cita médica cancelada", $"<b>Cita eliminada con el Doctor {profesional.nombres} {profesional.apellido_paterno} {profesional.apellido_materno} , a las {agendamiento.hora} , el dia {fechaFormateada} , por favor agende nuevamente.</ b >");

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

            DateTime fecha = DateTime.ParseExact(agendamiento.fecha, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

            string fechaFormateada = fecha.ToString("dd/MM/yyyy");
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
                DateTime fecha = DateTime.Parse(fechaIso);
                string fechaFormateada = fecha.ToString("dd/MM/yyyy");
                item.fecha = fechaFormateada;


            }
            List<HorasAgendadasDoctorModel> citasOrdenadas = listaAgendamientos
           .OrderBy(c => DateTime.ParseExact($"{c.fecha} {c.hora}", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture))
           .ToList();
            return listaAgendamientos;

        }
        public async Task<List<HorasAgendadasDoctorModel>?> ObtenerHorasProximasAgendadasPorDoctor(HorasAgendadasRequestModel request)
        {
            request = utils.CleanObject(request);
            AgendamientoDal agendamientoDal = new AgendamientoDal(_config);
            LoginBo loginBo = new LoginBo(_config);

            List<HorasAgendadasDoctorModel> listaAgendamientos = await agendamientoDal.obtenerHorasProximasAgendadasPorDoctor(request);

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
                DateTime fecha = DateTime.Parse(fechaIso);
                string fechaFormateada = fecha.ToString("dd/MM/yyyy");
                item.fecha = fechaFormateada;


            }
            List<HorasAgendadasDoctorModel> citasOrdenadas = listaAgendamientos
           .OrderBy(c => DateTime.ParseExact($"{c.fecha} {c.hora}", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture))
           .ToList();
            return listaAgendamientos;

        }
        public async Task<List<HorasAgendadasDoctorModel>?> ObtenerDiaSinDisponibilidadPorDoctor(HorasAgendadasRequestModel request)
        {
            request = utils.CleanObject(request);
            AgendamientoDal agendamientoDal = new AgendamientoDal(_config);
            LoginBo loginBo = new LoginBo(_config);

            List<HorasAgendadasDoctorModel> listaAgendamientos = await agendamientoDal.obtenerDiaSinDisponibilidadPorDoctor(request);



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
                    DateTime fecha = DateTime.Parse(fechaIso);
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
        public async Task<List<ObtenerTratamientoConsultaPaciente>?> ObtenerTratamientoConsultaPaciente(ObtenerTratamientoConsultaPacienteModel request, string perfil)
        {
            request = utils.CleanObject(request);
            AgendamientoDal agendamientoDal = new AgendamientoDal(_config);
            LoginBo loginBo = new LoginBo(_config);

            CrearEstadoAgendamientoModel estadoAgendamientoRequest = new CrearEstadoAgendamientoModel();
            estadoAgendamientoRequest.id_agendamiento = request.id_agendamiento.ToString();

            HorasAgendadasDoctorModel agendamiento = await agendamientoDal.obtenerAgendamientoPorId(estadoAgendamientoRequest.id_agendamiento.ToString());
            if (perfil.Equals("Profesional"))
            {
                request.id_paciente = agendamiento.id_paciente;
            }
            request.id_agendamiento = agendamiento.id_agendamiento;
            request.id_profesional = agendamiento.id_profesional;
            List<ObtenerTratamientoConsultaPaciente> listTratamientos = await agendamientoDal.ObtenerTratamientoConsultaPaciente(request);
            return listTratamientos;

        }
        public async Task<bool> CrearAgendamiento(CrearAgendamientoModel agendamientoRequest, string correo)
        {
            agendamientoRequest = utils.CleanObject(agendamientoRequest);
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

            DateTime fecha = DateTime.ParseExact(agendamiento.fecha, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

            string fechaFormateada = fecha.ToString("dd/MM/yyyy");
            await emailService.SendEmailAsync(correo, "cita realizada con éxito", $"<b>cita agendada con el Doctor {profesional.nombres} {profesional.apellido_paterno} {profesional.apellido_materno} , a las {agendamiento.hora} , el dia {fechaFormateada}</ b >");
            return true;


        }

        public async Task<bool> ModificarAgendamientoPorProfesional(ModificarAgendamientoProfesionalModel agendamientoRequest)
        {
            agendamientoRequest = utils.CleanObject(agendamientoRequest);
            AgendamientoDal agendamientoDal = new AgendamientoDal(_config);
            UsuarioDal usuarioDal = new UsuarioDal(_config);
            LoginBo loginBo = new LoginBo(_config);
            EmailService emailService = new EmailService(_config);
            DateTime fechaDesde = DateTime.ParseExact(agendamientoRequest.fechaDesde, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime fechaHasta = DateTime.ParseExact(agendamientoRequest.fechaHasta, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            for (var date = fechaDesde; date <= fechaHasta; date = date.AddDays(1))
            {
                ProfesionalModel profesional = await usuarioDal.ObtenerDoctorById(agendamientoRequest.id_profesional);
                DateTime horaInicio = DateTime.Today.AddHours(9);
                DateTime horaFin = DateTime.Today.AddHours(18);
                agendamientoRequest.fecha = date.ToString("yyyy/MM/dd");
                for (var hora = horaInicio; hora <= horaFin; hora = hora.AddMinutes(30))
                {
                    agendamientoRequest.hora = hora.ToString("HH:mm");
                    bool idAgendamiento = await agendamientoDal.modificarDisponibilidadPorProfesional(agendamientoRequest);


                }

                HorasAgendadasRequestModel horasAgendadasRequest = new();
                horasAgendadasRequest.id_profesional = agendamientoRequest.id_profesional;
                horasAgendadasRequest.fechaDesde = date.ToString("dd/MM/yyyy");
                horasAgendadasRequest.fechaHasta = date.ToString("dd/MM/yyyy");
                List<HorasAgendadasDoctorModel> listaAgendamientos = await agendamientoDal.obtenerHorasAgendadasPorDoctor(horasAgendadasRequest);

                foreach (var item in listaAgendamientos)
                {
                    DateTime fecha = DateTime.ParseExact(item.fecha, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                    string fechaFormateada = fecha.ToString("dd/MM/yyyy");

                    PacienteModel paciente = await loginBo.ObtenerPacienteById(item.id_paciente);

                    await emailService.SendEmailAsync(paciente.correo, "cita cancelada", $"<b>cita cancelada con el Doctor {profesional.nombres} {profesional.apellido_paterno} {profesional.apellido_materno} , a las {item.hora} , el dia {fechaFormateada} , por motivos de fuerza mayor , por favor agende su hora nuevamente.</ b >");


                }

            }
            return true;


        }

        public async Task<bool> ModificarAgendamiento(ModificarAgendamientoModel agendamientoRequest, string correo)
        {
            agendamientoRequest = utils.CleanObject(agendamientoRequest);
            AgendamientoDal agendamientoDal = new AgendamientoDal(_config);
            UsuarioDal usuarioDal = new UsuarioDal(_config);
            LoginBo loginBo = new LoginBo(_config);
            EmailService emailService = new EmailService(_config);
            HorasAgendadasDoctorModel agendamiento = await agendamientoDal.obtenerAgendamientoPorId(agendamientoRequest.id_agendamiento.ToString());
            ProfesionalModel profesional = await usuarioDal.ObtenerDoctorById(agendamiento.id_profesional);
            agendamientoRequest.id_profesional = profesional.id_profesional.ToString();
            await agendamientoDal.ModificarAgendamiento(agendamientoRequest);

            DateTime fecha = DateTime.ParseExact(agendamiento.fecha, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

            string fechaFormateada = fecha.ToString("dd/MM/yyyy");
            await emailService.SendEmailAsync(correo, "cita modificada con éxito", $"<b>cita agendada con el Doctor {profesional.nombres} {profesional.apellido_paterno} {profesional.apellido_materno} , a las {agendamiento.hora} , el dia {fechaFormateada}</ b >");
            return true;


        }
        public async Task<bool> ModificarDerivacionAgendamiento(ModificarAgendamientoModel agendamientoRequest, string correo)
        {
            agendamientoRequest = utils.CleanObject(agendamientoRequest);

            AgendamientoDal agendamientoDal = new AgendamientoDal(_config);
            UsuarioDal usuarioDal = new UsuarioDal(_config);
            LoginBo loginBo = new LoginBo(_config);
            EmailService emailService = new EmailService(_config);
            HorasAgendadasDoctorModel agendamiento = await agendamientoDal.obtenerAgendamientoPorId(agendamientoRequest.id_agendamiento.ToString());
            ProfesionalModel profesional = await usuarioDal.ObtenerDoctorById(agendamiento.id_profesional);
            agendamientoRequest.id_paciente = agendamiento.id_paciente;
            await agendamientoDal.ModificarDerivacionAgendamiento(agendamientoRequest);

            DateTime fecha = DateTime.ParseExact(agendamiento.fecha, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

            string fechaFormateada = fecha.ToString("dd/MM/yyyy");
            await emailService.SendEmailAsync(correo, "cita modificada con éxito", $"<b>cita agendada con el Doctor {profesional.nombres} {profesional.apellido_paterno} {profesional.apellido_materno} , a las {agendamiento.hora} , el dia {fechaFormateada}</ b >");
            return true;


        }
        public async Task<bool> CrearPuntuacionAtencionDoctor(CrearPuntuacionAtencionDoctorModel crearPuntuacionRequest)
        {
            crearPuntuacionRequest = utils.CleanObject(crearPuntuacionRequest);

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
            cargarImagenExamenModel = utils.CleanObject(cargarImagenExamenModel);

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
            consultaMedicaRequest = utils.CleanObject(consultaMedicaRequest);

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
            if (File.Exists("orden_medica.pdf"))
            {
                File.Delete("orden_medica.pdf");
            }
            await emailService.SendEmailAsync(paciente.correo, "Resumen de la atención", $"<b>Sr {paciente.nombres} {paciente.apellido_paterno} {paciente.apellido_materno}, se adjunta orden medica , según consulta del dia {DateTime.Now.ToString("dd/MM/yyyy")}</ b >", attachments);
            await emailService.SendEmailAsync(paciente.correo, "Evaluación del profesional", $"<b>Sr {paciente.nombres} {paciente.apellido_paterno} {paciente.apellido_materno}, recuerde que en su portal, puede evaluar la atención recibida por el profesional , según consulta del dia {DateTime.Now.ToString("dd/MM/yyyy")}</ b >");
            return true;


        }
    }
}
