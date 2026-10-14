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
    public partial class AddEdit : System.Windows.Forms.Form
    {

        public AddEdit()
        {
            InitializeComponent();

            btnAdd.Enabled = false;
            btnEdit.Enabled = false;
            LoadData();

        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void LoadData()
        {
            ClassConnect connect = new ClassConnect();
            using (MySqlConnection con = new MySqlConnection(connect.connect))
            {
                con.Open();
                string cmd = @"Select * FROM user;";
                MySqlDataAdapter ad = new MySqlDataAdapter();
                DataTable dt = new DataTable();

            }
        }

        private void Add()
        {
            ClassConnect connect = new ClassConnect();
            using (MySqlConnection con = new MySqlConnection(connect.connect))
            {
                con.Open();
                string art = textBox7.Text;
                string name = textBox1.Text;
                int unit = comboBox1.SelectedIndex;
                int cost = Convert.ToInt32(textBox2.Text);
                int maxAmout = Convert.ToInt32(textBox3.Text);
                string manufacturer = textBox4.Text;
                int supplier = comboBox2.SelectedIndex;
                int category = comboBox3.SelectedIndex;
                int amout = Convert.ToInt32(textBox4.Text);
                int quantityInStock = Convert.ToInt32(textBox5.Text);
                string description = textBox6.Text;
                string image = null;

                string cmd = $@"Insert Into (ProductArticleNumber, ProductName, UnitID,
                                                ProductCost, ProductDiscountAmountMax, ProductManufacturer,SupplierID, CategoryID,
                                                ProductDiscountAmount, ProductQuantityInStock,ProductDescription,ProductPhoto)
                                values ('{art}','{name}', '{unit}', '{cost}', '{maxAmout}', 
                                        '{manufacturer}', '{supplier}', '{category}', '{amout}',
                                           '{quantityInStock}', '{description}', '{image}'";

                MySqlCommand command = new MySqlCommand(cmd, con);

                if (string.IsNullOrEmpty(art))
                {

                }

            }
        }
    }
}
