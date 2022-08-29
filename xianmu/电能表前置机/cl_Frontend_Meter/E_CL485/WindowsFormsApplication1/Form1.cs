using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            E_CL485.CL_Rs485 cl = new E_CL485.CL_Rs485();
            cl.InitSetting(1, "9600 e 8 1", 3000, 10, "192.168.0.2", 10004, 20000,false);
            byte[] b1=new byte[1];
            cl.SendData(new byte[] { 1, 2 }, out b1);
        }
    }
}
