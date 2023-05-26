using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataHelper;

namespace StudyTask
{
    public class Program
    {
        static  void Main(string[] args)
        {
            //Console.WriteLine("异步方法调用前：" + Thread.CurrentThread.ManagedThreadId.ToString());
            //Test();
            //Console.WriteLine("异步方法调用后：" + Thread.CurrentThread.ManagedThreadId.ToString());


            //TaskEx.Run(async () => await GetDefineValue());
            //TaskEx.Run(GetDefineValue());
            Console.WriteLine("================");
            Console.WriteLine("====等待中===");
            Console.WriteLine("=============");
            Console.Read();

        }






        public async static Task Test123()
        {
            while (true)
            {
                Action action = () => {

                    var files = Directory.GetFileSystemEntries(string.Format(@"{0}\Tmp", System.AppDomain.CurrentDomain.BaseDirectory), "*.txt");
                    OracleHelper oracleHelper1 = new OracleHelper("localhost", 1521, "ORCL", "scott", "tiger", "");
                    string sql = "";
                    DataTable dt = oracleHelper1.ExecuteReader(sql);

                };
                Task task=TaskEx.Run(action);
                await task;
            }
        }
        public  async static Task GetDefineValue()
        {
         var   files = Directory.GetFileSystemEntries(string.Format(@"{0}\Tmp",System.AppDomain.CurrentDomain.BaseDirectory ), "*.txt");

            while (true)
            {
                Action action = () => {

                    for (int i = 0; i < 100; i++)
                    {
                        Console.WriteLine(i);
                        Thread.Sleep(1000);
                    }
                    Console.WriteLine("================");
                    Console.WriteLine("====计算中===");
                    Console.WriteLine("=============");
                };
                Task task = TaskEx.Run(action);             
                //Console.Read();
                await task;
            }
        }
        public async static void Test()
        {
            Console.WriteLine("异步方法等待前：" + Thread.CurrentThread.ManagedThreadId.ToString());
            Console.WriteLine("开始等待：" + DateTime.Now.ToString());
            await Wait();
            Console.WriteLine("异步方法等待后：" + Thread.CurrentThread.ManagedThreadId.ToString());
            Console.WriteLine("结束等待：" + DateTime.Now.ToString());
        }

        public async  static Task Wait()
        {
            Action action = () =>
            {
                Console.WriteLine("任务运行：" + Thread.CurrentThread.ManagedThreadId.ToString());
                Thread.Sleep(100);
                Console.WriteLine("任务运行后：" + Thread.CurrentThread.ManagedThreadId.ToString());
            };
            Task task = null;
            //    task= TaskEx.Run(action);
            task = new Task(action);
            task.Start();
            await task;



            Action action2 = () =>
            {
                Console.WriteLine("任务运行2：" + Thread.CurrentThread.ManagedThreadId.ToString());
                Thread.Sleep(100);
                Console.WriteLine("任务运行后2：" + Thread.CurrentThread.ManagedThreadId.ToString());
            };
            Task task2 = null;
            //    task= TaskEx.Run(action);
            task2 = new Task(action2);
            task2.Start();
            await task2;
        }
    }
}
