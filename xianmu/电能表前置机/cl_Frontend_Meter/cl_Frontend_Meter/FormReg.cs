using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Text;
using System.Windows.Forms;

namespace cl_Frontend_Meter
{
    public partial class FormReg : Form
    {
        public FormReg()
        {
            InitializeComponent();
        }

        private void FormReg_Load(object sender, EventArgs e)
        {
            textBox1.Text = Program.reg.CreateCode().ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string b1 = Program.reg.BoolCode(textBox1.Text, textBox2.Text.Replace("-", ""));
            if (b1=="czx")
            {
                Program.bolReg = true;
                MessageBox.Show("注册成功", "提示！");
                CLBase.g_WriteINI(Application.StartupPath + "\\System\\VirtualMeter.ini", "System", "Registered", textBox2.Text);
            }
            else
                MessageBox.Show("注册失败", "提示！");
            this.Close();
        }
    }
}
