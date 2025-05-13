using Microsoft.Extensions.Configuration;
using Modelo;
using MySql.Data.MySqlClient;
using Utilidades;

namespace Datos
{
    public class PasswordDal
    {
        private readonly IConfiguration _config;
        MySqlConexion? mysql = null;
        UtilidadesApiss utils = new UtilidadesApiss();

        public PasswordDal(IConfiguration config)
        {
            _config = config;
            mysql = new MySqlConexion(_config);
        }
        public async Task<PasswordPacienteModel> ObtenerPasswordPaciente(string usuarioRequest)
        {
            using MySqlConnection conexion = await mysql!.getConexion("bd1");
            try
            {
                PasswordPacienteModel usuario = new PasswordPacienteModel();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conexion;
                cmd.CommandText = $"select contrasena,id_paciente,id_contrasena_paciente from contrasena_paciente where id_paciente = ?id_paciente order by id_contrasena_paciente;";
                cmd.Parameters.Add("?id_paciente", MySqlDbType.VarChar).Value = usuarioRequest;
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    usuario.contrasena = reader["contrasena"].ToString();
                    usuario.id_paciente = Convert.ToInt64(reader["id_paciente"]);
                    usuario.id_contrasena = Convert.ToInt64(reader["id_contrasena_paciente"]);

                }

                return usuario;
            }
            catch (Exception ex)
            {

                await utils.CreateLogFileAsync(ex.Message); throw;
            }
            finally
            {
                await conexion.CloseAsync();
            }
        }
        public async Task<PasswordProfesionalModel> ObtenerPasswordProfesional(string usuarioRequest)
        {
            using MySqlConnection conexion = await mysql!.getConexion("bd1");
            try
            {
                PasswordProfesionalModel usuario = new PasswordProfesionalModel();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conexion;
                cmd.CommandText = $"select contrasena,id_profesional,id_contrasena_profesional from contrasena_profesional where id_profesional = ?id_profesional order by id_profesional;";
                cmd.Parameters.Add("?id_profesional", MySqlDbType.VarChar).Value = usuarioRequest;
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    usuario.contrasena = reader["contrasena"].ToString();
                    usuario.id_profesional = Convert.ToInt64(reader["id_profesional"]);
                    usuario.id_contrasena_profesional = Convert.ToInt64(reader["id_contrasena_profesional"]);

                }

                return usuario;
            }
            catch (Exception ex)
            {

                await utils.CreateLogFileAsync(ex.Message); throw;
            }
            finally
            {
                await conexion.CloseAsync();
            }
        }
        public async void CrearPasswordPaciente(PasswordPacienteModel usuarioRequest)
        {

            using MySqlConnection conexion = await mysql!.getConexion("bd1");
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conexion;
                cmd.CommandText = "INSERT INTO `sistemaodontologico`.`contrasena_paciente` (`id_paciente`, `contrasena`) VALUES (?id_paciente, ?contrasena);";

                cmd.Parameters.Add("?id_paciente", MySqlDbType.VarChar).Value = usuarioRequest.id_paciente;
                cmd.Parameters.Add("?contrasena", MySqlDbType.VarChar).Value = usuarioRequest.contrasena;

                await cmd.ExecuteNonQueryAsync();

            }
            catch (Exception ex)
            {

                await utils.CreateLogFileAsync(ex.Message); throw;
            }
            finally
            {
                await conexion.CloseAsync();
            }
        }
        public async Task<bool> CrearPasswordProfesional(PasswordProfesionalModel usuarioRequest)
        {

            using MySqlConnection conexion = await mysql!.getConexion("bd1");
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conexion;
                cmd.CommandText = "INSERT INTO `sistemaodontologico`.`contrasena_profesional` (`id_profesional`, `contrasena`) VALUES (?id_profesional, ?contrasena);";

                cmd.Parameters.Add("?id_profesional", MySqlDbType.VarChar).Value = usuarioRequest.id_profesional;
                cmd.Parameters.Add("?contrasena", MySqlDbType.VarChar).Value = usuarioRequest.contrasena;

                await cmd.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {

                await utils.CreateLogFileAsync(ex.Message); throw;
            }
            finally
            {
                await conexion.CloseAsync();
            }
        }
        /*
        public void CodigoRecuperacion(PasswordModels usuarioRequest)
        {

            using MySqlConnection conexion = mysql!.getConexion("bd1");
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conexion;
                cmd.CommandText = "UPDATE `bdpaises`.`Password` set cod_recover_password = ?cod_recover_password where usuario_id = ?usuario_id;";

                cmd.Parameters.Add("?cod_recover_password", MySqlDbType.VarChar).Value = usuarioRequest.cod_recover_password;
                cmd.Parameters.Add("?usuario_id", MySqlDbType.VarChar).Value = usuarioRequest.usuario_id;
                cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {

                await utils.CreateLogFileAsync(ex.Message); throw;
            }
            finally
            {
                conexion.Close();
            }
        }
        public void CambioPassword(PasswordModels usuarioRequest)
        {

            using MySqlConnection conexion = mysql!.getConexion("bd1");
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conexion;
                cmd.CommandText = "UPDATE `bdpaises`.`Password` set password = ?password where usuario_id = ?usuario_id;";

                cmd.Parameters.Add("?usuario_id", MySqlDbType.VarChar).Value = usuarioRequest.usuario_id;
                cmd.Parameters.Add("?password", MySqlDbType.VarChar).Value = usuarioRequest.password;

                cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {

                await utils.CreateLogFileAsync(ex.Message); throw;
            }
            finally
            {
                conexion.Close();
            }
        }
        */
    }
}
