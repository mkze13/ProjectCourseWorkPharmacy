using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.IO;
using OOOPharmacy.AdminForms;

namespace OOOPharmacy.ManagerForms.ManagerFormClasses
{
    internal class TableEmployees
    {
        DataBase dataBase = new DataBase();

        public void CreateColumns(DataGridView dataGridView)
        {
            try
            {
                dataGridView.Columns.Add("Guid", "Идентификатор");
                dataGridView.Columns["Guid"].ReadOnly = true;

                dataGridView.Columns.Add("Last_name", "Фамилия");
                dataGridView.Columns.Add("First_name", "Имя");
                dataGridView.Columns.Add("Middle_name", "Отчество");
                dataGridView.Columns.Add("Speciality", "Специальность");
                dataGridView.Columns.Add("City", "Город");
                dataGridView.Columns.Add("Street", "Улица");
                dataGridView.Columns.Add("House", "Дом");
                dataGridView.Columns.Add("Apartment", "Квартира");
                dataGridView.Columns.Add("Passport_series", "Серия паспорта");
                dataGridView.Columns.Add("Passport_number", "Номер паспорта");
                dataGridView.Columns.Add("Passport_issue_date", "Дата выдачи паспорта");
                dataGridView.Columns.Add("Login", "Логин пользователя");
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
                datagridview.Rows.Add(record.GetValue(0).ToString(), record.GetString(1), record.GetString(2), record.IsDBNull(3) ? "Нет данных" : record.GetString(3), record.GetString(4),
                record.GetString(5), record.GetString(6), record.IsDBNull(7), record.IsDBNull(8) ? "Нет данных" : record.GetString(8), record.GetString(9), record.GetString(10),
                    record.GetDateTime(11).ToString("dd-MM-yyyy"), record.GetString(12));
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

                string query = @"SELECT e.Guid, e.Last_name, e.First_name, e.Middle_name, e.Speciality, a.City, a.Street, a.House, a.Apartment, p.Passport_series, p.Passport_number,
                            p.Passport_issue_date, e.Login
                            FROM Employees e
                            JOIN Address_employees a ON e.Address_id = a.Address_id
                            JOIN Passport_employees p ON e.Passport_id = p.Passport_id";

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

        private void UpdateDatabase(object guid, object lastName, object firstName, object middleName,
                            object speciality, object city, object street, object house,
                            object apartment, object passportSeries, object passportNumber,
                            object passportIssueDate, object login)
        {
            try
            {

                dataBase.OpenConnection();


                string queryCheckUser = "SELECT COUNT(*) FROM Users WHERE Login = @login";
                SqlCommand commandCheckUser = new SqlCommand(queryCheckUser, dataBase.getConnection());
                commandCheckUser.Parameters.AddWithValue("@login", login);

                int userCount = (int)commandCheckUser.ExecuteScalar();

                if (userCount == 0)
                {
                    MessageBox.Show("Пользователь с таким логином не существует!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    dataBase.ClosedConnection();
                    return;
                }

                string queryAddress = @"
                                    UPDATE Address_employees 
                                    SET City = @city, Street = @street, House = @house, Apartment = @apartment
                                    WHERE Address_id = (SELECT Address_id FROM Employees WHERE Guid = @guid)";

                SqlCommand commandAddress = new SqlCommand(queryAddress, dataBase.getConnection());
                commandAddress.Parameters.AddWithValue("@city", city);
                commandAddress.Parameters.AddWithValue("@street", street);
                commandAddress.Parameters.AddWithValue("@house", house);

                if (apartment == null) commandAddress.Parameters.AddWithValue("@apartment", DBNull.Value);

                else commandAddress.Parameters.AddWithValue("@apartment", apartment);

                commandAddress.Parameters.AddWithValue("@guid", guid);

                commandAddress.ExecuteNonQuery();

                string queryPassport = @"
                                    UPDATE Passport_employees 
                                    SET Passport_series = @passportSeries, Passport_number = @passportNumber, Passport_issue_date = @passportIssueDate
                                    WHERE Passport_id = (SELECT Passport_id FROM Employees WHERE Guid = @guid)";

                SqlCommand commandPassport = new SqlCommand(queryPassport, dataBase.getConnection());
                commandPassport.Parameters.AddWithValue("@passportSeries", passportSeries);
                commandPassport.Parameters.AddWithValue("@passportNumber", passportNumber);
                commandPassport.Parameters.AddWithValue("@passportIssueDate", passportIssueDate);
                commandPassport.Parameters.AddWithValue("@guid", guid);

                commandPassport.ExecuteNonQuery();

                string queryEmployees = @"
                                    UPDATE Employees 
                                    SET Last_name = @lastName, First_name = @firstName, Middle_name = @middleName, 
                                        Speciality = @speciality, Login = @login
                                    WHERE Guid = @guid";

                SqlCommand commandEmployees = new SqlCommand(queryEmployees, dataBase.getConnection());
                commandEmployees.Parameters.AddWithValue("@guid", guid);
                commandEmployees.Parameters.AddWithValue("@lastName", lastName);
                commandEmployees.Parameters.AddWithValue("@firstName", firstName);

                if (middleName == null) commandEmployees.Parameters.AddWithValue("@middleName", DBNull.Value);
                else commandEmployees.Parameters.AddWithValue("@middleName", middleName);

                commandEmployees.Parameters.AddWithValue("@speciality", speciality);
                commandEmployees.Parameters.AddWithValue("@login", login);

                commandEmployees.ExecuteNonQuery();

                dataBase.ClosedConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка:" + ex);
            }

        } // обновляем данные в базе данных

        private void SaveDatabase(object lastName, object firstName, object middleName, object speciality,
                          object city, object street, object house, object apartment,
                          object passportSeries, object passportNumber, object passportIssueDate,
                          object login)
        {
            try
            {
                dataBase.OpenConnection();

                string queryAddress = @"
                                    INSERT INTO Address_employees (City, Street, House, Apartment) 
                                    VALUES (@city, @street, @house, @apartment);
                                    SELECT SCOPE_IDENTITY();";

                SqlCommand commandAddress = new SqlCommand(queryAddress, dataBase.getConnection());
                commandAddress.Parameters.AddWithValue("@city", city);
                commandAddress.Parameters.AddWithValue("@street", street);
                commandAddress.Parameters.AddWithValue("@house", house);

                if (apartment == null) commandAddress.Parameters.AddWithValue("@apartment", DBNull.Value);
                else commandAddress.Parameters.AddWithValue("@apartment", apartment);

                object addressId = commandAddress.ExecuteScalar();

                string queryPassport = @"
                                    INSERT INTO Passport_employees (Passport_series, Passport_number, Passport_issue_date) 
                                    VALUES (@passportSeries, @passportNumber, @passportIssueDate);
                                    SELECT SCOPE_IDENTITY();";

                SqlCommand commandPassport = new SqlCommand(queryPassport, dataBase.getConnection());
                commandPassport.Parameters.AddWithValue("@passportSeries", passportSeries);
                commandPassport.Parameters.AddWithValue("@passportNumber", passportNumber);
                commandPassport.Parameters.AddWithValue("@passportIssueDate", passportIssueDate);

                object passportId = commandPassport.ExecuteScalar();

                string queryEmployees = @"
                                    INSERT INTO Employees (Last_name, First_name, Middle_name, Speciality, Address_id, Passport_id, Login) 
                                    VALUES (@lastName, @firstName, @middleName, @speciality, @addressId, @passportId, @login)";

                SqlCommand commandEmployees = new SqlCommand(queryEmployees, dataBase.getConnection());
                commandEmployees.Parameters.AddWithValue("@lastName", lastName);
                commandEmployees.Parameters.AddWithValue("@firstName", firstName);

                if (middleName == null) commandEmployees.Parameters.AddWithValue("@middleName", DBNull.Value);
                else commandEmployees.Parameters.AddWithValue("@middleName", middleName);

                commandEmployees.Parameters.AddWithValue("@speciality", speciality);
                commandEmployees.Parameters.AddWithValue("@addressId", addressId);
                commandEmployees.Parameters.AddWithValue("@passportId", passportId);
                commandEmployees.Parameters.AddWithValue("@login", login);

                commandEmployees.ExecuteNonQuery();


                dataBase.ClosedConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка:" + ex);
            }


        } // добавляем данные в базу данных

        public void parametersForEmployees(DataGridView dataGrid)
        {
            try
            {
                bool saveSuccess = false;

                foreach (DataGridViewRow row in dataGrid.Rows)
                {
                    var guid = row.Cells["Guid"].Value;
                    var lastName = row.Cells["Last_name"].Value;
                    var firstName = row.Cells["First_name"].Value;
                    var middleName = row.Cells["Middle_name"].Value;
                    var speciality = row.Cells["Speciality"].Value;
                    var city = row.Cells["City"].Value;
                    var street = row.Cells["Street"].Value;
                    var house = row.Cells["House"].Value;
                    var apartment = row.Cells["Apartment"].Value;
                    var passportSeries = row.Cells["Passport_series"].Value;
                    var passportNumber = row.Cells["Passport_number"].Value;
                    var passportIssueDate = row.Cells["Passport_issue_date"].Value;
                    var login = row.Cells["Login"].Value;

                    if (!CheckIfCellIsEmpty(lastName) ||
                        !CheckIfCellIsEmpty(firstName) ||
                        !CheckIfCellIsEmpty(speciality) ||
                        !CheckIfCellIsEmpty(city) ||
                        !CheckIfCellIsEmpty(street) ||
                        !CheckIfCellIsEmpty(house) ||
                        !CheckIfCellIsEmpty(passportSeries) ||
                        !CheckIfCellIsEmpty(passportNumber) ||
                        !CheckIfCellIsEmpty(passportIssueDate) ||
                        !CheckIfCellIsEmpty(login))
                    {
                        MessageBox.Show("Введены пустые данные, запись не будет сохранена!", "Пустые поля", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        saveSuccess = true;
                        continue;
                    }

                    if (guid == null || string.IsNullOrWhiteSpace(guid.ToString()))
                    {
                        guid = "1";
                        SaveDatabase( lastName, firstName, middleName, speciality, city, street, house, apartment, passportSeries, passportNumber, passportIssueDate, login);
                    }
                    else
                    {
                        UpdateDatabase(guid, lastName, firstName, middleName, speciality, city, street, house, apartment, passportSeries, passportNumber, passportIssueDate, login);
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

        public void SearchEmployees(DataGridView datagridview, TextBoxBase textBoxForSearh)
        {
            try
            {
                datagridview.Rows.Clear();

                string query = @"
                            SELECT e.Guid, e.Last_name, e.First_name, e.Middle_name, e.Speciality, a.City, a.Street, a.House, a.Apartment,
                            p.Passport_series,p.Passport_number,p.Passport_issue_date, e.Login
                            FROM Employees e
                            JOIN Address_employees a ON e.Address_id = a.Address_id
                            JOIN Passport_employees p ON e.Passport_id = p.Passport_id
                            WHERE CONCAT(e.Guid, ' ', e.Last_name, ' ', e.First_name, ' ', e.Middle_name, ' ', e.Speciality, ' ', a.City,
                            ' ', a.Street, ' ', a.House, ' ', a.Apartment, ' ', p.Passport_series, ' ', p.Passport_number, ' ', p.Passport_issue_date,
                            ' ', e.Login) 
                            LIKE '%" + textBoxForSearh.Text + "%'";

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

        } //функция для реализации поиска

        public void DeleteEmployees(DataGridView dataGridView)
        {
            try
            {
                if (MessageBox.Show("Вы действительно хотите удалить данную запись?", "Предупреждение", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
                {
                    return;
                }

                dataBase.OpenConnection();

                var guid = dataGridView.SelectedRows[0].Cells["Guid"].Value.ToString();

                string getRelatedIdsQuery = @"
            SELECT Address_id, Passport_id FROM Employees WHERE Guid = @guid";

                SqlCommand getRelatedIdsCommand = new SqlCommand(getRelatedIdsQuery, dataBase.getConnection());
                getRelatedIdsCommand.Parameters.AddWithValue("@guid", guid);

                SqlDataReader reader = getRelatedIdsCommand.ExecuteReader();

                object addressId = null;
                object passportId = null;

                if (reader.Read())
                {
                    addressId = reader["Address_id"];
                    passportId = reader["Passport_id"];
                }

                reader.Close();


                string deleteEmployeeQuery = @"DELETE FROM Employees WHERE Guid = @guid";

                SqlCommand deleteEmployeeCommand = new SqlCommand(deleteEmployeeQuery, dataBase.getConnection());
                deleteEmployeeCommand.Parameters.AddWithValue("@guid", guid);

                deleteEmployeeCommand.ExecuteNonQuery();


                if (addressId != null)
                {
                    string deleteAddressQuery = @"DELETE FROM Address_employees WHERE Address_id = @addressId";

                    SqlCommand deleteAddressCommand = new SqlCommand(deleteAddressQuery, dataBase.getConnection());
                    deleteAddressCommand.Parameters.AddWithValue("@addressId", addressId);
                    deleteAddressCommand.ExecuteNonQuery();
                }

                if (passportId != null)
                {
                    string deletePassportQuery = @"DELETE FROM Passport_employees WHERE Passport_id = @passportId";

                    SqlCommand deletePassportCommand = new SqlCommand(deletePassportQuery, dataBase.getConnection());
                    deletePassportCommand.Parameters.AddWithValue("@passportId", passportId);
                    deletePassportCommand.ExecuteNonQuery();
                }

                dataBase.ClosedConnection();

                MessageBox.Show("Запись удалена!", "Уведомление", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка:" + ex);
            }
        }

    }
}
