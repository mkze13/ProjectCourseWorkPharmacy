using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.IO;
using System.Security.Policy;
using Microsoft.VisualBasic;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace OOOPharmacy.ManagerForms.ManagerFormClasses
{
    internal class TableProducts
    {
        DataBase dataBase = new DataBase();

        public void CreateColumns(DataGridView dataGridView)
        {
            dataGridView.Columns.Add("Article_product", "Артикул товара");
            dataGridView.Columns["Article_product"].ReadOnly = true;

            dataGridView.Columns.Add("Name_product", "Наименование");
            dataGridView.Columns.Add("Category", "Категория");
            dataGridView.Columns.Add("Price", "Цена");
            dataGridView.Columns.Add("Unit", "Единица измерения");
            dataGridView.Columns.Add("Manufacture_date", "Дата производства");
            dataGridView.Columns.Add("Expiration_date", "Срок годности");
            dataGridView.Columns.Add("Manufacture_name", "Производитель");
            dataGridView.Columns.Add("Manufacture_country", "Страна производства");
        }//создание столбцов

        private void ReadRow(DataGridView datagridview, IDataRecord record)
        {
            datagridview.Rows.Add(record.GetString(0), record.GetString(1), record.GetString(2), record.GetDecimal(3), record.GetString(4), record.GetDateTime(5).ToString("dd-MM-yyyy"),
                                  record.GetDateTime(6).ToString("dd-MM-yyyy"), record.GetString(7), record.GetString(8));
        }  //считываем строку

        public void RefreshDataGridView(DataGridView datagridview)
        {
            datagridview.Rows.Clear();

            string query = @"SELECT * FROM Products";

            SqlCommand command = new SqlCommand(query, dataBase.getConnection());

            dataBase.OpenConnection();

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                ReadRow(datagridview, reader);
            }

            reader.Close();

            dataBase.ClosedConnection();

        } //обновляем данные в таблице

        public void DeleteProduct(DataGridView dataGridView)
        {
            if (MessageBox.Show("Вы действительно хотите удалить данную запись?", "Предупреждение", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
            {
                return;
            }

            dataBase.OpenConnection();

            var article = dataGridView.SelectedRows[0].Cells["Article_product"].Value.ToString();

            string query = @"DELETE FROM Products WHERE Article_product = @article";

            SqlCommand deleteEmployeeCommand = new SqlCommand(query, dataBase.getConnection());
            deleteEmployeeCommand.Parameters.AddWithValue("@article", article);

            deleteEmployeeCommand.ExecuteNonQuery();

            dataBase.ClosedConnection();

            MessageBox.Show("Запись удалена!", "Уведомление", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void UpdateDatabase(object article, object name_product, object category, object price, object unit, object manufacture_date, object expiration_date, object manufacture_name, object manufacture_country)
        {
            try
            {
                dataBase.OpenConnection();

                string query = @"UPDATE Products 
                                    SET Name_product = @name, Category = @category, Price = @price, Unit = @unit, Manufacture_date = @md,
                                    Expiration_date = @ed, Manufacturer_name = @mn, Manufacturer_country = @mc
                                    WHERE Article_product = @article";

                SqlCommand command = new SqlCommand(query, dataBase.getConnection());
                command.Parameters.AddWithValue("@article", article);
                command.Parameters.AddWithValue("@name", name_product);
                command.Parameters.AddWithValue("@category", category);
                command.Parameters.AddWithValue("@price", price);
                command.Parameters.AddWithValue("@unit", unit);
                command.Parameters.AddWithValue("@md", manufacture_date);
                command.Parameters.AddWithValue("@ed", expiration_date);
                command.Parameters.AddWithValue("@mn", manufacture_name);
                command.Parameters.AddWithValue("@mc", manufacture_country);

                command.ExecuteNonQuery();

                dataBase.ClosedConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка:" + ex);
            }

        } // обновляем данные в базе данных

        private void SaveDatabase( object name_product, object category, object price, object unit, object manufacture_date, object expiration_date, object manufacture_name, object manufacture_country)
        {
            try
            {
                dataBase.OpenConnection();

                string query = @"INSERT INTO Products (Name_product,Category,Price,Unit,Manufacture_date,Expiration_date,Manufacturer_name,Manufacturer_country) 
                                    VALUES (@name, @category, @price, @unit, @md, @ed, @mn, @mc)";


                SqlCommand command = new SqlCommand(query, dataBase.getConnection());
                command.Parameters.AddWithValue("@name", name_product);
                command.Parameters.AddWithValue("@category", category);
                command.Parameters.AddWithValue("@price", price);
                command.Parameters.AddWithValue("@unit", unit);
                command.Parameters.AddWithValue("@md", manufacture_date);
                command.Parameters.AddWithValue("@ed", expiration_date);
                command.Parameters.AddWithValue("@mn", manufacture_name);
                command.Parameters.AddWithValue("@mc", manufacture_country);

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
                    var article = row.Cells["Article_product"].Value;
                    var name_product = row.Cells["Name_product"].Value;
                    var category = row.Cells["Category"].Value;
                    var price = row.Cells["Price"].Value;
                    var unit = row.Cells["Unit"].Value;
                    var manufacture_date = row.Cells["Manufacture_date"].Value;
                    var expiration_date = row.Cells["Expiration_date"].Value;
                    var manufacture_name = row.Cells["Manufacture_name"].Value;
                    var manufacture_country = row.Cells["Manufacture_country"].Value;

                    if (!CheckIfCellIsEmpty(name_product) ||
                        !CheckIfCellIsEmpty(category) ||
                        !CheckIfCellIsEmpty(price) ||
                        !CheckIfCellIsEmpty(unit) ||
                        !CheckIfCellIsEmpty(manufacture_date) ||
                        !CheckIfCellIsEmpty(manufacture_name) ||
                        !CheckIfCellIsEmpty(manufacture_country))
                    {
                        MessageBox.Show("Введены пустые данные, запись не будет сохранена!", "Пустые поля", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        saveSuccess = true;
                        continue;
                    }

                    if (article == null || string.IsNullOrWhiteSpace(article.ToString()))
                    {
                        SaveDatabase(name_product, category, price, unit, manufacture_date, expiration_date, manufacture_name, manufacture_country);
                    }
                    else
                    {
                        UpdateDatabase(article, name_product, category, price, unit, manufacture_date, expiration_date, manufacture_name, manufacture_country);
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

        public void SearchProducts(DataGridView dataGridView, TextBox textBox)
        {
            try
            {
                dataGridView.Rows.Clear();

                string query = @"
                            SELECT Article_product,Name_product,Category,Price,Unit,Manufacture_date,Expiration_date,Manufacturer_name,Manufacturer_country
                            FROM Products
                            WHERE CONCAT(Article_product, ' ', Name_product, ' ', Category, ' ', Price, ' ', Unit, ' ', Manufacture_date, 
                            ' ', Expiration_date, ' ', Manufacturer_name, ' ', Manufacturer_country) 
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

        public void DeleteExpiredProduct()
        {
            try
            {
                if (MessageBox.Show("Вы действительно хотите удалить просроченные товары?", "Предупреждение", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
                {
                    return;
                }

                DateTime checkDate = DateTime.Today;

                dataBase.OpenConnection();

                string query = @"DELETE FROM Products WHERE Expiration_date <= @date";

                SqlCommand deleteEmployeeCommand = new SqlCommand(query, dataBase.getConnection());
                deleteEmployeeCommand.Parameters.AddWithValue("@date", checkDate);

                deleteEmployeeCommand.ExecuteNonQuery();

                dataBase.ClosedConnection();

                MessageBox.Show("Просроченные товары удалены!", "Уведомление", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка:" + ex);
            }
        }

        public void RefreshPriceForCountry()
        {
           Form_Manager_Refresh_Price form = new Form_Manager_Refresh_Price();
            form.ShowDialog();
        }
    }
}
