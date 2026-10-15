using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.IO; 

namespace PR32
{
    public partial class AddEdit : Form
    {
        private string _currentArticleNumber;
        private string _photoFileName = null; 

        public AddEdit(string articleNumber = null)
        {
            InitializeComponent();
            _currentArticleNumber = articleNumber;

            // Подписки на валидацию
            textBox1.TextChanged += ValidateFields;
            textBox2.TextChanged += ValidateFields;
            textBox3.TextChanged += ValidateFields;
            textBox4.TextChanged += ValidateFields;
            textBox5.TextChanged += ValidateFields;
            textBox6.TextChanged += ValidateFields;
            textBox8.TextChanged += ValidateFields;

            cmdCat.SelectedIndexChanged += ValidateFields;
            cmdSup.SelectedIndexChanged += ValidateFields;
            cmdUnit.SelectedIndexChanged += ValidateFields;

            LoadComboBoxes();

            if (!string.IsNullOrEmpty(_currentArticleNumber))
            {
                LoadProductData(_currentArticleNumber);
                btnEdit.Enabled = true;
                btnAdd.Enabled = false;
            }
            else
            {
                btnAdd.Enabled = false;
                btnEdit.Enabled = false;
            }
        }

        private void btnImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
                openFileDialog.Title = "Выберите изображение товара";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    _photoFileName = Path.GetFileName(openFileDialog.FileName);

                    string targetDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
                    if (!Directory.Exists(targetDirectory))
                    {
                        Directory.CreateDirectory(targetDirectory);
                    }

                    string targetPath = Path.Combine(targetDirectory, _photoFileName);

                    File.Copy(openFileDialog.FileName, targetPath, true);

