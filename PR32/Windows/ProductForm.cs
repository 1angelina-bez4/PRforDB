using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace PR32
{
    public partial class ProductForm : System.Windows.Forms.Form
    {
        private const int PAGE_SIZE = 15;
        private int currentPage = 1;
        private int totalFilteredRecords = 0; 
        private int totalAllRecords = 0;      

        public ProductForm()
        {
            InitializeComponent();

     
            txtSearch.TextChanged += TxtSearch_TextChanged;
            cbSort.SelectedIndexChanged += CbSort_SelectedIndexChanged;
            cbDiscount.SelectedIndexChanged += CbDiscount_SelectedIndexChanged;

    
            txtSearch.Enter += TxtSearch_Enter;
            txtSearch.Leave += TxtSearch_Leave;
        }

        private void ProductForm_Load(object sender, EventArgs e)
        {
            // Инициализация ComboBox
            cbSort.Items.Clear();
            cbSort.Items.Add("По умолчанию");
            cbSort.Items.Add("По возрастанию цены");
            cbSort.Items.Add("По убыванию цены");
            cbSort.SelectedIndex = 0;

            cbDiscount.Items.Clear();
            cbDiscount.Items.Add("Все диапазоны");
            cbDiscount.Items.Add("0-9,99%");
            cbDiscount.Items.Add("10-14,99%");
            cbDiscount.Items.Add("15% и более");
            cbDiscount.SelectedIndex = 0;

            txtSearch.Text = "Поиск по наименованию...";
            txtSearch.ForeColor = Color.Gray;

            currentPage = 1;
            LoadData(currentPage);
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            if (txtSearch.ForeColor == Color.Gray) return; 
            currentPage = 1; 
            LoadData(currentPage);
        }

        private void CbSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentPage = 1;
            LoadData(currentPage);
        }

        private void CbDiscount_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentPage = 1;
            LoadData(currentPage);
        }

        private void TxtSearch_Enter(object sender, EventArgs e)
        {
            if (txtSearch.Text == "Поиск по наименованию...")
            {
                txtSearch.Text = "";
                txtSearch.ForeColor = Color.Black;
            }
        }

        private void TxtSearch_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                txtSearch.Text = "Поиск по наименованию...";
                txtSearch.ForeColor = Color.Gray;
            }
        }

        private void LoadData(int page)
        {
            string connect = $"host={Properties.Settings.Default.host};" +
                             $"uid={Properties.Settings.Default.uid};" +
                             $"pwd={Properties.Settings.Default.pwd};" +
                             $"database=trade;";

            try
            {
                using (MySqlConnection con = new MySqlConnection(connect))
                {
                    con.Open();

                    //Получаем ОБЩЕЕ количество товаров в базе (для "Y" в "X из Y")
                    using (MySqlCommand cmdAll = new MySqlCommand("SELECT COUNT(*) FROM product", con))
                    {
                        totalAllRecords = Convert.ToInt32(cmdAll.ExecuteScalar());
                    }

                    //Динамическое построение условий WHERE
                    List<string> whereClauses = new List<string>();
                    List<MySqlParameter> parameters = new List<MySqlParameter>();

                    //Поиск по наименованию
                    if (!string.IsNullOrWhiteSpace(txtSearch.Text) && txtSearch.Text != "Поиск по наименованию...")
                    {
                        whereClauses.Add("p.ProductName LIKE @search");
                        parameters.Add(new MySqlParameter("@search", "%" + txtSearch.Text + "%"));
                    }

                    //Фильтрация по скидке
                    string discountFilter = cbDiscount.SelectedItem?.ToString() ?? "Все диапазоны";
                    if (discountFilter == "0-9,99%")
                    {
                        whereClauses.Add("p.ProductDiscountAmountMax >= 0 AND p.ProductDiscountAmountMax < 10");
                    }
                    else if (discountFilter == "10-14,99%")
                    {
                        whereClauses.Add("p.ProductDiscountAmountMax >= 10 AND p.ProductDiscountAmountMax < 15");
                    }
                    else if (discountFilter == "15% и более")
                    {
                        whereClauses.Add("p.ProductDiscountAmountMax >= 15");
                    }

                    string whereSql = whereClauses.Count > 0 ? "WHERE " + string.Join(" AND ", whereClauses) : "";

                    string countQuery = $"SELECT COUNT(*) FROM product p {whereSql}";
                    using (MySqlCommand cmdCount = new MySqlCommand(countQuery, con))
                    {
                        cmdCount.Parameters.AddRange(parameters.ToArray());
                        totalFilteredRecords = Convert.ToInt32(cmdCount.ExecuteScalar());
                    }

                    // Динамическая сортировка
                    string orderBy = "ORDER BY p.ProductName ASC"; // По умолчанию
                    string sortOption = cbSort.SelectedItem?.ToString() ?? "По умолчанию";
                    if (sortOption == "По возрастанию цены")
                        orderBy = "ORDER BY p.ProductCost ASC";
                    else if (sortOption == "По убыванию цены")
                        orderBy = "ORDER BY p.ProductCost DESC";


                    int offset = (page - 1) * PAGE_SIZE;
                    string query = $@"
                        SELECT 
                            p.ProductArticleNumber,
                            p.ProductName, 
                            p.ProductCost, 
                            p.ProductManufacturer, 
                            p.ProductDescription, 
                            p.ProductDiscountAmountMax,
                            p.ProductPhoto
                        FROM product p
                        {whereSql}
                        {orderBy}
                        LIMIT @limit OFFSET @offset;";

                    parameters.Add(new MySqlParameter("@limit", PAGE_SIZE));
                    parameters.Add(new MySqlParameter("@offset", offset));

                    // Настройка DataGridView (только при первой загрузке)
                    if (dataGridView1.Columns.Count == 0)
                    {
                        SetupDataGridView();
                    }
                    else
                    {
                        dataGridView1.Rows.Clear();
                    }

                    // Заглушка для фото
                    Image defaultImage = null;
                    string defaultImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Image", "picture.png");
                    if (File.Exists(defaultImagePath))
                    {
                        defaultImage = Image.FromFile(defaultImagePath);
                    }

                    int rowsAdded = 0;

                    //Чтение и отображение данных
                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddRange(parameters.ToArray());
                        using (MySqlDataReader read = cmd.ExecuteReader())
                        {
                            while (read.Read())
                            {
                                string id = Convert.ToString(read[0]) ?? "";
                                string title = Convert.ToString(read[1]) ?? "Без названия";
                                string costStr = Convert.ToString(read[2]) ?? "0";
                                string manufacturer = Convert.ToString(read[3]) ?? "Не указан";
                                string description = Convert.ToString(read[4]) ?? "Нет описания";
                                string discountStr = Convert.ToString(read[5]) ?? "0";

                                double cost = 0;
                                double discount = 0;
                                double.TryParse(costStr, out cost);
                                double.TryParse(discountStr, out discount);

                                // Формирование цены с зачеркиванием
                                string priceDisplay = $"{cost} руб.";
                                if (discount > 0)
                                {
                                    double finalPrice = cost - (cost * (discount / 100.0));
                                    string finalPriceStr = finalPrice.ToString("0.##");
                                    priceDisplay = $"{AddStrikethrough(costStr)} {finalPriceStr} руб.";
                                }

                                string infoText = $"Наименование: {title}\n" +
                                                  $"Производитель: {manufacturer}\n" +
                                                  $"Описание: {description}\n" +
                                                  $"Цена: {priceDisplay}";

                                // Загрузка картинки
                                Image img = defaultImage;
                                string dbValue = Convert.ToString(read[6]);
                                if (!string.IsNullOrWhiteSpace(dbValue))
                                {
                                    string cleanFileName = Path.GetFileName(dbValue);
                                    string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Image", cleanFileName);
                                    if (File.Exists(fullPath))
                                    {
                                        img = Image.FromFile(fullPath);
                                    }
                                }

                                string discountText = (discount == 0) ? "Нет" : $"{discount}%";

                                int rowIndex = dataGridView1.Rows.Add(id, img, infoText, discountText, "Подробнее");
                                dataGridView1.Rows[rowIndex].Height = 120;
                                rowsAdded++;

                                if (discount > 15)
                                {
                                    dataGridView1.Rows[rowIndex].DefaultCellStyle.BackColor = Color.FromArgb(127, 255, 0);
                                }
                            }
                        }
                    }

                    lblStatus.Text = $"Выведено: {rowsAdded} из {totalFilteredRecords} (Всего в базе: {totalAllRecords})";

                    Pagination();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string AddStrikethrough(string text)
        {
            return string.Join("\u0336", text.ToCharArray()) + "\u0336";
        }

        void SetupDataGridView()
        {
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();

            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridView1.DefaultCellStyle.Padding = new Padding(5);

            // Колонка 0: ID (скрытая)
            DataGridViewTextBoxColumn idcolumn = new DataGridViewTextBoxColumn { Name = "ID", Visible = false };
            dataGridView1.Columns.Add(idcolumn);

            // Колонка 1: Фото
            DataGridViewImageColumn photo = new DataGridViewImageColumn
            {
                HeaderText = "Фото",
                Name = "Photo",
                Width = 120,
                ImageLayout = DataGridViewImageCellLayout.Zoom
            };
            dataGridView1.Columns.Add(photo);

            // Колонка 2: Информация
            DataGridViewTextBoxColumn info = new DataGridViewTextBoxColumn
            {
                HeaderText = "Информация о товаре",
                Name = "info",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                MinimumWidth = 150,
                DefaultCellStyle = { WrapMode = DataGridViewTriState.True, Font = new Font("Comic Sans MS", 11F, FontStyle.Regular), Padding = new Padding(5) }
            };
            dataGridView1.Columns.Add(info);

            // Колонка 3: Скидка
            DataGridViewTextBoxColumn disc = new DataGridViewTextBoxColumn
            {
                HeaderText = "Скидка",
                Name = "Discount",
                Width = 90,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter, Font = new Font("Comic Sans MS", 12F, FontStyle.Bold), ForeColor = Color.SeaGreen }
            };
            dataGridView1.Columns.Add(disc);

        }

        void Pagination()
        {
            for (int j = this.Controls.Count - 1; j >= 0; j--)
            {
                if (this.Controls[j].Name != null && this.Controls[j].Name.StartsWith("page"))
                {
                    this.Controls.RemoveAt(j);
                }
            }

            int totalPagesCalc = (int)Math.Ceiling((double)totalFilteredRecords / PAGE_SIZE);
            if (totalPagesCalc == 0) totalPagesCalc = 1;

            if (totalPagesCalc <= 1)
            {
                linkLabel1.Visible = false;
                return;
            }

            linkLabel1.Visible = true;

            int startX = linkLabel1.Location.X;
            int startY = linkLabel1.Location.Y;
            int step = 40;


            for (int i = 0; i < totalPagesCalc; ++i)
            {
                LinkLabel ll = new LinkLabel();
                ll.Text = (i + 1).ToString();
                ll.Name = "page" + i;
                ll.AutoSize = true;

                ll.Location = new Point(startX + (i * step), startY);

                ll.LinkColor = Color.FromArgb(102, 255, 102);
                ll.ActiveLinkColor = Color.FromArgb(80, 200, 80);
                ll.VisitedLinkColor = Color.FromArgb(102, 255, 102);
                ll.Font = new Font("Comic Sans MS", 13.8f, FontStyle.Bold);

                ll.Click += LinkLabel_Click;

                if (i + 1 == currentPage)
                    ll.LinkBehavior = LinkBehavior.NeverUnderline;
                else
                    ll.LinkBehavior = LinkBehavior.AlwaysUnderline;

                this.Controls.Add(ll);
                ll.BringToFront();
            }
        }
        private void LinkLabel_Click(object sender, EventArgs e)
        {
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is LinkLabel ll && ll.Name.StartsWith("page"))
                {
                    ll.LinkBehavior = LinkBehavior.AlwaysUnderline;
                }
            }

            LinkLabel l = sender as LinkLabel;
            l.LinkBehavior = LinkBehavior.NeverUnderline;

            currentPage = Convert.ToInt32(l.Text);
            LoadData(currentPage);
        }


        private void button1_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            this.Hide();
            new AddEdit().ShowDialog();
            this.Show();
        }
    }
}