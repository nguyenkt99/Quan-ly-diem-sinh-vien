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
    public partial class Frpt_INDSSV : Form
    {
        public Frpt_INDSSV()
        {
            InitializeComponent();
        }

        private void Frpt_INDSSV_Load(object sender, EventArgs e)
        {
            
            cmbKhoa.DataSource = Program.bds_dspm;
            cmbKhoa.DisplayMember = "TENCN";
            cmbKhoa.ValueMember = "TENSERVER";
            cmbKhoa.SelectedIndex = Program.mKhoa;
            if(Program.mGroup == "Khoa")
                cmbKhoa.Enabled = false;

            if(Program.mGroup == "PGV")
                Program.bds_dspm.Filter = "TENCN <> 'Phòng kế toán'";

            this.DS.EnforceConstraints = false;
            this.lOPTableAdapter.Connection.ConnectionString = Program.connstr;
            this.lOPTableAdapter.Fill(this.DS.LOP);
        }

        private void lOPBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.bdsLOP.EndEdit();
            this.tableAdapterManager.UpdateAll(this.DS);

        }

        private void cmbKhoa_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cmbKhoa.SelectedValue != null)
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

            if(Program.KetNoi() == 0)
            {
                MessageBox.Show("Lỗi chuyển khoa", "Thông báo!", MessageBoxButtons.OK);
                return;
            }
            else
            {
                this.DS.EnforceConstraints = false;
                this.lOPTableAdapter.Connection.ConnectionString = Program.connstr;
                this.lOPTableAdapter.Fill(this.DS.LOP);
            }
        }

        private void btnDong_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            if(txtMaLop.Text.Trim() == "")
            {
                MessageBox.Show("Xin vui lòng chọn lớp!");
                return;
            }
            Xrpt_INDSSV rpt = new Xrpt_INDSSV(txtMaLop.Text.Trim());
            rpt.lblLop.Text = cmbLop.Text;
            ReportPrintTool print = new ReportPrintTool(rpt);
            print.ShowPreviewDialog();

        }
    }
}
