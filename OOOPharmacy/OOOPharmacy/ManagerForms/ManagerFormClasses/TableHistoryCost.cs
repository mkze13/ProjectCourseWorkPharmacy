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
    internal class TableHistoryCost
    {
        DataBase dataBase = new DataBase();

        public void CreateColumns(DataGridView dataGridView)
        {
            try
            {
                dataGridView.Columns.Add("Id_history_cost", "Идентификатор");
                dataGridView.Columns.Add("Date_of_change", "Дата изменения");
                dataGridView.Columns.Add("Article_product", "Артикул товара");
                dataGridView.Columns.Add("Old_price", "Старая цена");
                dataGridView.Columns.Add("New_price", "Новая цена");
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
                datagridview.Rows.Add(record.GetInt32(0), record.GetDateTime(1).ToString("dd-MM-yyyy"), record.GetString(2), record.GetDecimal(3), record.GetDecimal(4));
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

                string query = @"SELECT * FROM History_Cost";

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

        public void SearchHistoryCost(DataGridView dataGridView, TextBox textBox)
        {
            try
            {
                dataGridView.Rows.Clear();

                string query = @"
                            SELECT Id_history_cost,Date_of_change,Article_product,Old_price,New_price
                            FROM History_Cost
                            WHERE CONCAT(Id_history_cost, ' ', Date_of_change, ' ', Article_product, ' ', Old_price, ' ', New_price) 
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
