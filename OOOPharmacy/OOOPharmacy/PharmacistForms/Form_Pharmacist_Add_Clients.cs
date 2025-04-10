using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OOOPharmacy.PharmacistForms
{
    public partial class Form_Pharmacist_Add_Clients : Form
    {
        DataBase dataBase = new DataBase();

        public Form_Pharmacist_Add_Clients()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBoxName.Text = "";
            textBoxLastName.Text = "";
            textBoxMiddleName.Text = "";
            textBoxPhone.Text = "";
            textBoxEmail.Text = "";

            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                dataBase.OpenConnection();

                string query = @"Insert INTO Clients (Last_name_client, First_name_client, Middle_name_client, Number_phone_client, Email_client) 
                                VALUES (@lastName, @firstName, @midName, @phone, @email)";

                SqlCommand command = new SqlCommand(query, dataBase.getConnection());
                command.Parameters.AddWithValue("@lastName", textBoxLastName.Text);
                command.Parameters.AddWithValue("@firstName", textBoxName.Text) ;
                if (textBoxMiddleName.Text == null || string.IsNullOrWhiteSpace(textBoxMiddleName.Text)) command.Parameters.AddWithValue("@midName", DBNull.Value);
                else command.Parameters.AddWithValue("@midName", textBoxMiddleName.Text);
                command.Parameters.AddWithValue("@phone", textBoxPhone.Text);
                if (textBoxEmail.Text == null || string.IsNullOrWhiteSpace(textBoxEmail.Text)) command.Parameters.AddWithValue("@email", DBNull.Value);
                else command.Parameters.AddWithValue("@email", textBoxEmail.Text);

                command.ExecuteNonQuery();

                dataBase.ClosedConnection();

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка:" + ex);
            }
        }
    }
}
