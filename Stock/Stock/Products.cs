using System;
using System.Collections.Generic;
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
    public partial class Products : Form
    {
        public Products()
        {
            InitializeComponent();
        }
          
        private void dgvProducts_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure want to add/update?", "Message",MessageBoxButtons.YesNo,MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                if (Validation())
                {

                    SqlConnection con = new SqlConnection("Data Source=LAPTOP-2B38MO48\\SQLEXPRESS;Initial Catalog=StockManagement;Integrated Security=True");

                    //Insert Logic
                    con.Open();


                    var sqlQuery = "";
                    try
                    {
                        if (IfProductsExists(con, txtProductCode.Text))
                        {

                            sqlQuery = @"UPDATE [dbo].[tbl_products] SET [ProductName] = '" + txtProductName.Text + "',[ProductStatus] = '" + cmbStatus.SelectedIndex + "' WHERE [ProductCode] = '" + txtProductCode.Text + "'";

                            MessageBox.Show("Product succefully updated.");
                        }
                        else
                        {
                            sqlQuery = @"INSERT INTO [dbo].[tbl_products]([ProductCode],[ProductName],[ProductStatus])VALUES 
                            ('" + txtProductCode.Text + "','" + txtProductName.Text + "','" + cmbStatus.SelectedIndex + "')";
                            MessageBox.Show("Product succefully added.");
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

                    ResetRecords();
                } 
            }
        }

        private bool IfProductsExists(SqlConnection con, String productCode)
        {
            SqlDataAdapter sda = new SqlDataAdapter("SELECT 1 FROM [tbl_products] WHERE [ProductCode] ='" + productCode+"'", con);
            DataTable dt = new DataTable();
            sda.Fill(dt);

            if (dt.Rows.Count > 0)
                return true;
            else
                return false;
        }

        private void Products_Load(object sender, EventArgs e)
        {
            ResetRecords();
        }

        public void LoadData()
        {
            SqlConnection con = new SqlConnection("Data Source=LAPTOP-2B38MO48\\SQLEXPRESS;Initial Catalog=StockManagement;Integrated Security=True");
            SqlDataAdapter sda = new SqlDataAdapter("SELECT * FROM [StockManagement].[dbo].[tbl_products]", con);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            dgvProducts.Rows.Clear();
            foreach (DataRow item in dt.Rows)
            {
                int n = dgvProducts.Rows.Add();
                dgvProducts.Rows[n].Cells[0].Value = item["ProductCode"].ToString();
                dgvProducts.Rows[n].Cells[1].Value = item["ProductName"].ToString();

                if ((bool)item["ProductStatus"])
                {
                    dgvProducts.Rows[n].Cells[2].Value = "Active";
                }
                else
                {
                    dgvProducts.Rows[n].Cells[2].Value = "Deactive";
                }
            }
        }

        private void dgvProducts_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Clear();
            btnDelete.Show();
            btnAdd.Text = "Update";
            cmbStatus.SelectedIndex = 0;
            txtProductCode.Text = dgvProducts.SelectedRows[0].Cells[0].Value.ToString();
            txtProductCode.ReadOnly = true;
            txtProductName.Text = dgvProducts.SelectedRows[0].Cells[1].Value.ToString();
            if(dgvProducts.SelectedRows[0].Cells[2].Value.ToString() == "Active")
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
            DialogResult dialogResult = MessageBox.Show("Are you sure want to delete?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                if (Validation())
                {
                    SqlConnection con = new SqlConnection("Data Source=LAPTOP-2B38MO48\\SQLEXPRESS;Initial Catalog=StockManagement;Integrated Security=True");
                    var sqlQuery = "";


                    try
                    {
                        if (IfProductsExists(con, txtProductCode.Text))
                        {
                            con.Open();
                            sqlQuery = @"DELETE FROM[dbo].[tbl_products] WHERE [ProductCode] ='" + txtProductCode.Text + "'";
                            SqlCommand cmd = new SqlCommand(sqlQuery, con);
                            cmd.ExecuteNonQuery();
                            errorProvider1.Clear();
                            MessageBox.Show("Product succefully deleted.","Message",MessageBoxButtons.OK,MessageBoxIcon.Information);

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



                    ResetRecords();
                } 
            }
          
        }

        private void Clear()
        {
            txtProductCode.Clear();
            txtProductName.Clear();
            cmbStatus.SelectedIndex = 0;
            txtProductCode.Focus();
            errorProvider1.Clear();
            errorProvider2.Clear();
        }

        private void ResetRecords()
        {
            txtProductCode.ReadOnly = false;
            Clear();
            btnAdd.Text = "Add";
            btnDelete.Hide();
            LoadData();
            errorProvider1.Clear();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure want to reset?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if(dialogResult == DialogResult.Yes)
            {
                ResetRecords();
            }
            
        }

        private bool Validation()
        {
            if(string.IsNullOrEmpty(txtProductCode.Text))
            {
                errorProvider1.Clear();
                errorProvider1.SetError(txtProductCode, "Product Code Required");
            }
            if (string.IsNullOrEmpty(txtProductName.Text))
            {
                errorProvider2.Clear();
                errorProvider2.SetError(txtProductName, "Product Name Required");
            }

            bool result = false;
            if(!string.IsNullOrEmpty(txtProductCode.Text)&& !string.IsNullOrEmpty(txtProductName.Text)&& cmbStatus.SelectedIndex>-1)
            {
                result = true;
            }

            return result;

        }

        private void Products_FormClosed(object sender, FormClosedEventArgs e)
        {
            StockMain.productFormOpen = false;
        }
    }

}
