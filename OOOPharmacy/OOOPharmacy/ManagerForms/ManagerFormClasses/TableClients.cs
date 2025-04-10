using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OOOPharmacy.ManagerForms.ManagerFormClasses
{
    internal class TableClients
    {
        DataBase dataBase = new DataBase();

        public void CreateColumns(DataGridView dataGridView)
        {
            try
            {
                dataGridView.Columns.Add("Id_clients", "Идентификатор");
                dataGridView.Columns["Id_clients"].ReadOnly = true;

                dataGridView.Columns.Add("Last_name_client", "Фамилия");
                dataGridView.Columns.Add("First_name_client", "Имя");
                dataGridView.Columns.Add("Middle_name_client", "Отчество");
                dataGridView.Columns.Add("Number_phone_client", "Номер телефона");
                dataGridView.Columns.Add("Email_client", "Электронная почта");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка:" + ex);
            }
        }//создание столбцов

        private void ReadRow(DataGridView datagridview, IDataRecord record)
        {
            try
            {
                datagridview.Rows.Add(record.GetInt32(0), record.GetString(1), record.GetString(2), record.GetString(3), record.GetString(4), record.GetString(5));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка:" + ex);
            }
        }  //считываем строку

        public void RefreshDataGridView(DataGridView datagridview)
        {
            try
            {
                datagridview.Rows.Clear();

                string query = @"SELECT * FROM Clients";

                SqlCommand command = new SqlCommand(query, dataBase.getConnection());

                dataBase.OpenConnection();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    ReadRow(datagridview, reader);
                }

                reader.Close();

                dataBase.ClosedConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка:" + ex);
            }

        } //обновляем данные в таблице

        public void DeleteClients(DataGridView dataGridView)
        {
            try
            {

                if (MessageBox.Show("Вы действительно хотите удалить данную запись?", "Предупреждение", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
                {
                    return;
                }

                dataBase.OpenConnection();

                var id = dataGridView.SelectedRows[0].Cells["Id_clients"].Value.ToString();

                string query = @"DELETE FROM Clients WHERE Id_client = @id";

                SqlCommand deleteEmployeeCommand = new SqlCommand(query, dataBase.getConnection());
                deleteEmployeeCommand.Parameters.AddWithValue("@id", id);

                deleteEmployeeCommand.ExecuteNonQuery();

                dataBase.ClosedConnection();

                MessageBox.Show("Запись удалена!", "Уведомление", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка:" + ex);
            }
        }

        private void UpdateDatabase(object id, object lastName, object firstName, object middleName, object phone, object email)
        {
            try
            {
                dataBase.OpenConnection();

                string queryAddress = @"UPDATE Clients 
                                    SET Last_name_client = @lastName, First_name_client = @firstName, Middle_name_client = @middleName, Number_phone_client = @phone, Email_client = @email
                                    WHERE Id_client = @id";

                SqlCommand command = new SqlCommand(queryAddress, dataBase.getConnection());
                command.Parameters.AddWithValue("@firstName", firstName);
                command.Parameters.AddWithValue("@lastName", lastName);
                command.Parameters.AddWithValue("@middleName", middleName);
                command.Parameters.AddWithValue("@phone", phone);
                command.Parameters.AddWithValue("@email", email);
                command.Parameters.AddWithValue("@id", id);

                command.ExecuteNonQuery();

                dataBase.ClosedConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка:" + ex);
            }

        } // обновляем данные в базе данных

        public void parametersForClients(DataGridView dataGrid)
        {
            try
            {
                bool saveSuccess = false;

                foreach (DataGridViewRow row in dataGrid.Rows)
                {
                    var id = row.Cells["Id_clients"].Value;
                    var lastName = row.Cells["Last_name_client"].Value;
                    var firstName = row.Cells["First_name_client"].Value;
                    var middleName = row.Cells["Middle_name_client"].Value;
                    var phone = row.Cells["Number_phone_client"].Value;
                    var email = row.Cells["Email_client"].Value;

                    if (!CheckIfCellIsEmpty(id) ||
                        !CheckIfCellIsEmpty(lastName) ||
                        !CheckIfCellIsEmpty(firstName) ||
                        !CheckIfCellIsEmpty(middleName) ||
                        !CheckIfCellIsEmpty(phone) ||
                        !CheckIfCellIsEmpty(email))
                    {
                        MessageBox.Show("Введены пустые данные, запись не будет сохранена!", "Пустые поля", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        saveSuccess = true;
                        continue;
                    }

                    UpdateDatabase(id, lastName, firstName, middleName, phone, email);
                }

                if (!saveSuccess)
                {
                    MessageBox.Show("Данные сохранены!", "Уведомление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка:" + ex);
            }

        } //устанавливаем параметры для обновления данных в базе данных

        private bool CheckIfCellIsEmpty(object cellValue)
        {
            if (cellValue == null || string.IsNullOrWhiteSpace(cellValue.ToString()))
            {
                return false; // если значение пустое
            }
            return true; //если значение не пустое
        }

        public void SearchClients(DataGridView dataGridView, TextBox textBox)
        {
            try
            {
                dataGridView.Rows.Clear();

                string query = @"
                            SELECT Id_client, Last_name_client, First_name_client, Middle_name_client, Number_phone_client, Email_client
                            FROM Clients
                            WHERE CONCAT(Id_client, ' ', Last_name_client, ' ', First_name_client, ' ', Middle_name_client, ' ', Number_phone_client, ' ', Email_client) 
                            LIKE '%" + textBox.Text + "%'";

                SqlCommand command = new SqlCommand(query, dataBase.getConnection());

                dataBase.OpenConnection();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    ReadRow(dataGridView, reader);
                }

                reader.Close();
                dataBase.ClosedConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка:" + ex);
            }
        }

    }
}
