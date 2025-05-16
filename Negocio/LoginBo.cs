using Datos;
using Microsoft.Extensions.Configuration;
using Modelo;
using System.Globalization;
using Utilidades;
namespace Negocio
{
    public class LoginBo
    {
        private readonly IConfiguration _config;
        private UtilidadesApiss util = new UtilidadesApiss();
        public LoginBo(IConfiguration config)
        {
            _config = config;

        }


        public LoginBo() { }


        public async Task<PacienteModel> ObtenerPaciente(string rut)
        {
            PacienteModel usuario = new PacienteModel();
            UsuarioDal usuarioDal = new UsuarioDal(_config);
            string rutSinDv = util.getRutWithoutDv(rut);
            usuario = await usuarioDal.ObtenerPaciente(util.CleanObject(rutSinDv));
            return usuario;

        }
        public async Task<PacienteModel> ObtenerPacienteAdmin(string rut)
        {
            PacienteModel usuario = new PacienteModel();
            UsuarioDal usuarioDal = new UsuarioDal(_config);
            string rutSinDv = util.getRutWithoutDv(rut);
            usuario = await usuarioDal.ObtenerPacienteAdmin(util.CleanObject(rutSinDv));
            return usuario;

        }
        public async Task<bool> EliminarProfesional(string profesional_id)
        {
            PacienteModel usuario = new PacienteModel();
            UsuarioDal usuarioDal = new UsuarioDal(_config);
            AgendamientoDal agendamientoDal = new AgendamientoDal(_config);
            LoginBo loginBo = new LoginBo(_config);
            ModificarAgendamientoProfesionalModel agendamientoRequest = new();
            EmailService emailService = new EmailService(_config);
            agendamientoRequest.id_profesional = util.CleanObject(profesional_id);
            agendamientoRequest.fechaDesde = DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

            agendamientoRequest.fechaHasta = DateTime.Now
                                       .AddDays(60)
                                       .ToString("dd/MM/yyyy",
                                                 CultureInfo.InvariantCulture);
            DateTime fechaDesde = DateTime.ParseExact(agendamientoRequest.fechaDesde, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime fechaHasta = DateTime.ParseExact(agendamientoRequest.fechaHasta, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            for (var date = fechaDesde; date <= fechaHasta; date = date.AddDays(1))
            {
                HorasAgendadasRequestModel horasAgendadasRequest = new();
                horasAgendadasRequest.id_profesional = agendamientoRequest.id_profesional;
                horasAgendadasRequest.fechaDesde = date.ToString("dd/MM/yyyy");
                horasAgendadasRequest.fechaHasta = date.ToString("dd/MM/yyyy");
                List<HorasAgendadasDoctorModel> listaAgendamientos = await agendamientoDal.obtenerHorasAgendadasPorDoctor(horasAgendadasRequest);
                string inputFormat = "dd/MM/yyyy HH:mm:ss";

                ProfesionalModel profesional = await usuarioDal.ObtenerDoctorById(util.CleanObject(profesional_id));
                DateTime horaInicio = DateTime.Today.AddHours(9);
                DateTime ahora = DateTime.Now;
                DateTime horaFin = DateTime.Today.AddHours(18);
                agendamientoRequest.fecha = date.ToString("yyyy/MM/dd");
                for (var hora = horaInicio; hora <= horaFin; hora = hora.AddMinutes(30))
                {
                    HorasAgendadasDoctorModel horaEncontrada = listaAgendamientos.Find(x => x.hora == hora.ToString("HH:mm") && DateTime.ParseExact(x.fecha, inputFormat, CultureInfo.InvariantCulture).ToString("dd/MM/yyyy") == date.ToString("dd/MM/yyyy"));
                    if (horaEncontrada != null && !string.IsNullOrEmpty(horaEncontrada.id_agendamiento) && ahora < hora)
                    {
                        PacienteModel paciente = await loginBo.ObtenerPacienteById(horaEncontrada.id_paciente);


                        await agendamientoDal.EliminarAgendamientoPaciente(horaEncontrada.id_agendamiento);

                        await emailService.SendEmailAsync(paciente.correo, "cita cancelada", $"<b>cita cancelada con el Doctor {profesional.nombres} {profesional.apellido_paterno} {profesional.apellido_materno} , a las {hora.ToString("HH:mm")} , el dia {date.ToString("dd/MM/yyyy")} , por motivos de fuerza mayor , por favor agende su hora nuevamente.</ b >");

                    }

                }

            }

            return await usuarioDal.EliminarProfesional(util.CleanObject(profesional_id));

        }
        public async Task<bool> EliminarPaciente(string paciente_id)
        {
            PacienteModel usuario = new PacienteModel();
            UsuarioDal usuarioDal = new UsuarioDal(_config);
            return await usuarioDal.EliminarPaciente(util.CleanObject(paciente_id));

        }
        public async Task<bool> ModificarPerfilPaciente(string paciente_id, string perfil)
        {
            PacienteModel usuario = new PacienteModel();
            UsuarioDal usuarioDal = new UsuarioDal(_config);
            return await usuarioDal.ModificarPerfilPaciente(util.CleanObject(paciente_id), util.CleanObject(perfil));

        }
        public async Task<bool> ModificarPerfilProfesional(string profesional_id, string perfil)
        {
            PacienteModel usuario = new PacienteModel();
            UsuarioDal usuarioDal = new UsuarioDal(_config);
            return await usuarioDal.ModificarPerfilProfesional(util.CleanObject(profesional_id), util.CleanObject(perfil));

        }
        public async Task<PacienteModel> ObtenerPacienteById(string id_paciente)
        {
            PacienteModel usuario = new PacienteModel();
            UsuarioDal usuarioDal = new UsuarioDal(_config);
            usuario = await usuarioDal.ObtenerPacienteById(util.CleanObject(id_paciente));
            return usuario;

        }
        public void GuardarRespuestasIniciales(RespuestasInicialesModel respuestasInicialesModel)
        {
            respuestasInicialesModel = util.CleanObject(respuestasInicialesModel);

            PacienteModel usuario = new PacienteModel();
            UsuarioDal usuarioDal = new UsuarioDal(_config);
            usuarioDal.GuardarRespuestasPersonales(util.CleanObject(respuestasInicialesModel));

        }
        public async Task<ProfesionalModel> ObtenerProfesional(string rut)
        {
            ProfesionalModel usuario = new ProfesionalModel();
            UsuarioDal usuarioDal = new UsuarioDal(_config);
            string rutSinDv = util.getRutWithoutDv(rut);
            usuario = await usuarioDal.ObtenerProfesional(util.CleanObject(rutSinDv));
            return usuario;

        }
        public async Task<ProfesionalModel> ObtenerProfesionalAdmin(string rut)
        {
            ProfesionalModel usuario = new ProfesionalModel();
            UsuarioDal usuarioDal = new UsuarioDal(_config);
            string rutSinDv = util.getRutWithoutDv(rut);
            usuario = await usuarioDal.ObtenerProfesionalAdmin(util.CleanObject(rutSinDv));
            return usuario;

        }
        public async Task<ProfesionalModel> ObtenerAdministrador(string rut)
        {
            ProfesionalModel usuario = new ProfesionalModel();
            UsuarioDal usuarioDal = new UsuarioDal(_config);
            string rutSinDv = util.getRutWithoutDv(rut);
            usuario = await usuarioDal.ObtenerAdministrador(util.CleanObject(rutSinDv));
            return usuario;

        }
        public async Task<List<ProfesionalModel>> ObtenerTodosProfesionales()
        {
            List<ProfesionalModel> usuario = new();
            UsuarioDal usuarioDal = new UsuarioDal(_config);
            UtilitarioDal utilitarioDal = new UtilitarioDal(_config);
            usuario = await usuarioDal.ObtenerProfesionalesAdministrador();
            List<EspecialidadesModel> listEpecialidades = await utilitarioDal.ObtenerEspecialidades();

            foreach (var item in usuario)
            {
                var especialidad = listEpecialidades.Find(x => x.id_especialidad == item.id_especialidad.ToString());
                var descripcion = especialidad != null ? especialidad.descripcion_especialidad : "Sin especialidad";

                item.especialidad = descripcion;
            }

            return usuario;

        }
        public async Task<List<PacienteModel>> ObtenerTodosPacientes()
        {
            List<PacienteModel> usuario = new();
            UsuarioDal usuarioDal = new UsuarioDal(_config);
            usuario = await usuarioDal.ObtenerPacientesAdministrador();
            return usuario;

        }
        public async Task<bool> CrearPaciente(PacienteModel usuarioRequest)
        {
            UsuarioDal usuarioDal = new UsuarioDal(_config);
            usuarioRequest.dv = util.getDv(usuarioRequest.rut);
            usuarioRequest.rut = util.getRutWithoutDv(usuarioRequest.rut);
            await usuarioDal.CrearPaciente(util.CleanObject(usuarioRequest));
            return true;


        }
        public async Task<long> CrearProfesional(ProfesionalModel usuarioRequest)
        {
            usuarioRequest = util.CleanObject(usuarioRequest);

            UsuarioDal usuarioDal = new UsuarioDal(_config);
            usuarioRequest.dv = util.getDv(usuarioRequest.rut);
            usuarioRequest.rut = util.getRutWithoutDv(usuarioRequest.rut);
            return await usuarioDal.CrearProfesional(util.CleanObject(usuarioRequest));


        }
    }
}