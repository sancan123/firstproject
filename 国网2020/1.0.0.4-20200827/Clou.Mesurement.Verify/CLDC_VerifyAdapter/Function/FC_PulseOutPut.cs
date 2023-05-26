
using System;
using CLDC_DataCore;
using CLDC_DataCore.Const;

namespace CLDC_VerifyAdapter.Function
{
    /// <summary>
    /// 脉冲输出功能
    /// </summary>
    class FC_PulseOutPut : FunctionBase
    {
        public FC_PulseOutPut(object plan)
            : base(plan) 
        {
            
        }

        /// <summary>
        /// 重写基类测试方法
        /// </summary>
        /// <param name="ItemNumber">检定方案序号</param>
        public override void Verify()
        {
            ResultNames = new string[] { "日计时结论", "时段投切结论", "需量结论", "结论" };
            base.Verify();
            if (Stop) return;                   //假如当前停止检定，则退出
            string strCurItem = "";
            int iTestNum = 0;
            string keyitem = ((int)CLDC_Comm.Enum.Cus_FunctionItem.脉冲输出功能).ToString().PadLeft(3, '0');
            string[] strResultKey = new string[BwCount];
            object[] objResultValue = new object[BwCount];

            CLDC_Comm.Enum.Cus_Clfs _clfs = CLDC_Comm.Enum.Cus_Clfs.单相;
            CLDC_DataCore.Model.DnbModel.DnbInfo.MeterBasicInfo MeterFirstInfo = null;
            MeterFirstInfo = GlobalUnit.g_CUS.DnbData.MeterGroup[GlobalUnit.FirstYaoJianMeter];
            _clfs = (CLDC_Comm.Enum.Cus_Clfs)MeterFirstInfo.Mb_intClfs;
            if (_clfs == CLDC_Comm.Enum.Cus_Clfs.单相)
                iTestNum = 3;
            else
                iTestNum = 4;
            for (int i = 1; i <= iTestNum; i++)
            {
                strCurItem = ItemKey + i.ToString("D2");//00601
                ClearItemData(strCurItem);
            }            

            if (Stop) return;                   //假如当前停止检定，则退出
            bool[] arrResult = new bool[BwCount];
            
            for (int k = 1; k <= iTestNum; k++)
            {
                strCurItem = ItemKey + k.ToString("D2");//00601

                for (int i = 0; i < BwCount; i++)
                {
                    arrResult[i] = true;
                }
            }
            if (Stop) return;
            //电表选择时钟通道 0日计时、1需量、2时段投切
            if (!PowerOn())
            {
                throw new Exception("控制源输出失败！");
            }
            //
            bool[,] pulseResult = new bool[iTestNum, BwCount];
            bool[] dnmcResult = new bool[BwCount];//直接试验合格，不做判定
            bool[] rjsResult = new bool[BwCount];
            bool[] sdtqResult = new bool[BwCount];
            bool[] xlResult = new bool[BwCount];
            for (int i = 0; i < BwCount; i++)
            {
                dnmcResult[i] = true;
                rjsResult[i] = false;
                sdtqResult[i] = false;
                xlResult[i] = true;
            }

            rjsResult = MeterProtocolAdapter.Instance.SetPulseCom(0);//日计时
            //时段投切
            sdtqResult = MeterProtocolAdapter.Instance.SetPulseCom(2);//时段投切试验

            if (iTestNum == 4)
            {
                xlResult = MeterProtocolAdapter.Instance.SetPulseCom(1);//需量
            }

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
 
                if (rjsResult[i] && sdtqResult[i] && xlResult[i])
                {
                    arrResult[i] = true;
                }
                else
                {
                    arrResult[i] = false;
                }

            }
            for (int i = 0; i < BwCount; i++)
            {               
                if (Stop) return;                   //假如当前停止检定，则退出
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
 
                ResultDictionary["结论"][i] = arrResult[i] ? Variable.CTG_HeGe : Variable.CTG_BuHeGe;
                ResultDictionary["日计时结论"][i] = rjsResult[i] ? Variable.CTG_HeGe : Variable.CTG_BuHeGe;
                ResultDictionary["时段投切结论"][i]= sdtqResult[i] ? Variable.CTG_HeGe : Variable.CTG_BuHeGe;
                ResultDictionary["需量结论"][i] = xlResult[i] ? Variable.CTG_HeGe : Variable.CTG_BuHeGe;

            }

            UploadTestResult("日计时结论");
            UploadTestResult("时段投切结论");
            UploadTestResult("需量结论");
            UploadTestResult("结论");

        }

       
        /// <summary>
        /// 初始化设备参数,计算每一块表需要检定的圈数
        /// </summary>
        /// <returns></returns>
        private bool InitEquipment()
        {
            if (Stop) return false;                   //假如当前停止检定，则退出            
            MessageController.Instance.AddMessage("开始升电压...");
            if (Stop) return false;                   //假如当前停止检定，则退出
            if (!PowerOn())
            {
                MessageController.Instance.AddMessage("升电压失败! ");
                return false;
            }
            if (Stop) return false;                   //假如当前停止检定，则退出
            
            return true;
        }
 
    }
}
