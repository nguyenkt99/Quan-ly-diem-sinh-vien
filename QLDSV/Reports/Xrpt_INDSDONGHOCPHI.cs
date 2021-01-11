﻿using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace QLDSV.Reports
{
    public partial class Xrpt_INDSDONGHOCPHI : DevExpress.XtraReports.UI.XtraReport
    {
        public Xrpt_INDSDONGHOCPHI(string maLop, string nienKhoa, int hocKy)
        {
            InitializeComponent();
            this.sqlDataSource1.Connection.ConnectionString = Program.connstr;
            this.sqlDataSource1.Queries[0].Parameters[0].Value = maLop;
            this.sqlDataSource1.Queries[0].Parameters[1].Value = nienKhoa;
            this.sqlDataSource1.Queries[0].Parameters[2].Value = hocKy;
            this.sqlDataSource1.Fill();
        }

    }
}