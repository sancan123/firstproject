using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
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

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }
    }
}
