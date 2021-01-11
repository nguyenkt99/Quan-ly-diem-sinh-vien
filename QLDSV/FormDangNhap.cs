using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace QLDSV
{
    public partial class FormDangNhap : Form
    {
        public FormDangNhap()
        {
            InitializeComponent();
        }

        private void FormDangNhap_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'dS_DSPM.V_DS_PHANMANH' table. You can move, or remove it, as needed.
            this.v_DS_PHANMANHTableAdapter.Fill(this.dS_DSPM.V_DS_PHANMANH);
            cmbKhoa.SelectedIndex = 1;
            cmbKhoa.SelectedIndex = 0;

        }

        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            if(txtLogin.Text.Trim() == "")
            {
                MessageBox.Show("Vui lòng nhập tài khoản", "Lỗi đăng nhập", MessageBoxButtons.OK);
                txtLogin.Focus();
                return;
            }

            Program.mlogin = txtLogin.Text;
            Program.password = txtPass.Text;
            if (Program.KetNoi() == 0)
                return;
            //MessageBox.Show("Dang nhap thanh cong!", "Success", MessageBoxButtons.OK);
            Program.mKhoa = cmbKhoa.SelectedIndex;
            Program.bds_dspm = bdsDSPM;
            Program.mloginDN = Program.mlogin;
            Program.passwordDN = Program.password;
            Program.formMain.affterLogin();
            this.Visible = false; // Đóng (ẩn) form đăng nhập
        }

        private void cmbKhoa_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cmbKhoa.SelectedValue != null)
                Program.servername = cmbKhoa.SelectedValue.ToString();
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
