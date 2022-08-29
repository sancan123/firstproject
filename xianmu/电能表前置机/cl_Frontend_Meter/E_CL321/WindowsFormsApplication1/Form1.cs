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
        E_CL321.CL321 cl321 = new E_CL321.CL321();
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

            cl321.InitSetting(1, 3000, 300, "192.168.0.2", 10004, 20000, false);

            cl321.Connect(1, out str_OutFrame);

            // revalue = CL321.


        }
    }
}
