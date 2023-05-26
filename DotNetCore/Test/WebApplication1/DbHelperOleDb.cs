using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.OleDb;

namespace WebApplication1
{
    public class DbHelperOleDb
    {
        private static OleDbConnection conn = new OleDbConnection();
        private static OleDbCommand cmd =new OleDbCommand ();
        private static OleDbDataAdapter adapter = new OleDbDataAdapter();

        public static void Query()
        {

        }

        
        public static int ExcuteSql()
        {
            return 0;
        }
    }
}