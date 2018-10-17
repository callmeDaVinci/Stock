using CrystalDecisions.CrystalReports.Engine;
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

namespace Stock.ReportForm
{
    public partial class StockReport : Form
    {
        ReportDocument cryrpt = new ReportDocument();

        public StockReport()
        {
            InitializeComponent();
        }

        private void StockReport_Load(object sender, EventArgs e)
        {

        }

        private void btnShowReport_Click(object sender, EventArgs e)
        {
           
            
            cryrpt.Load(@"D:\CodeBase\Stock\Stock\Stock\Reports\StockReport.rpt");
            SqlConnection con = new SqlConnection("Data Source=LAPTOP-2B38MO48\\SQLEXPRESS;Initial Catalog=StockManagement;Integrated Security=True");
            con.Open();
            DataTable dt = new DataTable();
            SqlDataAdapter sda = new SqlDataAdapter("SELECT * FROM [tbl_stock] WHERE CAST(TransDate as Date) between '" + dateTimePicker1.Value.ToString("MM/dd/yyyy") + "' and '" + dateTimePicker2.Value.ToString("MM/dd/yyyy") + "'", con);

            sda.Fill(dt);

            cryrpt.SetDataSource(dt);

            cryrpt.SetParameterValue("@FromDate", dateTimePicker1.Value.ToString("dd/MM/yyyy"));
            cryrpt.SetParameterValue("@ToDate", dateTimePicker2.Value.ToString("dd/MM/yyyy"));
            crvStockReport.ReportSource = cryrpt;
            con.Close();
        }
    }
}
