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
    public partial class FormLop : Form
    {
        private int vitri = -1;
        private Boolean isEdit = false;
        Stack<String> stackUndo;
        private string strUndo = "";
        private string tenLopRollback = "";
        public FormLop()
        {
            InitializeComponent();
        }

        private void lOPBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.bdsLOP.EndEdit();
            this.tableAdapterManager.UpdateAll(this.DS);
        }

        private void FormLop_Load(object sender, EventArgs e)
        {
            stackUndo = new Stack<string>();

            cmbKhoa.DataSource = Program.bds_dspm;
            cmbKhoa.DisplayMember = "TENCN";
            cmbKhoa.ValueMember = "TENSERVER";
            cmbKhoa.SelectedIndex = Program.mKhoa;
            Program.bds_dspm.Filter = "TENCN <> 'Phòng kế toán'";
            if (Program.mGroup != "PGV")
                cmbKhoa.Enabled = false;

            this.DS.EnforceConstraints = false;
            this.lOPTableAdapter.Connection.ConnectionString = Program.connstr;
            this.lOPTableAdapter.Fill(this.DS.LOP);
            this.sINHVIENTableAdapter.Connection.ConnectionString = Program.connstr;
            this.sINHVIENTableAdapter.Fill(this.DS.SINHVIEN);
            showHideButton2();
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
                DS.EnforceConstraints = false;
                this.lOPTableAdapter.Connection.ConnectionString = Program.connstr;
                this.lOPTableAdapter.Fill(this.DS.LOP);
                stackUndo.Clear();
            }
        }

        private void btnThem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            isEdit = false;
            vitri = bdsLOP.Position;
            bdsLOP.AddNew();
            txtMaKhoa.Text = ((DataRowView)bdsLOP[0])["MAKH"].ToString().Trim();
            showHideButton1();
        }

        private void btnSua_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            tenLopRollback = ((DataRowView)bdsLOP[bdsLOP.Position])["TENLOP"].ToString();

            isEdit = true;
            vitri = bdsLOP.Position;
            txtMaLop.ReadOnly = true;
            showHideButton1();
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            bdsLOP.CancelEdit();
            this.lOPTableAdapter.Fill(this.DS.LOP);
            bdsLOP.Position = vitri;
            showHideButton2();
        }

        private void btnGhi_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if(txtMaLop.Text.Trim().Length > 8)
            {
                MessageBox.Show("Mã lớp tối đa 8 ký tự! Vui lòng kiểm tra lại!");
                txtMaLop.Focus();
                return;
            }
            if (txtMaLop.Text.Trim() == "")
            {
                MessageBox.Show("Mã lớp không được bỏ trống !");
                txtMaLop.Focus();
                return;
            }

            if (txtTenLop.Text.Trim() == "")
            {
                MessageBox.Show("Mã tên lớp không được bỏ trống !");
                txtTenLop.Focus();
                return;
            }
           
            if (!isEdit)
            {
                if (Program.conn.State == ConnectionState.Closed) Program.conn.Open();
                SqlCommand sqlcmd = new SqlCommand("dbo.SP_KIEMTRALOPTONTAI", Program.conn);
                sqlcmd.CommandType = CommandType.StoredProcedure;
                sqlcmd.Parameters.Add("@MALOP", SqlDbType.Char).Value = txtMaLop.Text;
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
                if (!isEdit)
                    strUndo = "DELETE FROM LOP WHERE MALOP=" + "'" + txtMaLop.Text + "'";
                else
                    strUndo = "UPDATE LOP SET TENLOP=N'" + tenLopRollback + "'" + "WHERE MALOP='" + txtMaLop.Text + "'";
                stackUndo.Push(strUndo);

                bdsLOP.Position = vitri;
                bdsLOP.EndEdit();
                bdsLOP.ResetCurrentItem(); //Chua biet de lam gi!
                this.lOPTableAdapter.Connection.ConnectionString = Program.connstr;
                this.lOPTableAdapter.Update(this.DS.LOP);

                showHideButton2();
                txtMaLop.ReadOnly = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi ghi lớp.\n" + ex.Message);
                btnGhi.Enabled = true;
                return;
            }
        }
       
        private void btnXoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (bdsLOP.Count == 0)
                return;

            if(bdsSV.Count > 0)
            {
                MessageBox.Show("Không thể xóa lớp này vì Lớp đã có sinh viên.");
                return;
            }

            DialogResult ds = MessageBox.Show("Bạn có muốn xóa lớp này?", "Thông báo!", MessageBoxButtons.YesNo);
            if (ds == DialogResult.Yes)
            {
                try
                {
                    strUndo = "INSERT INTO LOP(MALOP,TENLOP,MAKH) VALUES('" + txtMaLop.Text + "', N'" + txtTenLop.Text + "','" + txtMaKhoa.Text + "')";
                    bdsLOP.RemoveCurrent();
                    this.lOPTableAdapter.Update(this.DS.LOP);
                    stackUndo.Push(strUndo);
                    showHideButton2();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xóa lớp.\n" + ex.Message);
                }
            }
        }


        private void btnThoat_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void btnLamMoi_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            vitri = this.bdsLOP.Position;
            this.lOPTableAdapter.Connection.ConnectionString = Program.connstr;
            this.lOPTableAdapter.Fill(this.DS.LOP);
            this.bdsLOP.Position = vitri;
        }

        private void btnPhucHoi_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            bdsLOP.CancelEdit();
            if (Program.KetNoi() == 0)
                return;
            String lenh = stackUndo.Pop();
            int n = Program.ExecSqlNonQuery(lenh, Program.connstr);
            this.lOPTableAdapter.Fill(this.DS.LOP);
            showHideButton2();
        }

        private void showHideButton1()
        {
            txtMaKhoa.ReadOnly = true;
            gcLop.Enabled = false;
            btnGhi.Enabled = true;
            cmbKhoa.Enabled = false;
            groupBox1.Enabled = true;
            btnThem.Enabled = btnSua.Enabled = btnLamMoi.Enabled = btnXoa.Enabled = false;
        }

        private void showHideButton2()
        {
            txtMaKhoa.ReadOnly = false;
            gcLop.Enabled = true;
            btnGhi.Enabled = false;
            if (Program.mGroup == "PGV") cmbKhoa.Enabled = true;
            groupBox1.Enabled = false;
            btnThem.Enabled = btnSua.Enabled = btnLamMoi.Enabled = btnXoa.Enabled = true;
            if (stackUndo.Count > 0)
                btnPhucHoi.Enabled = true;
            else
                btnPhucHoi.Enabled = false;
        }

    }
}
