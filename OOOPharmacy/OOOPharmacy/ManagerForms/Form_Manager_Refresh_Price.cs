using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OOOPharmacy.ManagerForms
{
    public partial class Form_Manager_Refresh_Price : Form
    {
        DataBase dataBase = new DataBase();


        public Form_Manager_Refresh_Price()
        {
            InitializeComponent();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(textBoxCountry.Text) && !string.IsNullOrWhiteSpace(textBoxPrice.Text))
                {

                    decimal percent = int.Parse(textBoxPrice.Text);
                    string country = textBoxCountry.Text;

                    if (percent < 0) MessageBox.Show("Неверные данные!", "Проценты не могут быть меньше нуля!", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    if (!radioButtonUp.Checked) percent = percent * -1;

                    decimal totalPercent = 1 + percent / 100;

                    string updateQuery = @"UPDATE Products SET Price = Price * @percent WHERE Manufacturer_country = @country";

                    dataBase.OpenConnection();

                    SqlCommand command = new SqlCommand(updateQuery, dataBase.getConnection());

                    command.Parameters.AddWithValue("@percent", totalPercent);
                    command.Parameters.AddWithValue("@country", country);

                    command.ExecuteNonQuery();

                    dataBase.ClosedConnection();

                    MessageBox.Show("Цена Обновлена!", "Уведомление", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.Close();
                }
                else MessageBox.Show("Введите данные!", "Пустые поля", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка:" + ex);
            }
        }
    }
}
