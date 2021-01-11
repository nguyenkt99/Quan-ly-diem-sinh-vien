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
    public partial class FormTaoTaiKhoan : Form
    {
        public FormTaoTaiKhoan(string mGroup)
        {
            InitializeComponent();
            if (String.Compare(mGroup, "PGV") == 0)
            {
                cmbNhom.Items.Add("PGV");
                cmbNhom.Items.Add("Khoa");
                cmbNhom.Items.Add("User");
            }
            else if (String.Compare(mGroup, "PKeToan") == 0)
            {
                cmbNhom.Items.Add("PKeToan");
            }
            else if (String.Compare(mGroup, "Khoa") == 0)
            {
                cmbNhom.Items.Add("Khoa");
                cmbNhom.Items.Add("User");
            }
            cmbNhom.SelectedIndex = 0;

            if (cmbMaGV.SelectedValue != null)
                txtHoTen.Text = cmbMaGV.SelectedValue.ToString();
        }

        private void FormTaoTaiKhoan_Load(object sender, EventArgs e)
        {
            this.gETDSGVTableAdapter.Connection.ConnectionString = Program.connstrDN;
            this.gETDSGVTableAdapter.Fill(this.DS.GETDSGV);
        }

        private void cmbMaGV_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cmbMaGV.SelectedValue != null)
            {
                txtHoTen.Text = cmbMaGV.SelectedValue.ToString();
            }
        }

        private void btnDangKi_Click(object sender, EventArgs e)
        {
            String tendangnhap = txtTaiKhoan.Text;
            String matkhau = txtMatKhau.Text;
            String ten = cmbMaGV.Text.Trim(); //ten user
            int result = 0;
            if (tendangnhap.Trim().Length == 0 || matkhau.Trim().Length == 0 || ten.Trim().Length == 0)
            {
                MessageBox.Show("Không thể tạo tài khoản khi dữ có dữ liệu rỗng");
                return;
            }

            try
            {
                SqlConnection sqlConnection = new SqlConnection(Program.connstrDN);
                SqlCommand sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                //kiem tra
                sqlCommand.CommandText = "sp_TAOTAIKHOAN";
                sqlCommand.Parameters.Add(new SqlParameter("@tendangnhap", tendangnhap));
                sqlCommand.Parameters.Add(new SqlParameter("@matkhau", matkhau));
                sqlCommand.Parameters.Add(new SqlParameter("@tennguoidung", ten));
                sqlCommand.Parameters.Add(new SqlParameter("@phanquyen", cmbNhom.Text));
                SqlParameter sqlParameter = new SqlParameter("@return", System.Data.SqlDbType.Int, sizeof(int));
                sqlParameter.Direction = System.Data.ParameterDirection.ReturnValue;
                sqlCommand.Parameters.Add(sqlParameter);
                sqlConnection.Open();
                sqlCommand.ExecuteNonQuery();

                result = (int)sqlCommand.Parameters["@return"].Value;
                sqlConnection.Close();

                if (result == 0)
                {
                    MessageBox.Show("Tạo tài khoản thành công");
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tạo login.\n" + ex.Message);
            }
        }

        private void btnDong_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
