using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OOOPharmacy
{
    internal class DataBase
    {
        //Строка подключения к базе данных

        SqlConnection connectionString = new SqlConnection(@"Data Source=DESKTOP-38T5I3P\SQLEXPRESS;Initial Catalog=OOPharmacy;Integrated Security=True;");

        public void OpenConnection()
        {
            try
            {
                if (connectionString.State == System.Data.ConnectionState.Closed) //Проверяем состояние строки после последней сетевой операции
                {
                    connectionString.Open(); //открываем соединение
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка:" + ex);
            }
        }

        public void ClosedConnection()
        {
            if (connectionString.State == System.Data.ConnectionState.Open) //Проверяем состояние строки после последней сетевой операции
            {
                connectionString.Close(); //закрываем соединение
            }
        }

        public SqlConnection getConnection()
        {
            try
            {
                return connectionString;//возвращаем строку подключения
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Ошибка:" + ex);
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка:" + ex);
                return null;
            }
        } 
    }
}
