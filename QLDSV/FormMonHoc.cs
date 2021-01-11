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
    public partial class FormMonHoc : Form
    {
        private int vitri = 0;
        private Boolean isEdit = false;
        private Stack<string> stackUndo;
        private string strUndo = "";
        private string tenMHRollback = "";
        public FormMonHoc()
        {
            InitializeComponent();
        }

        private void FormMonHoc_Load(object sender, EventArgs e)
        {
            stackUndo = new Stack<string>();
            
            this.DS.EnforceConstraints = false;
            this.mONHOCTableAdapter.Connection.ConnectionString = Program.connstr;
            this.mONHOCTableAdapter.Fill(this.DS.MONHOC);
            this.dIEMTableAdapter.Connection.ConnectionString = Program.connstr;
            this.dIEMTableAdapter.Fill(this.DS.DIEM);

            showHideButton2();
        }

        private void mONHOCBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.bdsMH.EndEdit();
            this.tableAdapterManager.UpdateAll(this.DS);

        }

        private void btnDong_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void btnThem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            isEdit = false;
            vitri = bdsMH.Position;
            bdsMH.AddNew();
            txtMaMH.ReadOnly = false;
            showHideButton1();
        }

        

        private void btnGhi_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if(txtMaMH.Text.Trim() == "")
            {
                MessageBox.Show("Mã môn học không được để trống!");
                txtMaMH.Focus();
                return;
            }

            if (txtTenMH.Text.Trim() == "")
            {
                MessageBox.Show("Tên môn học không được để trống!");
                txtTenMH.Focus();
                return;
            }

            if(!isEdit)
            {
                if (Program.conn.State == ConnectionState.Closed) Program.conn.Open();
                SqlCommand sqlcmd = new SqlCommand("dbo.SP_KIEMTRAMAMHTONTAI", Program.conn);
                sqlcmd.CommandType = CommandType.StoredProcedure;
                sqlcmd.Parameters.Add("@MAMH", SqlDbType.VarChar).Value = txtMaMH.Text;
                sqlcmd.Parameters.Add("@RET", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                sqlcmd.ExecuteNonQuery();
                String ret = sqlcmd.Parameters["@RET"].Value.ToString();
                if (ret == "1")
                {
                    MessageBox.Show("Mã môn học đã tồn tại. Vui lòng kiểm tra lại!");
                    return;
                }
            }

            try
            {
                if(!isEdit)
                    strUndo = "DELETE FROM MONHOC WHERE MAMH='" + txtMaMH.Text + "'";
                else
                    strUndo = "UPDATE MONHOC SET TENMH=N'" + tenMHRollback + "' WHERE MAMH='" + txtMaMH.Text + "'";
                Console.WriteLine(strUndo);
                stackUndo.Push(strUndo);

                bdsMH.Position = vitri;

                bdsMH.EndEdit();
                bdsMH.ResetCurrentItem();
                this.mONHOCTableAdapter.Connection.ConnectionString = Program.connstr;
                this.mONHOCTableAdapter.Update(this.DS.MONHOC);

                showHideButton2();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi ghi môn học.\n" + ex.Message);
                btnGhi.Enabled = true;
                return;
            }
        }

        private void btnXoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //vitri = bdsMH.Position;

            if (Program.conn.State == ConnectionState.Closed) Program.conn.Open();
            SqlCommand sqlcmd = new SqlCommand("dbo.SP_KIEMTRAMONHOCCODIEM", Program.conn);
            sqlcmd.CommandType = CommandType.StoredProcedure;
            sqlcmd.Parameters.Add("@MAMH", SqlDbType.VarChar).Value = txtMaMH.Text.Trim();
            sqlcmd.Parameters.Add("@RET", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
            sqlcmd.ExecuteNonQuery();
            String ret = sqlcmd.Parameters["@RET"].Value.ToString();
            if (ret == "1")
            {
                MessageBox.Show("Môn học này đã nhập điểm nên không thể xóa. Vui lòng kiểm tra lại !");
                return;
            }

            if (bdsMH.Count == 0)
                return;
            //if(bdsDIEM.Count > 0)
            //{
            //    MessageBox.Show("Môn học đã nhập điểm nên không thể xóa!");
            //    return;
            //}

            DialogResult ds = MessageBox.Show("Bạn có muốn xóa môn học này không?", "Thông báo", MessageBoxButtons.YesNo);
            if(ds == DialogResult.Yes)
            {
                try
                {
                    strUndo = "INSERT INTO MONHOC(MAMH, TENMH) VALUES('" + txtMaMH.Text + "', " + "N'" + txtTenMH.Text + "')";
                    bdsMH.RemoveCurrent();
                    stackUndo.Push(strUndo);
                    Console.WriteLine(strUndo);
                    this.mONHOCTableAdapter.Update(DS.MONHOC);
                    showHideButton2();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xóa môn học.\n" + ex.Message);
                }
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            bdsDIEM.CancelEdit();
            this.mONHOCTableAdapter.Fill(DS.MONHOC);

            showHideButton2();
        }

        private void btnSua_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            isEdit = true;
            vitri = bdsMH.Position;
            tenMHRollback = ((DataRowView)bdsMH[bdsMH.Position])["TENMH"].ToString();
            txtMaMH.ReadOnly = true;
            showHideButton1();
        }

        private void btnLamMoi_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            vitri = this.bdsMH.Position;
            this.mONHOCTableAdapter.Connection.ConnectionString = Program.connstr;
            this.mONHOCTableAdapter.Fill(this.DS.MONHOC);

            this.bdsMH.Position = vitri;
        }

        private void btnPhucHoi_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            bdsMH.CancelEdit();
            if (Program.KetNoi() == 0)
                return;
            String lenh = stackUndo.Pop();
            int n = Program.ExecSqlNonQuery(lenh, Program.connstr);
            this.mONHOCTableAdapter.Fill(this.DS.MONHOC);
            showHideButton2();
        }

        private void showHideButton1()
        {
            gcMH.Enabled = false;
            groupBox1.Enabled = true;
            btnGhi.Enabled = true;
            btnThem.Enabled = btnSua.Enabled = btnLamMoi.Enabled = btnXoa.Enabled = false;
        }

        private void showHideButton2()
        {
            if (stackUndo.Count > 0)
                btnPhucHoi.Enabled = true;
            else
                btnPhucHoi.Enabled = false;

            groupBox1.Enabled = false;
            gcMH.Enabled = true;
            btnGhi.Enabled = false;
            btnThem.Enabled = btnSua.Enabled = btnLamMoi.Enabled = btnXoa.Enabled = true;
        }
    }
}
