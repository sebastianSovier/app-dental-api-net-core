using Datos;
using Microsoft.Extensions.Configuration;
using Modelo;
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
            UsuarioDal usuarioDal = new UsuarioDal(_config);
            usuarioRequest.dv = util.getDv(usuarioRequest.rut);
            usuarioRequest.rut = util.getRutWithoutDv(usuarioRequest.rut);
            return await usuarioDal.CrearProfesional(util.CleanObject(usuarioRequest));


        }
    }
}