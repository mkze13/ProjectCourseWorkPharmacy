using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http.Headers;

namespace OOOPharmacy.AdminForms
{
    internal class TableUsers
    {
        DataBase dataBase = new DataBase();
        int index = 0;
        public List<User> usersList = new List<User>();

        public void CreateColumns(DataGridView dataGridView)
        {
            dataGridView.Columns.Add("Login", "Логин");
            dataGridView.Columns.Add("Password", "Пароль");
            dataGridView.Columns.Add("Role", "Роль");
        }//создание столбцов

        private void ReadRow(DataGridView datagridview, IDataRecord record)
        {
            datagridview.Rows.Add(record.GetString(0), record.GetString(1), record.GetString(2));
        }  //считываем строку

        public void RefreshDataGridView(DataGridView datagridview, CheckBox checkBox)
        {
            datagridview.Rows.Clear();
            checkBox.Checked = false;
            LoadUsers();

            string query = $"SELECT * FROM Users";

            SqlCommand command = new SqlCommand(query, dataBase.getConnection());

            dataBase.OpenConnection();

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                ReadRow(datagridview, reader);
            }

            reader.Close();

        } //обновляем данные в таблице

        public void UpdateUser(string login, string oldlogin, string password, string role)
        {
           
            string query = "UPDATE Users SET Login = @login, Password = @password, Role = @role WHERE Login = @oldlogin";

            using (SqlCommand command = new SqlCommand(query, dataBase.getConnection()))
            {
                command.Parameters.AddWithValue("@login", login);
                command.Parameters.AddWithValue("@password", password);
                command.Parameters.AddWithValue("@role", role);
                command.Parameters.AddWithValue("@oldlogin", oldlogin);

                dataBase.OpenConnection();
                command.ExecuteNonQuery();
                dataBase.ClosedConnection();

                MessageBox.Show("Данные успешно обновлены!");
            }
        } //изменяем и обновляем данные пользователя

        public void LoadUsers()
        {
            string query = "SELECT Login, Password, Role FROM Users";

            using (SqlCommand command = new SqlCommand(query, dataBase.getConnection()))
            {
                dataBase.OpenConnection();
                SqlDataReader reader = command.ExecuteReader();

                usersList.Clear();  

                while (reader.Read())
                {
                    usersList.Add(new User
                    {
                        Login = reader["Login"].ToString(),
                        Password = reader["Password"].ToString(),
                        Role = reader["Role"].ToString()
                    });
                }

                dataBase.ClosedConnection();

            }
        }

        public void UpdateTextBoxesInfo(int index, TextBox textBoxLogin, TextBox textBoxPassword, ComboBox comboBoxRole)
        {
            if (usersList.Count > 0 && index >= 0 && index < usersList.Count)
            {
                textBoxLogin.Text = usersList[index].Login;
                textBoxPassword.Text = usersList[index].Password;
                comboBoxRole.Text = usersList[index].Role;
            }
        }
    }
    public class User
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
