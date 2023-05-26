// See https://aka.ms/new-console-template for more information
using CLou.Mis.Common;

Console.WriteLine("Hello, World!");

OracleHelper oracleHelper1 = new OracleHelper("localhost", 1521, "orcl.szclou.com", "scott", "tiger", "");

string sql123 = "select * from  MT_INTUIT_MET_CONC";
var   data = oracleHelper1.ExecuteReader(sql123);
