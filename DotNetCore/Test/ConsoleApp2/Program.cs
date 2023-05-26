using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ConsoleApp2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            OracleHelper oracleHelper1 = new OracleHelper("127.0.0.1", 1521, "orcl.szclou.com", "scott", "tiger", "");

            string sql123 = "select * from  MT_INTUIT_MET_CONC";
            var data = oracleHelper1.ExecuteReader(sql123);
        }
    }
}
