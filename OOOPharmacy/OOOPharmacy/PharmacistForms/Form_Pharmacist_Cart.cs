using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace OOOPharmacy.PharmacistForms
{
    public partial class Form_Pharmacist_Cart : Form
    {
        DataBase dataBase = new DataBase();

        bool checkClient = false;
        int IdSale;
        public string pharmacistLogin; 

        public Form_Pharmacist_Cart()
        {
            InitializeComponent();

            dataGridViewCart.Columns.Add("Name_product", "Название товара");
            dataGridViewCart.Columns.Add("Category", "Категория");
            dataGridViewCart.Columns.Add("Price", "Цена");
            dataGridViewCart.Columns.Add("Quantity", "Количество");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridViewCart.Rows.Clear();
            textBoxPhone.Text = "";
            comboBoxMethod.Text = "";
            this.Close();
        }

        public void AddToCart(string name, string category, decimal price)
        {
            foreach (DataGridViewRow row in dataGridViewCart.Rows)
            {
                if (row.Cells["Name_product"].Value != null && row.Cells["Name_product"].Value.ToString() == name)
                {
                    int quantity = Convert.ToInt32(row.Cells["Quantity"].Value);
                    row.Cells["Quantity"].Value = quantity + 1;
                    return; 
                }
            }

            dataGridViewCart.Rows.Add(name, category, price, 1);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridViewCart.SelectedRows.Count > 0)
            {
                var selectedRow = dataGridViewCart.SelectedRows[0];
                int quantity = Convert.ToInt32(selectedRow.Cells["Quantity"].Value);

                if (quantity > 1)
                {
                    selectedRow.Cells["Quantity"].Value = quantity - 1;
                }
                else
                {
                    dataGridViewCart.Rows.Remove(selectedRow);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DateTime date = DateTime.Now;
            
            string method = "";

            if (comboBoxMethod.Text == "Наличные") method = "Наличные";
            else if (comboBoxMethod.Text == "Карта") method = "Карта";
            else MessageBox.Show("Выберите способ оплаты!", "Пустые поля", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            decimal totalPrice = 0;

            foreach (DataGridViewRow row in dataGridViewCart.Rows)
            {
                int quantity = Convert.ToInt32(row.Cells["Quantity"].Value);
                decimal price = Convert.ToDecimal(row.Cells["Price"].Value);

                totalPrice = totalPrice + (quantity * price);
                if (checkClient) totalPrice = totalPrice - (totalPrice * Convert.ToDecimal(0.1));
            }

            saleProducts(date, method, totalPrice, pharmacistLogin);

            parametersCartProducts();

            dataGridViewCart.Rows.Clear();
            textBoxPhone.Text = "";
            comboBoxMethod.Text = "";
            this.Close();
        }

        private void parametersCartProducts()
        {
            try
            {
                foreach (DataGridViewRow row in dataGridViewCart.Rows)
                {
                    var idSale = IdSale;
                    var name = row.Cells["Name_product"].Value;
                    var quantity = row.Cells["Quantity"].Value;
                    object idClient = null;

                    if(checkClient)
                    {
                        dataBase.OpenConnection();

                        string queryGetId = @"Select Id_client FROM Clients WHERE Number_phone_client = @phone";

                        SqlCommand command = new SqlCommand(queryGetId, dataBase.getConnection());
                        command.Parameters.AddWithValue("@phone", textBoxPhone.Text);

                        idClient = command.ExecuteScalar();
                    }

                    SaleInfoProducts(idSale, name, quantity, idClient);
                }

        }catch (Exception ex)
            {
                MessageBox.Show("Ошибка:" + ex);
            }
}

        private void SaleInfoProducts(object idSale, object name, object quantity, object idClient)
        {
            try
            {
                dataBase.OpenConnection();

                string queryGetArticle = @"Select Article_product FROM Products WHERE Name_product = @name";

                SqlCommand commandArticle = new SqlCommand(queryGetArticle, dataBase.getConnection());
                commandArticle.Parameters.AddWithValue("@name", name);

                string article = commandArticle.ExecuteScalar().ToString();

                string query = @"INSERT INTO Sale_info (Id_sale, Article_product,Quantity,Id_client) VAlUES (@id, @article, @quantity, @idClient)";

                SqlCommand command = new SqlCommand(query, dataBase.getConnection());

                command.Parameters.AddWithValue("@id", idSale);
                command.Parameters.AddWithValue("@article", article);
                command.Parameters.AddWithValue("@quantity", quantity);
                if (idClient == null) command.Parameters.AddWithValue("@idClient", DBNull.Value);
                else command.Parameters.AddWithValue("@idClient", idClient);

                command.ExecuteNonQuery();

                dataBase.ClosedConnection();
            }catch(Exception ex)
            {
                MessageBox.Show("Ошибка:" + ex);
            }
        }

        private void saleProducts(DateTime date, string method, decimal totalPrice, string login)
        {
            try
            {
                dataBase.OpenConnection();

                string query = @"Insert INTO Sales (Date_sale, Payment_method, Total_price_sale, Login) OUTPUT INSERTED.Id_sale VALUES (@date, @method, @totalPrice, @login)";

                SqlCommand command = new SqlCommand(query, dataBase.getConnection());
                command.Parameters.AddWithValue("@date", date);
                command.Parameters.AddWithValue("@method", method);
                command.Parameters.AddWithValue("@totalPrice", totalPrice);
                command.Parameters.AddWithValue("@login", login);

                IdSale = (int)command.ExecuteScalar();

                dataBase.ClosedConnection();
            }catch(Exception ex)
            {
                MessageBox.Show("Ошибка:" + ex);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form_Pharmacist_Add_Clients add_Clients = new Form_Pharmacist_Add_Clients();
            add_Clients.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            dataBase.OpenConnection();

            string query = @"Select Count(*) FROM Clients Where Number_phone_client = @number";

            SqlCommand command = new SqlCommand(query, dataBase.getConnection());

            command.Parameters.AddWithValue("@number", textBoxPhone.Text);

            int count = (int)command.ExecuteScalar(); 

            if (count > 0)
            {
                MessageBox.Show("Пользователь найден");
                checkClient = true;
            }
            else
            {
                MessageBox.Show("Пользователь не найден");
                checkClient = false;
            }
        }
    }
}
