using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraReports.UI;

namespace QLDSV.Reports
{
    public partial class Frpt_INDSDONGHOCPHI : Form
    {
        public Frpt_INDSDONGHOCPHI()
        {
            InitializeComponent();
        }

        private void Frpt_INDSDONGHOCPHI_Load(object sender, EventArgs e)
        {
            txtMaLop.Focus();
            cmbHocKy.SelectedIndex = 0;
        }


        private void btnPreview_Click(object sender, EventArgs e)
        {
            if (txtMaLop.Text == "" || txtNienKhoa.Text == "")
            {
                MessageBox.Show("Mã lớp trống. Vui lòng kiểm tra lại!");
                txtMaLop.Focus();
                return;
            }

            if (txtNienKhoa.Text == "")
            {
                MessageBox.Show("Niên khóa trống. Vui lòng kiểm tra lại!");
                txtNienKhoa.Focus();
                return;
            }

            if (Program.KetNoi() == 0) return;

            String lenh = "SELECT 1 FROM [dbo].LOP WHERE MALOP ='" + this.txtMaLop.Text + "'";
            Program.myReader = Program.ExecSqlDataReader(lenh, Program.connstr);
            if (!Program.myReader.HasRows)
            {
                MessageBox.Show("Lớp không tồn tại. Vui lòng kiểm tra lại!");
                Program.myReader.Close();
                return;
            }
            Program.myReader.Close();

            string maLop = txtMaLop.Text.ToString();
            string nienKhoa = txtNienKhoa.Text.ToString();
            int hocKy = int.Parse(cmbHocKy.Text);
            Xrpt_INDSDONGHOCPHI rpt = new Xrpt_INDSDONGHOCPHI(maLop, nienKhoa, hocKy);
            rpt.lblMaLop.Text = maLop;
            rpt.lblNienKhoa.Text = nienKhoa;
            rpt.lblHocKy.Text = hocKy.ToString();
            ReportPrintTool print = new ReportPrintTool(rpt);
            print.ShowPreviewDialog();
        }

        private void btnDong_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
