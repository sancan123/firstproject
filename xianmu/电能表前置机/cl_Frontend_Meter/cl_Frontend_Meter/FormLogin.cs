using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Text;
using System.Windows.Forms;

namespace cl_Frontend_Meter
{
    public partial class FormLogin : Form
    {
        public FormLogin()
        {
            InitializeComponent();
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {
            Program.UpSideAgreement = (CLBase.g_GetINI(Application.StartupPath + "\\System\\VirtualMeter.ini", "System", "UpSideAgreement", "698.45"));
            Program.UpSideBaudRate = (CLBase.g_GetINI(Application.StartupPath + "\\System\\VirtualMeter.ini", "System", "UpSideBaudRate", "2400,e,8,1"));
            Program.Element = (CLBase.g_GetINI(Application.StartupPath + "\\System\\VirtualMeter.ini", "System", "Element", "合元"));

            comboBox1.Text = Program.UpSideAgreement;
            comboBox2.Text = Program.UpSideBaudRate;
            comboBox3.Text = Program.Element;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Program.UpSideAgreement = comboBox1.Text;
            Program.UpSideBaudRate = comboBox2.Text;
            Program.Element = comboBox3.Text;

            CLBase.g_WriteINI(Application.StartupPath + "\\System\\VirtualMeter.ini", "System", "UpSideAgreement", Program.UpSideAgreement);
            CLBase.g_WriteINI(Application.StartupPath + "\\System\\VirtualMeter.ini", "System", "UpSideBaudRate", Program.UpSideBaudRate);
            CLBase.g_WriteINI(Application.StartupPath + "\\System\\VirtualMeter.ini", "System", "Element", Program.Element);

            this.Visible = false;
            FrmMain frm = new FrmMain();
            frm.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
