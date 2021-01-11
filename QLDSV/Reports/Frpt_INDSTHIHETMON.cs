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
    public partial class Frpt_INDSTHIHETMON : Form
    {
        public Frpt_INDSTHIHETMON()
        {
            InitializeComponent();
        }

        private void mONHOCBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.bdsMH.EndEdit();
            this.tableAdapterManager.UpdateAll(this.DS);

        }

        private void Frpt_INDSTHIHETMON_Load(object sender, EventArgs e)
        {
            cmbKhoa.DataSource = Program.bds_dspm;
            cmbKhoa.DisplayMember = "TENCN";
            cmbKhoa.ValueMember = "TENSERVER";
            cmbKhoa.SelectedIndex = Program.mKhoa;

            cmbLanThi.SelectedIndex = 0;
            if (Program.mGroup == "Khoa")
                cmbKhoa.Enabled = false;

            if (Program.mGroup == "PGV")
                Program.bds_dspm.Filter = "TENCN <> 'Phòng kế toán'";

            this.DS.EnforceConstraints = false;
            this.lOPTableAdapter.Connection.ConnectionString = Program.connstr;
            this.lOPTableAdapter.Fill(this.DS.LOP);
            this.mONHOCTableAdapter.Connection.ConnectionString = Program.connstr;
            this.mONHOCTableAdapter.Fill(this.DS.MONHOC);
        }

        private void cmbKhoa_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbKhoa.SelectedValue != null)
            {
                if (cmbKhoa.SelectedValue.ToString() != "System.Data.DataRowView")
                {
                    Program.servername = cmbKhoa.SelectedValue.ToString();
                }
            }

            if (cmbKhoa.SelectedIndex != Program.mKhoa)
            {
                Program.mlogin = Program.remotelogin;
                Program.password = Program.remotepassword;
            }
            else
            {
                Program.mlogin = Program.mloginDN;
                Program.password = Program.passwordDN;
            }

            if (Program.KetNoi() == 0)
            {
                MessageBox.Show("Lỗi chuyển khoa", "Thông báo!", MessageBoxButtons.OK);
                return;
            }
            else
            {
                this.DS.EnforceConstraints = false;
                this.lOPTableAdapter.Connection.ConnectionString = Program.connstr;
                this.lOPTableAdapter.Fill(this.DS.LOP);
                this.mONHOCTableAdapter.Connection.ConnectionString = Program.connstr;
                this.mONHOCTableAdapter.Fill(this.DS.MONHOC);
            }
        }

        private void btnDong_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            //if (txtMaLop.Text.Trim() == "")
            //{
            //    MessageBox.Show("Xin vui lòng chọn lớp!");
            //    return;
            //}
            string maLop = txtMaLop.Text.Trim();
            string maMH = txtMaMH.Text.Trim();
            int lanThi = int.Parse(cmbLanThi.Text.ToString());

            Xrpt_INDSTHIHETMON rpt = new Xrpt_INDSTHIHETMON(maLop, maMH, lanThi);
            rpt.lblLop.Text = cmbLop.Text;
            rpt.lblMH.Text = cmbMH.Text;
            rpt.lblNgayThi.Text = dateNgayThi.Text;
            rpt.lblLanThi.Text = cmbLanThi.Text;
            ReportPrintTool print = new ReportPrintTool(rpt);
            print.ShowPreviewDialog();
        }
    }
}
