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
    public partial class FormSinhVien : Form
    {
        private int vitriLop = 0;
        private int vitri = 0;
        private int refreshKhoa = 0;
        private Boolean isEdit = false; // Vào hàm ghi thì sẽ bỏ qua SP check trùng mã sv
        private Boolean isEditLop = false;

        public FormSinhVien()
        {
            InitializeComponent();
        }

        private void sINHVIENBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.bdsSV.EndEdit();
            this.tableAdapterManager.UpdateAll(this.DS);

        }

        private void lOPBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.bdsLOP.EndEdit();
            this.tableAdapterManager.UpdateAll(this.DS);
        }

        private void FormSinhVien_Load(object sender, EventArgs e)
        {          
            cmbKhoa.DataSource = Program.bds_dspm;
            cmbKhoa.DisplayMember = "TENCN";
            cmbKhoa.ValueMember = "TENSERVER";
            cmbKhoa.SelectedIndex = Program.mKhoa;
            Program.bds_dspm.Filter = "TENCN <> 'Phòng kế toán'";
            if (Program.mGroup != "PGV")
                cmbKhoa.Enabled = false;

            DS.EnforceConstraints = false;
            this.lOPTableAdapter.Connection.ConnectionString = Program.connstr;         
            this.lOPTableAdapter.Fill(this.DS.LOP);
            this.sINHVIENTableAdapter.Connection.ConnectionString = Program.connstr;
            this.sINHVIENTableAdapter.Fill(this.DS.SINHVIEN);
            this.dIEMTableAdapter.Connection.ConnectionString = Program.connstr;
            this.dIEMTableAdapter.Fill(this.DS.DIEM);
            //maLop = ((DataRowView)bdsSV[0])["MALOP"].ToString();

            groupBox1.Enabled = groupBox2.Enabled = false;
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
                refreshKhoa = cmbKhoa.SelectedIndex;

                DS.EnforceConstraints = false;
                this.lOPTableAdapter.Connection.ConnectionString = Program.connstr;
                this.lOPTableAdapter.Fill(this.DS.LOP);
                this.sINHVIENTableAdapter.Connection.ConnectionString = Program.connstr;
                this.sINHVIENTableAdapter.Fill(this.DS.SINHVIEN);
            }
        }

        private void btnThem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ckbPhai.Checked = true;
            isEdit = false;
            vitri = bdsSV.Position;
            bdsSV.AddNew();
            txtMaLop.Text = ((DataRowView)bdsLOP[bdsLOP.Position])["MALOP"].ToString();
            txtMaLop.ReadOnly = true;
            showHideButton1();
        }
       
        private void btnSua_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            isEdit = true;

            vitri = bdsSV.Position;
            txtMaLop.ReadOnly = true;
            txtMaSV.ReadOnly = true;//
            showHideButton1();
        }

        private void btnXoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //vitri = bdsSV.Position;

            if (bdsSV.Count == 0)
                return;

            if (Program.conn.State == ConnectionState.Closed) Program.conn.Open();
            SqlCommand sqlcmd = new SqlCommand("DBO.SP_KIEMTRASVCOHOCPHI", Program.conn);
            sqlcmd.CommandType = CommandType.StoredProcedure;
            sqlcmd.Parameters.Add("@MASV", SqlDbType.Char).Value = txtMaSV.Text;
            sqlcmd.Parameters.Add("@RET", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
            sqlcmd.ExecuteNonQuery();
            string ret = sqlcmd.Parameters["@RET"].Value.ToString();
            if(ret == "1")
            {
                MessageBox.Show("Sinh viên đã có học phí nên không thể xóa.");
                return;
            }

            if (bdsDIEM.Count > 0)
            {
                MessageBox.Show("Sinh viên đã có điểm, không thể xóa!");
                return;
            }

            DialogResult ds = MessageBox.Show("Bạn có muốn xóa sinh viên này?", "Thông báo!", MessageBoxButtons.YesNo);
            if (ds == DialogResult.Yes)
            {
                try
                {
                    bdsSV.RemoveCurrent();
                    this.sINHVIENTableAdapter.Connection.ConnectionString = Program.connstr;
                    this.sINHVIENTableAdapter.Update(this.DS.SINHVIEN);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xóa Sinh viên.\n" + ex.Message);
                }
            }
        }

        private void btnLamMoi_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            vitri = bdsSV.Position;

            this.sINHVIENTableAdapter.Connection.ConnectionString = Program.connstr;
            this.sINHVIENTableAdapter.Fill(this.DS.SINHVIEN);

            cmbKhoa.SelectedIndex = refreshKhoa;
            bdsSV.Position = vitri;
        }

        private void btnGhi_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (txtMaSV.Text.Trim() == "")
            {
                MessageBox.Show("Mã SV không được bỏ trống!");
                txtMaSV.Focus();
                return;
            }

            if (txtMaLop.Text.Trim() == "")
            {
                MessageBox.Show("Mã lớp không được bỏ trống!");
                txtMaLop.Focus();
                return;
            }

            if (txtHo.Text.Trim() == "")
            {
                MessageBox.Show("Họ không được bỏ trống!");
                txtHo.Focus();
                return;
            }

            if (txtTen.Text.Trim() == "")
            {
                MessageBox.Show("Tên không được bỏ trống!");
                txtTen.Focus();
                return;
            }

            if(txtNgaySinh.Text == "")
            {
                MessageBox.Show("Ngày sinh không được bỏ trống!");
                txtTen.Focus();
                return;
            }

            if (DateTime.Now.Year - txtNgaySinh.DateTime.Year < 0)
            {
                MessageBox.Show("Ngày sinh không hợp lệ. Vui lòng kiểm tra lại!");
                txtTenLop.Focus();
                return;
            }

            if (!isEdit)
            {
                if (Program.conn.State == ConnectionState.Closed) Program.conn.Open();
                SqlCommand sqlcmd = new SqlCommand("dbo.SP_KIEMTRAMASVTONTAI", Program.conn);
                sqlcmd.CommandType = CommandType.StoredProcedure;
                sqlcmd.Parameters.Add("@MASV", SqlDbType.Char).Value = txtMaSV.Text;
                sqlcmd.Parameters.Add("@RET", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                sqlcmd.ExecuteNonQuery();
                String ret = sqlcmd.Parameters["@RET"].Value.ToString();
                if (ret == "1")
                {
                    MessageBox.Show("Mã sinh viên đã tồn tại. Vui lòng kiểm tra lại!", "Thông báo !", MessageBoxButtons.OK);
                    return;
                }
            }

            try
            {
                bdsSV.Position = vitri;

                bdsSV.EndEdit();
                bdsSV.ResetCurrentItem();
                this.sINHVIENTableAdapter.Connection.ConnectionString = Program.connstr;
                this.sINHVIENTableAdapter.Update(this.DS.SINHVIEN);
                txtMaSV.ReadOnly = false;
                showHideButton2();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi ghi nhân viên.\n" + ex.Message, "", MessageBoxButtons.OK);
                //btnGhi.Enabled = true;
                return;
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            bdsSV.CancelEdit();
            this.sINHVIENTableAdapter.Connection.ConnectionString = Program.connstr;
            this.sINHVIENTableAdapter.Fill(this.DS.SINHVIEN);
            bdsSV.Position = vitri;
            txtMaSV.ReadOnly = false;

            showHideButton2();
        }

        private void btnThoat_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void tsmThem_Click(object sender, EventArgs e)
        {
            isEditLop = false;
            vitriLop = bdsLOP.Position;
            bdsLOP.AddNew();
            txtMaKhoa.Text = ((DataRowView)bdsLOP[0])["MAKH"].ToString().Trim();
            txtML.Focus();
            showHideButtonLop1();
        }

        private void tsmSua_Click(object sender, EventArgs e)
        {
            isEditLop = true;
            vitriLop = bdsLOP.Position;
            txtML.ReadOnly = true;
            txtML.ReadOnly = true;
            showHideButtonLop1();
        }

        private void tsmLamMoi_Click(object sender, EventArgs e)
        {
            vitriLop = this.bdsLOP.Position;
            this.lOPTableAdapter.Connection.ConnectionString = Program.connstr;
            this.lOPTableAdapter.Fill(this.DS.LOP);

            this.bdsLOP.Position = vitriLop;
        }

        private void tsmXoa_Click(object sender, EventArgs e)
        {
            if (bdsLOP.Count == 0)
                return;

            if (bdsSV.Count > 0)
            {
                MessageBox.Show("Không thể xóa lớp này vì Lớp đã có sinh viên.");
                return;
            }

            DialogResult ds = MessageBox.Show("Bạn có muốn xóa lớp này?", "Thông báo!", MessageBoxButtons.YesNo);
            if (ds == DialogResult.Yes)
            {
                try
                {
                    bdsLOP.RemoveCurrent();
                    this.lOPTableAdapter.Update(this.DS.LOP);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xóa lớp.\n" + ex.Message);
                }
            }
        }

        private void btnGhiLop_Click(object sender, EventArgs e)
        {
            if (txtML.Text.Trim().Length > 8)
            {
                MessageBox.Show("Mã lớp tối đa 8 ký tự! Vui lòng kiểm tra lại!");
                txtML.Focus();
                return;
            }
            if (txtML.Text.Trim() == "")
            {
                MessageBox.Show("Mã lớp không được bỏ trống !");
                txtML.Focus();
                return;
            }

            if (txtTenLop.Text.Trim() == "")
            {
                MessageBox.Show("Mã tên lớp không được bỏ trống !");
                txtTenLop.Focus();
                return;
            }

            if(DateTime.Now.Year - txtNgaySinh.DateTime.Year < 0)
            {
                MessageBox.Show("Ngày sinh không hợp lệ. Vui lòng kiểm tra lại!");
                txtTenLop.Focus();
                return;
            }

            if (!isEditLop)
            {
                if (Program.conn.State == ConnectionState.Closed) Program.conn.Open();
                SqlCommand sqlcmd = new SqlCommand("dbo.SP_KIEMTRALOPTONTAI", Program.conn);
                sqlcmd.CommandType = CommandType.StoredProcedure;
                sqlcmd.Parameters.Add("@MALOP", SqlDbType.Char).Value = txtML.Text;
                sqlcmd.Parameters.Add("@TENLOP", SqlDbType.NVarChar).Value = txtTenLop.Text;
                sqlcmd.Parameters.Add("@RET", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                sqlcmd.ExecuteNonQuery();
                String ret = sqlcmd.Parameters["@RET"].Value.ToString();
                if (ret == "1")
                {
                    MessageBox.Show("Lớp này đã tồn tại. Vui lòng kiểm tra lại!");
                    return;
                }
            }

            try
            {
                bdsLOP.Position = vitriLop;
                bdsLOP.EndEdit();
                bdsLOP.ResetCurrentItem();
                this.lOPTableAdapter.Connection.ConnectionString = Program.connstr;
                this.lOPTableAdapter.Update(this.DS.LOP);
                showHideButtonLop2();
                txtML.ReadOnly = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi ghi lớp.\n" + ex.Message);
                btnGhiLop.Enabled = true;
                return;
            }
        }

        private void btnHuyLop_Click(object sender, EventArgs e)
        {
            bdsLOP.CancelEdit();
            this.lOPTableAdapter.Fill(this.DS.LOP);
            bdsLOP.Position = vitriLop;
            showHideButtonLop2();
        }

        private void showHideButton1()
        {
            gcLop.Enabled = gcSV.Enabled = false;
            groupBox1.Enabled = true;
            btnGhi.Enabled = true;
            cmbKhoa.Enabled = false;
            btnThem.Enabled = btnSua.Enabled = btnLamMoi.Enabled = btnXoa.Enabled = false;
        }

        private void showHideButton2()
        {
            if (Program.mGroup == "PGV") cmbKhoa.Enabled = true;
            groupBox1.Enabled = false;
            gcLop.Enabled = gcSV.Enabled = true;
            btnGhi.Enabled = false;
            btnThem.Enabled = btnSua.Enabled = btnLamMoi.Enabled = btnXoa.Enabled = true;
        }

        private void showHideButtonLop1()
        {
            gcLop.Enabled = gcSV.Enabled = false;
            cmbKhoa.Enabled = false;
            groupBox2.Enabled = true;
            btnGhiLop.Enabled = true;
            btnGhi.Enabled = btnThem.Enabled = btnSua.Enabled = btnLamMoi.Enabled = btnXoa.Enabled = false;
        }

        private void showHideButtonLop2()
        {
            if (Program.mGroup == "PGV") cmbKhoa.Enabled = true;
            gcLop.Enabled = gcSV.Enabled = true;
            groupBox2.Enabled = false;
            btnGhiLop.Enabled = false;
            btnGhi.Enabled = btnThem.Enabled = btnSua.Enabled = btnLamMoi.Enabled = btnXoa.Enabled = true;
        }
    }
}