                    MessageBox.Show($"Изображение \"{_photoFileName}\" успешно выбрано и сохранено.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    /pictureBox2.Image = Image.FromFile(targetPath);
                }
            }
        }

        private void LoadProductData(string art)
        {
            ClassConnect connect = new ClassConnect();
            using (MySqlConnection con = new MySqlConnection(connect.connect))
            {
                con.Open();
                string query = $"SELECT * FROM product WHERE ProductArticleNumber = '{art.Replace("'", "''")}'";

                using (MySqlCommand cmd = new MySqlCommand(query, con))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        textBox8.Text = reader["ProductArticleNumber"].ToString();
                        textBox1.Text = reader["ProductName"].ToString();

                        cmdUnit.SelectedValue = reader["UnitID"];
                        textBox2.Text = reader["ProductCost"].ToString();
                        textBox3.Text = reader["ProductDiscountAmountMax"].ToString();
                        textBox4.Text = reader["ProductManufacturer"].ToString();

                        cmdSup.SelectedValue = reader["SupplierID"];
                        cmdCat.SelectedValue = reader["CategoryID"];

                        textBox5.Text = reader["ProductDiscountAmount"].ToString();
                        textBox6.Text = reader["ProductQuantityInStock"].ToString();
                        textBox7.Text = reader["ProductDescription"].ToString();

                        if (!reader.IsDBNull(reader.GetOrdinal("ProductPhoto")))
                        {
                            _photoFileName = reader["ProductPhoto"].ToString();

                            string imgPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", _photoFileName);
                             if (File.Exists(imgPath)) pictureBox2.Image = Image.FromFile(imgPath);
                        }
                        else
                        {
                            _photoFileName = null;
                        }
                    }
                }
            }
            ValidateFields(null, null);
        }

        private void ValidateFields(object sender, EventArgs e)
        {
            bool isTextBoxesValid = !string.IsNullOrWhiteSpace(textBox8.Text) &&
                                    !string.IsNullOrWhiteSpace(textBox1.Text) &&
                                    !string.IsNullOrWhiteSpace(textBox2.Text) &&
                                    !string.IsNullOrWhiteSpace(textBox3.Text) &&
                                    !string.IsNullOrWhiteSpace(textBox4.Text) &&
                                    !string.IsNullOrWhiteSpace(textBox5.Text) &&
                                    !string.IsNullOrWhiteSpace(textBox6.Text);

            bool isComboBoxesValid = cmdCat.SelectedIndex >= 0 &&
                                     cmdSup.SelectedIndex >= 0 &&
                                     cmdUnit.SelectedIndex >= 0;

            bool isNumbersValid = int.TryParse(textBox2.Text, out _) &&
                                  int.TryParse(textBox3.Text, out _) &&
                                  int.TryParse(textBox5.Text, out _) &&
                                  int.TryParse(textBox6.Text, out _);

            bool isValid = isTextBoxesValid && isComboBoxesValid && isNumbersValid;

            if (!string.IsNullOrEmpty(_currentArticleNumber))
            {
                btnEdit.Enabled = isValid;
                btnAdd.Enabled = false;
            }
            else
            {
                btnAdd.Enabled = isValid;
                btnEdit.Enabled = false;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            ClassConnect connect = new ClassConnect();

            try
            {
                using (MySqlConnection con = new MySqlConnection(connect.connect))
                {
                    con.Open();

                    string art = textBox8.Text.Trim().Replace("'", "''");
                    string name = textBox1.Text.Trim().Replace("'", "''");
                    string manufacturer = textBox4.Text.Trim().Replace("'", "''");
                    string description = textBox7.Text.Trim().Replace("'", "''");

                    int unitId = Convert.ToInt32(cmdUnit.SelectedValue);
                    int supplierId = Convert.ToInt32(cmdSup.SelectedValue);
                    int categoryId = Convert.ToInt32(cmdCat.SelectedValue);

                    int cost = Convert.ToInt32(textBox2.Text);
                    int maxAmount = Convert.ToInt32(textBox3.Text);
                    int amount = Convert.ToInt32(textBox5.Text);
                    int quantity = Convert.ToInt32(textBox6.Text);


                    string photoDbValue = string.IsNullOrEmpty(_photoFileName)
                        ? "NULL"
                        : $"'{_photoFileName.Replace("'", "''")}'";

                    string query = $@"INSERT INTO product 
                                      (ProductArticleNumber, ProductName, UnitID, ProductCost, 
                                       ProductDiscountAmountMax, ProductManufacturer, SupplierID, 
                                       CategoryID, ProductDiscountAmount, ProductQuantityInStock, 
                                       ProductDescription, ProductPhoto)
                                      VALUES 
                                      ('{art}', '{name}', {unitId}, {cost}, {maxAmount}, 
                                       '{manufacturer}', {supplierId}, {categoryId}, {amount}, 
                                       {quantity}, '{description}', {photoDbValue})";

                    using (MySqlCommand command = new MySqlCommand(query, con))
                    {
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Товар успешно добавлен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении:\n{ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_currentArticleNumber))
            {
                MessageBox.Show("Сначала загрузите данные товара для редактирования!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ClassConnect connect = new ClassConnect();

            try
            {
                using (MySqlConnection con = new MySqlConnection(connect.connect))
                {
                    con.Open();

                    string art = textBox8.Text.Trim().Replace("'", "''");
                    string name = textBox1.Text.Trim().Replace("'", "''");
                    string manufacturer = textBox4.Text.Trim().Replace("'", "''");
                    string description = textBox7.Text.Trim().Replace("'", "''");

                    int unitId = Convert.ToInt32(cmdUnit.SelectedValue);
                    int supplierId = Convert.ToInt32(cmdSup.SelectedValue);
                    int categoryId = Convert.ToInt32(cmdCat.SelectedValue);

                    int cost = Convert.ToInt32(textBox2.Text);
                    int maxAmount = Convert.ToInt32(textBox3.Text);
                    int amount = Convert.ToInt32(textBox5.Text);
                    int quantity = Convert.ToInt32(textBox6.Text);

                    string photoDbValue = string.IsNullOrEmpty(_photoFileName)
                        ? "NULL"
                        : $"'{_photoFileName.Replace("'", "''")}'";

                    string whereArt = _currentArticleNumber.Replace("'", "''");

                    string query = $@"UPDATE product SET 
                                      ProductArticleNumber = '{art}',
                                      ProductName = '{name}', 
                                      UnitID = {unitId}, 
                                      ProductCost = {cost}, 
                                      ProductDiscountAmountMax = {maxAmount}, 
                                      ProductManufacturer = '{manufacturer}', 
                                      SupplierID = {supplierId}, 
                                      CategoryID = {categoryId}, 
                                      ProductDiscountAmount = {amount}, 
                                      ProductQuantityInStock = {quantity}, 
                                      ProductDescription = '{description}',
                                      ProductPhoto = {photoDbValue}
                                      WHERE ProductArticleNumber = '{whereArt}'";

                    using (MySqlCommand command = new MySqlCommand(query, con))
                    {
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Данные товара обновлены!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при редактировании:\n{ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            _currentArticleNumber = null;
            _photoFileName = null; 

            textBox8.Clear();
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();
            textBox7.Clear();

            cmdUnit.SelectedIndex = -1;
            cmdSup.SelectedIndex = -1;
            cmdCat.SelectedIndex = -1;



            btnAdd.Enabled = false;
            btnEdit.Enabled = false;
        }

        private void LoadComboBoxes()
        {
            ClassConnect connect = new ClassConnect();
            using (MySqlConnection con = new MySqlConnection(connect.connect))
            {
                con.Open();

                MySqlDataAdapter adUnit = new MySqlDataAdapter("SELECT * FROM unit", con);
                DataTable dtUnit = new DataTable();
                adUnit.Fill(dtUnit);
                cmdUnit.DataSource = dtUnit;
                cmdUnit.DisplayMember = "UnitName";
                cmdUnit.ValueMember = "UnitID";
                cmdUnit.SelectedIndex = -1;

                MySqlDataAdapter adSup = new MySqlDataAdapter("SELECT * FROM supplier", con);
                DataTable dtSup = new DataTable();
                adSup.Fill(dtSup);
                cmdSup.DataSource = dtSup;
                cmdSup.DisplayMember = "SupplierName";
                cmdSup.ValueMember = "SupplierID";
                cmdSup.SelectedIndex = -1;

                MySqlDataAdapter adCat = new MySqlDataAdapter("SELECT * FROM category", con);
                DataTable dtCat = new DataTable();
                adCat.Fill(dtCat);
                cmdCat.DataSource = dtCat;
                cmdCat.DisplayMember = "CategoryName";
                cmdCat.ValueMember = "CategoryID";
                cmdCat.SelectedIndex = -1;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}