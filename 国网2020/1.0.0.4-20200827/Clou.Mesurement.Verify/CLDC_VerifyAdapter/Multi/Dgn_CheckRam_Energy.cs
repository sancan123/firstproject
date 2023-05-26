
using System;
using CLDC_DataCore;
using System.Collections.Generic;
using System.Threading;
using CLDC_DataCore.Const;
//////using ClInterface;
//using ClAmMeterController;
using CLDC_Comm.Enum;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
//using ClInterface;

namespace CLDC_VerifyAdapter.Multi
{
    class Dgn_CheckRam_Energy : DgnBase
    {

        public Dgn_CheckRam_Energy(object plan)
            : base(plan)
        { }

        /// <summary>
        /// 重写基类函数
        /// </summary>
        /// <param name="ItemNumber">检定序号</param>
        public override void Verify()
        {
            base.Verify();

            if (!PowerOn())
            {
                MessageController.Instance.AddMessage("源输出失败", 6, 2);
                return;
            }
            if (Stop)
            {
                return;
            }
            /*功能参数:1+3|1|2|3|4*/
            string[] arrStrResultKey = new string[BwCount];
            List<float[]> lstWugongDl = new List<float[]>();

            string[] para = this.PrjPara;
            if (para.Length != 5) return;
            string[] JiShuanGongShi = para[0].Split('+');
            for (int i = 0; i < BwCount; i++)
            {
                float[] tmp = new float[5];
                CLDC_DataCore.Function.Common.Memset(ref tmp, -1F);
                lstWugongDl.Add(tmp);
            }
            if (Stop)
            {
                return;
            }
            //if (JiShuanGongShi.Length != 2) return;
            //第一步：读取电量
            //if (!Adapter.Adpater485.ReadEnergy(enmPDirectType.正向无功, enmTariffType.总))
            //{
            //    CatchException(new
            //        Exception("读取被检表电量失败"));
            //    return;
            //}
            //FillWuGongDl(0, ref lstWugongDl);
            ////读取四个相限电量
            //for (int k = 1; k < 5; k++)
            //{
            //    int XiangXian = k + 3;
            //    enmPDirectType PDType = (enmPDirectType)XiangXian;
            //    if (!Adapter.Adpater485.ReadEnergy(PDType))
            //    {
            //        CatchException(new Exception("读取" + PDType.ToString() + "电量失败"));
            //        return;
            //    }
            //    FillWuGongDl(k, ref lstWugongDl);
            //}
            //开始运行
            MessageController.Instance.AddMessage("开始按各象限输出...");
            double[] phi = { 80, 110, 260, 280 };
            for (int r = 0; r < 4; r++)
            {
                if (Stop)
                {
                    return;
                }
                if (!runXiangXian(r + 1, float.Parse(para[r + 1]), phi[r]))
                    break;
            }
            //if (!Adapter.ComAdpater.PowerOnOnlyU())
            //{
            //    MessageController.Instance.AddMessage("控制源输出失败");
            //    return;
            //}


            MessageController.Instance.AddMessage("读取试验后电量");
            //读取正向无功及反向无功电量
            //Dictionary<int, float[]> dicEnergy = new Dictionary<int, float[]>();
            byte energytype = 2;
            byte tari = 0;
           float[] arrEnergy = MeterProtocolAdapter.Instance.ReadEnergy(energytype,tari);
            //bool ret = Helper.Rs485Helper.Instance.ReadEnergy(1 , 5, ref dicEnergy);
            //if (!Adapter.Adpater485.ReadEnergy(enmPDirectType.正向无功, enmTariffType.总))
            //if (!ret)
            //{
            //    CatchException(new
            //        Exception("读取被检表电量失败"));
            //    return;
            //}
            FillWuGongDl(0, ref lstWugongDl, arrEnergy);   //正向无功总
            //读取四个相限试验后电量
            for (int k = 0; k < 4; k++)
            {
                if (Stop)
                {
                    return;
                }
                int XiangXian = k + 4;
                //enmPDirectType PDType = (enmPDirectType)XiangXian;
                //if (!Adapter.Adpater485.ReadEnergy(PDType))
                arrEnergy = MeterProtocolAdapter.Instance.ReadEnergy((byte)XiangXian,0x00);
                //if(!Helper.Rs485Helper.Instance.ReadEnergy((byte)XiangXian , 0,ref dicEnergy))
                //{
                   // Comm.MessageController.Instance.AddMessage("读取" + PDType.ToString() + "电量失败");
                   // return;
                //}
                FillWuGongDl(k + 1, ref lstWugongDl, arrEnergy);
            }

            //关闭
            Helper.EquipHelper.Instance.PowerOff();
            //Adapter.ComAdpater.PowerOff();
            MessageController.Instance.AddMessage("正在计算结果");
            string strKey = ItemKey;
            for (int n = 0; n < BwCount; n++)
            {
                float z = lstWugongDl[n][0];
                float az = 0F;
                for (int j = 0; j < JiShuanGongShi.Length; j++)
                {
                    az += lstWugongDl[n][int.Parse(JiShuanGongShi[j])];
                }
                MeterBasicInfo curMeter = Helper.MeterDataHelper.Instance.Meter(n);
                MeterDgn curResult;
                if (!curMeter.MeterDgns.ContainsKey(strKey))
                {
                    curResult = new MeterDgn();
                    curResult.Md_PrjID = strKey;
                    curResult.Md_PrjName = Cus_DgnItem.电量寄存器检查.ToString();
                    curMeter.MeterDgns.Add(strKey, curResult);
                }
                else
                {
                    curResult = curMeter.MeterDgns[strKey];
                }
                arrStrResultKey[n] = ItemKey;
                if (z - az < 0.02)
                    curResult.Md_chrValue = Variable.CTG_HeGe;
                else
                    curResult.Md_chrValue = Variable.CTG_BuHeGe;
                //出结果
                //Comm.MessageArgs.DgnArgs _message = new Comm.MessageArgs.DgnArgs();
                //_message.BW = n;
                //_message.DngResult = curResult;
                //RaiseVerifyData(_message);
                
                
            }
         //   GlobalUnit.g_CUS.SaveTempDB(Cus_DBTableName.METER_COMMUNICATION, arrStrResultKey);
            

        }

        

