using OOOPharmacy.AdminForms;
using OOOPharmacy.AdminForms.AdminFormClass;
using OOOPharmacy.ManagerForms;
using OOOPharmacy.PharmacistForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OOOPharmacy
{
    public partial class Form_authorization : Form
    {


        DataBase database = new DataBase();
        Logs logs = new Logs();
        EncryptionUsers encryptionUsers = new EncryptionUsers();
        public string login;

        public Form_authorization()
        {
            InitializeComponent();
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Application.Exit(); //выход из приложения
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e) //показать/скрыть пароль
        {
            if (checkBoxShowPassword.Checked)
            {
                PasswordBox.PasswordChar = '\0';
            }
            else
            {
                PasswordBox.PasswordChar = '*';
            }
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            /*try {*/
                var userPassword = PasswordBox.Text;
                login = LoginBox.Text;

                var incorrectPassword = encryptionUsers.Encryption(userPassword);

                if (!(string.IsNullOrWhiteSpace(userPassword)) && !(string.IsNullOrWhiteSpace(login)))
                {

                    SqlDataAdapter adapter = new SqlDataAdapter();
                    DataTable table = new DataTable();

                    string query = $"SELECT Login, Password FROM Users WHERE Login = @login COLLATE Latin1_General_BIN AND Password = @password COLLATE Latin1_General_BIN";

                    SqlCommand command = new SqlCommand(query, database.getConnection());

                    command.Parameters.Add("@login", login);
                    command.Parameters.Add("@password", incorrectPassword);

                    adapter.SelectCommand = command;
                    adapter.Fill(table);

                    if (table.Rows.Count == 1)
                    {

                        //входим в систему по ролям
                        string queryRole = $"SELECT Speciality FROM Employees WHERE Login = '{login}'";

                        SqlCommand commandRole = new SqlCommand(queryRole, database.getConnection());

                        database.OpenConnection();

                        var roleUser = commandRole.ExecuteScalar().ToString();


                        if (roleUser == "Администратор")
                        {
                            logs.LogUsers(login, roleUser);
                            Form_Admin_Main formAdmin = new Form_Admin_Main();
                            this.Hide();
                            formAdmin.Show();
                        }//переход на форму администратора 

                        if (roleUser == "Менеджер")
                        {
                            logs.LogUsers(login, roleUser);
                            Form_Manager_Main formManager = new Form_Manager_Main();
                            this.Hide();
                            formManager.ShowDialog();
                        }//переход на форму менеджера

                        if (roleUser == "Фармацевт")
                        {
                            logs.LogUsers(login, roleUser);
                            Form_Pharmacist_Main formPharmacist = new Form_Pharmacist_Main();
                            formPharmacist.login = login;
                            this.Hide();
                            formPharmacist.ShowDialog();
                        }// переход на форму фармацевта

                        database.ClosedConnection();

                    } //если пользователь найден вход в систему
                    else
                    {
                        MessageBox.Show("Пользователь с такими данными не найден!", "Ошибка входа", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }//вывод сообщения "пользователь с такими данными не найден"

                } //проверка на пустые поля
                else
                {
                    MessageBox.Show("Введите данные!", "Пустые поля", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                } //вывод сообщения пользователю "пустые поля"
           /* }
            catch (Exception ex) 
            {
                MessageBox.Show("Ошибка:" + ex);
            }*/
        }   
    }   

}
