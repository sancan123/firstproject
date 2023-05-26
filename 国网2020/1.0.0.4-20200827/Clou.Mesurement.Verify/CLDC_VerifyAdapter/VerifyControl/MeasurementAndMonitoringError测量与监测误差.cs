using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CLDC_DataCore;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_DataCore.Function;
using CLDC_DataCore.Const;
using CLDC_Comm.Enum;
using CLDC_VerifyAdapter.Helper;

namespace CLDC_VerifyAdapter
{
    class MeasurementAndMonitoringError:VerifyBase
    {


           #region ----------构造函数----------

        public MeasurementAndMonitoringError(object plan)
            : base(plan)
        {
        }

        protected override string ResultKey
        {

            //get { throw new NotImplementedException(); }
            get { return null; }
        }

        protected override string ItemKey
        {
            //get { throw new NotImplementedException(); }
            get { return null; }
        }


        protected override bool CheckPara()
        {
            ResultNames = new string[] { "测试时间", "A相电压(测-标-误差)", "A相电流", "B相电压", "B相电流",
                                         "C相电压","C相电流","总有功功率","总无功功率","A相有功功率",
                                         "B相有功功率","C相有功功率","A相无功功率","B相无功功率","C相无功功率",
                                         "功率因数","频率", "结论", "不合格原因" };
            return true;
        }

        #endregion                
        public override void Verify()
        {
            base.Verify();
         
           bool[] Result = new bool[BwCount];
           string[] Fail = new string[BwCount];
           string[] array = VerifyPara.Split('|');         
           int GLYJ = 1;
           string glys = "1.0";
           float testI = 0;
           float testU= 0;
           float pl = 50;
           bool isyougong = true;
           Cus_PowerFangXiang glfx = Cus_PowerFangXiang.正向有功;
           if (!string.IsNullOrEmpty(array[0]))
           {
               switch (array[0])
               {
                   case "正向有功":
                       isyougong = true;
                       glfx = Cus_PowerFangXiang.正向有功;
                       break;
                   case "正向无功":
                       isyougong = false;
                       glfx = Cus_PowerFangXiang.正向无功;
                       break;
                   case "反向有功":
                       isyougong = true;
                       glfx = Cus_PowerFangXiang.反向有功;
                       break;
                   case "反向无功":
                       isyougong = false;
                       glfx = Cus_PowerFangXiang.反向无功;
                       break;
                

               }
           }
           else
           {
               System.Windows.Forms.MessageBox.Show("请检查方案参数是否正确");
           }

           if (!string.IsNullOrEmpty(array[1]))
           {
               switch (array[1])
               {
                   case "H":
                       GLYJ = 1;
                       break;
                   case "A":
                       GLYJ = 2;
                       break;
                   case "B":
                       GLYJ = 3;
                       break;
                   case "C":
                       GLYJ = 4;
                       break;
                   default:
                       GLYJ = 1;
                       break;


               }
           }
           else
           {
               System.Windows.Forms.MessageBox.Show("请检查方案参数是否正确");
           }

           if (!string.IsNullOrEmpty(array[2]))
           {
               glys = array[2];
           }
           else
           {
               System.Windows.Forms.MessageBox.Show("请检查方案参数是否正确");
           }
           if (!string.IsNullOrEmpty(array[3]))
           {
               testU = (float.Parse(array[3].ToString().Trim('%')) / 100 ) *GlobalUnit.U;
           }
           else
           {
               System.Windows.Forms.MessageBox.Show("请检查方案参数是否正确");
           }

           if (!string.IsNullOrEmpty(array[4]))
           {
               testI = Number.GetCurrentByIb(array[4], Helper.MeterDataHelper.Instance.Meter(GlobalUnit.FirstYaoJianMeter).Mb_chrIb, Helper.MeterDataHelper.Instance.Meter(GlobalUnit.FirstYaoJianMeter).Mb_BlnHgq);
           }
           else
           {
               System.Windows.Forms.MessageBox.Show("请检查方案参数是否正确");
           }

           if (!string.IsNullOrEmpty(array[5]))
           {
               pl = float.Parse(array[5]);
           }
           else
           {
               System.Windows.Forms.MessageBox.Show("请检查方案参数是否正确");
           }
           bool bPowerOn = PowerOn();
           MeterProtocolAdapter.Instance.SouthCheckBlueToothIdentity();

           MessageController.Instance.AddMessage("正在升源");

           if (Helper.EquipHelper.Instance.PowerOn(testU, testU, testU, testI, testI, testI, GLYJ, pl, glys, isyougong, false, false, (int)glfx) == false)
           {
               //Stop = true;
               //return;
           }
           ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 20);

