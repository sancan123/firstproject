using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CLDC_DataCore;
using System.Data;
using CLDC_Comm;
using CLDC_DataCore.Const;
using CLDC_Comm.Enum;
using CLDC_DataCore.Function;
using CLDC_DataCore.Struct;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_VerifyAdapter.VerifyService;
using System.Collections.Generic;

namespace CLDC_VerifyAdapter
{
    public partial class Form_Ariticial : Form
    {
        public Form_Ariticial( string Verify)
        {
            InitializeComponent();
             
            Txt_JCXM.Text = Verify;
            int g = 0;
            for (int i = 0; i < GlobalUnit.g_CUS.DnbData._Bws; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn == false) continue;
                dataGridView1.Rows.Add();
                
                dataGridView1.Rows[g].Cells[0].Value = (i + 1).ToString();
                dataGridView1.Rows[g].Cells[1].Value = "合格";
                g++;
            }
           

        }






        private void dataGridView1_CellContentClick(object sender, System.Windows.Forms.DataGridViewCellEventArgs e)
        {
            throw new System.NotImplementedException();
        }
        private void btn_Enter_Click(object sender, EventArgs e)
        {
               MeterBasicInfo _meter = null;
               int h = 0;
               bool result = true;
               GlobalUnit.ManualResult = new string[GlobalUnit.g_CUS.DnbData._Bws];
               GlobalUnit.ManualShuju = new string[GlobalUnit.g_CUS.DnbData._Bws];
               for (int j = 0; j < GlobalUnit.g_CUS.DnbData._Bws; j++)
               {
                     
                   _meter = Helper.MeterDataHelper.Instance.Meter(j);
               //    _meter.MeterArtificial = null;
                   if (!_meter.YaoJianYn)
                   {
                       continue;
                   }
                   if (dataGridView1.Rows[h].Cells[1].Value == null)
                   {
                       MessageBox.Show("请输入结论！！！");
                       result = false;
                   }
                   if (dataGridView1.Rows[h].Cells[2].Value == null)
                   {
                      
                       GlobalUnit.ManualShuju[j] = "";
                       GlobalUnit.ManualResult[j] = dataGridView1.Rows[h].Cells[1].Value.ToString();
                   }
                   else
                   {
                       GlobalUnit.ManualResult[j] = dataGridView1.Rows[h].Cells[1].Value.ToString();
                       GlobalUnit.ManualShuju[j] = dataGridView1.Rows[h].Cells[2].Value.ToString();
                   }
             
             
                h++;
               }
               if (result)
               {
                   this.Close();
               }
              

        }

    }
}
