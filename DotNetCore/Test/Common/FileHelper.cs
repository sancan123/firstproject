using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

namespace Common
{
    public class FileHelper
    {
        /// <summary>
        /// 获取文件绝对路径
        /// </summary>
        /// <param name="rootpath">文件所在目录的根目录</param>
        /// <param name="filename">文件名</param>
        /// <returns></returns>
        public string GetFilepath(string rootpath, string filename)
        {
            //组合成绝对路径
            string conbinePath = System.IO.Path.Combine(rootpath, filename);

            return conbinePath;
        }
        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="content"></param>
        public void writefile(string path, string content)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            StreamReader sr = new StreamReader(fs);
            StreamWriter sw = new StreamWriter(fs);
            if (sr.ReadToEnd() != null)
            {
                sw.WriteLine("\r\n" + content);
            }
            else
            {
                sw.WriteLine(content);
            }
            sr.Close();
            sw.Close();
            fs.Close();

        }


        #region
        public void ConvertTextFileToImage(String textFile, String imageFile)
        {
            //设置画布字体
            System.Drawing.Font drawFont = new System.Drawing.Font("宋体", 12);
            //实例一个画布起始位置为1.1
            System.Drawing.Bitmap image = new System.Drawing.Bitmap(1, 1);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(image);
            //读取文本内容 原理就跟水印字体一样.o(∩_∩)o 哈哈....
            String text = System.IO.File.ReadAllText(textFile, Encoding.GetEncoding("GB2312"));
            System.Drawing.SizeF sf = g.MeasureString(text, drawFont, 1024); //设置一个显示的宽度   
            image = new System.Drawing.Bitmap(image, new System.Drawing.Size(Convert.ToInt32(sf.Width), Convert.ToInt32(sf.Height)));
            g = System.Drawing.Graphics.FromImage(image);
            g.Clear(System.Drawing.Color.White);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            g.DrawString(text, drawFont, System.Drawing.Brushes.Black, new System.Drawing.RectangleF(new System.Drawing.PointF(0, 0), sf));
            image.Save(imageFile, System.Drawing.Imaging.ImageFormat.Png);
            g.Dispose();
            image.Dispose();
        }

        #endregion
    }
}
