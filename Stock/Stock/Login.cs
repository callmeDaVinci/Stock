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
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtUsername.Clear();
            txtPassword.Clear();
            txtUsername.Focus();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            //TO-DO: check login username and password
            SqlConnection con = new SqlConnection("Data Source=LAPTOP-2B38MO48\\SQLEXPRESS;Initial Catalog=StockManagement;Integrated Security=True");
            SqlDataAdapter sda = new SqlDataAdapter(@"SELECT * FROM[dbo].[tbl_Login] WHERE Username = '"+txtUsername.Text+"' AND Password = '"+txtPassword.Text+"'",con);

            DataTable dt = new DataTable();
            sda.Fill(dt);

            if (dt.Rows.Count == 1)
            {
                StockMain main = new StockMain();
                main.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Invalid username or password.","Error",MessageBoxButtons.AbortRetryIgnore,MessageBoxIcon.Error);
                btnClear_Click(sender, e);
            }
        }

        private void Login_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }  
}
