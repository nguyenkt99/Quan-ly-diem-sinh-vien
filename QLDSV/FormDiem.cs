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
    public partial class FormDiem : Form
    {
        Boolean huy = false;

        public FormDiem()
        {
            InitializeComponent();
        }

        private void lOPBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.bdsLOP.EndEdit();
            this.tableAdapterManager.UpdateAll(this.DS);
        }

        private void FormDiem_Load(object sender, EventArgs e)
        {
            showHideOption1();

            cmbKhoa.DataSource = Program.bds_dspm;
            cmbKhoa.DisplayMember = "TENCN";
            cmbKhoa.ValueMember = "TENSERVER";
            cmbKhoa.SelectedIndex = Program.mKhoa;
            Program.bds_dspm.Filter = "TENCN <> 'Phòng kế toán'";
            if (Program.mGroup != "PGV")
                cmbKhoa.Enabled = false;

            cmbLanThi.SelectedIndex = 0;

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
                DS.EnforceConstraints = false;
                this.lOPTableAdapter.Connection.ConnectionString = Program.connstr;
                this.lOPTableAdapter.Fill(this.DS.LOP);
            }
        }

        private void btnBatDau_Click(object sender, EventArgs e)
        {
            showHideOption2();
            huy = false;
            string maLop = cmbLop.SelectedValue.ToString().Trim();
            string maMH = cmbMH.SelectedValue.ToString().Trim();
            int lanThi = Int16.Parse(cmbLanThi.Text);

            if (Program.conn.State == ConnectionState.Closed) Program.conn.Open();
            SqlCommand sqlcmd = new SqlCommand("dbo.SP_KTLOPTHILANMOT", Program.conn);
            sqlcmd.CommandType = CommandType.StoredProcedure;
            sqlcmd.Parameters.Add("@MALOP", SqlDbType.VarChar).Value = maLop;
            sqlcmd.Parameters.Add("@MAMH", SqlDbType.VarChar).Value = maMH;
            sqlcmd.Parameters.Add("@LAN", SqlDbType.SmallInt).Value = 1;
            sqlcmd.Parameters.Add("@RET", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
            sqlcmd.ExecuteNonQuery();
            int ret = Int16.Parse(sqlcmd.Parameters["@RET"].Value.ToString());
            Program.conn.Close();
            if (ret == 1)
            {
                // kt thi lan 2 chua?
                if (Program.conn.State == ConnectionState.Closed) Program.conn.Open();
                SqlCommand sqlcmd3 = new SqlCommand("dbo.SP_KTLOPTHILANMOT", Program.conn);
                sqlcmd3.CommandType = CommandType.StoredProcedure;
                sqlcmd3.Parameters.Add("@MALOP", SqlDbType.VarChar).Value = maLop;
                sqlcmd3.Parameters.Add("@MAMH", SqlDbType.VarChar).Value = maMH;
                sqlcmd3.Parameters.Add("@LAN", SqlDbType.SmallInt).Value = 2;
                sqlcmd3.Parameters.Add("@RET", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                sqlcmd3.ExecuteNonQuery();
                int ret3 = (int)sqlcmd3.Parameters["@RET"].Value;
                Program.conn.Close();

                //------------
                if (lanThi == 1 )
                {
                    if(ret3 == 1)
                    {
                        MessageBox.Show("Lớp này đã thi 2 lần");
                        showHideOption1();
                    }
                    this.sP_DANHSACHNHAPDIEMTableAdapter.Connection.ConnectionString = Program.connstr;
                    this.sP_DANHSACHNHAPDIEMTableAdapter.Fill(this.DS.SP_DANHSACHNHAPDIEM, maLop, maMH, (short)lanThi);
                    return;
                }
                
            }
            else
            {
                if (lanThi == 2)
                {
                    showHideOption1();
                    MessageBox.Show("Lớp này chưa thi môn này lần 1");
                    return;
                }
            }

            if (lanThi == 2)
            {
                if (Program.conn.State == ConnectionState.Closed) Program.conn.Open();
                SqlCommand sqlcmd2 = new SqlCommand("dbo.SP_KTLOPTHILANMOT", Program.conn);
                sqlcmd2.CommandType = CommandType.StoredProcedure;
                sqlcmd2.Parameters.Add("@MALOP", SqlDbType.VarChar).Value = maLop;
                sqlcmd2.Parameters.Add("@MAMH", SqlDbType.VarChar).Value = maMH;
                sqlcmd2.Parameters.Add("@LAN", SqlDbType.SmallInt).Value = 2;
                sqlcmd2.Parameters.Add("@RET", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                sqlcmd2.ExecuteNonQuery();
                int ret2 = (int)sqlcmd2.Parameters["@RET"].Value;
                Program.conn.Close();
                if (ret2 == 1)
                {
                    showHideOption1();
                    this.sP_DANHSACHNHAPDIEMTableAdapter.Connection.ConnectionString = Program.connstr;
                    this.sP_DANHSACHNHAPDIEMTableAdapter.Fill(this.DS.SP_DANHSACHNHAPDIEM, maLop, maMH, (short)lanThi);
                    MessageBox.Show("Lớp này đã thi lần 2");
                    return;
                }
            }

            try
            {
                String lenh = "SELECT MASV FROM SINHVIEN WHERE MALOP ='" + maLop + "'";
                SqlDataReader kts = Program.ExecSqlDataReader(lenh, Program.connstr);
                int k = 0;
                SqlConnection sqlConnection = new SqlConnection(Program.connstr);
                sqlConnection.Open();
                while (kts.Read())	//đọc từng dòng
                {
                    SqlCommand sqlcmd3 = new SqlCommand("dbo.SP_THEMDSSVNHAPDIEM", sqlConnection);
                    sqlcmd3.CommandType = CommandType.StoredProcedure;
                    sqlcmd3.Parameters.Add("@MASV", SqlDbType.VarChar).Value = kts.GetString(0);
                    sqlcmd3.Parameters.Add("@MAMH", SqlDbType.VarChar).Value = maMH;
                    sqlcmd3.Parameters.Add("@LAN", SqlDbType.SmallInt).Value = lanThi;
                    sqlcmd3.Parameters.Add("@RET", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                    sqlcmd3.ExecuteNonQuery();
                    int ret3 = (int)sqlcmd3.Parameters["@RET"].Value;
                    k++;
                }

                if (k == 0)
                {
                    showHideOption1();
                    MessageBox.Show("Lớp không có sinh viên");
                    return;
                }

                this.sP_DANHSACHNHAPDIEMTableAdapter.Connection.ConnectionString = Program.connstr;
                this.sP_DANHSACHNHAPDIEMTableAdapter.Fill(this.DS.SP_DANHSACHNHAPDIEM, maLop, maMH, (short)lanThi);
                kts.Close();
                sqlConnection.Close();

                // Không có sv nào diểm dưới 5
                if (sP_DANHSACHNHAPDIEMDataGridView.Rows.Count == 1)
                {
                    showHideOption1();
                    MessageBox.Show("Lớp không có sinh viên dưới 5 điểm. Xin kiểm tra lại!");
                    return;
                }
                huy = true;  // đã thêm điểm tạm là 0 vào nên huy = true để xóa đúng sinh viên (rollback)
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        private void btnGhi_Click(object sender, EventArgs e)
        {
            string maMH = cmbMH.SelectedValue.ToString().Trim();
            int lanThi = Int16.Parse(cmbLanThi.Text);

            int numOfRows = bdsDSNHAPDIEM.Count;

            for (int i = 0; i < numOfRows; i++)
            {
                float diem = float.Parse(((DataRowView)bdsDSNHAPDIEM[i])["DIEM"].ToString());
                if (diem < 0.0 || diem > 10.0)
                {
                    MessageBox.Show("Điểm không hợp lệ. Mời bạn nhập lại (0 <= diem <= 10).");
                    this.sP_DANHSACHNHAPDIEMDataGridView.Rows[i].Selected = true;                    
                    return;
                }
            }

            DialogResult ds = MessageBox.Show("Ghi điểm vào CSDL?", "Thông báo!", MessageBoxButtons.YesNo);
            if (ds == DialogResult.Yes)
            {
                for (int i = 0; i < numOfRows; i++)
                {
                    string maSV = ((DataRowView)bdsDSNHAPDIEM[i])["MASV"].ToString();
                    string diem = ((DataRowView)bdsDSNHAPDIEM[i])["DIEM"].ToString();
                    SqlConnection sqlConnection = new SqlConnection(Program.connstr);
                    sqlConnection.Open();
                    SqlCommand sqlcmd = new SqlCommand("dbo.SP_SUADIEMSV", sqlConnection);
                    sqlcmd.CommandType = CommandType.StoredProcedure;
                    sqlcmd.Parameters.Add("@MASV", SqlDbType.VarChar).Value = maSV;
                    sqlcmd.Parameters.Add("@MAMH", SqlDbType.VarChar).Value = maMH;
                    sqlcmd.Parameters.Add("@LAN", SqlDbType.SmallInt).Value = lanThi;
                    sqlcmd.Parameters.Add("@DIEM", SqlDbType.Float).Value = diem;
                    sqlcmd.Parameters.Add("@RET", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                    sqlcmd.ExecuteNonQuery();
                    int ret3 = (int)sqlcmd.Parameters["@RET"].Value;
                }
                MessageBox.Show("Ghi điểm thành công!");
                showHideOption1();
            }  
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            showHideOption1();
            string maLop = cmbLop.SelectedValue.ToString().Trim();
            string maMH = cmbMH.SelectedValue.ToString().Trim();
            int lanThi = Int16.Parse(cmbLanThi.Text);
            if (!huy)
                return;
            try
            {
                String lenh = "SELECT MASV FROM SINHVIEN WHERE MALOP ='" + maLop + "'";
                SqlDataReader kts = Program.ExecSqlDataReader(lenh, Program.connstr);
                int k = 0;
                SqlConnection sqlConnection = new SqlConnection(Program.connstr);
                sqlConnection.Open();
                while (kts.Read())	//đọc từng dòng
                {
                    SqlCommand sqlcmd3 = new SqlCommand("dbo.SP_HUYTHEMDSSVNHAPDIEM", sqlConnection);
                    sqlcmd3.CommandType = CommandType.StoredProcedure;
                    sqlcmd3.Parameters.Add("@MASV", SqlDbType.VarChar).Value = kts.GetString(0);
                    sqlcmd3.Parameters.Add("@MAMH", SqlDbType.VarChar).Value = maMH;
                    sqlcmd3.Parameters.Add("@LAN", SqlDbType.SmallInt).Value = lanThi;
                    sqlcmd3.Parameters.Add("@RET", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                    sqlcmd3.ExecuteNonQuery();
                    int ret3 = (int)sqlcmd3.Parameters["@RET"].Value;
                    k++;
                }
                if (k == 0) { MessageBox.Show("Lớp không có sinh viên"); }
                this.sP_DANHSACHNHAPDIEMTableAdapter.Connection.ConnectionString = Program.connstr;
                this.sP_DANHSACHNHAPDIEMTableAdapter.Fill(this.DS.SP_DANHSACHNHAPDIEM, maLop, maMH, (short)lanThi);
                kts.Close();
                sqlConnection.Close();
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        private void btnDong_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void showHideOption1()
        {
            btnBatDau.Enabled = true;
            btnGhi.Enabled = false;
            btnHuy.Enabled = false;
            btnDong.Enabled = true;
            groupBox1.Enabled = true;
            sP_DANHSACHNHAPDIEMDataGridView.Enabled = false;
        }

        private void showHideOption2()
        {
            btnBatDau.Enabled = false;
            btnGhi.Enabled = true;
            btnHuy.Enabled = true;
            btnDong.Enabled = false;
            groupBox1.Enabled = false;
            sP_DANHSACHNHAPDIEMDataGridView.Enabled = true;
        }
    }
}
