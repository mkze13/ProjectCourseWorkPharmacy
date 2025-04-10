using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;


namespace OOOPharmacy.PharmacistForms
{
    public partial class Form_Pharmacist_Main : Form
    {

        DataBase dataBase = new DataBase();
        Form_Pharmacist_Cart cartForm = new Form_Pharmacist_Cart();

        public int counterRecord = 0;

        public string login;

        public Form_Pharmacist_Main()
        {
            InitializeComponent();
        }

        
        private void CreateColumns()
        {
            dataGridViewProducts.Columns.Add("Id_branch_product", "Код записи");
            dataGridViewProducts.Columns.Add("Name_product", "Название товара");
            dataGridViewProducts.Columns.Add("Category", "Категория");
            dataGridViewProducts.Columns.Add("Price", "Цена");
            dataGridViewProducts.Columns.Add("Count_in_stock", "В наличии");
            dataGridViewProducts.Columns.Add("Adress", "Адрес магазина");
            dataGridViewProducts.Columns.Add("Manufacture_date", "Дата изготовления");
            dataGridViewProducts.Columns.Add("Expiration_date", "Срок годности");
        }//создание  столбцов для таблицы

        private void ReadRow(DataGridView datagridview, IDataRecord record)
        {
            datagridview.Rows.Add(record.GetInt32(0), record.GetString(1), record.GetString(2), record.GetDecimal(3), record.GetInt32(4), 
                record.GetString(5), record.GetDateTime(6).ToString("dd-MM-yyyy"), record.GetDateTime(6).ToString("dd-MM-yyyy"));
        }//добавляем строку таблицы с данными


        private void RefreshDataGridView(DataGridView datagridview)
        {
            datagridview.Rows.Clear();

            string query = @"
            SELECT pb.Id_branch_product, p.Name_product, p.Category, p.Price, pb.Count_in_stock, CONCAT('г ', b.City_branches, ', ул ', b.Street_branches, ', д ', b.House_branches) AS Address, p.Manufacture_date, p.Expiration_date
            FROM Products p
            JOIN Branches_products pb ON p.Article_product = pb.Article_product
            JOIN Branches b ON pb.Id_branch = b.Id_branch";


            SqlCommand command = new SqlCommand(query, dataBase.getConnection());

            dataBase.OpenConnection();

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                ReadRow(datagridview, reader);
            }

            reader.Close();

        } //добавляем и обновляем данные в таблице

        private void BackButton_Click(object sender, EventArgs e)
        {
            Form_authorization formAuthorization = new Form_authorization();
            this.Hide();

            formAuthorization.ShowDialog();
        }//кнопка назад

