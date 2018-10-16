using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stock
{
    public partial class Stock : Form
    {
        public Stock()
        {
            InitializeComponent();
        }

        private void Stock_Load(object sender, EventArgs e)
        {
            
            ResetRecord();
            Search();
        }

        private void dtpDate_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                txtProductName.Focus(); 
            }
        }

        private void txtProductName_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                if(dgview.Rows.Count > 0)
                {
                    txtProductCode.Text = dgview.SelectedRows[0].Cells[0].Value.ToString();
                    txtProductName.Text = dgview.SelectedRows[0].Cells[1].Value.ToString();
                    this.dgview.Visible = false;
                    txtQuantity.Focus();
                }
                else
                {
                    this.dgview.Visible = false;
                }
            }
        }
        bool change = true;
        private void proCode_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if(change)
            {
                change = false;
                txtProductCode.Text = dgview.SelectedRows[0].Cells[0].Value.ToString();
                txtProductName.Text = dgview.SelectedRows[0].Cells[1].Value.ToString();
                this.dgview.Visible = false;
                txtQuantity.Focus();
                change = true;
            }
        }

        private void txtProductCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (txtProductCode.Text.Length > 0)
                {
                    txtQuantity.Focus();
                }
                else
                {
                    txtProductCode.Focus();
                }
            }
        }

        private void txtQuantity_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (txtQuantity.Text.Length > 0)
                {
                    cmbStatus.Focus();
                }
                else
                {
                    txtQuantity .Focus();
                }
            }
        }

        private void txtQuantity_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(!char.IsNumber(e.KeyChar) & (Keys)e.KeyChar != Keys.Back & e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            ResetRecord();
        }

        private void Clear()
        {
            dtpDate.Value = DateTime.Now;
            txtProductName.Clear();
            txtProductCode.Clear();
            txtQuantity.Clear();
            cmbStatus.SelectedIndex = 0;
            this.ActiveControl = dtpDate;
            cmbStatus.SelectedIndex = 0;
        }

        private void ResetRecord()
        {
            txtProductCode.ReadOnly = false;
            Clear();
            btnAdd.Text = "Add";
            btnDelete.Hide();
            LoadData();
        }

        private bool Validation()
        {
            bool result = false;

            if(string.IsNullOrEmpty(txtProductName.Text))
            {
                errorProvider1.SetError(txtProductName,"Product Name Required");
            }
            else
            {
                errorProvider1.Clear();
            }
            if(string.IsNullOrEmpty(txtProductCode.Text))
            {
                errorProvider2.SetError(txtProductCode, "Product Code Required");
            }
            else
            {
                errorProvider2.Clear();
            }
            if (string.IsNullOrEmpty(txtQuantity.Text))
            {
                errorProvider3.SetError(txtQuantity, "Quantity Required");
            }
            else
            {
                errorProvider3.Clear();
            }
            if (!string.IsNullOrEmpty(txtProductCode.Text) && !string.IsNullOrEmpty(txtProductName.Text) && !string.IsNullOrEmpty(txtQuantity.Text))
            {
                errorProvider1.Clear();
                errorProvider2.Clear();
                errorProvider3.Clear();
                result = true;
            }
            return result;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            
            if (Validation())
            {
                DialogResult dialogResult = MessageBox.Show("Are you sure want to add/update?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                {

                    SqlConnection con = new SqlConnection("Data Source=LAPTOP-2B38MO48\\SQLEXPRESS;Initial Catalog=StockManagement;Integrated Security=True");

                    //Insert Logic
                    con.Open();


                    var sqlQuery = "";
                    try
                    {
                        if (ifProductExists(con, txtProductCode.Text))
                        {

                            sqlQuery = @"UPDATE [dbo].[tbl_stock] SET [ProductName] = '" + txtProductName.Text + "',[ProductStatus] = '" + cmbStatus.SelectedIndex + "', [Quantity] = '" + txtQuantity.Text + "', [TransDate] = '" + dtpDate.Value.ToString("MM/dd/yyyy  hh:mm") + "' WHERE [ProductCode] = '" + txtProductCode.Text + "'";

                            MessageBox.Show("Product stock succefully updated.");
                        }
                        else
                        {
                            sqlQuery = @"INSERT INTO [dbo].[tbl_stock]([ProductCode],[ProductName],[TransDate],[Quantity],[ProductStatus])VALUES 
                            ('" + txtProductCode.Text + "','" + txtProductName.Text + "','" + dtpDate.Value.ToString("MM/dd/yyyy  hh:mm") + "' ,'" + txtQuantity.Text+ "','" + cmbStatus.SelectedIndex + "')";
                            MessageBox.Show("New product stock succefully added.");
                        }

                        SqlCommand cmd = new SqlCommand(sqlQuery, con);
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                        con.Close();

                    }

                    ResetRecord();
                }
            }
        }

        public void LoadData()
        {
            SqlConnection con = new SqlConnection("Data Source=LAPTOP-2B38MO48\\SQLEXPRESS;Initial Catalog=StockManagement;Integrated Security=True");
            SqlDataAdapter sda = new SqlDataAdapter("SELECT * FROM [StockManagement].[dbo].[tbl_stock]", con);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            dgvStock.Rows.Clear();
            foreach (DataRow item in dt.Rows)
            {
                int n = dgvStock.Rows.Add();
                dgvStock.Rows[n].Cells["dgSNo"].Value = n+1;
                dgvStock.Rows[n].Cells["dgProCode"].Value = item["ProductCode"].ToString();
                dgvStock.Rows[n].Cells["dgProName"].Value = item["ProductName"].ToString();
                dgvStock.Rows[n].Cells["dgQty"].Value = float.Parse(item["Quantity"].ToString());
                dgvStock.Rows[n].Cells["dgDate"].Value = item["TransDate"].ToString();
                
                if ((bool)item["ProductStatus"])
                {
                    dgvStock.Rows[n].Cells["dgStatus"].Value = "Active";
                }
                else
                {
                    dgvStock.Rows[n].Cells["dgStatus"].Value = "Deactive";
                }

            }
            if (dgvStock.Rows.Count > 0)
            {
                lblTotalProducts.Text = "Total Product: " + dgvStock.Rows.Count.ToString();
                float totQty = 0;
                for (int i = 0; i < dgvStock.Rows.Count; ++i)
                {
                    totQty += float.Parse(dgvStock.Rows[i].Cells["dgQty"].Value.ToString());
                    lblTotalQuantity.Text = "Total Quantity: " + totQty.ToString();
                }
            }
            else
            {
                lblTotalProducts.Text = "Total Product: 0";
                lblTotalQuantity.Text = "Total Quantity: 0";
            }
        }

        private bool ifProductExists(SqlConnection con, string productCode)
        {
            SqlDataAdapter sda = new SqlDataAdapter("SELECT 1 FROM [tbl_stock] WHERE [ProductCode] ='" + productCode + "'", con);
            DataTable dt = new DataTable();
            sda.Fill(dt);

            if (dt.Rows.Count > 0)
                return true;
            else
                return false;
        }

        private void dgvStock_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvStock_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Clear();
            btnDelete.Show();
            btnAdd.Text = "Update";
            cmbStatus.SelectedIndex = 0;

            txtProductCode.Text = dgvStock.SelectedRows[0].Cells["dgProCode"].Value.ToString();
            txtProductCode.ReadOnly = true;

            txtProductName.Text = dgvStock.SelectedRows[0].Cells["dgProName"].Value.ToString();

            txtQuantity.Text = dgvStock.SelectedRows[0].Cells["dgQty"].Value.ToString();

            dtpDate.Value = DateTime.Now;

            if (dgvStock.SelectedRows[0].Cells["dgStatus"].Value.ToString() == "Active")
            {

                cmbStatus.SelectedIndex = 1;
            }
            else
            {
                cmbStatus.SelectedIndex = 0;

            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            
            if (Validation())
            {
                DialogResult dialogResult = MessageBox.Show("Are you sure want to delete?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                {
                    SqlConnection con = new SqlConnection("Data Source=LAPTOP-2B38MO48\\SQLEXPRESS;Initial Catalog=StockManagement;Integrated Security=True");
                    var sqlQuery = "";


                    try
                    {
                        if (ifProductExists(con, txtProductCode.Text))
                        {
                            con.Open();
                            sqlQuery = @"DELETE FROM[dbo].[tbl_stock] WHERE [ProductCode] ='" + txtProductCode.Text + "'";
                            SqlCommand cmd = new SqlCommand(sqlQuery, con);
                            cmd.ExecuteNonQuery();
                            errorProvider1.Clear();
                            MessageBox.Show("Product succefully deleted.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        }
                        else
                        {
                            MessageBox.Show("Record not exists.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                        con.Close();
                    }



                    ResetRecord();
                }
            }
        }

        private DataGridView dgview;
        private DataGridViewTextBoxColumn dgviewcol1;
        private DataGridViewTextBoxColumn dgviewcol2;

        void Search()
        {
            dgview = new DataGridView();
            dgviewcol1 = new DataGridViewTextBoxColumn();
            dgviewcol2 = new DataGridViewTextBoxColumn();

            this.dgview.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgview.Columns.AddRange(new DataGridViewColumn[] {this.dgviewcol1, this.dgviewcol2});
            this.dgview.Name = "dgview";
            dgview.Visible = false;
            this.dgviewcol1.Visible = false;
            this.dgviewcol2.Visible = false;
            this.dgview.AllowUserToAddRows = false;
            this.dgview.RowHeadersVisible = false;
            this.dgview.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            this.Controls.Add(dgview);
            this.dgview.ReadOnly = true;
            dgview.BringToFront();
        }

        void Search(int LX, int LY, int DW, int DH, string ColName, String ColSize)
        {
            this.dgview.Location = new Point(LX, LY);
            this.dgview.Size = new Size(DW, DH);

            string[] ClSize = ColSize.Split(',');
            for(int i = 0; i < ClSize.Length; i++)
            {
                if(int.Parse(ClSize[i]) != 0)
                {
                    dgview.Columns[i].Width = int.Parse(ClSize[i]);
                }
                else
                {
                    dgview.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    
                }
            }

            string[] ClName = ColName.Split(',');

            for(int i = 0; i < ClName.Length; i++)
            {
                this.dgview.Columns[i].HeaderText = ClName[i];
                this.dgview.Columns[i].Visible = true;
            }
        }

        private void txtProductName_TextChanged(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection("Data Source=LAPTOP-2B38MO48\\SQLEXPRESS;Initial Catalog=StockManagement;Integrated Security=True");
            if (txtProductName.Text.Length > 0)
            {
                this.dgview.Visible = true;
                dgview.BringToFront();
                Search(259,105,430,200,"Pro Code, Pro Name","100,0");
                this.dgview.MouseDoubleClick += new MouseEventHandler(this.proCode_MouseDoubleClick);

                
                //Insert Logic
                con.Open();
                SqlDataAdapter sda = new SqlDataAdapter("SELECT ProductCode, ProductName FROM [tbl_products] WHERE [ProductName] like '%" + txtProductName.Text + "%'", con);
                DataTable dt = new DataTable();
                sda.Fill(dt);

                dgview.Rows.Clear();
                foreach(DataRow row in dt.Rows)
                {
                    int n = dgview.Rows.Add();
                    dgview.Rows[n].Cells[0].Value = row["ProductCode"].ToString();
                    dgview.Rows[n].Cells[1].Value = row["ProductName"].ToString();
                }

            }
            else
            {
                dgview.Visible = false;
            }



            con.Close();
           
        }
    } 
}
