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
    internal class TableOrders
    {
        DataBase dataBase = new DataBase();

        public void CreateColumns(DataGridView dataGridView)
        {
            try
            {
                dataGridView.Columns.Add("Id_order", "Идентификатор");
                dataGridView.Columns["Id_order"].ReadOnly = true;

                dataGridView.Columns.Add("Id_suppliers", "Код поставщика");
                dataGridView.Columns.Add("Id_branch", "Код магазина");
                dataGridView.Columns.Add("Total_price", "Общая цена заказа");
                dataGridView.Columns.Add("Status", "Статус");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка:" + ex);
            }
        }//создание  столбцов для таблицы

        private void ReadRow(DataGridView datagridview, IDataRecord record)
        {
            try
            {
                datagridview.Rows.Add(record.GetInt32(0), record.GetInt32(1), record.GetInt32(2), record.GetDecimal(3), record.GetString(4));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка:" + ex);
            }
        }//добавляем строку таблицы с данными


        public void RefreshDataGridView(DataGridView datagridview)
        {
            try
            {
                datagridview.Rows.Clear();

                string query = @"SELECT * FROM Orders";


                SqlCommand command = new SqlCommand(query, dataBase.getConnection());

                dataBase.OpenConnection();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    ReadRow(datagridview, reader);
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка:" + ex);
            }

        } //добавляем и обновляем данные в таблице

        public void DeleteOrders(DataGridView dataGridView)
        {
            try
            {
                if (MessageBox.Show("Вы действительно хотите удалить данную запись?", "Предупреждение", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
                {
                    return;
                }

                dataBase.OpenConnection();

                var id = dataGridView.SelectedRows[0].Cells["Id_order"].Value.ToString();

                string query = @"DELETE FROM Orders WHERE Id_order = @id";

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

        private void UpdateDatabase(object id, object idSuppliers, object idBranch, object price, object status)
        {
            try
            {
                dataBase.OpenConnection();

                string query = @"UPDATE Orders 
                                    SET Id_suppliers = @idSuppliers, Id_branch = @idBranch, Total_price = @price, Status = @status
                                    WHERE Id_order = @id";

                SqlCommand command = new SqlCommand(query, dataBase.getConnection());
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@idSuppliers", idSuppliers);
                command.Parameters.AddWithValue("@idBranch", idBranch);
                command.Parameters.AddWithValue("@price", price);
                command.Parameters.AddWithValue("@status", status);

                command.ExecuteNonQuery();

                dataBase.ClosedConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка:" + ex);
            }

        } // обновляем данные в базе данных

        private void SaveDatabase(object idSuppliers, object idBranch, object price, object status)
        {
            try
            {
                dataBase.OpenConnection();

                string query = @"INSERT INTO Orders (Id_suppliers,Id_branch,Total_price,Status) 
                                    VALUES (@idSuppliers, @idBranch, @price, @status)";


                SqlCommand command = new SqlCommand(query, dataBase.getConnection());
                command.Parameters.AddWithValue("@idSuppliers", idSuppliers);
                command.Parameters.AddWithValue("@idBranch", idBranch);
                command.Parameters.AddWithValue("@price", price);
                command.Parameters.AddWithValue("@status", status);

                command.ExecuteNonQuery();

                dataBase.ClosedConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка:" + ex);
            }

        } // добавляем данные в базу данных

        public void parametersForOrders(DataGridView dataGrid)
        {
            try
            {
                bool saveSuccess = false;

                foreach (DataGridViewRow row in dataGrid.Rows)
                {

                    var id = row.Cells["Id_order"].Value;
                    var idSuppliers = row.Cells["Id_suppliers"].Value;
                    var idBranch = row.Cells["Id_branch"].Value;
                    var price = row.Cells["Total_price"].Value;
                    var status = row.Cells["Status"].Value;

                    if (!CheckIfCellIsEmpty(idSuppliers) ||
                        !CheckIfCellIsEmpty(idBranch) ||
                        !CheckIfCellIsEmpty(price) ||
                        !CheckIfCellIsEmpty(status))
                    {
                        MessageBox.Show("Введены пустые данные, запись не будет сохранена!", "Пустые поля", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        saveSuccess = true;
                        continue;
                    }

                    if (id == null || string.IsNullOrWhiteSpace(id.ToString()))
                    {
                        SaveDatabase(idSuppliers, idBranch, price, status);
                    }
                    else
                    {
                        UpdateDatabase(id, idSuppliers, idBranch, price, status);
                    }
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

        public void SearchOrders(DataGridView dataGridView, TextBox textBox)
        {
            try
            {
                dataGridView.Rows.Clear();

                string query = @"SELECT Id_order,Id_suppliers,Id_branch,Total_price,Status
                            FROM Orders
                            WHERE CONCAT(Id_order, ' ', Id_suppliers, ' ', Id_branch, ' ', Total_price, ' ', Status) 
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