           string[] result = new string[BwCount];
           string[] resultB = new string[BwCount];
           string[] resultE = new string[BwCount];
           float[] Error = new float[BwCount];
           for (int i = 0; i < BwCount; i++)
           {
               if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
               {                                    
                       ResultDictionary["结论"][i] = "合格";                 
               }
           }



            MessageController.Instance.AddMessage("正在读取A相电压");
            float[] A_U= MeterProtocolAdapter.Instance.ReadData("02010100", 2, 1);

            float A_UB = GlobalUnit.g_StrandMeterU[0];

           
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (A_U[i] == -1 || A_U[i] == 0)
                    {
                        result[i] = "A相电压读取失败";
                        ResultDictionary["结论"][i] = "不合格";
                        ResultDictionary["A相电压(测-标-误差)"][i] = A_U[i].ToString() + "|" + A_UB.ToString() + "|" + "-999";
                    }
                    else if (A_UB == 0)
                    {
                        resultB[i] = "A相标准电压读取失败";
                        ResultDictionary["结论"][i] = "不合格";
                        ResultDictionary["A相电压(测-标-误差)"][i] = A_U[i].ToString() + "|" + A_UB.ToString() + "|" + "-999";
                    }
                    else
                    {
                        Error[i] = (A_U[i] - A_UB) / A_UB;

                        if (Error[i] > 1)
                        {
                            resultE[i] = "A相电压误差超差";
                            ResultDictionary["结论"][i] = "不合格";
                            ResultDictionary["A相电压(测-标-误差)"][i] = A_U[i].ToString() + "|" + A_UB.ToString() + "|" + Error[i].ToString("0.0000");
                        }
                        else
                        {
                            ResultDictionary["A相电压(测-标-误差)"][i] = A_U[i].ToString() + "|" + A_UB.ToString() + "|" + Error[i].ToString("0.0000");
                        }
                    }                   
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "A相电压(测-标-误差)", ResultDictionary["A相电压(测-标-误差)"]);

            MessageController.Instance.AddMessage("正在读取A相电流");
            float[] A_I = MeterProtocolAdapter.Instance.ReadData("02020100", 3, 3);

