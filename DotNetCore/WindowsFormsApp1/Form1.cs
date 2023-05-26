using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
          Task.Run(async()=> await Test(this.textBox1));
        }

        private   void Form1_Load(object sender, EventArgs e)
        {
               
        }

        public async static Task Test123(System.Windows.Forms.TextBox textBox)
        {
            DateTime startdatetime = DateTime.Now;
            string num12="", num13;
            while (true)
            {
                string start = DateTime.Now.ToString();             
                //textBox.Text += start + "\n\r";
                var files = Directory.GetFileSystemEntries(string.Format(@"{0}\Tmp", System.AppDomain.CurrentDomain.BaseDirectory), "*.txt");
                if (string.IsNullOrEmpty(files[0])) continue;                
                Action action = () => {
                    long num = 0;
                    for (int i = 0; i <=10000000; i++)
                    {
                        num++;
                        if (i==10000000)
                        {
                            num12+=DateTime.Now.ToString();
                            //textBox.Text+=num12 + "\n\r";
                        }
                    }

                    //OracleHelper oracleHelper1 = new OracleHelper("localhost", 1521, "ORCL", "scott", "tiger", "");
                    //string sql = "";
                    //DataTable dt = oracleHelper1.ExecuteReader(sql);
                    File.Delete(files[0]);
                };
                Task task = Task.Run(action);
                await task;                
                //if (DateTime.Now.Subtract(startdatetime).TotalMilliseconds >= 30000)
                //    break;
                //string enddatetime = DateTime.Now.ToString();
                
            }
            
        }



        public async static Task Test(System.Windows.Forms.TextBox textBox)
        {
            DateTime startdatetime = DateTime.Now;
            string num12 = "", num13;
            while (true)
            {
                string start = DateTime.Now.ToString();
                //textBox.Text += start + "\n\r";
                var files = Directory.GetFileSystemEntries(string.Format(@"{0}\Tmp", System.AppDomain.CurrentDomain.BaseDirectory), "*.txt");
                if (files.Count()<=0) continue;
                Action action = () => {
                    long num = 0;
                    for (int i = 0; i <= 1000000000; i++)
                    {
                        num++;
                        if (i==1000000000)
                        {
                            num12 += DateTime.Now.ToString();
                            //textBox.Text+=num12 + "\n\r";
                        }
                    }
                    Thread.Sleep(10000);
                    //OracleHelper oracleHelper1 = new OracleHelper("localhost", 1521, "ORCL", "scott", "tiger", "");
                    //string sql = "";
                    //DataTable dt = oracleHelper1.ExecuteReader(sql);
                    File.Delete(files[0]);
                };
                Task task = Task.Run(action);
                await task;
                //if (DateTime.Now.Subtract(startdatetime).TotalMilliseconds >= 30000)
                //    break;
                //string enddatetime = DateTime.Now.ToString();

            }

        }

        public async static Task Test4(System.Windows.Forms.TextBox textBox)
        {
            DateTime startdatetime = DateTime.Now;
            string num12 = "", num13;
            while (true)
            {
                string start = DateTime.Now.ToString();
                //textBox.Text += start + "\n\r";
                var files = Directory.GetFileSystemEntries(string.Format(@"{0}\Tmp", System.AppDomain.CurrentDomain.BaseDirectory), "*.txt");
                if (files.Count() >= 0) continue;
                Action action = () => {
                    long num = 0;
                    for (int i = 0; i <= 10000000; i++)
                    {
                        num++;
                        if (i == 10000000)
                        {
                            num12 += DateTime.Now.ToString();
                            //textBox.Text+=num12 + "\n\r";
                        }
                    }

                    //OracleHelper oracleHelper1 = new OracleHelper("localhost", 1521, "ORCL", "scott", "tiger", "");
                    //string sql = "";
                    //DataTable dt = oracleHelper1.ExecuteReader(sql);
                    //File.Delete(files[0]);
                };
                Task task = Task.Run(action);
                await task;
                //if (DateTime.Now.Subtract(startdatetime).TotalMilliseconds >= 30000)
                //    break;
                //string enddatetime = DateTime.Now.ToString();

            }

        }
        private async void button1_Click(object sender, EventArgs e)
        {
            //this.textBox1.Text= string.Empty; ;
            await Test123(textBox1);
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            var t1 = Task.Run(() =>
            {
                System.Diagnostics.Debug.WriteLine("button1 Thread ID :" + Thread.CurrentThread.ManagedThreadId);
                return "开始计算..";
            });
            var t3 = Task.Run(() =>
            {
                Debug.WriteLine("button1 Thread ID :" + Thread.CurrentThread.ManagedThreadId);
                Thread.Sleep(3000);
                return " 计算完成! ";
            });

            int sum = 0;
            Debug.WriteLine("button1 Thread ID :" + Thread.CurrentThread.ManagedThreadId);
            var t2 = Task.Run(() =>
            {
                System.Diagnostics.Debug.WriteLine("button1 Thread ID :" + Thread.CurrentThread.ManagedThreadId);
                for (int i = 0; i < 10000; i++)
                {
                    sum += i;
                }
                return sum.ToString();
            });

            textBox1.Text += await t1;
            await Task.Delay(TimeSpan.FromSeconds(1));
            textBox1.Text += await t2;
            textBox1.Text += await t3;
        }

        private void dataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            var grid = sender as DataGridView;
            var rowIdx = (e.RowIndex + 1).ToString();

            var centerFormat = new StringFormat()
            {
                // right alignment might actually make more sense for numbers
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            var headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
            e.Graphics.DrawString(rowIdx, this.Font, SystemBrushes.ControlText, headerBounds, centerFormat);


        }
    }
}
