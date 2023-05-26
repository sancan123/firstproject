using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataHelper;

namespace WindowsFormsApp1
{
    public partial class Form2 : Form
    {
       
        public Form2()
        {
            InitializeComponent();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
      
        private void Form2_Load(object sender, EventArgs e)
        {                     
            DataModel dataModel = null;
            dataModel = DataModel.Load(@"C:\Users\00076427\Desktop\DotNetCore\WindowsFormsApp1\bin\Debug\Send_20230515174132384_113.dat") as DataModel;
            string writeTime = DateTime.Now.ToString("yyyyMMddHHmmss");
          string a=  (DateTime.ParseExact(writeTime, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture)).ToString("yyyy-MM-dd HH:mm:ss");

            string va="20230512164550";
            string dt = DateTime.ParseExact(va, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture).ToString();
            DateTime datetime ;
            DateTime.TryParse(va, out datetime);
         string s=   va.Insert(4,"-").Insert(7,"-").Insert(10,"  ").Insert(13,":").Insert(16,":");
            string times = (DateTime.Parse(s)).ToString();
            string time = (DateTime.Parse("2023-05-12 15:53:23")).ToString("yyyy-MM-dd");
        }
    }
}
