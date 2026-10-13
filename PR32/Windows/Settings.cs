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


namespace PR32
{
    public partial class Settings : System.Windows.Forms.Form
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            localhost_t.Text = Properties.Settings.Default.host;
            user_t.Text = Properties.Settings.Default.uid;
            passwd_t.Text = Properties.Settings.Default.pwd;
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default["host"]= localhost_t.Text;
            Properties.Settings.Default["uid"] = user_t.Text;
            Properties.Settings.Default["pwd"] = passwd_t.Text;

            Properties.Settings.Default.Save();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string connect = $"host={localhost_t.Text}; uid={user_t.Text};pwd={passwd_t.Text};database=trade;";

            MySqlConnection con = new MySqlConnection(connect);

            try
            {
                con.Open();

                MessageBox.Show($"Успешное подключение к БД", "Ок", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Properties.Settings.Default.host = localhost_t.Text;
                Properties.Settings.Default.uid = user_t.Text;
                Properties.Settings.Default.pwd = passwd_t.Text;
                Properties.Settings.Default.Save();

                this.DialogResult = DialogResult.OK;
                this.Close();
                con.Close();

            } catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения к {connect}\n{ex.Message}", "ОШИБКА", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
