namespace PR32.Windows
{
    partial class Order
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Order));
            this.btnExit = new System.Windows.Forms.Button();
            this.btnAddToCart = new System.Windows.Forms.Button();
            this.lblPickup = new System.Windows.Forms.Label();
            this.cmbPickupPoints = new System.Windows.Forms.ComboBox();
            this.lblProduct = new System.Windows.Forms.Label();
            this.cmbProducts = new System.Windows.Forms.ComboBox();
            this.lblQuantity = new System.Windows.Forms.Label();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.dgvCart = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCart)).BeginInit();
            this.SuspendLayout();
            // 
            // btnExit
            // 
            this.btnExit.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnExit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(255)))), ((int)(((byte)(204)))));
            this.btnExit.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnExit.Font = new System.Drawing.Font("Comic Sans MS", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnExit.ForeColor = System.Drawing.Color.SeaGreen;
            this.btnExit.Location = new System.Drawing.Point(201, 649);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(303, 60);
            this.btnExit.TabIndex = 38;
            this.btnExit.Text = "Выйти";
            this.btnExit.UseVisualStyleBackColor = false;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnAddToCart
            // 
            this.btnAddToCart.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnAddToCart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(255)))), ((int)(((byte)(204)))));
            this.btnAddToCart.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAddToCart.Font = new System.Drawing.Font("Comic Sans MS", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnAddToCart.ForeColor = System.Drawing.Color.SeaGreen;
            this.btnAddToCart.Location = new System.Drawing.Point(558, 649);
            this.btnAddToCart.Name = "btnAddToCart";
            this.btnAddToCart.Size = new System.Drawing.Size(303, 60);
            this.btnAddToCart.TabIndex = 39;
            this.btnAddToCart.Text = "Оформить";
            this.btnAddToCart.UseVisualStyleBackColor = false;
            // 
            // lblPickup
            // 
            this.lblPickup.AutoSize = true;
            this.lblPickup.Location = new System.Drawing.Point(21, 28);
            this.lblPickup.Name = "lblPickup";
            this.lblPickup.Size = new System.Drawing.Size(232, 42);
            this.lblPickup.TabIndex = 40;
            this.lblPickup.Text = "Пункт выдачи:";
            // 
            // cmbPickupPoints
            // 
            this.cmbPickupPoints.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPickupPoints.FormattingEnabled = true;
            this.cmbPickupPoints.Location = new System.Drawing.Point(259, 25);
            this.cmbPickupPoints.Name = "cmbPickupPoints";
            this.cmbPickupPoints.Size = new System.Drawing.Size(721, 50);
            this.cmbPickupPoints.TabIndex = 41;
            // 
            // lblProduct
            // 
            this.lblProduct.AutoSize = true;
            this.lblProduct.Location = new System.Drawing.Point(194, 98);
            this.lblProduct.Name = "lblProduct";
            this.lblProduct.Size = new System.Drawing.Size(111, 42);
            this.lblProduct.TabIndex = 42;
            this.lblProduct.Text = "Товар:";
            // 
            // cmbProducts
            // 
            this.cmbProducts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProducts.FormattingEnabled = true;
            this.cmbProducts.Location = new System.Drawing.Point(314, 95);
            this.cmbProducts.Name = "cmbProducts";
            this.cmbProducts.Size = new System.Drawing.Size(455, 50);
            this.cmbProducts.TabIndex = 43;
            // 
            // lblQuantity
            // 
            this.lblQuantity.AutoSize = true;
            this.lblQuantity.Location = new System.Drawing.Point(252, 157);
            this.lblQuantity.Name = "lblQuantity";
            this.lblQuantity.Size = new System.Drawing.Size(196, 42);
            this.lblQuantity.TabIndex = 44;
            this.lblQuantity.Text = "Количество:";
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(490, 155);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.numericUpDown1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(146, 49);
            this.numericUpDown1.TabIndex = 45;
            this.numericUpDown1.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // dgvCart
            // 
            this.dgvCart.AllowUserToAddRows = false;
            this.dgvCart.AllowUserToDeleteRows = false;
            this.dgvCart.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(255)))), ((int)(((byte)(102)))));
            this.dgvCart.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCart.Location = new System.Drawing.Point(28, 226);
            this.dgvCart.Name = "dgvCart";
            this.dgvCart.ReadOnly = true;
            this.dgvCart.RowHeadersWidth = 51;
            this.dgvCart.RowTemplate.Height = 24;
            this.dgvCart.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCart.Size = new System.Drawing.Size(952, 272);
            this.dgvCart.TabIndex = 46;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(30, 511);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(127, 42);
            this.label1.TabIndex = 47;
            this.label1.Text = "Скидка:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(551, 501);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(184, 42);
            this.label2.TabIndex = 48;
            this.label2.Text = "Без скидки:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(289, 587);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(113, 42);
            this.label3.TabIndex = 49;
            this.label3.Text = "Итого:";
            // 
            // Order
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(18F, 42F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1006, 721);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dgvCart);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.lblQuantity);
            this.Controls.Add(this.cmbProducts);
            this.Controls.Add(this.lblProduct);
            this.Controls.Add(this.cmbPickupPoints);
            this.Controls.Add(this.lblPickup);
            this.Controls.Add(this.btnAddToCart);
            this.Controls.Add(this.btnExit);
            this.Font = new System.Drawing.Font("Comic Sans MS", 18F);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(255)))), ((int)(((byte)(102)))));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(7, 8, 7, 8);
            this.Name = "Order";
            this.Text = "Оформление заказа";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnAddToCart;
        private System.Windows.Forms.Label lblPickup;
        private System.Windows.Forms.ComboBox cmbPickupPoints;
        private System.Windows.Forms.Label lblProduct;
        private System.Windows.Forms.ComboBox cmbProducts;
        private System.Windows.Forms.Label lblQuantity;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.DataGridView dgvCart;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}