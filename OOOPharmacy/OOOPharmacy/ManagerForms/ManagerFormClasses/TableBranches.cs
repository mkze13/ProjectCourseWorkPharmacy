using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32.SafeHandles;

namespace OOOPharmacy.ManagerForms.ManagerFormClasses
{
    internal class TableBranches
    {
        DataBase dataBase = new DataBase();

        public void CreateColumns(DataGridView dataGridView)
        {
            try
            {
                dataGridView.Columns.Add("Id_branch", "Идентификатор");
                dataGridView.Columns["Id_branch"].ReadOnly = true;

                dataGridView.Columns.Add("Street_branches", "Улица");
                dataGridView.Columns.Add("House_branches", "Дом");
                dataGridView.Columns.Add("City_branches", "Город");
                dataGridView.Columns.Add("Number_phone_branches", "Город");
            } catch (Exception ex)
            {
                MessageBox.Show("Ошибка:" + ex);
            }
            
        }//создание  столбцов для таблицы

        private void ReadRow(DataGridView datagridview, IDataRecord record)
        {
            try
            {
                datagridview.Rows.Add(record.GetInt32(0), record.GetString(1), record.GetString(2), record.GetString(3), record.GetString(4));
            }catch (Exception ex)
            {
                MessageBox.Show("Ошибка:" + ex);
            }
        }//добавляем строку таблицы с данными


        public void RefreshDataGridView(DataGridView datagridview)
        {
            try
            {
                datagridview.Rows.Clear();

                string query = @"SELECT * FROM Branches";


                SqlCommand command = new SqlCommand(query, dataBase.getConnection());

                dataBase.OpenConnection();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    ReadRow(datagridview, reader);
                }

                reader.Close();
            } catch (Exception ex)
            {
                MessageBox.Show("Ошибка:" + ex);
            }

        } //добавляем и обновляем данные в таблице

        public void DeleteBranches(DataGridView dataGridView)
        {
            try
            {
                if (MessageBox.Show("Вы действительно хотите удалить данную запись?", "Предупреждение", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
                {
                    return;
                }

                dataBase.OpenConnection();

                var id = dataGridView.SelectedRows[0].Cells["Id_branch"].Value.ToString();

                string query = @"DELETE FROM Branches WHERE Id_branch = @id";

                SqlCommand deleteEmployeeCommand = new SqlCommand(query, dataBase.getConnection());
                deleteEmployeeCommand.Parameters.AddWithValue("@id", id);

                deleteEmployeeCommand.ExecuteNonQuery();

                dataBase.ClosedConnection();

                MessageBox.Show("Запись удалена!", "Уведомление", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }catch(Exception ex)
            {
                MessageBox.Show("Ошибка:" + ex);
            }
        }

        private void UpdateDatabase(object id, object street, object house, object city, object numberPhone)
        {
            try
            {
                dataBase.OpenConnection();

                string query = @"UPDATE Branches 
                                    SET Street_branches = @street, House_branches = @house, City_branches = @city, Number_phone_branches = @numberPhone
                                    WHERE Id_branch = @id";

                SqlCommand command = new SqlCommand(query, dataBase.getConnection());
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@street", street);
                command.Parameters.AddWithValue("@house", house);
                command.Parameters.AddWithValue("@city", city);
                command.Parameters.AddWithValue("@numberPhone", numberPhone);

                command.ExecuteNonQuery();

                dataBase.ClosedConnection();

            }catch(Exception ex)
            {
                MessageBox.Show("Ошибка:" + ex);
            }

        } // обновляем данные в базе данных

        private void SaveDatabase(object street, object house, object city, object numberPhone)
        {
            try
            {
                dataBase.OpenConnection();

                string query = @"INSERT INTO Branches (Street_branches,House_branches,City_branches,Number_phone_branches) 
                                    VALUES (@street, @house, @city, @numberPhone)";


                SqlCommand command = new SqlCommand(query, dataBase.getConnection());
                command.Parameters.AddWithValue("@street", street);
                command.Parameters.AddWithValue("@house", house);
                command.Parameters.AddWithValue("@city", city);
                command.Parameters.AddWithValue("@numberPhone", numberPhone);

                command.ExecuteNonQuery();

                dataBase.ClosedConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка:" + ex);
            }

        } // добавляем данные в базу данных

        public void parametersForBranches(DataGridView dataGrid)
        {
            try
            {
                bool saveSuccess = false;

                foreach (DataGridViewRow row in dataGrid.Rows)
                {

                    var id = row.Cells["Id_branch"].Value;
                    var street = row.Cells["Street_branches"].Value;
                    var house = row.Cells["House_branches"].Value;
                    var city = row.Cells["City_branches"].Value;
                    var numberPhone = row.Cells["Number_phone_branches"].Value;

                    if (!CheckIfCellIsEmpty(street) ||
                        !CheckIfCellIsEmpty(house) ||
                        !CheckIfCellIsEmpty(city) ||
                        !CheckIfCellIsEmpty(numberPhone))
                    {
                        MessageBox.Show("Введены пустые данные, запись не будет сохранена!", "Пустые поля", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        saveSuccess = true;
                        continue;
                    }

                    if (id == null || string.IsNullOrWhiteSpace(id.ToString()))
                    {
                        SaveDatabase(street, house, city, numberPhone);
                    }
                    else
                    {
                        UpdateDatabase(id, street, house, city, numberPhone);
                    }
                }

                if (!saveSuccess)
                {
                    MessageBox.Show("Данные сохранены!", "Уведомление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }catch (Exception ex)
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

        public void SearchBranches(DataGridView dataGridView, TextBox textBox)
        {
            try
            {
                dataGridView.Rows.Clear();

                string query = @"SELECT Id_branch,Street_branches,House_branches,City_branches,Number_phone_branches
                            FROM Branches
                            WHERE CONCAT(Id_branch, ' ', Street_branches, ' ', House_branches, ' ', City_branches, ' ', Number_phone_branches) 
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
            }catch (Exception ex)
            {
                MessageBox.Show("Ошибка:" + ex);
            }
        }
    }
}
