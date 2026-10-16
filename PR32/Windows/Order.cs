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

namespace PR32.Windows
{
    public partial class Order : Form
    {
        private string articleNumber;
        public Order(string articleNumber)
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        void fillDGRV()
        {
            ClassConnect connect = new ClassConnect();
            using (MySqlConnection con = new MySqlConnection(connect.connect))
            {
                con.Open();

            }
        }
    }
}
