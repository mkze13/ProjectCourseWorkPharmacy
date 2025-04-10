using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OOOPharmacy.AdminForms.AdminFormClass
{
    internal class Logs
    {
        private static string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"..\..\Resource\Logs.txt");

        public void LogUsers(string userName, string role)
        {
            string timeLog = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string logMessage = $"{userName} ({role}) вошел в систему в {timeLog}";

            try
            {
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine(logMessage);
                }
            }
            catch (Exception ex)
            {
                // Обработка ошибок записи в файл
                MessageBox.Show($"Ошибка при записи в лог-файл: {ex.Message}");
            }
        }

        public string ReadLogFile()
        {
            try
            {
                if (File.Exists(logFilePath))
                {
                    return File.ReadAllText(logFilePath);
                }
                else
                {
                    return "Файл логов не найден.";
                }
            }
            catch (Exception ex)
            {
                return $"Ошибка при чтении лог-файла: {ex.Message}";
            }
        }
    }
}