        //填充电量
        private void FillWuGongDl(int Desc, ref List<float[]> lstDl, float[] arrEnergy)
        {
            /*
             [0] --------------正向无功前总
             [1]---------------正向无功前一
             [2]---------------正向无功前二
             * [3]---------------正向无功前三
             * [4]---------------正向无功前四
             */
            string strKey = ((int)Cus_DgnItem.读取电量).ToString("D3") + "09";    //电量储存在无功电量中
            //string strKey2 = string.Empty;
            for (int k = 0; k < BwCount; k++)
            {
               // strKey2 = k.ToString();
                MeterBasicInfo meterInfo = Helper.MeterDataHelper.Instance.Meter(k);
                if (!meterInfo.YaoJianYn)                   //不检定的表直接路过
                    continue;
                //if (!dicEnergy.ContainsKey(k))
                //    continue;
                MeterDgn curResult;

                lstDl[k][Desc] = arrEnergy[k];
                if (!meterInfo.MeterDgns.ContainsKey(strKey))
                {
                    curResult = new MeterDgn();
                    curResult.Md_PrjID = strKey;
                    curResult.Md_PrjName = "电量";
                    meterInfo.MeterDgns.Add(strKey, curResult);
                }
                else
                {
                    curResult = meterInfo.MeterDgns[strKey];
                }

                string strTmpValue = "";

                for (int h = 0; h < lstDl[k].Length; h++)
                {
                    if (lstDl[k][h] > 0)
                        strTmpValue += lstDl[k][h].ToString("") + "|";
                    else
                        strTmpValue += "|";
                }
                //去掉最后一个|
                strTmpValue = strTmpValue.Substring(0, strTmpValue.Length - 1);
                curResult.Md_chrValue = strTmpValue;
                
                
                //Thread.Sleep(200);
            }
            
        }
        /// <summary>
        /// 分象限输出
        /// </summary>
        /// <param name="xianXian">第几象限</param>
        /// <param name="runTime">需要运行时长(分)</param>
        /// <param name="Phi">相位角度</param>
        /// <returns></returns>

        private bool runXiangXian(int xianXian, float runTime, double Phi)
        {
            int runSeconds = (int)runTime * 60;
            if (GlobalUnit.IsDemo) runSeconds /= 10;
            string glys = Math.Sin(Phi).ToString();
            
            DateTime startTime = DateTime.Now;
            bool bResult = Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, _xIb * GlobalUnit.Ib, (int)Cus_PowerYuanJian.H, (int)PowerFangXiang, glys, IsYouGong, false);
            //bool bResult = Adapter.ComAdpater.SetTestPoint(CLDC_DataCore.Const.GlobalUnit.U, CLDC_DataCore.Const.GlobalUnit.Imax, FangXiangStr + glys, Cus_PowerYuanJian.H, IsP);
            if (!bResult) return false;
            //Check.Require(bResult, string.Format("第{0}象限控制源输出失败" + Adapter.ComAdpater.m_IComAdpater.LostMessage, xianXian));
            string strDes = string.Empty;
            while (true)
            {
                Thread.Sleep(1000);
                int pastTime = CLDC_DataCore.Function.DateTimes.DateDiff(startTime);
                if (pastTime >= runSeconds)
                    break;
                strDes = string.Format("正在累计第{0}象限电量，需要{1}分，已经运行{2}分", xianXian, runTime, (pastTime / 60F).ToString ("F2"));
                MessageController.Instance.AddMessage(strDes);
            }
            return true;
        }
        /// <summary>
        /// 清理数据
        /// </summary>
        protected override void ClearItemData()
        {


            base.ClearItemData();
        }
    }
}
