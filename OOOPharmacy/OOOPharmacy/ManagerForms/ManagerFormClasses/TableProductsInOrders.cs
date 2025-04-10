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
    internal class TableProductsInOrders
    {
        DataBase dataBase = new DataBase();

        public void CreateColumns(DataGridView dataGridView)
        {
            try
            {
                dataGridView.Columns.Add("Id_record", "Идентификатор");
                dataGridView.Columns["Id_record"].ReadOnly = true;

                dataGridView.Columns.Add("Id_order", "Номер заказа");
                dataGridView.Columns.Add("Article_product", "Артикул товара");
                dataGridView.Columns.Add("Quantity", "Количество");
                dataGridView.Columns.Add("Price_per_unit", "Цена");
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
                datagridview.Rows.Add(record.GetInt32(0), record.GetInt32(1), record.GetString(2), record.GetInt32(3), record.GetDecimal(4));
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

                string query = @"SELECT * FROM Products_in_orders";


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

        public void DeleteProductsInOrders(DataGridView dataGridView)
        {
            try
            {
                if (MessageBox.Show("Вы действительно хотите удалить данную запись?", "Предупреждение", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
                {
                    return;
                }

                dataBase.OpenConnection();

                var id = dataGridView.SelectedRows[0].Cells["Id_record"].Value.ToString();

                string query = @"DELETE FROM Products_in_orders WHERE Id_record = @id";

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

        private void UpdateDatabase(object id, object idOrder, object article, object quntity, object price)
        {
            try
            {
                dataBase.OpenConnection();

                string query = @"UPDATE Products_in_orders 
                                    SET Id_order = @idOrder, Article_product = @article, Quantity = @quntity, Price_per_unit = @price
                                    WHERE Id_record = @id";

                SqlCommand command = new SqlCommand(query, dataBase.getConnection());
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@idOrder", idOrder);
                command.Parameters.AddWithValue("@article", article);
                command.Parameters.AddWithValue("@quntity", quntity);
                command.Parameters.AddWithValue("@price", price);

                command.ExecuteNonQuery();

                dataBase.ClosedConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка:" + ex);
            }

        } // обновляем данные в базе данных

        private void SaveDatabase(object idOrder, object article, object quntity, object price)
        {
            try
            {
                dataBase.OpenConnection();

                string query = @"INSERT INTO Products_in_orders (Id_order,Article_product,Quantity,Price_per_unit) 
                                    VALUES (@idOrder, @article, @quntity, @price)";


                SqlCommand command = new SqlCommand(query, dataBase.getConnection());
                command.Parameters.AddWithValue("@idOrder", idOrder);
                command.Parameters.AddWithValue("@article", article);
                command.Parameters.AddWithValue("@quntity", quntity);
                command.Parameters.AddWithValue("@price", price);

                command.ExecuteNonQuery();

                dataBase.ClosedConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка:" + ex);
            }

        } // добавляем данные в базу данных

        public void parametersForProductsInOrders(DataGridView dataGrid)
        {
            try
            {
                bool saveSuccess = false;

                foreach (DataGridViewRow row in dataGrid.Rows)
                {

                    var id = row.Cells["Id_record"].Value;
                    var idOrder = row.Cells["Id_order"].Value;
                    var article = row.Cells["Article_product"].Value;
                    var quntity = row.Cells["Quantity"].Value;
                    var price = row.Cells["Price_per_unit"].Value;

                    if (!CheckIfCellIsEmpty(idOrder) ||
                        !CheckIfCellIsEmpty(article) ||
                        !CheckIfCellIsEmpty(quntity) ||
                        !CheckIfCellIsEmpty(price))
                    {
                        MessageBox.Show("Введены пустые данные, запись не будет сохранена!", "Пустые поля", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        saveSuccess = true;
                        continue;
                    }

                    if (id == null || string.IsNullOrWhiteSpace(id.ToString()))
                    {
                        SaveDatabase(idOrder, article, quntity, price);
                    }
                    else
                    {
                        UpdateDatabase(id, idOrder, article, quntity, price);
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

        public void SearchProductsInOrders(DataGridView dataGridView, TextBox textBox)
        {
            try
            {
                dataGridView.Rows.Clear();

                string query = @"SELECT Id_record,Id_order,Article_product,Quantity,Price_per_unit
                            FROM Products_in_orders
                            WHERE CONCAT(Id_record, ' ', Id_order, ' ', Article_product, ' ', Quantity, ' ', Price_per_unit) 
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
