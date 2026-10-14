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
        string art;
        string name ;
        int unit ;
        int cost;
        int maxAmout;
        string manufacturer;
        int supplier;
        int category;
        int amout;
        int quantityInStock;
        string description;
        string image;

        public AddEdit()
        {
            InitializeComponent();
           
            textBox1.TextChanged += ValidateFields;
            textBox2.TextChanged += ValidateFields;
            textBox3.TextChanged += ValidateFields;
            textBox4.TextChanged += ValidateFields;
            textBox5.TextChanged += ValidateFields;
            textBox6.TextChanged += ValidateFields;

            cmdCat.SelectedIndexChanged += ValidateFields;
            cmdSup.SelectedIndexChanged += ValidateFields;
            cmdUnit.SelectedIndexChanged += ValidateFields;

            btnAdd.Enabled = false;
            btnEdit.Enabled = false;
        }

        private void ValidateFields(object sender, EventArgs e)
        {
            bool isTextBoxesValid = !string.IsNullOrWhiteSpace(textBox1.Text) &&
                                    !string.IsNullOrWhiteSpace(textBox2.Text) &&
                                    !string.IsNullOrWhiteSpace(textBox3.Text) &&
                                    !string.IsNullOrWhiteSpace(textBox4.Text) &&
                                    !string.IsNullOrWhiteSpace(textBox5.Text) &&
                                    !string.IsNullOrWhiteSpace(textBox6.Text); 
            bool isComboBoxesValid = cmdCat.SelectedIndex >= 0 &&
                                     cmdSup.SelectedIndex >= 0 &&
                                     cmdUnit.SelectedIndex >= 0;
            bool isFormValid = isTextBoxesValid && isComboBoxesValid;

            btnAdd.Enabled = isFormValid;
            btnEdit.Enabled = isFormValid;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            ClassConnect connect = new ClassConnect();
            using (MySqlConnection con = new MySqlConnection(connect.connect))
            {
                con.Open();
                string art = textBox8.Text;
                string name = textBox1.Text;
                int unit = cmdUnit.SelectedIndex;
                int cost = Convert.ToInt32(textBox2.Text);
                int maxAmout = Convert.ToInt32(textBox3.Text);
                string manufacturer = textBox4.Text;
                int supplier = cmdCat.SelectedIndex;
                int category = cmdSup.SelectedIndex;
                int amout = Convert.ToInt32(textBox5.Text);
                int quantityInStock = Convert.ToInt32(textBox6.Text);
                string description = textBox7.Text;
                string image = null;


                string cmd = $@"Insert Into product (ProductArticleNumber, ProductName, UnitID,
                                ProductCost, ProductDiscountAmountMax, ProductManufacturer, SupplierID, CategoryID,
                                ProductDiscountAmount, ProductQuantityInStock, ProductDescription, ProductPhoto)
                Values ('{art}', '{name}', {unit}, {cost}, {maxAmout}, 
                        '{manufacturer}', {supplier}, {category}, {amout},
                        {quantityInStock}, '{description}', NULL);";


                MySqlCommand command = new MySqlCommand(cmd, con);

                

                MySqlDataReader reader = command.ExecuteReader();
                MessageBox.Show("Сообщение", "Товар добавлен!", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        void LoadComboBox1()
        {
            ClassConnect connect = new ClassConnect();
            using (MySqlConnection con = new MySqlConnection(connect.connect))
            {
                con.Open();
                string cmd = @"Select * FROM unit;";
                MySqlDataAdapter ad = new MySqlDataAdapter(cmd, con);
                DataTable dt = new DataTable();
                ad.Fill(dt);
                cmdUnit.DataSource = dt;
               
                cmdUnit.DisplayMember = "UnitName";
                cmdUnit.ValueMember = "UnitID";
                cmdUnit.SelectedIndex = -1;

                string commandSup = @"Select * FROM supplier;";
                MySqlDataAdapter ad1 = new MySqlDataAdapter(commandSup, con);
                DataTable dt1 = new DataTable();
                ad1.Fill(dt1);
                cmdSup.DataSource = dt1;
                cmdSup.DisplayMember = "SupplierName";
                cmdSup.ValueMember = "SupplierID";
                cmdSup.SelectedIndex = -1;


                string commandCat = @"Select * FROM category;";
                MySqlDataAdapter ad3 = new MySqlDataAdapter(commandCat, con);
                DataTable dt3 = new DataTable();
                ad3.Fill(dt3);
                cmdCat.DataSource = dt3;
                cmdCat.DisplayMember = "CategoryName";
                cmdCat.ValueMember = "CategoryID";
                cmdCat.SelectedIndex = -1;
            }
        }
        private void AddEdit_Load(object sender, EventArgs e)
        {
            ClassConnect connect = new ClassConnect();
            using (MySqlConnection con = new MySqlConnection(connect.connect))
            {
                con.Open();
                string cmd = @"Select * FROM user;";
                MySqlDataAdapter ad = new MySqlDataAdapter();
                DataTable dt = new DataTable();

            }
            

            LoadComboBox1();
        }

    }
}
