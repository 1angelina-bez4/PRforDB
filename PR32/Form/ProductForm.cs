using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.IO;

namespace PR32
{
    public partial class ProductForm : Form
    {
        private const int PAGE_SIZE = 15;
        private int currentPage = 1;
        private int totalPages = 0;
        
        public ProductForm()
        {
            InitializeComponent();

        }

        private void userForm_Load(object sender, EventArgs e)
        {
            currentPage = 1;
            LoadData(currentPage);

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

                    int totalRecords = 0;
                    string countQuery = "SELECT COUNT(*) FROM product;";

                    using (MySqlCommand cmd = new MySqlCommand(countQuery, con))
                    {
                        totalRecords = Convert.ToInt32(cmd.ExecuteScalar());
                        totalPages = (int)Math.Ceiling((double)totalRecords / PAGE_SIZE);
                        if (totalPages == 0) totalPages = 1;
                    }
                    int offset = (page - 1) * PAGE_SIZE;

                    string query = $@"
                        SELECT 
                            p.ProductArticleNumber,
                            p.ProductName, 
                            p.ProductCost, 
                            p.ProductManufacturer, 
                            p.ProductDescription, 
                            p.ProductDiscountAmount,
                            p.ProductPhoto
                        FROM product p
                        ORDER BY p.ProductName
                        LIMIT {PAGE_SIZE} OFFSET {offset};";

             
                    //настрока только при  1 загрузке
                    if (dataGridView1.Columns.Count == 0)
                    {
                        SetupDataGridView();
                    }
                    else
                    {
                        dataGridView1.Rows.Clear(); //очистка только строк перед переключением
                    }
                    Image defaultImage = null;
                    string defaultImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Image", "picture.png");
                    if (File.Exists(defaultImagePath))
                    {
                        defaultImage = Image.FromFile(defaultImagePath);
                    }
                    //чтение таблицы
                    using (MySqlCommand cmd = new MySqlCommand(query, con))
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

                            //парсим числа для вычислений
                            double cost = 0;
                            double discount = 0;
                            double.TryParse(costStr, out cost);
                            double.TryParse(discountStr, out discount);

                            // формирование отображения цены (с зачеркиванием, если есть скидка)
                            string priceDisplay = $"{cost} руб.";
                            if (discount > 0)
                            {
                                double finalPrice = cost - (cost * (discount / 100.0));
                                //удаление лишние нули после запятой
                                string finalPriceStr = finalPrice.ToString("0.##");
                                priceDisplay = $"{AddStrikethrough(costStr)} {finalPriceStr} руб.";
                            }

                            // вывод информации как по макету
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

                            // Формирование текста скидки
                            string discountText = (discount == 0) ? "Нет" : $"{discount}%";

                            // Добавление строки
                            int rowIndex = dataGridView1.Rows.Add(id, img, infoText, discountText, "Подробнее");
                            dataGridView1.Rows[rowIndex].Height = 120;

                            //подстветка если скидка больше 15%
                            if (discount > 15)
                            {
                                dataGridView1.Rows[rowIndex].DefaultCellStyle.BackColor = Color.FromArgb(127, 255, 0);
                            }
                        }
                    }

                    //вызыв пагинации
                    Pagination();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

       // Вспомогательный метод для зачеркивания текста с помощью Unicode-символа
        private string AddStrikethrough(string text)
        {
            // Символ \u0336 - это "COMBINING LONG STROKE OVERLAY", который рисует линию поверх предыдущего символа
            return string.Join("\u0336", text.ToCharArray()) + "\u0336";
        }


        void SetupDataGridView()
        {
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();

            //настройка таблицы

            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
            dataGridView1.DefaultCellStyle.Padding = new Padding(5);

            //колонка 0: (id)
            DataGridViewTextBoxColumn idcolumn = new DataGridViewTextBoxColumn();
            idcolumn.Name = "ID";
            idcolumn.Visible = false;
            dataGridView1.Columns.Add(idcolumn);

            //колонка 1: (photo)
            DataGridViewImageColumn photo = new DataGridViewImageColumn();
            photo.HeaderText = "Фото";
            photo.Name = "Photo";
            photo.Width = 150;
            photo.ImageLayout = DataGridViewImageCellLayout.Zoom;
            dataGridView1.Columns.Add(photo);

            //колонка 2:(Name product)
            DataGridViewTextBoxColumn info = new DataGridViewTextBoxColumn();
            info.HeaderText = "Информация о товаре";
            info.Name = "info";
            info.Width  = 150;
            info.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            info.DefaultCellStyle.Font = new Font("Comic Sans MS", 14F, FontStyle.Regular);
            info.DefaultCellStyle.Padding = new Padding(5);
            dataGridView1.Columns.Add(info);

            //колонка 3: (Discount)
            DataGridViewTextBoxColumn disc = new DataGridViewTextBoxColumn();
            disc.HeaderText = "Скидка";
            disc.Name = "Discount";
            disc.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            disc.DefaultCellStyle.Font = new Font("Comic Sans MS", 14F, FontStyle.Bold);
            disc.DefaultCellStyle.ForeColor = Color.SeaGreen;
            dataGridView1.Columns.Add(disc);

            
        }
        void Pagination()
        {
            for (int j = 0, count = this.Controls.Count; j < count; ++j)
                this.Controls.RemoveByKey("page" + j);

            if (totalPages <= 1) return;

            int x = 0;
            int y = this.ClientSize.Height - 60; // Рисуем на 60 пикселей выше нижнего края формы
            int step = 40;

            for (int i = 0; i < totalPages; ++i)
            {
                LinkLabel ll = new LinkLabel();
                ll.Text = (i + 1).ToString();
                ll.Name = "page" + i;
                ll.AutoSize = true;
                ll.Location = new Point(x, y);

                // Цвет и шрифт
                ll.LinkColor = Color.FromArgb(102, 255, 102);
                ll.ActiveLinkColor = Color.FromArgb(80, 200, 80);
                ll.VisitedLinkColor = Color.FromArgb(102, 255, 102);
                ll.Font = new Font("Comic Sans MS", 13.8f, FontStyle.Bold);

                ll.Click += LinkLabel_Click;

                if (i + 1 == currentPage)
                    ll.LinkBehavior = LinkBehavior.NeverUnderline;

                this.Controls.Add(ll);
                ll.BringToFront();

                x += step;
            }
        }

        private void LinkLabel_Click(object sender, EventArgs e)
        {
            foreach (var ctrl in this.Controls)
                if (ctrl is LinkLabel)
                    (ctrl as LinkLabel).LinkBehavior = LinkBehavior.AlwaysUnderline;

            LinkLabel l = sender as LinkLabel;
            l.LinkBehavior = LinkBehavior.NeverUnderline;

            currentPage = Convert.ToInt32(l.Text);
            LoadData(currentPage);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}