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

            cmbTable.Items.Add("categories");
            cmbTable.Items.Add("product");
            cmbTable.Items.Add("role");
            cmbTable.Items.Add("sale");
            cmbTable.Items.Add("saleitem");
            cmbTable.Items.Add("storehouse");
            cmbTable.Items.Add("suppliers");
            cmbTable.Items.Add("user");

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
            ClassConnect connection = new ClassConnect();

            try
            {
                //Читаем CSV
                string[] lines = File.ReadAllLines(filePath);

                if (lines.Length < 2)
                {
                    MessageBox.Show("Файл пуст или содержит только заголовки.", "Ошибка");
                    return;
                }

                // Первая строка — заголовки (названия колонок)
                string[] headers = lines[0].Split(';');   // или ',' — зависит от CSV

                using (MySqlConnection con = new MySqlConnection(connection.connect))
                {
                    con.Open();

                    int successCount = 0;
                    int errorCount = 0;

                    //Начинаем со 2-й строки (первая — заголовки)
                    for (int i = 1; i < lines.Length; i++)
                    {
                        string[] values = lines[i].Split(';');

                        if (values.Length != headers.Length)
                        {
                            errorCount++;
                            continue;   // пропускаем битую строку
                        }

                        
                        string columns = string.Join(", ", headers);
                        string parameters = string.Join(", ", headers.Select(h => "@" + h.Trim()));

                        string query = $"INSERT INTO {tableName} ({columns}) VALUES ({parameters})";

                        using (MySqlCommand cmd = new MySqlCommand(query, con))
                        {
                            for (int j = 0; j < headers.Length; j++)
                            {
                                cmd.Parameters.AddWithValue("@" + headers[j].Trim(), values[j].Trim());
                            }

                            try
                            {
                                cmd.ExecuteNonQuery();
                                successCount++;
                            }
                            catch
                            {
                                errorCount++;
                            }
                        }
                    }

                    MessageBox.Show(
                        $"Импорт завершён!",
                        "Результат",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
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
    }
}
