using System.Collections.Generic;
using CLDC_Comm.Enum;
using CLDC_DataCore.Struct;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_DataCore.Const;
using System.Threading;
using System.Windows.Forms;
using CLDC_DataCore;

namespace CLDC_VerifyAdapter.PrepareTest
{
    public class PreBase : VerifyBase
    {
        public PreBase(object plan)
            : base(plan)
        {

        }

        /// <summary>
        /// 多功能项目ID
        /// </summary>
        protected override string ItemKey
        {
            get
            {
                return "";
            }
        }

               /// <summary>
        /// 结论数据结点
        /// </summary>
        protected override string ResultKey
        {
            get
            {
                return string.Format("{0}",(int)Cus_MeterResultPrjID.多功能试验);   
            }
        }


        /// <summary>
        /// 费率名称转化到费率编号
        /// </summary>
        /// <param name="FeiLvName"></param>
        /// <param name="bwIndex"></param>
        /// <returns>被检表的第几个费率</returns>
        protected int SwitchFeiLvNameToOrder(string FeiLvName, int bwIndex)
        {
            if (FeiLvName == "总") return 0;
            if (FeiLvName.Length > 1) return 0;
            string[] arrSourceFL = new string[] { "总", "尖", "峰", "平", "谷" };
            int sourceOrder = 0;
            for (int j = 0; j < arrSourceFL.Length; j++)
            {
                sourceOrder = j;
                if (arrSourceFL[j].Equals(FeiLvName))
                {
                    break;
                }
            }
            //  sourceOrder++;
            return sourceOrder;

            /*
           int returnOrder = 0;
            string meterFeiLvOrder = Helper.MeterDataHelper.Instance.Meter(bwIndex).DgnProtocol.TariffOrderType;
            for (int k = 0; k < meterFeiLvOrder.Length; k++)
            {
                returnOrder = k;
                if (meterFeiLvOrder.Substring(k, 1) == sourceOrder.ToString())
                {
                    break;
                }
            }
            return returnOrder;
        */

        }


        /// <summary>
        /// 获取多功能协议配置参数
        /// </summary>
        /// <param name="Index"></param>
        /// <param name="PramKey"></param>
        /// <param name="DefaultValue"></param>
        /// <returns></returns>

        protected int getType(int Index, string PramKey, int DefaultValue)
        {
            Dictionary<string, string> DgnPram = Helper.MeterDataHelper.Instance.Meter(Index).DgnProtocol.DgnPras;
            if (!DgnPram.ContainsKey(PramKey))
            {
                return DefaultValue;
            }
            string[] Arr_Pram = DgnPram[PramKey].Split('|');
            if (Arr_Pram.Length == 2)
            {
                return int.Parse(Arr_Pram[0]);
            }
            else
            {
                return DefaultValue;
            }
        }
        /// <summary>
        /// 获取一块表多功能协议节点值
        /// </summary>
        /// <param name="Index"></param>
        /// <param name="PramKey"></param>
        /// <returns></returns>
        protected string[] getType(int Index, string PramKey)
        {
            Dictionary<string, string> DgnPram = Helper.MeterDataHelper.Instance.Meter(Index).DgnProtocol.DgnPras;
            if (!DgnPram.ContainsKey(PramKey))
            {
                return new string[0];
            }
            return DgnPram[PramKey].Split('|');
        }


    }
}
