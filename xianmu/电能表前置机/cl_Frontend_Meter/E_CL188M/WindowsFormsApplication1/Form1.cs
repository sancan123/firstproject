using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using E_CL188M;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        CL188M cl188m = new CL188M();
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] str_OutFrame = new string[0];
            bool[] bol_Positions = new bool[40];
            for (int i = 0; i < 10; i++)
            {
                bol_Positions[i] = true;
            }

            cl188m.InitSetting(54, 3000, 10, "192.168.0.2", 10004, 30000);

            //cl188m.Connect()

            cl188m.StartRemoteSignals(bol_Positions, 4, 0, 0, 0, 0, out str_OutFrame);

            // revalue = cl188m.


        }
    }
}
