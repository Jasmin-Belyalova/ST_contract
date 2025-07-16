using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Student_Contract
{
    class DB
    {
        // данные о соединении
        MySqlConnection connection = new MySqlConnection("server=localhost;port=3306;user=root;password=root;database=contract"); // данные берутся с локального сервера при подключении mysql
                                                                                                                                  // БД к какой коннектим          

        public void openConnection() // открытие подключения
        {
            if (connection.State == System.Data.ConnectionState.Closed)// если состояние соединения == выключено, то включаем
                connection.Open();
        }

        public void closeConnection() // закрытие подключения
        {
            if (connection.State == System.Data.ConnectionState.Open)// если состояние соединения == вкл, то выключаем
                connection.Close();
        }

        public MySqlConnection GetConnection()// получаем это самое соединение
        {
            return connection;
        }
    }
}
