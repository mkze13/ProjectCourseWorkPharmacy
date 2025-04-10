using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OOOPharmacy.AdminForms.AdminFormClass
{
    internal class EncryptionUsers
    {
        public string Encryption(string IncorrectPassword)
        {
            char[] array = IncorrectPassword.ToCharArray(); //создаем массив символов
            int n = array.Length; //получаем длину  
            for (int i = 0; i < n / 2; i++) 
            {
                char temp = array[i];
                array[i] = array[n - i - 1];
                array[n - i - 1] = temp;
            } //меняем символы местами
            return new string(array); //возвращаем нужный пароль
        } // шифрование/дешифрование

        public void Decryption(DataGridView dataGridView)
        {
            int rowIndex = 0;
            while (rowIndex < dataGridView.Rows.Count)
            {
                var password = dataGridView.Rows[rowIndex].Cells[1].Value;

                var newPassword = Encryption(Convert.ToString(password));

                dataGridView.Rows[rowIndex].Cells[1].Value = newPassword;

                rowIndex++; 
            }
        }
    }
}
