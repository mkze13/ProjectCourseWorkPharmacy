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
  
    internal class TableBranchesProducts
    {
        DataBase dataBase = new DataBase();

        public void CreateColumns(DataGridView dataGridView)
        {
            try
            {
                dataGridView.Columns.Add("Id_branch_product", "Код записи");
                dataGridView.Columns["Id_branch_product"].ReadOnly = true;

                dataGridView.Columns.Add("Id_branch", "Код магазина");
                dataGridView.Columns.Add("Article_product", "Артикул товара");
                dataGridView.Columns.Add("Count_in_stock", "В наличии");
            }catch (Exception ex)
            {
                MessageBox.Show("Ошибка:" + ex);
            }
        }//создание  столбцов для таблицы

        private void ReadRow(DataGridView datagridview, IDataRecord record)
        {
            try 
            { 
                datagridview.Rows.Add(record.GetInt32(0), record.GetInt32(1), record.GetString(2), record.GetInt32(3)); 
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

                string query = @"SELECT Id_branch_product, Id_branch, Article_product, Count_in_stock FROM Branches_products";


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

        public void DeleteBranchesProducts(DataGridView dataGridView)
        {
            try
            {

                if (MessageBox.Show("Вы действительно хотите удалить данную запись?", "Предупреждение", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
                {
                    return;
                }

                dataBase.OpenConnection();

                var id = dataGridView.SelectedRows[0].Cells["Id_branch_product"].Value.ToString();

                string query = @"DELETE FROM Branches_products WHERE Id_branch_product = @id";

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

        private void UpdateDatabase(object idBranchProduct, object idBranch, object article, object count)
        {
            try
            {
                dataBase.OpenConnection();

                string query = @"UPDATE Branches_products SET Id_branch = @idBranch, Article_product = @article, Count_in_stock = @count
                                    WHERE Id_branch_product = @idBranchProduct";

                SqlCommand command = new SqlCommand(query, dataBase.getConnection());
                command.Parameters.AddWithValue("@idBranch", idBranch);
                command.Parameters.AddWithValue("@article", article);
                command.Parameters.AddWithValue("@count", count);
                command.Parameters.AddWithValue("@idBranchProduct", idBranchProduct);

                command.ExecuteNonQuery();

                dataBase.ClosedConnection();
            }catch (Exception ex)
            {
                MessageBox.Show("Ошибка:" + ex);
            }

        } // обновляем данные в базе данных

        private void SaveDatabase(object idBranch, object article, object count)
        {
            try
            {
                dataBase.OpenConnection();

                string query = @"INSERT INTO Branches_products (Id_branch,Article_product,Count_in_stock) 
                                    VALUES (@idBranch, @article, @count)";


                SqlCommand command = new SqlCommand(query, dataBase.getConnection());
                command.Parameters.AddWithValue("@idBranch", idBranch);
                command.Parameters.AddWithValue("@article", article);
                command.Parameters.AddWithValue("@count", count);

                command.ExecuteNonQuery();

                dataBase.ClosedConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка:" + ex);
            }

        } // добавляем данные в базу данных

        public void parametersForProducts(DataGridView dataGrid)
        {
            try
            {
                bool saveSuccess = false;

                foreach (DataGridViewRow row in dataGrid.Rows)
                {
                    var idBranchProduct = row.Cells["Id_branch_product"].Value;
                    var idBranch = row.Cells["Id_branch"].Value;
                    var article = row.Cells["Article_product"].Value;
                    var count = row.Cells["Count_in_stock"].Value;

                    if (!CheckIfCellIsEmpty(idBranch) ||
                        !CheckIfCellIsEmpty(article) ||
                        !CheckIfCellIsEmpty(count))
                    {
                        MessageBox.Show("Введены пустые данные, запись не будет сохранена!", "Пустые поля", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        saveSuccess = true;
                        continue;
                    }

                    if (idBranchProduct == null || string.IsNullOrWhiteSpace(article.ToString()))
                    {
                        SaveDatabase(idBranch, article, count);
                    }
                    else
                    {
                        UpdateDatabase(idBranchProduct, idBranch, article, count);
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
        public void SearchBranchesProducts(DataGridView dataGridView, TextBox textBox)
        {
            try
            {
                dataGridView.Rows.Clear();

                string query = @"SELECT Id_branch_product,Id_branch,Article_product,Count_in_stock
                            FROM Branches_products
                            WHERE CONCAT(Id_branch_product, ' ', Id_branch, ' ', Article_product, ' ', Count_in_stock) 
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
