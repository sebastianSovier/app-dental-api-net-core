using Datos;
using Microsoft.Extensions.Configuration;
using Modelo;
using Utilidades;

namespace Negocio
{
    public class PasswordBo
    {
        private readonly IConfiguration _config;
        private UtilidadesApiss util = new UtilidadesApiss();
        public PasswordBo(IConfiguration config)
        {
            _config = config;

        }


        public PasswordBo() { }


        public async Task<PasswordPacienteModel> ObtenerPasswordPaciente(string usuario_id)
        {
            PasswordPacienteModel password = new PasswordPacienteModel();
            PasswordDal passwordDal = new PasswordDal(_config);
            password = await passwordDal.ObtenerPasswordPaciente(usuario_id);
            return password;

        }
        public async Task<PasswordProfesionalModel> ObtenerPasswordProfesional(string usuario_id)
        {
            PasswordProfesionalModel password = new PasswordProfesionalModel();
            PasswordDal passwordDal = new PasswordDal(_config);
            password = await passwordDal.ObtenerPasswordProfesional(usuario_id);
            return password;

        }
        /*  public PasswordModels GenerarCodigoRecuperacion(PasswordModels passwordRequest)
          {
              passwordRequest = util.CleanObject(passwordRequest);

              PasswordDal passwordDal = new PasswordDal(_config);
              PasswordModels password = new();
              passwordRequest.cod_recover_password = util.generateRandomNumber();
              passwordDal.CodigoRecuperacion(passwordRequest);
              password.cod_recover_password = passwordRequest.cod_recover_password;
              return password;

          }
          public void CambioPassword(PasswordModels passwordRequest)
          {
              passwordRequest = util.CleanObject(passwordRequest);
              PasswordDal passwordDal = new PasswordDal(_config);

              passwordDal.CambioPassword(passwordRequest);


          }*/
        public void CrearPasswordPaciente(PasswordPacienteModel passwordRequest)
        {
            passwordRequest = util.CleanObject(passwordRequest);
            PasswordDal passwordDal = new PasswordDal(_config);
            passwordRequest.contrasena = util.HashPassword(passwordRequest.contrasena);
            passwordDal.CrearPasswordPaciente(passwordRequest);


        }
        public async Task<bool> CrearPasswordProfesional(PasswordProfesionalModel passwordRequest)
        {
            passwordRequest = util.CleanObject(passwordRequest);
            PasswordDal passwordDal = new PasswordDal(_config);
            passwordRequest.contrasena = util.HashPassword(passwordRequest.contrasena);

            await passwordDal.CrearPasswordProfesional(passwordRequest);
            return true;

        }
    }
}
