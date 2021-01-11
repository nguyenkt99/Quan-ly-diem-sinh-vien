using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLDSV
{
    public partial class FormHocPhi : Form
    {
        public FormHocPhi()
        {
            InitializeComponent();
            showHideOption1();
            txtHoTen.ReadOnly = true;
            txtMaLop.ReadOnly = true;
        }

        private void btnDong_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }


        private void btnTim_Click(object sender, EventArgs e)
        {
            if (Program.KetNoi() == 0) return;

            String lenh = "SELECT HO, TEN, MALOP FROM [dbo].SINHVIEN WHERE MASV ='" + this.txtMaSV.Text + "' and NGHIHOC = 'false'";
            Program.myReader = Program.ExecSqlDataReader(lenh, Program.connstr);
            if (!Program.myReader.HasRows)
            {
                showHideOption1();
                MessageBox.Show("Không tìm thấy thông tin sinh viên này hoặc sinh viên đã nghỉ học", "Thông báo", MessageBoxButtons.OK);
                Program.myReader.Close();
                return;
            }
            Program.myReader.Read();

            this.txtHoTen.Text = Program.myReader.GetString(0) + " " + Program.myReader.GetString(1);
            this.txtMaLop.Text = Program.myReader.GetString(2);
            Program.myReader.Close();
            try
            {
                showHideOption2();
                this.sP_DSDONGHOCPHITableAdapter.Connection.ConnectionString = Program.connstr;
                this.sP_DSDONGHOCPHITableAdapter.Fill(this.DS.SP_DSDONGHOCPHI, this.txtMaSV.Text); 
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }

            txtSoTienDong.Text = "";

        }

        private void btnThem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            showHideOption3();
            int soTienDaDong = Int32.Parse(((DataRowView)bdsDSDONGHOCPHI[bdsDSDONGHOCPHI.Position])["SOTIENDADONG"].ToString());
            int hocPhi = Int32.Parse(((DataRowView)bdsDSDONGHOCPHI[bdsDSDONGHOCPHI.Position])["HOCPHI"].ToString());
            if (soTienDaDong == hocPhi)
            {
                MessageBox.Show("Sinh viên đã đóng đủ học phí kì này!");
                showHideOption2();
                return;
            }
            else
            {
                txtSoTienDong.Text = (hocPhi - soTienDaDong).ToString();
            }
        }

        private void btnGhi_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string maSV = "";
            string nienKhoa = "";
            int hocKy = 0;
            int soTienDong = 0;
            int soTienDaDong = 0;
            int hocPhi = 0;

            if (txtSoTienDong.Text == "")
            {
                MessageBox.Show("Vui lòng nhập vào số tiền cần đóng!");
                return;
            }

            try
            {
                soTienDong = Int32.Parse(txtSoTienDong.Text);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Số tiền không hợp lệ. Vui lòng kiểm tra lại!\n" + ex.Message);
                return;
            }

            maSV = txtMaSV.Text.Trim();
            nienKhoa = txtNienKhoa.Text;
            hocKy = Int32.Parse(txtHocKy.Text);
            soTienDong = Int32.Parse(txtSoTienDong.Text);
            soTienDaDong = Int32.Parse(((DataRowView)bdsDSDONGHOCPHI[bdsDSDONGHOCPHI.Position])["SOTIENDADONG"].ToString());
            hocPhi = Int32.Parse((((DataRowView)bdsDSDONGHOCPHI[bdsDSDONGHOCPHI.Position])["HOCPHI"].ToString()));

            if (soTienDong <= 0)
            {
                MessageBox.Show("Số tiền đóng phải lớn hơn 0.");
                return;
            }

            if (soTienDong + soTienDaDong > hocPhi)
            {
                MessageBox.Show("Số tiền đóng quá số tiền nợ: " + (hocPhi - soTienDaDong));
                return;
            }

            if (Program.KetNoi() == 0) return;

            String lenh = "EXEC SP_DONGHOCPHI '" + maSV + "','" + nienKhoa + "','" + hocKy + "','" + soTienDong + "'";
            int result = Program.ExecSqlNonQuery(lenh, Program.connstr);
            if (result == 1)
            {
                showHideOption2();
                this.sP_DSDONGHOCPHITableAdapter.Connection.ConnectionString = Program.connstr;
                this.sP_DSDONGHOCPHITableAdapter.Fill(this.DS.SP_DSDONGHOCPHI, this.txtMaSV.Text);
                MessageBox.Show("Ðóng tiền thành công", "Thông báo", MessageBoxButtons.OK);
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            showHideOption2();
        }

        private void showHideOption1() // mở FormHocPhi
        {
            btnGhi.Enabled = false;
            btnThem.Enabled = false;
            groupBox1.Enabled = true;
            groupBox2.Enabled = false;
            sP_DSDONGHOCPHIGridControl.Visible = false;
        }

        private void showHideOption2() // khi tìm ra sv để đóng học phí
        {
            btnGhi.Enabled = false;
            btnThem.Enabled = true;
            groupBox1.Enabled = true;
            groupBox2.Enabled = false;
            sP_DSDONGHOCPHIGridControl.Visible = true;
            sP_DSDONGHOCPHIGridControl.Enabled = true;
        }

        private void showHideOption3() // khi nhấn thêm để đóng học phí
        {
            btnGhi.Enabled = true;
            btnThem.Enabled = false;
            groupBox1.Enabled = false;
            groupBox2.Enabled = true;
            txtNienKhoa.ReadOnly = true;
            txtHocKy.ReadOnly = true;
            txtHocPhi.ReadOnly = true;
            txtSoTienDong.ReadOnly = false;
            sP_DSDONGHOCPHIGridControl.Visible = true;
            sP_DSDONGHOCPHIGridControl.Enabled = false;
        }
    }
}
