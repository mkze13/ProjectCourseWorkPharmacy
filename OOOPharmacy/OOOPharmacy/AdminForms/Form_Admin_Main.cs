using OOOPharmacy.AdminForms.AdminFormClass;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace OOOPharmacy.AdminForms
{
    public partial class Form_Admin_Main : Form
    {
        DataBase dataBase = new DataBase();
        TableUsers tableUsers = new TableUsers();
        EncryptionUsers encryptionUsers = new EncryptionUsers();
        Logs logs = new Logs();
        public string oldLogin;
        public bool showPassword = false;
        int index = 0;

        public Form_Admin_Main()
        {
            InitializeComponent();
        }


        private void ExitButton_Click(object sender, EventArgs e)
        {
            Form_authorization form = new Form_authorization();
            this.Hide();
            form.ShowDialog();
        }

        private void Form_Admin_Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void пользователиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.Visible = true;
            tabControl2.Visible = false;
        }

        private void Form_Admin_Main_Load(object sender, EventArgs e) 
        {
            tableUsers.CreateColumns(dataGridViewUsers);
            tableUsers.RefreshDataGridView(dataGridViewUsers, checkBoxShowPasswords);
            tableUsers.UpdateTextBoxesInfo(index, textBoxChangeLogin, textBoxChangePassword);
            toolStripLabelLast.Text = tableUsers.usersList.Count.ToString();
            toolStripLabelFirst.Text = "1";
            string logContent = logs.ReadLogFile();
            richTextBoxLogs.Text = logContent;


        } //действия при загрузке формы

        private void button3_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите удалить данную запись?", "Предупреждение", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
            {
                return;
            }

            dataBase.OpenConnection();

            var login = dataGridViewUsers.SelectedRows[0].Cells["Login"].Value.ToString();

            string query = $"DELETE FROM Users WHERE Login = '{login}'";

            SqlCommand command = new SqlCommand(query, dataBase.getConnection());

            command.ExecuteNonQuery();

            dataBase.ClosedConnection();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            tableUsers.RefreshDataGridView(dataGridViewUsers, checkBoxShowPasswords);
            toolStripLabelLast.Text = tableUsers.usersList.Count.ToString();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxShowPasswords.Checked) 
            {
                encryptionUsers.Decryption(dataGridViewUsers);
                showPassword = false;
            }
            else
            {
                encryptionUsers.Decryption(dataGridViewUsers);
                showPassword = true;
            }
        }

        private void buttonChange_Click(object sender, EventArgs e)
        {
            if (dataGridViewUsers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Пожалуйста, выберите запись для изменения.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            oldLogin = dataGridViewUsers.SelectedCells[0].Value.ToString();

            if (showPassword == true)
            {
                textBoxChangePassword.Text = encryptionUsers.Encryption(dataGridViewUsers.SelectedCells[1].Value.ToString());
            }
            else
            {
                textBoxChangePassword.Text = dataGridViewUsers.SelectedCells[1].Value.ToString();
            }

            textBoxChangeLogin.Text = dataGridViewUsers.SelectedCells[0].Value.ToString();

            tabControl1.SelectTab(0);
        } //переход к форме изменения

        private void buttonAddUser_Click(object sender, EventArgs e)
        {
            string login = textBoxLogin.Text;
            string StartPassword = textBoxPassword.Text;

            string password = encryptionUsers.Encryption(StartPassword); //шифруем пароль

            if ((login != null) && (password != null))
            {
                string query = $"INSERT INTO Users(Login, Password) VALUES('{login}','{password}')";

                SqlCommand command = new SqlCommand(query, dataBase.getConnection());

                dataBase.OpenConnection();

                if (command.ExecuteNonQuery() == 1) //проверка добавления строки
                {
                    MessageBox.Show("Пользователь успешно добавлен!", "Добавление пользователя", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Ползователь не был добавлен!", "Добавление пользователя", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                dataBase.ClosedConnection();

                textBoxLogin.Text = "";
                textBoxPassword.Text = "";
            } //проверка и добавление записи
            else
            {
                MessageBox.Show("Введите данные!", "Пустые поля", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }//добавление нового пользователя

        private void buttonChange_Click_1(object sender, EventArgs e)
        {
            string login = textBoxChangeLogin.Text;
            string password = textBoxChangePassword.Text;

            tableUsers.UpdateUser(login, oldLogin, password);
        }

        private void buttonPrevious_Click(object sender, EventArgs e)
        {
            if (index > 0)
            {
                index--;
                toolStripLabelFirst.Text = (index + 1).ToString();
                tableUsers.UpdateTextBoxesInfo(index,textBoxChangeLogin, textBoxChangePassword);
            }
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            if (index < tableUsers.usersList.Count - 1)
            {
                
                index++;
                toolStripLabelFirst.Text = (index + 1).ToString();
                tableUsers.UpdateTextBoxesInfo(index, textBoxChangeLogin, textBoxChangePassword);
            }
        }

        private void резервноеКопированиеToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void логиПользователейToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.Visible = false;
            tabControl2.Visible = true;
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void загрузитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form_authorization form = new Form_authorization();
            form.Show();

        }
    }
}
