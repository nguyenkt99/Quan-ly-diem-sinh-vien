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
    public partial class Frpt_INPHIEUDIEM : Form
    {
        public Frpt_INPHIEUDIEM()
        {
            InitializeComponent();
        }

        private void Frpt_INPHIEUDIEM_Load(object sender, EventArgs e)
        {
            cmbKhoa.DataSource = Program.bds_dspm;
            cmbKhoa.DisplayMember = "TENCN";
            cmbKhoa.ValueMember = "TENSERVER";
            cmbKhoa.SelectedIndex = Program.mKhoa;

            if (Program.mGroup == "Khoa")
                cmbKhoa.Enabled = false;

            if (Program.mGroup == "PGV")
                Program.bds_dspm.Filter = "TENCN <> 'Phòng kế toán'";

            btnPreview.Enabled = false;
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            if (Program.KetNoi() == 0) return;

            String lenh = "SELECT HO + ' ' + TEN FROM [dbo].SINHVIEN WHERE MASV ='" + this.txtMaSV.Text + "'";
            Program.myReader = Program.ExecSqlDataReader(lenh, Program.connstr);
            if (!Program.myReader.HasRows)
            {
                MessageBox.Show("Không tìm thấy thông tin sinh viên này");
                Program.myReader.Close();
                this.txtHoTen.Text = "";
                this.btnPreview.Enabled = false;
                return;
            }
            Program.myReader.Read();
            this.txtHoTen.Text = Program.myReader.GetString(0);
            Program.myReader.Close();

            btnPreview.Enabled = true;
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            string maSV = txtMaSV.Text;
            Xrpt_INPHIEUDIEM rpt = new Xrpt_INPHIEUDIEM(maSV);
            rpt.lblHoTen.Text = txtHoTen.Text;
            rpt.lblMaSV.Text = txtMaSV.Text;
            ReportPrintTool print = new ReportPrintTool(rpt);
            print.ShowPreviewDialog();
        }

        private void btnDong_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
