using System;
using QLDSV.Reports;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace QLDSV
{
    public partial class FormMain : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            FormDangNhap formDangNhap = new FormDangNhap();
            formDangNhap.MdiParent = this;
            formDangNhap.Show();

            this.ribbonPage2.Visible = this.ribbonPage3.Visible = this.ribbonPage4.Visible = false;
            this.btnDangXuat.Enabled = false;
            this.btnTaoTaiKhoan.Enabled = false;
        }

        public void affterLogin()
        {
            string strLenh = "exec SP_DANGNHAP '" + Program.mlogin + "'";
            SqlDataReader myReader = Program.ExecSqlDataReader(strLenh, Program.connstr);
            if (myReader == null) return;
            myReader.Read();

            Program.username = myReader.GetString(0);
            if (Convert.IsDBNull(Program.username))
            {
                MessageBox.Show("Login bạn nhập không có quyền truy cập dữ liệu\n Bạn hãy xem lại", "Lỗi", MessageBoxButtons.OK);
            }
            Program.mHoten = myReader.GetString(1);
            Program.mGroup = myReader.GetString(2);
            Program.connstrDN = "Data Source=" + Program.servername + ";Initial Catalog=" + Program.database + ";User ID=" +
                      Program.mlogin + ";password=" + Program.password;
            myReader.Close();
            Program.conn.Close();
            this.MANV.Text = "Mã GV: " + Program.username;
            this.HOTEN.Text = "Họ tên: " + Program.mHoten;
            this.NHOM.Text = "Nhóm: " + Program.mGroup;

            this.ribbonPage2.Visible = this.ribbonPage3.Visible = true;
            this.ribbonPage4.Visible = false;
            //this.Visible = false; // Đóng form đăng nhập
            this.btnDangNhap.Enabled = false;
            this.btnDangXuat.Enabled = true;
            this.btnTaoTaiKhoan.Enabled = true;

            if (Program.mGroup == "User")
            {
                this.btnTaoTaiKhoan.Enabled = false;
                this.ribbonPage3.Visible = false;
            }
            else if (Program.mGroup == "PKeToan")
            {
                this.ribbonPage2.Visible = this.ribbonPage3.Visible = false;
                ribbonPage4.Visible = true;
            }
        }

        private Form CheckExists(Type ftype)
        {
            foreach (Form f in this.MdiChildren)
                if (f.GetType() == ftype)
                    return f;
            return null;
        }

        private void btnDangNhap_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Form frm = this.CheckExists(typeof(FormDangNhap));
            if (frm != null) frm.Activate();
            else
            {
                FormDangNhap f = new FormDangNhap();
                f.MdiParent = this;
                f.Show();
            }
        }

        private void btnDangXuat_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            foreach (Form frm in this.MdiChildren)
            {
                frm.Dispose();
                frm.Close();
            }
            this.MANV.Text = "Mã NV: ";
            this.HOTEN.Text = "Họ tên: ";
            this.NHOM.Text = "Nhóm: ";

            this.btnDangNhap.Enabled = true;
            FormDangNhap formDangNhap = new FormDangNhap();
            formDangNhap.MdiParent = this;
            formDangNhap.Show();
            this.ribbonPage2.Visible = this.ribbonPage3.Visible = this.ribbonPage4.Visible = false;
            this.btnDangXuat.Enabled = false;
            this.btnTaoTaiKhoan.Enabled = false;
        }

        private void btnSinhVien_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Form frm = this.CheckExists(typeof(FormSinhVien));
            if(frm != null)
            {
                frm.Activate();
            }
            else
            {
                FormSinhVien formSinhVien = new FormSinhVien();
                formSinhVien.MdiParent = this;
                formSinhVien.Show();
            }
        }  

        private void btnLop_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Form frm = this.CheckExists(typeof(FormLop));
            if(frm != null)
            {
                frm.Activate();
            }
            else
            {
                FormLop formLop = new FormLop();
                formLop.MdiParent = this;
                formLop.Show();
            }
        }

        private void btnMonHoc_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Form frm = this.CheckExists(typeof(FormMonHoc));
            if (frm != null)
            {
                frm.Activate();
            }
            else
            {
                FormMonHoc formMonHoc = new FormMonHoc();
                formMonHoc.MdiParent = this;
                formMonHoc.Show();
            }
        }

        private void btnDiem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Form frm = this.CheckExists(typeof(FormDiem));
            if (frm != null)
            {
                frm.Activate();
            }
            else
            {
                FormDiem formDiem = new FormDiem();
                formDiem.MdiParent = this;
                formDiem.Show();
            }
        }

        private void btnDongHocPhi_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Form frm = this.CheckExists(typeof(FormHocPhi));
            if (frm != null)
            {
                frm.Activate();
            }
            else
            {
                FormHocPhi formHocPhi = new FormHocPhi();
                formHocPhi.MdiParent = this;
                formHocPhi.Show();
            }
        }

        private void btnTaoTaiKhoan_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Form frm = this.CheckExists(typeof(FormTaoTaiKhoan));
            if (frm != null)
            {
                frm.Activate();
            }
            else
            {
                FormTaoTaiKhoan formTaoTaiKhoan = new FormTaoTaiKhoan(Program.mGroup);
                formTaoTaiKhoan.MdiParent = this;
                formTaoTaiKhoan.Show();
            }
        }

        private void btnInDSSV_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Form frm = this.CheckExists(typeof(Frpt_INDSSV));
            if (frm != null)
            {
                frm.Activate();
            }
            else
            {
                Frpt_INDSSV frpt_INDSSV = new Frpt_INDSSV();
                frpt_INDSSV.MdiParent = this;
                frpt_INDSSV.Show();
            }
        }

        private void btnInDSTHM_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Form frm = this.CheckExists(typeof(Frpt_INDSTHIHETMON));
            if (frm != null)
            {
                frm.Activate();
            }
            else
            {
                Frpt_INDSTHIHETMON frpt_INDSTHIHETMON = new Frpt_INDSTHIHETMON();
                frpt_INDSTHIHETMON.MdiParent = this;
                frpt_INDSTHIHETMON.Show();
            }
        }

        private void btnInBangDiemMH_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Form frm = this.CheckExists(typeof(Frpt_INBANGDIEMMONHOC));
            if (frm != null)
            {
                frm.Activate();
            }
            else
            {
                Frpt_INBANGDIEMMONHOC frpt_INBANGDIEMMONHOC = new Frpt_INBANGDIEMMONHOC();
                frpt_INBANGDIEMMONHOC.MdiParent = this;
                frpt_INBANGDIEMMONHOC.Show();
            }
        }

        private void btnInPhieuDiem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Form frm = this.CheckExists(typeof(Frpt_INPHIEUDIEM));
            if (frm != null)
            {
                frm.Activate();
            }
            else
            {
                Frpt_INPHIEUDIEM frpt_INPHIEUDIEM = new Frpt_INPHIEUDIEM();
                frpt_INPHIEUDIEM.MdiParent = this;
                frpt_INPHIEUDIEM.Show();
            }
        }

        private void btnInDSDongHP_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Form frm = this.CheckExists(typeof(Frpt_INDSDONGHOCPHI));
            if (frm != null)
            {
                frm.Activate();
            }
            else
            {
                Frpt_INDSDONGHOCPHI frpt_INDSDONGHOCPHI = new Frpt_INDSDONGHOCPHI();
                frpt_INDSDONGHOCPHI.MdiParent = this;
                frpt_INDSDONGHOCPHI.Show();
            }
        }

        private void btnInBangDiemTongKet_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Form frm = this.CheckExists(typeof(Frpt_INBANGDIEMTONGKET));
            if(frm != null)
            {
                frm.Activate();
            }
            else
            {
                Frpt_INBANGDIEMTONGKET frpt_INBANGDIEMTONGKET = new Frpt_INBANGDIEMTONGKET();
                frpt_INBANGDIEMTONGKET.MdiParent = this;
                frpt_INBANGDIEMTONGKET.Show();
            }
        }
    }
}
