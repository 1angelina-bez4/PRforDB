using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace PR32
{
    public partial class ProductForm : System.Windows.Forms.Form
    {
        private const int PAGE_SIZE = 15;
        private int currentPage = 1;
        private int totalPages = 0;
        int secInd = 0;

        public ProductForm()
        {
            InitializeComponent();
        }

        private void userForm_Load(object sender, EventArgs e)
        {
            currentPage = 1;
            LoadData(currentPage);
            Pagination();
            this.KeyPreview = true;
            timer1.Interval = 1000;
            timer1.Tick += timer1_Tick;
            timer1.Start();

            this.KeyDown += ResetTimer;
            this.MouseClick += ResetTimer;

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
                    }

                    totalPages = (int)Math.Ceiling((double)totalRecords / PAGE_SIZE);
                    if (totalPages == 0) totalPages = 1;

                    int offset = (page - 1) * PAGE_SIZE;

                    string query = $@"
                        SELECT 
                            p.ProductArticleNumber AS 'Артикул',
                            p.ProductName AS 'Наименование',
                            p.ProductCost AS 'Цена',
                            p.ProductManufacturer AS 'Производитель',
                            p.ProductQuantityInStock AS 'Количество',
                            p.ProductDiscountAmount AS 'Скидка %',
                            u.UnitName AS 'Ед. изм.',
                            s.SupplierName AS 'Поставщик',
                            c.CategoryName AS 'Категория'
                        FROM product p
                        JOIN unit u ON p.UnitID = u.UnitID
                        JOIN supplier s ON p.SupplierID = s.SupplierID
                        JOIN category c ON p.CategoryID = c.CategoryID
                        ORDER BY p.ProductName
                        LIMIT {PAGE_SIZE} OFFSET {offset};";

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(query, con))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dataGridView1.DataSource = dt;
                    }
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки: " + ex.Message,
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
         

        private void timer1_Tick(object sender, EventArgs e)
        {
            secInd++;
            if (secInd >= 5)
            {
                timer1.Stop();
                this.Close();
                MessageBox.Show("Блокировка экрана, подождите", "Сообщение",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                new Auth().ShowDialog();
                
               
            }
        }

        private void ResetTimer(object sender, EventArgs e)
        {
            secInd = 0;
        }
        void Pagination()
        {
            for (int j = 0, count = this.Controls.Count; j < count; ++j)
                this.Controls.RemoveByKey("page" + j);

            if (totalPages <= 1) return;

            int x = 10;
            int y = dataGridView1.Bottom + 10;
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