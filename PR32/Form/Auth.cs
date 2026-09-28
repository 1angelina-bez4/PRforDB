using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Configuration;


namespace PR32
{
    public partial class Auth : Form
    {
        int count;
        string tcaptcha;

        public Auth()
        {
            InitializeComponent();
        }

        private void Auth_Load(object sender, EventArgs e)
        {
            pictureBox3.Visible = false;
            txtcaptcha.Visible = false;

            //добавленный класс для работы  с получением настроек.
            Configuration currentConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            //получение данных из настроек
            string host = Properties.Settings.Default.host;
            string uid = Properties.Settings.Default.uid;
            string pwd = Properties.Settings.Default.pwd;

            string connect_Set = $"host={host};uid={uid};pwd={pwd};database=trade;";

            if (count > 1)
            {
                this.CreateImage(pictureBox3.Width, pictureBox3.Height);
            }
            
        }
        private void ShowCaptcha()
        {
            pictureBox3.Image = CreateImage(pictureBox3.Width, pictureBox3.Height);
            pictureBox3.Visible = true;
            txtcaptcha.Visible = true;
            txtcaptcha.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string login = login_t.Text.Trim();
            string password = passwd_t.Text.Trim();

            if (string.IsNullOrEmpty(login) || login == "Логин" || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите логин и пароль", "ОШИБКА",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                count++;
                CheckAttempts();
                return;
            }

            if (pictureBox3.Visible)
            {
                string captchaInput = txtcaptcha.Text.Trim();

                if (tcaptcha != captchaInput)
                {
                    MessageBox.Show("Неверная капча. Попробуйте ещё раз.", "ОШИБКА",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);

                    count++;
                    CheckAttempts();
                    txtcaptcha.Clear();
                    ShowCaptcha();   // обновляем капчу
                    return;
                }
            }

            string host = Properties.Settings.Default.host;
            string uid = Properties.Settings.Default.uid;
            string pwd = Properties.Settings.Default.pwd;

            string connect = $"server={host};uid={uid};pwd={pwd};database=trade;";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connect))
                {
                    connection.Open();
                    string query = @"SELECT UserID, UserRole 
                             FROM user 
                             WHERE UserLogin = @login AND UserPassword = @password;";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@login", login);
                        cmd.Parameters.AddWithValue("@password", password);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                count = 0;

                                int userId = reader.GetInt32("UserID");
                                int roleId = reader.GetInt32("UserRole");

                                MessageBox.Show($"Вход выполнен! Роль: {roleId}",
                                    "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                this.Hide();
                                Import import = new Import();
                                import.ShowDialog();
                                this.Close(); // закрываем Auth
                            }
                            else
                            {
                                MessageBox.Show("Неверный логин или пароль", "Ошибка входа",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                                count++;
                                CheckAttempts();

                                passwd_t.Clear();
                                passwd_t.Focus();

                                if (count >= 2)
                                {
                                    ShowCaptcha();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения:\n{ex.Message}",
                    "ОШИБКА", MessageBoxButtons.OK, MessageBoxIcon.Error);

                this.Hide();

                Settings settings = new Settings();
                if (settings.ShowDialog() == DialogResult.OK)
                {
                    this.Show();
                }
                else
                {
                    this.Close();
                }
            }
        }

        private void CheckAttempts()
        {
            if (count >= 3)
            {
                MessageBox.Show("Превышено количество попыток. Программа будет закрыта.",
                    "Блокировка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Application.Exit();
            }
        }


        private Bitmap CreateImage(int Width, int Height)
        {
            Random random = new Random();
            Bitmap result = new Bitmap(Width, Height);

            int Xpos = Width / 4; 
            int Ypos = Height / 4;

            //Добавление различным цветов для текста 
            Brush[] color =
            {
                Brushes.Black,
                Brushes.Red,
                Brushes.RoyalBlue,
                Brushes.Green,
                Brushes.White
            };

            //Добавление различным линий для текста
            Pen[] colorLine =
            {
                Pens.Black,
                Pens.Red,
                Pens.RoyalBlue,
                Pens.Green,
                Pens.White
            };

            FontStyle[] fontStyles =
            {
                FontStyle.Regular,
                FontStyle.Bold,
                FontStyle.Italic
            };

            Int16[] rotate = { 1, -1, 2, -2, 3, -3, 4, -4, 5, -5, 6, -6 };

            Graphics g = Graphics.FromImage((Image)result);

            g.Clear(Color.Gray);

            g.RotateTransform(random.Next(rotate.Length));

            tcaptcha = String.Empty;
            string ALF = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

            for(int i = 0; i<4; i++)
            {
                tcaptcha += ALF[random.Next(ALF.Length)];
            }

            g.DrawString(tcaptcha,
                        new Font("Arial", 35, fontStyles[random.Next(fontStyles.Length)]),
                        color[random.Next(color.Length)],
                        new PointF(Xpos, Ypos));


            g.DrawString(tcaptcha,
                new Font("Arial", 35, fontStyles[random.Next(fontStyles.Length)]),
                color[random.Next(color.Length)],
                new PointF(Xpos, Ypos));

            //Добавим немного помех
            //Линии из углов
            g.DrawLine(colorLine[random.Next(colorLine.Length)],
                       new Point(0, 0),
                       new Point(Width - 1, Height - 1));

            g.DrawLine(colorLine[random.Next(colorLine.Length)],
                       new Point(0, Height - 1),
                       new Point(Width - 1, 0));


            //Белые точки
            for (int i = 0; i < Width; ++i)
                for (int j = 0; j < Height; ++j)
                    if (random.Next() % 20 == 0)
                        result.SetPixel(i, j, Color.White);

            return result;

        }
    }
}