        private void Form_Pharmacist_Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }// обработка закрытия формы

        private void Form_Pharmacist_Main_Load(object sender, EventArgs e)
        {
            CreateColumns();
            LoadBranchAddresses();
            RefreshDataGridView(dataGridViewProducts);

            labelCounterCart.Text = counterRecord.ToString();

        }//загрузка формы

        private void LoadBranchAddresses()
        {
            string query = "SELECT CONCAT('г ', City_branches, ', ул ', Street_branches, ', д ', House_branches) AS Address FROM Branches";

            SqlCommand command = new SqlCommand(query, dataBase.getConnection());

            dataBase.OpenConnection();

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                comboBox1.Items.Add(reader.GetString(0));
            }

            reader.Close();
            dataBase.ClosedConnection();
        }//добавляем адреса в comboBox

        private void Search(DataGridView datagridview)
        {
            datagridview.Rows.Clear();

            string query = $@"
                            SELECT pb.Id_branch_product, p.Name_product, p.Category, p.Price, pb.Count_in_stock, 
                            CONCAT('г ', b.City_branches, ', ул ', b.Street_branches, ', д ', b.House_branches) AS Address, 
                            p.Manufacture_date, p.Expiration_date
                            FROM Products p
                            JOIN Branches_products pb ON p.Article_product = pb.Article_product
                            JOIN Branches b ON pb.Id_branch = b.Id_branch
                            WHERE CONCAT(pb.Id_branch_product, p.Name_product, p.Category, p.Price, pb.Count_in_stock, 
                            'г ', b.City_branches, ' ул ', b.Street_branches, ' д ', b.House_branches, 
                            p.Manufacture_date, p.Expiration_date) LIKE '%" + textBox1.Text + "%'";



            SqlCommand command = new SqlCommand(query, dataBase.getConnection());

            dataBase.OpenConnection();

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                ReadRow(datagridview, reader);
            }

            reader.Close();

        } //функция для реализации поиска

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Search(dataGridViewProducts);
        } //поиск

        private void addressFilter(DataGridView datagridview, string address) //функция для реализации фильтрации
        {
            datagridview.Rows.Clear();

            string query = $@"
                            SELECT pb.Id_branch_product, p.Name_product, p.Category, p.Price, pb.Count_in_stock, 
                            CONCAT('г ', b.City_branches, ', ул ', b.Street_branches, ', д ', b.House_branches) AS Address, 
                            p.Manufacture_date, p.Expiration_date
                            FROM Products p
                            JOIN Branches_products pb ON p.Article_product = pb.Article_product
                            JOIN Branches b ON pb.Id_branch = b.Id_branch
                            WHERE CONCAT('г ', b.City_branches, ', ул ', b.Street_branches, ', д ', b.House_branches) = '{address}'";   

            SqlCommand command = new SqlCommand(query, dataBase.getConnection());

            dataBase.OpenConnection();

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                ReadRow(datagridview, reader);
            }

            reader.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) //фильтрация
        {
            string address = comboBox1.SelectedItem.ToString();
            addressFilter(dataGridViewProducts, address);
        }

        private void button1_Click(object sender, EventArgs e) //сброс
        {
            RefreshDataGridView(dataGridViewProducts);
            comboBox1.Text = "";
            textBox1.Text = "";
            counterRecord = 0;
            labelCounterCart.Text = counterRecord.ToString();
        }

        private void mySort(DataGridView dataGridView, string checkField, string typeSort)
        {
            dataGridView.Rows.Clear();

            string address = comboBox1.Text;

            string query = "";

            if (!(address == ""))
            {
                query = $@"
                        SELECT pb.Id_branch_product, p.Name_product, p.Category, p.Price, pb.Count_in_stock, 
                        CONCAT('г ', b.City_branches, ', ул ', b.Street_branches, ', д ', b.House_branches) AS Address, 
                        p.Manufacture_date, p.Expiration_date
                        FROM Products p
                        JOIN Branches_products pb ON p.Article_product = pb.Article_product
                        JOIN Branches b ON pb.Id_branch = b.Id_branch
                        WHERE CONCAT('г ', b.City_branches, ', ул ', b.Street_branches, ', д ', b.House_branches) = '{address}'
                        ORDER BY {checkField} {typeSort}";
            }
            else
            {
                query = $@"
                        SELECT pb.Id_branch_product, p.Name_product, p.Category, p.Price, pb.Count_in_stock, 
                        CONCAT('г ', b.City_branches, ', ул ', b.Street_branches, ', д ', b.House_branches) AS Address, 
                        p.Manufacture_date, p.Expiration_date
                        FROM Products p
                        JOIN Branches_products pb ON p.Article_product = pb.Article_product
                        JOIN Branches b ON pb.Id_branch = b.Id_branch
                        ORDER BY {checkField} {typeSort}";
            }



            SqlCommand command = new SqlCommand(query, dataBase.getConnection());

            dataBase.OpenConnection();

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                ReadRow(dataGridView, reader);
            }

            reader.Close();
            dataBase.ClosedConnection();
        }//реализация сортировки

        private void sortButton_Click(object sender, EventArgs e)
        {
            string checkField;
            string typeSort;

            if (upRadioButton.Checked)
            {
                typeSort = "ASC"; 
            }
            else if (downRadioButton.Checked)
            {
                typeSort = "DESC";
            }
            else
            {
                return;
            }

            if (priceRadioButton.Checked)
            {
                checkField = "Price";
                mySort(dataGridViewProducts, checkField, typeSort);
            }
            if (quantityRadioButton.Checked)
            {
                checkField = "Count_in_stock";
                mySort(dataGridViewProducts, checkField, typeSort);
            }
            if (dateRadioButton.Checked)
            {
                checkField = "Manufacture_date";
                mySort(dataGridViewProducts, checkField, typeSort);
            }
        }//сортировка

        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                counterRecord++;
                labelCounterCart.Text = counterRecord.ToString();

                var row = dataGridViewProducts.Rows[e.RowIndex];
                string name = row.Cells["Name_product"].Value.ToString();
                string category = row.Cells["Category"].Value.ToString();
                decimal price = Convert.ToDecimal(row.Cells["Price"].Value);

                cartForm.AddToCart(name, category, price);
               

            }
            
        }

        private void buttonCart_Click(object sender, EventArgs e)
        {
            cartForm.pharmacistLogin = login;
            cartForm.ShowDialog();
            counterRecord = 0;
            labelCounterCart.Text = "0";
        }
    }
}
