using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace PR32
{
    public partial class Import : Form
    {
        public Import()
        {
            InitializeComponent();
        }

        private void Import_Load(object sender, EventArgs e)
        {
            cmbTable.Items.Clear();

            cmbTable.Items.Add("category");
            cmbTable.Items.Add("supplier");
            cmbTable.Items.Add("unit");
            cmbTable.Items.Add("role");
            cmbTable.Items.Add("product");
            cmbTable.Items.Add("user");
            cmbTable.Items.Add("pickuppoint");
            cmbTable.Items.Add("orderstatus");
            cmbTable.Items.Add("order");
            cmbTable.Items.Add("orderproduct");

            txtFilePath.ReadOnly = true;
            txtFilePath.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(cmbTable.SelectedItem == null)
            {
                MessageBox.Show("Выберите таблицу.", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(txtFilePath.Text))
            {
                MessageBox.Show("Выберите CSV-файл.", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string tableName = cmbTable.SelectedItem.ToString();
            string filePath = txtFilePath.Text;

            if (!File.Exists(filePath))
            {
                MessageBox.Show("Файл не найден.", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ImportCsvToTable(tableName, filePath);
        }

        private void ImportCsvToTable(string tableName, string filePath)
        {
            string host = Properties.Settings.Default.host;
            string uid = Properties.Settings.Default.uid;
            string pwd = Properties.Settings.Default.pwd;

            string connect = $"host={host};uid={uid};pwd={pwd};database=trade;";

            try
            {
                
                string[] lines = File.ReadAllLines(filePath, Encoding.GetEncoding(1251));

                if (lines.Length < 2)
                {
                    MessageBox.Show("Файл пуст или содержит только заголовки.", "Ошибка");
                    return;
                }

                //Убираем кавычки и пробелы из заголовков
                string[] headers = lines[0]
                    .Split(';')
                    .Select(h => h.Trim().Trim('"').Trim())
                    .ToArray();

                using (MySqlConnection con = new MySqlConnection(connect))
                {
                    con.Open();

                    int successCount = 0;
                    int errorCount = 0;
                    string lastError = "";
                    int lastErrorRow = -1;

                    for (int i = 1; i < lines.Length; i++)
                    {
                        // Убираем кавычки и пробелы из значений
                        string[] values = lines[i]
                            .Split(';')
                            .Select(v => v.Trim().Trim('"').Trim())
                            .ToArray();

                        if (values.Length != headers.Length)
                        {
                            errorCount++;
                            lastError = $"Несовпадение колонок: {values.Length} vs {headers.Length}";
                            lastErrorRow = i;
                            continue;
                        }

                        string columns = string.Join(", ", headers.Select(h => $"`{h}`"));
                        string parameters = string.Join(", ", headers.Select(h => "@" + h));

                        string query = $"INSERT INTO {tableName} ({columns}) VALUES ({parameters})";

                        using (MySqlCommand cmd = new MySqlCommand(query, con))
                        {
                            for (int j = 0; j < headers.Length; j++)
                            {
                                cmd.Parameters.AddWithValue("@" + headers[j], values[j]);
                            }

                            try
                            {
                                cmd.ExecuteNonQuery();
                                successCount++;
                            }
                            catch (Exception ex)
                            {
                                errorCount++;
                                lastError = ex.Message;
                                lastErrorRow = i;
                            }
                        }
                    }

                    string message = $"Импорт завершён!\n\n" +
                                     $"Успешно: {successCount}\n" +
                                     $"Ошибок: {errorCount}";

                    if (!string.IsNullOrEmpty(lastError))
                    {
                        message += $"\n\nПоследняя ошибка (строка {lastErrorRow}):\n{lastError}";
                    }

                    MessageBox.Show(message, "Результат",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.Hide();
                    new userForm().ShowDialog();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка импорта: " + ex.Message,
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog file = new OpenFileDialog())
            {
                file.Filter = "CSV файлы (*.csv)|*.csv|Все файлы (*.*)|*.*";

                file.Title = "Выберите CSV-файл для импорта";

                if (file.ShowDialog() == DialogResult.OK)
                {
                    txtFilePath.Text = file.FileName;
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();
            new userForm().ShowDialog();
            this.Close();
        }
    
    }
}
