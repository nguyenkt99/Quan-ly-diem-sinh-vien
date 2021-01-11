﻿using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;

namespace QLDSV.Reports
{
    public partial class Xrpt_INBANGDIEMTONGKET : DevExpress.XtraReports.UI.XtraReport
    {
        public Xrpt_INBANGDIEMTONGKET(string maLop)
        {
            InitializeComponent();
            this.sqlDataSource1.Connection.ConnectionString = Program.connstr;
            this.sqlDataSource1.Queries[0].Parameters[0].Value = maLop;
            this.sqlDataSource1.Fill();
        }

    }
}
