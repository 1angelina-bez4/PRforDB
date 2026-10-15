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
        public Auth()
        {
            InitializeComponent();
        }

        private void Auth_Load(object sender, EventArgs e)
        {
            //добавленный класс для работы  с получением настроек.
            Configuration currentConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            //получение данных из настроек
            string host = Properties.Settings.Default.host;
            string uid = Properties.Settings.Default.uid;
            string pwd = Properties.Settings.Default.pwd;
     
            string connect_Set = $"host={host};uid={uid};pwd={pwd};database=db_avto;";


            try
            {
                string login = login_t.Text;
                string password = passwd_t.Text;

                if (string.IsNullOrEmpty(login))
                {
                    MessageBox.Show($"Введите логин.", "ОШИБКА", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                if (string.IsNullOrEmpty(password))
                {
                    MessageBox.Show($"Введите логин.", "ОШИБКА", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                using (MySqlConnection connection = new MySqlConnection(connect_Set))
                {
                    connection.Open();

                    string select = $@"Select * From user WHERE UserLogin = {login}, UserPassword = {password};";

                    MySqlCommand cmd = new MySqlCommand(select, connection);

                   
                }

            } catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения к {connect_Set}\n", "ОШИБКА", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Hide();
                Settings settingsForm = new Settings();
                settingsForm.ShowDialog();
                this.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string login = login_t.Text.Trim();
            string password = passwd_t.Text.Trim();

            if (string.IsNullOrEmpty(login) || login == "Логин")
            {
                MessageBox.Show("Введите логин.", "ОШИБКА",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите пароль.", "ОШИБКА",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string host = Properties.Settings.Default.host;
            string uid = Properties.Settings.Default.uid;
            string pwd = Properties.Settings.Default.pwd;

            string connect = $"host={host};uid={uid};pwd={pwd};database=db_avto;";
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connect))
                {
                    connection.Open();


                    string query = $@"SELECT UserID, UserRole 
                                     FROM user 
                                     WHERE UserLogin = '{login}' AND UserPassword = '{password}';";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int userId = reader.GetInt32("UserID");
                                int roleId = reader.GetInt32("UserRole");

                                MessageBox.Show($"Вход выполнен! Роль: {roleId}",
                                    "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                this.Hide();
                                Import import = new Import();
                                import.ShowDialog();
                            }
                            else
                            {
                                MessageBox.Show("Неверный логин или пароль", "Ошибка входа",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                                passwd_t.Clear();
                                passwd_t.Focus();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения:\n{ex.Message}",
                    "ОШИБКА", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Открыть форму настроек
                this.Hide();
                new Settings().ShowDialog();
                this.Close();
            }
        }

    }
}