            float A_IB = GlobalUnit.g_StrandMeterI[0];


            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (A_I[i] == -1 || A_I[i] == 0)
                    {
                        result[i] = "A相电流读取失败";
                        ResultDictionary["结论"][i] = "不合格";
                        ResultDictionary["A相电流"][i] = A_I[i].ToString() + "|" + A_IB.ToString() + "|" + "-999";
                    }
                    else if (A_IB == 0)
                    {
                        resultB[i] = "A相标准电流读取失败";
                        ResultDictionary["结论"][i] = "不合格";
                        ResultDictionary["A相电流"][i] = A_I[i].ToString() + "|" + A_IB.ToString() + "|" + "-999";
                    }
                    else
                    {
                        Error[i] = (A_I[i] - A_IB) / A_UB;

                        if (Error[i] > 1)
                        {
                            resultE[i] = "A相电流误差超差";
                            ResultDictionary["结论"][i] = "不合格";
                            ResultDictionary["A相电流"][i] = A_I[i].ToString() + "|" + A_IB.ToString() + "|" + Error[i].ToString("0.0000");
                        }
                        else
                        {
                            ResultDictionary["A相电流"][i] = A_I[i].ToString() + "|" + A_IB.ToString() + "|" + Error[i].ToString("0.0000");
                        }
                    }
                }
            }

            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "A相电流", ResultDictionary["A相电流"]);





            if (glfx == Cus_PowerFangXiang.正向有功 || glfx == Cus_PowerFangXiang.反向有功)
            {
           
            MessageController.Instance.AddMessage("正在读取总有功功率");
            float[] P_P = MeterProtocolAdapter.Instance.ReadData("02030000", 3, 1);

            float P_PP = GlobalUnit.g_StrandMeterP[0];


            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (P_P[i] == -1 || P_P[i] == 0)
                    {
                        result[i] = "总有功功率读取失败";
                        ResultDictionary["结论"][i] = "不合格";
                        ResultDictionary["总有功功率"][i] = P_P[i].ToString() + "|" + P_PP.ToString() + "|" + "-999";
                    }
                    else if (P_PP == 0)
                    {
                        resultB[i] = "标准总有功功率读取失败";
                        ResultDictionary["结论"][i] = "不合格";
                        ResultDictionary["总有功功率"][i] = P_P[i].ToString() + "|" + P_PP.ToString() + "|" + "-999";
                    }
                    else
                    {
                        Error[i] = (P_P[i] - P_PP) / P_PP;

                        if (Error[i] > 1)
                        {
                            resultE[i] = "总有功功率误差超差";
                            ResultDictionary["结论"][i] = "不合格";
                            ResultDictionary["总有功功率"][i] = P_P[i].ToString() + "|" + P_PP.ToString() + "|" + Error[i].ToString("0.0000");
                        }
                        else
                        {
                            ResultDictionary["总有功功率"][i] = P_P[i].ToString() + "|" + P_PP.ToString() + "|" + Error[i].ToString("0.0000");
                        }
                    }
                }
            }

            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "总有功功率", ResultDictionary["总有功功率"]);



            MessageController.Instance.AddMessage("正在读取A相有功功率");
            float[] P_A = MeterProtocolAdapter.Instance.ReadData("02030100", 3, 1);

            float P_PA = GlobalUnit.g_StrandMeterPFY[0];


            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (P_A[i] == -1 || P_A[i] == 0)
                    {
                        result[i] = "A相有功功率读取失败";
                        ResultDictionary["结论"][i] = "不合格";
                        ResultDictionary["A相有功功率"][i] = P_A[i].ToString() + "|" + P_PA.ToString() + "|" + "-999";
                    }
                    else if (P_PA == 0)
                    {
                        resultB[i] = "标准A相有功功率读取失败";
                        ResultDictionary["结论"][i] = "不合格";
                        ResultDictionary["A相有功功率"][i] = P_A[i].ToString() + "|" + P_PA.ToString() + "|" + "-999";
                    }
                    else
                    {
                        Error[i] = (P_A[i] - P_PA) / P_PA;

                        if (Error[i] > 1)
                        {
                            resultE[i] = "A相有功功率误差超差";
                            ResultDictionary["结论"][i] = "不合格";
                            ResultDictionary["A相有功功率"][i] = P_A[i].ToString() + "|" + P_PA.ToString() + "|" + Error[i].ToString("0.0000");
                        }
                        else
                        {
                            ResultDictionary["A相有功功率"][i] = P_A[i].ToString() + "|" + P_PA.ToString() + "|" + Error[i].ToString("0.0000");
                        }
                    }
                }
            }

            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "A相有功功率", ResultDictionary["A相有功功率"]);


            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    ResultDictionary["总无功功率"][i] = "0|0|0";
                    ResultDictionary["A相无功功率"][i] = "0|0|0";
                   
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "总无功功率", ResultDictionary["总无功功率"]);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 1);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "A相无功功率", ResultDictionary["A相无功功率"]);
            
            
            }
            else
            {

                for (int i = 0; i < BwCount; i++)
                {
                    if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                    {
                        ResultDictionary["总有功功率"][i] = "0|0|0";
                        ResultDictionary["A相有功功率"][i] = "0|0|0";

                    }
                }
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "总有功功率", ResultDictionary["总有功功率"]);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 1);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "A相有功功率", ResultDictionary["A相有功功率"]);


            MessageController.Instance.AddMessage("正在读取总无功功率");
            float[] Q_Q = MeterProtocolAdapter.Instance.ReadData("02040000", 3, 1);

            float Q_QQ = GlobalUnit.g_StrandMeterP[1];

            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (Q_Q[i] == -1 || Q_Q[i] == 0)
                    {
                        result[i] = "总无功功率读取失败";
                        ResultDictionary["结论"][i] = "不合格";
                        ResultDictionary["总无功功率"][i] = Q_Q[i].ToString() + "|" + Q_QQ.ToString() + "|" + "-999";
                    }
                    else if (Q_QQ == 0)
                    {
                        resultB[i] = "标准总无功功率读取失败";
                        ResultDictionary["结论"][i] = "不合格";
                        ResultDictionary["总无功功率"][i] = Q_Q[i].ToString() + "|" + Q_QQ.ToString() + "|" + "-999";
                    }
                    else
                    {
                        Error[i] = (Q_Q[i] - Q_QQ) / Q_QQ;

                        if (Error[i] > 1)
                        {
                            resultE[i] = "总无功功率误差超差";
                            ResultDictionary["结论"][i] = "不合格";
                            ResultDictionary["总无功功率"][i] = Q_Q[i].ToString() + "|" + Q_QQ.ToString() + "|" + Error[i].ToString("0.0000");
                        }
                        else
                        {
                            ResultDictionary["总无功功率"][i] = Q_Q[i].ToString() + "|" + Q_QQ.ToString() + "|" + Error[i].ToString("0.0000");
                        }
                    }
                }
            }

            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "总无功功率", ResultDictionary["总无功功率"]);



            MessageController.Instance.AddMessage("正在读取A相无功功率");
            float[] Q_A = MeterProtocolAdapter.Instance.ReadData("02040100", 3, 1);

            float Q_QA = GlobalUnit.g_StrandMeterQFY[0];

            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (Q_A[i] == -1 || Q_A[i] == 0)
                    {
                        result[i] = "A相无功功率读取失败";
                        ResultDictionary["结论"][i] = "不合格";
                        ResultDictionary["A相无功功率"][i] = Q_A[i].ToString() + "|" + Q_QA.ToString() + "|" + "-999";
                    }
                    else if (Q_QA == 0)
                    {
                        resultB[i] = "标准A相无功功率读取失败";
                        ResultDictionary["结论"][i] = "不合格";
                        ResultDictionary["A相无功功率"][i] = Q_A[i].ToString() + "|" + Q_QA.ToString() + "|" + "-999";
                    }
                    else
                    {
                        Error[i] = (Q_A[i] - Q_QA) / Q_QA;

                        if (Error[i] > 1)
                        {
                            resultE[i] = "A相无功功率误差超差";
                            ResultDictionary["结论"][i] = "不合格";
                            ResultDictionary["A相无功功率"][i] = Q_A[i].ToString() + "|" + Q_QA.ToString() + "|" + Error[i].ToString("0.0000");
                        }
                        else
                        {
                            ResultDictionary["A相无功功率"][i] = Q_A[i].ToString() + "|" + Q_QA.ToString() + "|" + Error[i].ToString("0.0000");
                        }
                    }
                }
            }

            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "A相无功功率", ResultDictionary["A相无功功率"]);


            }
            MessageController.Instance.AddMessage("正在读取功率因数");
            float[] GLYS = MeterProtocolAdapter.Instance.ReadData("02060000", 2, 3);

            string GLYSB = glys;

            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (GLYS[i] == -1 || GLYS[i] == 0)
                    {
                        result[i] = "功率因数读取失败";
                        ResultDictionary["结论"][i] = "不合格";
                        ResultDictionary["功率因数"][i] = GLYS[i].ToString() + "|" + GLYSB + "|" + "-999";
                    }
                   
                    else
                    {
                       // Error[i] = (Q_A[i] - Q_QA) / Q_QA;


                        ResultDictionary["功率因数"][i] = GLYS[i].ToString() + "|" + GLYSB + "|";
                       
                    }
                }
            }

            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "功率因数", ResultDictionary["功率因数"]);


            MessageController.Instance.AddMessage("正在读取频率");
            float[] PL = MeterProtocolAdapter.Instance.ReadData("02800002", 2, 2);

            float PLB = pl;

            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (PL[i] == -1 || PL[i] == 0)
                    {
                        result[i] = "频率读取失败";
                        ResultDictionary["结论"][i] = "不合格";
                        ResultDictionary["频率"][i] = PL[i].ToString() + "|" + PLB.ToString() + "|" + "-999";
                    }
                    else if (PLB == 0)
                    {
                        resultB[i] = "标准频率读取失败";
                        ResultDictionary["结论"][i] = "不合格";
                        ResultDictionary["频率"][i] = PL[i].ToString() + "|" + PLB.ToString() + "|" + "-999";
                    }
                    else
                    {
                        Error[i] = (PL[i] - PLB) / PLB;

                        if (Error[i] > 1)
                        {
                            resultE[i] = "频率误差超差";
                            ResultDictionary["结论"][i] = "不合格";
                            ResultDictionary["频率"][i] = PL[i].ToString() + "|" + PLB.ToString() + "|" + Error[i].ToString("0.0000");
                        }
                        else
                        {
                            ResultDictionary["频率"][i] = PL[i].ToString() + "|" + PLB.ToString() + "|" + Error[i].ToString("0.0000");
                        }
                    }
                }
            }

            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "频率", ResultDictionary["频率"]);



            if (GlobalUnit.clfs != Cus_Clfs.单相)
            {
                if (GlobalUnit.clfs != Cus_Clfs.三相三线)
                {
               
            MessageController.Instance.AddMessage("正在读取B相电压");
            float[] B_U = MeterProtocolAdapter.Instance.ReadData("02010200", 2, 1);

            float B_UB = GlobalUnit.g_StrandMeterU[1];

            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (B_U[i] == -1 || B_U[i] == 0)
                    {
                        result[i] = "B相电压读取失败";
                        ResultDictionary["结论"][i] = "不合格";
                        ResultDictionary["B相电压"][i] = B_U[i].ToString() + "|" + B_UB.ToString() + "|" + "-999";
                    }
                    else if (PLB == 0)
                    {
                        resultB[i] = "标准B相电压读取失败";
                        ResultDictionary["结论"][i] = "不合格";
                        ResultDictionary["B相电压"][i] = B_U[i].ToString() + "|" + B_UB.ToString() + "|" + "-999";
                    }
                    else
                    {
                        Error[i] = (B_U[i] - B_UB) / B_UB;

                        if (Error[i] > 1)
                        {
                            resultE[i] = "B相电压误差超差";
                            ResultDictionary["结论"][i] = "不合格";
                            ResultDictionary["B相电压"][i] = B_U[i].ToString() + "|" + B_UB.ToString() + "|" + Error[i].ToString("0.0000");
                        }
                        else
                        {
                            ResultDictionary["B相电压"][i] = B_U[i].ToString() + "|" + B_UB.ToString() + "|" + Error[i].ToString("0.0000");
                        }
                    }
                }
            }

            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "B相电压", ResultDictionary["B相电压"]);
                }
                else
                {
                    for (int i = 0; i < BwCount; i++)
                    {
                        if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                        {
                            ResultDictionary["B相电压"][i] = "0|0|0";
                          

                        }
                    }
                    MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "B相电压", ResultDictionary["B相电压"]);
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 1);
                }

            MessageController.Instance.AddMessage("正在读取C相电压");
            float[] C_U = MeterProtocolAdapter.Instance.ReadData("02010300", 2, 1);

            float C_UB = GlobalUnit.g_StrandMeterU[2];
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (C_U[i] == -1 || C_U[i] == 0)
                    {
                        result[i] = "C相电压读取失败";
                        ResultDictionary["结论"][i] = "不合格";
                        ResultDictionary["C相电压"][i] = C_U[i].ToString() + "|" + C_UB.ToString() + "|" + "-999";
                    }
                    else if (C_UB == 0)
                    {
                        resultB[i] = "标准C相电压读取失败";
                        ResultDictionary["结论"][i] = "不合格";
                        ResultDictionary["C相电压"][i] = C_U[i].ToString() + "|" + C_UB.ToString() + "|" + "-999";
                    }
                    else
                    {
                        Error[i] = (C_U[i] - C_UB) / C_UB;

                        if (Error[i] > 1)
                        {
                            resultE[i] = "C相电压误差超差";
                            ResultDictionary["结论"][i] = "不合格";
                            ResultDictionary["C相电压"][i] = C_U[i].ToString() + "|" + C_UB.ToString() + "|" + Error[i].ToString("0.0000");
                        }
                        else
                        {
                            ResultDictionary["C相电压"][i] = C_U[i].ToString() + "|" + C_UB.ToString() + "|" + Error[i].ToString("0.0000");
                        }
                    }
                }
            }

            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "C相电压", ResultDictionary["C相电压"]);

            if (GlobalUnit.clfs != Cus_Clfs.三相三线)
            {


                MessageController.Instance.AddMessage("正在读取B相电流");
                float[] B_I = MeterProtocolAdapter.Instance.ReadData("02020200", 3, 3);

                float B_IB = GlobalUnit.g_StrandMeterI[1];

                for (int i = 0; i < BwCount; i++)
                {
                    if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                    {
                        if (B_I[i] == -1 || B_I[i] == 0)
                        {
                            result[i] = "B相电流读取失败";
                            ResultDictionary["结论"][i] = "不合格";
                            ResultDictionary["B相电流"][i] = B_I[i].ToString() + "|" + B_IB.ToString() + "|" + "-999";
                        }
                        else if (B_IB == 0)
                        {
                            resultB[i] = "标准B相电流读取失败";
                            ResultDictionary["结论"][i] = "不合格";
                            ResultDictionary["B相电流"][i] = B_I[i].ToString() + "|" + B_IB.ToString() + "|" + "-999";
                        }
                        else
                        {
                            Error[i] = (B_I[i] - B_IB) / B_IB;

                            if (Error[i] > 1)
                            {
                                resultE[i] = "B相电流误差超差";
                                ResultDictionary["结论"][i] = "不合格";
                                ResultDictionary["B相电流"][i] = B_I[i].ToString() + "|" + B_IB.ToString() + "|" + Error[i].ToString("0.0000");
                            }
                            else
                            {
                                ResultDictionary["B相电流"][i] = B_I[i].ToString() + "|" + B_IB.ToString() + "|" + Error[i].ToString("0.0000");
                            }
                        }
                    }
                }

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "B相电流", ResultDictionary["B相电流"]);

            }
            else
            {
                for (int i = 0; i < BwCount; i++)
                {
                    if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                    {
                        ResultDictionary["B相电流"][i] = "0|0|0";


                    }
                }
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "B相电流", ResultDictionary["B相电流"]);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 1);
            }
            MessageController.Instance.AddMessage("正在读取C相电流");
            float[] C_I= MeterProtocolAdapter.Instance.ReadData("02020300",3, 3);

            float C_IB = GlobalUnit.g_StrandMeterI[2];
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (C_I[i] == -1 || C_I[i] == 0)
                    {
                        result[i] = "C相电流读取失败";
                        ResultDictionary["结论"][i] = "不合格";
                        ResultDictionary["C相电流"][i] = C_I[i].ToString() + "|" + C_IB.ToString() + "|" + "-999";
                    }
                    else if (C_IB == 0)
                    {
                        resultB[i] = "标准C相电流读取失败";
                        ResultDictionary["结论"][i] = "不合格";
                        ResultDictionary["C相电流"][i] = C_I[i].ToString() + "|" + C_IB.ToString() + "|" + "-999";
                    }
                    else
                    {
                        Error[i] = (C_I[i] - C_IB) / C_IB;

                        if (Error[i] > 1)
                        {
                            resultE[i] = "C相电流误差超差";
                            ResultDictionary["结论"][i] = "不合格";
                            ResultDictionary["C相电流"][i] = C_I[i].ToString() + "|" + C_IB.ToString() + "|" + Error[i].ToString("0.0000");
                        }
                        else
                        {
                            ResultDictionary["C相电流"][i] = C_I[i].ToString() + "|" + C_IB.ToString() + "|" + Error[i].ToString("0.0000");
                        }
                    }
                }
            }

            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "C相电流", ResultDictionary["C相电流"]);


            if (glfx == Cus_PowerFangXiang.正向有功 || glfx == Cus_PowerFangXiang.反向有功)
            {
                if (GlobalUnit.clfs != Cus_Clfs.三相三线)
                {

                    MessageController.Instance.AddMessage("正在读取B相有功功率");
                    float[] P_B = MeterProtocolAdapter.Instance.ReadData("02030200", 3, 1);

                    float P_PB = GlobalUnit.g_StrandMeterPFY[1];

                    for (int i = 0; i < BwCount; i++)
                    {
                        if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                        {
                            if (P_B[i] == -1 || P_B[i] == 0)
                            {
                                result[i] = "B相有功功率读取失败";
                                ResultDictionary["结论"][i] = "不合格";
                                ResultDictionary["B相有功功率"][i] = P_B[i].ToString() + "|" + P_PB.ToString() + "|" + "-999";
                            }
                            else if (P_PB == 0)
                            {
                                resultB[i] = "标准B相有功功率读取失败";
                                ResultDictionary["结论"][i] = "不合格";
                                ResultDictionary["B相有功功率"][i] = P_B[i].ToString() + "|" + P_PB.ToString() + "|" + "-999";
                            }
                            else
                            {
                                Error[i] = (P_B[i] - P_PB) / P_PB;

                                if (Error[i] > 1)
                                {
                                    resultE[i] = "B相有功功率误差超差";
                                    ResultDictionary["结论"][i] = "不合格";
                                    ResultDictionary["B相有功功率"][i] = P_B[i].ToString() + "|" + P_PB.ToString() + "|" + Error[i].ToString("0.0000");
                                }
                                else
                                {
                                    ResultDictionary["B相有功功率"][i] = P_B[i].ToString() + "|" + P_PB.ToString() + "|" + Error[i].ToString("0.0000");
                                }
                            }
                        }
                    }

                    MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "B相有功功率", ResultDictionary["B相有功功率"]);
                }
                else
                {
                    for (int i = 0; i < BwCount; i++)
                    {
                        if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                        {
                            ResultDictionary["B相有功功率"][i] = "0|0|0";


                        }
                    }
                    MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "B相有功功率", ResultDictionary["B相有功功率"]);
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 1);
                }

                MessageController.Instance.AddMessage("正在读取C相有功功率");
                float[] P_C = MeterProtocolAdapter.Instance.ReadData("02030300", 3, 1);

                float P_PC = GlobalUnit.g_StrandMeterPFY[2];

                for (int i = 0; i < BwCount; i++)
                {
                    if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                    {
                        if (P_C[i] == -1 || P_C[i] == 0)
                        {
                            result[i] = "C相有功功率读取失败";
                            ResultDictionary["结论"][i] = "不合格";
                            ResultDictionary["C相有功功率"][i] = P_C[i].ToString() + "|" + P_PC.ToString() + "|" + "-999";
                        }
                        else if (P_PC == 0)
                        {
                            resultB[i] = "标准C相有功功率读取失败";
                            ResultDictionary["结论"][i] = "不合格";
                            ResultDictionary["C相有功功率"][i] = P_C[i].ToString() + "|" + P_PC.ToString() + "|" + "-999";
                        }
                        else
                        {
                            Error[i] = (P_C[i] - P_PC) / P_PC;

                            if (Error[i] > 1)
                            {
                                resultE[i] = "C相有功功率误差超差";
                                ResultDictionary["结论"][i] = "不合格";
                                ResultDictionary["C相有功功率"][i] = P_C[i].ToString() + "|" + P_PC.ToString() + "|" + Error[i].ToString("0.0000");
                            }
                            else
                            {
                                ResultDictionary["C相有功功率"][i] = P_C[i].ToString() + "|" + P_PC.ToString() + "|" + Error[i].ToString("0.0000");
                            }
                        }
                    }
                }

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "C相有功功率", ResultDictionary["C相有功功率"]);



                for (int i = 0; i < BwCount; i++)
                {
                    if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                    {
                        ResultDictionary["B相无功功率"][i] = "0|0|0";
                        ResultDictionary["C相无功功率"][i] = "0|0|0";

                    }
                }
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "B相无功功率", ResultDictionary["B相无功功率"]);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 1);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "C相无功功率", ResultDictionary["C相无功功率"]);
            



            }
            else
            {

                for (int i = 0; i < BwCount; i++)
                {
                    if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                    {
                        ResultDictionary["B相有功功率"][i] = "0|0|0";
                        ResultDictionary["C相有功功率"][i] = "0|0|0";

                    }
                }
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "B相有功功率", ResultDictionary["B相有功功率"]);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 1);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "C相有功功率", ResultDictionary["C相有功功率"]);

                if (GlobalUnit.clfs != Cus_Clfs.三相三线)
                {


                    MessageController.Instance.AddMessage("正在读取B相无功功率");
                    float[] Q_B = MeterProtocolAdapter.Instance.ReadData("02040200", 3, 1);

                    float Q_QB = GlobalUnit.g_StrandMeterQFY[1];

                    for (int i = 0; i < BwCount; i++)
                    {
                        if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                        {
                            if (Q_B[i] == -1 || Q_B[i] == 0)
                            {
                                result[i] = "B相无功功率读取失败";
                                ResultDictionary["结论"][i] = "不合格";
                                ResultDictionary["B相无功功率"][i] = Q_B[i].ToString() + "|" + Q_QB.ToString() + "|" + "-999";
                            }
                            else if (Q_QB == 0)
                            {
                                resultB[i] = "标准B相无功功率读取失败";
                                ResultDictionary["结论"][i] = "不合格";
                                ResultDictionary["B相无功功率"][i] = Q_B[i].ToString() + "|" + Q_QB.ToString() + "|" + "-999";
                            }
                            else
                            {
                                Error[i] = (Q_B[i] - Q_QB) / Q_QB;

                                if (Error[i] > 1)
                                {
                                    resultE[i] = "B相无功功率误差超差";
                                    ResultDictionary["结论"][i] = "不合格";
                                    ResultDictionary["B相无功功率"][i] = Q_B[i].ToString() + "|" + Q_QB.ToString() + "|" + Error[i].ToString("0.0000");
                                }
                                else
                                {
                                    ResultDictionary["B相无功功率"][i] = Q_B[i].ToString() + "|" + Q_QB.ToString() + "|" + Error[i].ToString("0.0000");
                                }
                            }
                        }
                    }

                    MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "B相无功功率", ResultDictionary["B相无功功率"]);

                }
                else
                {
                    for (int i = 0; i < BwCount; i++)
                    {
                        if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                        {
                            ResultDictionary["B相无功功率"][i] = "0|0|0";


                        }
                    }
                    MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "B相无功功率", ResultDictionary["B相无功功率"]);
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 1);
                }
            MessageController.Instance.AddMessage("正在读取C相无功功率");
            float[] Q_C = MeterProtocolAdapter.Instance.ReadData("02040300", 3, 1);

            float Q_QC = GlobalUnit.g_StrandMeterQFY[2];

            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (Q_C[i] == -1 || Q_C[i] == 0)
                    {
                        result[i] = "C相无功功率读取失败";
                        ResultDictionary["结论"][i] = "不合格";
                        ResultDictionary["C相无功功率"][i] = Q_C[i].ToString() + "|" + Q_QC.ToString() + "|" + "-999";
                    }
                    else if (Q_QC == 0)
                    {
                        resultB[i] = "标准C相电流读取失败";
                        ResultDictionary["结论"][i] = "不合格";
                        ResultDictionary["C相电流"][i] = Q_C[i].ToString() + "|" + Q_QC.ToString() + "|" + "-999";
                    }
                    else
                    {
                        Error[i] = (Q_C[i] - Q_QC) / Q_QC;

                        if (Error[i] > 1)
                        {
                            resultE[i] = "C相无功功率误差超差";
                            ResultDictionary["结论"][i] = "不合格";
                            ResultDictionary["C相无功功率"][i] = Q_C[i].ToString() + "|" + Q_QC.ToString() + "|" + Error[i].ToString("0.0000");
                        }
                        else
                        {
                            ResultDictionary["C相无功功率"][i] = Q_C[i].ToString() + "|" + Q_QC.ToString() + "|" + Error[i].ToString("0.0000");
                        }
                    }
                }
            }

            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "C相无功功率", ResultDictionary["C相无功功率"]);
        

            
            }
            }
            else
            {
                for (int i = 0; i < BwCount; i++)
                {
                    if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                    {
                        ResultDictionary["B相电压"][i] = "0|0|0";
                        ResultDictionary["C相电压"][i] = "0|0|0";
                        ResultDictionary["B相电流"][i] = "0|0|0";
                        ResultDictionary["C相电流"][i] = "0|0|0";
                        ResultDictionary["B相有功功率"][i] = "0|0|0";
                        ResultDictionary["C相有功功率"][i] = "0|0|0";
                        ResultDictionary["B相无功功率"][i] = "0|0|0";
                        ResultDictionary["C相无功功率"][i] = "0|0|0";

                    }
                }
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "B相电压", ResultDictionary["B相无功功率"]);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 1);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "C相电压", ResultDictionary["C相无功功率"]);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 1);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "B相电流", ResultDictionary["B相无功功率"]);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 1);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "C相电流", ResultDictionary["C相无功功率"]);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 1);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "B相有功功率", ResultDictionary["C相无功功率"]);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 1);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "C相有功功率", ResultDictionary["C相无功功率"]);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 1);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "B相无功功率", ResultDictionary["C相无功功率"]);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 1);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "C相无功功率", ResultDictionary["C相无功功率"]);
            

            }

            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    ResultDictionary["不合格原因"][i] = result[i]+"," + resultB[i] +"," +resultE[i];
                }
            }

            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "结论", ResultDictionary["结论"]);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 1);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "不合格原因", ResultDictionary["不合格原因"]);
            
          

        }

      

    }
}
