using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DataHelper
{
    public class FormatHelper
    {
        /// <summary>
        /// datatable转list
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<Dictionary<string, object>> DataTableToList(DataTable dt)
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            foreach (DataRow dr in dt.Rows)
            {
                Dictionary<string, object> result = new Dictionary<string, object>();
                foreach (DataColumn dc in dt.Columns)
                {
                    result.Add(dc.ColumnName, dr[dc].ToString());
                }
                list.Add(result);
            }
            return list;
        }
        /// <summary>
        /// list转datatable
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DataTable ListToDataTable(List<Dictionary<string, object>> list)
        {
            DataTable dt = new DataTable();
            foreach (Dictionary<string, object> dic in list)
            {
                DataRow row = dt.Rows.Add();
                foreach (KeyValuePair<string, object> kvp in dic)
                {
                    if (!dt.Columns.Contains(kvp.Key))
                        dt.Columns.Add(kvp.Key);
                    row[kvp.Key] = kvp.Value;
                }
            }
            return dt;
        }
    }
}
