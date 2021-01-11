using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace QLDSV.Reports
{
    public partial class Xrpt_INDSTHIHETMON : DevExpress.XtraReports.UI.XtraReport
    {
        public Xrpt_INDSTHIHETMON(string maLop, string maMH, int lanThi)
        {
            InitializeComponent();
            this.sqlDataSource1.Connection.ConnectionString = Program.connstr;
            this.sqlDataSource1.Queries[0].Parameters[0].Value = maLop;
            this.sqlDataSource1.Queries[0].Parameters[1].Value = maMH;
            this.sqlDataSource1.Queries[0].Parameters[2].Value = lanThi;
            this.sqlDataSource1.Fill();
        }

    }
}
