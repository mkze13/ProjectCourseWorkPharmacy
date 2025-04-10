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
    internal class TableSales
    {
        DataBase dataBase = new DataBase();

        public void CreateColumns(DataGridView dataGridView)
        {
            try
            {
                dataGridView.Columns.Add("Id_record_sale", "Идентификатор");
                dataGridView.Columns.Add("Date_sale", "Дата продажи");
                dataGridView.Columns.Add("Payment_method", "Метод оплаты");
                dataGridView.Columns.Add("Article_product", "Артикул товара");
                dataGridView.Columns.Add("Name_product", "Наименование товара");
                dataGridView.Columns.Add("Category", "Категория");
                dataGridView.Columns.Add("Price", "Цена за единицу товара");
                dataGridView.Columns.Add("Quantity", "Количество");
                dataGridView.Columns.Add("Id_client", "Код клиента (при наличии)");
                dataGridView.Columns.Add("Total_price_sale", "Сумма продажи");
                dataGridView.Columns.Add("Login", "Логин сотрудника");
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
                datagridview.Rows.Add(record.GetInt32(0), record.GetDateTime(1), record.GetString(2), record.GetString(3), record.GetString(4),
                    record.GetString(5), record.GetDecimal(6), record.GetInt32(7), record.GetInt32(8), record.GetDecimal(9), record.GetString(10));
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

                string query = @"SELECT si.Id_record_sale, s.Date_sale, s.Payment_method, si.Article_product, p.Name_product, p.Category,
                            p.Price, si.Quantity, si.Id_client, s.Total_price_sale, s.Login
                            FROM Sale_info si
                            JOIN Sales s ON si.Id_sale = s.Id_sale
                            JOIN Products p ON si.Article_product = p.Article_product";


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

        public void SearchSales(DataGridView dataGridView, TextBox textBox)
        {
            try
            {
                dataGridView.Rows.Clear();

                string query = @"SELECT si.Id_record_sale, s.Date_sale, s.Payment_method, si.Article_product, p.Name_product, p.Category,
                            p.Price, si.Quantity, si.Id_client, s.Total_price_sale, s.Login
                            FROM Sale_info si
                            JOIN Sales s ON si.Id_sale = s.Id_sale
                            JOIN Products p ON si.Article_product = p.Article_product
                            WHERE CONCAT(si.Id_record_sale, ' ', s.Date_sale, ' ', s.Payment_method, ' ', si.Article_product, ' ', 
                            p.Name_product, ' ', p.Category, ' ',p.Price, ' ', si.Quantity, ' ', si.Id_client, ' ',
                            s.Total_price_sale, ' ', s.Login) 
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
