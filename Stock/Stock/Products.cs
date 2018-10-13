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
            SqlConnection con = new SqlConnection("Data Source=LAPTOP-2B38MO48\\SQLEXPRESS;Initial Catalog=StockManagement;Integrated Security=True");

            //Insert Logic
            con.Open();

            var sqlQuery = "";

            if (IfProductsExists(con, txtProductCode.Text))
            {
                sqlQuery = @"UPDATE [dbo].[tbl_products] SET [ProductName] = '" + txtProductName.Text + "',[ProductStatus] = '" + cmbStatus.SelectedIndex + "' WHERE [ProductCode] = '" + txtProductCode.Text + "'";
            }
            else
            {
                sqlQuery = @"INSERT INTO [dbo].[tbl_products]([ProductCode],[ProductName],[ProductStatus])VALUES 
                            ('" + txtProductCode.Text + "','" + txtProductName.Text + "','" + cmbStatus.SelectedIndex + "')";
            }

            SqlCommand cmd = new SqlCommand(sqlQuery, con);
            cmd.ExecuteNonQuery();

            con.Close();
            Clear();
            LoadData();



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
            cmbStatus.SelectedIndex = 1;
            txtProductCode.Focus();
            LoadData();
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
            cmbStatus.Text = "";
           
            txtProductCode.Text = dgvProducts.SelectedRows[0].Cells[0].Value.ToString();
            txtProductName.Text = dgvProducts.SelectedRows[0].Cells[1].Value.ToString();
            cmbStatus.SelectedText = dgvProducts.SelectedRows[0].Cells[2].Value.ToString();

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection("Data Source=LAPTOP-2B38MO48\\SQLEXPRESS;Initial Catalog=StockManagement;Integrated Security=True");
            var sqlQuery = "";

            

            if (IfProductsExists(con,txtProductCode.Text))
            {
                con.Open();
                sqlQuery = @"DELETE FROM[dbo].[tbl_products] WHERE [ProductCode] ='" + txtProductCode.Text + "'";
                SqlCommand cmd = new SqlCommand(sqlQuery, con);
                cmd.ExecuteNonQuery();
                con.Close();
                Clear();
            }
            else
            {
                MessageBox.Show("Record not exists.");
            }

            
            LoadData();
        }

        private void Clear()
        {
            txtProductCode.Clear();
            txtProductName.Clear();
            txtProductCode.Focus();
        }
    }
}
