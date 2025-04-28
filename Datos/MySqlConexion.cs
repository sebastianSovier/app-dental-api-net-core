using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace Datos
{
    public class MySqlConexion
    {
        private readonly IConfiguration _config;

        public MySqlConexion(IConfiguration config)
        {
            _config = config;
        }

        public async Task<MySqlConnection> getConexion(string ID_CONEXION)
        {
            MySqlConnection? conexion = null;
            try
            {

                if (_config.GetConnectionString(ID_CONEXION) != null)
                {
                    conexion = new MySqlConnection();
                    conexion.ConnectionString = _config.GetConnectionString(ID_CONEXION);
                    await conexion.OpenAsync();
                }
                else
                {

                    throw new Exception("La fuente de datos " + ID_CONEXION + "no existe");
                }

            }
            catch (Exception e)
            {

                throw new Exception("Error tecnico de conexion " + e.Message);
            }

            return conexion;
        }
    }
}
