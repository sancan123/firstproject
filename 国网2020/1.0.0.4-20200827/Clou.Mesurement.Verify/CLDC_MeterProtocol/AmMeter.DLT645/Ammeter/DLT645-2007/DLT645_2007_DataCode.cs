/*-----------------------------------------------------------------------------------
 * Copyright(C) 2011 深圳市科陆软件有限公司电测事业部
 * 文件名: DLT645_2007_DataCode.cs
 * 文件功能描述: DL/T645-2007数据标识表
 * 创建标识: ShiHe 20110316
 * 修改标识:
 * 修改描述:
 *-----------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Text;
using CLDC_MeterProtocol.Ammeter.DLT645;
using CLDC_MeterProtocol.Ammeter.DLT645.Comm.Class;

namespace CLDC_MeterProtocol.Ammeter.DLT6452007
{
    /// <summary>
    /// DL/T645-2007数据标识表
    /// </summary>
    public class DLT645_2007_DataCode : IDictionary
    {
        private List<DataInfo> _DataCodes;
        private XmlFile _ConfigFile;
        private bool _IsReadFile;
        private TreePointer _Pointer; //搜索方式

        public DLT645_2007_DataCode()
        {
            _Pointer = new TreePointer();
            _DataCodes = new List<DataInfo>();
            _IsReadFile = false;
            if (!_IsReadFile)
                InitDataCode();
            else
            {
                _ConfigFile = new XmlFile("XML\\DataCode.xml");
                InitDataCode_RF();
            }
        }

        /// <summary>
        /// 根据标识码搜索数据项
        /// </summary>
        /// <param name="strKey">标识码(DI3-DI2-DI1-DI0)</param>
        /// <returns>如果搜索失败，则返回空列表</returns>
        public List<DataInfo> Search(string strKey)
        {
            List<DataInfo> lstDatas = new List<DataInfo>();
            byte[] byts_Key = DLTFun.HexStrToBytsDataA(DLTFun.AdjustmentStrSize(strKey, 8, true), 4);
            IList<int> lstIndexs = _Pointer.FindData(byts_Key);  //yonsion debug
            foreach (int s4_Index in lstIndexs)
            {
                lstDatas.Add(_DataCodes[s4_Index]);
            }
            return lstDatas;
        }

        //初始化数据编码表，必须按标识码从小到大顺序添加 2012-03-30 zrz
        /// <summary>
        /// 2011-8-23 陈大伟修改
        /// </summary>
        private void InitDataCode()
        {
//#if lydugold
//            #region ==== 表A.1 电能量数据标识编码表 ====
//            /*组合有功*/
//            //总电能
//            DataInfo data_ZhTotalEnergy = new DataInfo();
//            data_ZhTotalEnergy.InitParam(3, true, false);
//            data_ZhTotalEnergy.AddDataField(4, 0);
//            _DataCodes.Add(data_ZhTotalEnergy);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("00000000", 4), _DataCodes.Count - 1);

//            //定点 瞬时冻结命令
//            DataInfo data_RunState16 = new DataInfo();
//            data_RunState16.InitParam(3, true, true);
//            data_RunState16.AddDataField(4, 0);
//            _DataCodes.Add(data_RunState16);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("00000016", 4), _DataCodes.Count - 1);


//            //修改密码
//            DataInfo data_RunState17 = new DataInfo();
//            data_RunState17.InitParam(3, true, true);
//            data_RunState17.AddDataField(12, 0);
//            _DataCodes.Add(data_RunState17);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("00000018", 4), _DataCodes.Count - 1);
//            /* 正向有功 */
//            //总电能
//            DataInfo data_ZYTotalEnergy = new DataInfo();
//            data_ZYTotalEnergy.InitParam(3, true, false);
//            data_ZYTotalEnergy.AddDataField(4, 0);
//            _DataCodes.Add(data_ZYTotalEnergy);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("00010000", 4), _DataCodes.Count - 1);
//            //费率1电能
//            DataInfo data_ZY1Energy = new DataInfo();
//            data_ZY1Energy.InitParam(3, true, false);
//            data_ZY1Energy.AddDataField(4, 0);
//            _DataCodes.Add(data_ZY1Energy);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("00010100", 4), _DataCodes.Count - 1);
//            //费率2电能
//            DataInfo data_ZY2Energy = new DataInfo();
//            data_ZY2Energy.InitParam(3, true, false);
//            data_ZY2Energy.AddDataField(4, 0);
//            _DataCodes.Add(data_ZY2Energy);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("00010200", 4), _DataCodes.Count - 1);
//            //费率3电能
//            DataInfo data_ZY3Energy = new DataInfo();
//            data_ZY3Energy.InitParam(3, true, false);
//            data_ZY3Energy.AddDataField(4, 0);
//            _DataCodes.Add(data_ZY3Energy);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("00010300", 4), _DataCodes.Count - 1);
//            //费率4电能
//            DataInfo data_ZY4Energy = new DataInfo();
//            data_ZY4Energy.InitParam(3, true, false);
//            data_ZY4Energy.AddDataField(4, 0);
//            _DataCodes.Add(data_ZY4Energy);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("00010400", 4), _DataCodes.Count - 1);

//            /* 正向有功 */
//            //总最大需量
//            DataInfo data_ZYTotalDemand = new DataInfo();
//            data_ZYTotalDemand.InitParam(3, true, false);
//            data_ZYTotalDemand.AddDataField(4, 0);
//            _DataCodes.Add(data_ZYTotalDemand);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("01010000", 4), _DataCodes.Count - 1);
//            //费率1最大需量
//            DataInfo data_ZY1Demand = new DataInfo();
//            data_ZY1Demand.InitParam(3, true, false);
//            data_ZY1Demand.AddDataField(4, 0);
//            _DataCodes.Add(data_ZY1Demand);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("01010100", 4), _DataCodes.Count - 1);
//            //费率2最大需量
//            DataInfo data_ZY2Demand = new DataInfo();
//            data_ZY2Demand.InitParam(3, true, false);
//            data_ZY2Demand.AddDataField(4, 0);
//            _DataCodes.Add(data_ZY2Demand);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("01010200", 4), _DataCodes.Count - 1);
//            //费率3最大需量
//            DataInfo data_ZY3Demand = new DataInfo();
//            data_ZY3Demand.InitParam(3, true, false);
//            data_ZY3Demand.AddDataField(4, 0);
//            _DataCodes.Add(data_ZY3Demand);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("01010300", 4), _DataCodes.Count - 1);
//            //费率4最大需量
//            DataInfo data_ZY4Demand = new DataInfo();
//            data_ZY4Demand.InitParam(3, true, false);
//            data_ZY4Demand.AddDataField(4, 0);
//            _DataCodes.Add(data_ZY4Demand);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("01010400", 4), _DataCodes.Count - 1);

//            //剩余电量
//            DataInfo data_SpareEnergy = new DataInfo();
//            data_SpareEnergy.InitParam(3, true, false);
//            data_SpareEnergy.AddDataField(4, 2);
//            _DataCodes.Add(data_ZY4Energy);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("00900100", 4), _DataCodes.Count - 1);
//            //剩余金额
//            DataInfo data_SpareMoney = new DataInfo();
//            data_SpareMoney.InitParam(3, true, false);
//            data_SpareMoney.AddDataField(4, 2);
//            _DataCodes.Add(data_SpareMoney);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("00900200", 4), _DataCodes.Count - 1);
//            #endregion ==== 表A.1 电能量数据标识编码表 ====
//            #region ==== 表A.3 参变量数据标识编码表 ====
//            //A相电压
//            DataInfo data_Ua = new DataInfo();
//            data_Ua.InitParam(3, true, false);
//            data_Ua.AddDataField(2, 0);
//            _DataCodes.Add(data_Ua);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("02010100", 4), _DataCodes.Count - 1);
//            //B相电压
//            DataInfo data_Ub = new DataInfo();
//            data_Ub.InitParam(3, true, false);
//            data_Ub.AddDataField(2, 0);
//            _DataCodes.Add(data_Ub);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("02010200", 4), _DataCodes.Count - 1);
//            //C相电压
//            DataInfo data_Uc = new DataInfo();
//            data_Uc.InitParam(3, true, false);
//            data_Uc.AddDataField(2, 0);
//            _DataCodes.Add(data_Uc);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("02010300", 4), _DataCodes.Count - 1);
//            //A相电流
//            DataInfo data_Ia = new DataInfo();
//            data_Ia.InitParam(3, true, false);
//            data_Ia.AddDataField(3, 0);
//            _DataCodes.Add(data_Ia);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("02020100", 4), _DataCodes.Count - 1);
//            //B相电流
//            DataInfo data_Ib = new DataInfo();
//            data_Ib.InitParam(3, true, false);
//            data_Ib.AddDataField(3, 0);
//            _DataCodes.Add(data_Ib);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("02020200", 4), _DataCodes.Count - 1);
//            //C相电流
//            DataInfo data_Ic = new DataInfo();
//            data_Ic.InitParam(3, true, false);
//            data_Ic.AddDataField(3, 0);
//            _DataCodes.Add(data_Ic);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("02020300", 4), _DataCodes.Count - 1);

//            //阶梯电价
//            DataInfo data_MeterJtdj = new DataInfo();
//            data_MeterJtdj.InitParam(3, true, false);
//            data_MeterJtdj.AddDataField(4, 2);
//            _DataCodes.Add(data_ZY4Energy);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("0280000B", 4), _DataCodes.Count - 1);

//            //日期及星期
//            DataInfo data_DateAndWeek = new DataInfo();
//            data_DateAndWeek.InitParam(3, true, true);
//            data_DateAndWeek.AddDataField(1, 0); //星期
//            data_DateAndWeek.AddDataField(1, 0); //日
//            data_DateAndWeek.AddDataField(1, 0); //月
//            data_DateAndWeek.AddDataField(1, 0); //年
//            _DataCodes.Add(data_DateAndWeek);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000101", 4), _DataCodes.Count - 1);
//            //时间
//            DataInfo data_Time = new DataInfo();
//            data_Time.InitParam(3, true, true);
//            data_Time.AddDataField(1, 0); //秒
//            data_Time.AddDataField(1, 0); //分
//            data_Time.AddDataField(1, 0); //时
//            _DataCodes.Add(data_Time);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000102", 4), _DataCodes.Count - 1);

//            //最大需量周期
//            data_Time = new DataInfo();
//            data_Time.InitParam(3, true, true);
//            data_Time.AddDataField(1, 0);
//            _DataCodes.Add(data_Time);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000103", 4), _DataCodes.Count - 1);
//            //滑差时间
//            data_Time = new DataInfo();
//            data_Time.InitParam(3, true, true);
//            data_Time.AddDataField(1, 0);
//            _DataCodes.Add(data_Time);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000104", 4), _DataCodes.Count - 1);
//            // 校表脉冲宽度
//            data_Time = new DataInfo();
//            data_Time.InitParam(3, true, false);
//            data_Time.AddDataField(2, 0);
//            _DataCodes.Add(data_Time);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000105", 4), _DataCodes.Count - 1);

//            //两套时区表切换时间
//            data_Time = new DataInfo();
//            data_Time.InitParam(3, true, true);
//            data_Time.AddDataField(1, 0);
//            data_Time.AddDataField(1, 0);
//            data_Time.AddDataField(1, 0);
//            data_Time.AddDataField(1, 0);
//            data_Time.AddDataField(1, 0);
//            _DataCodes.Add(data_Time);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000106", 4), _DataCodes.Count - 1);
//            //两套日时段表切换时间
//            data_Time = new DataInfo();
//            data_Time.InitParam(3, true, true);
//            data_Time.AddDataField(1, 0);
//            data_Time.AddDataField(1, 0);
//            data_Time.AddDataField(1, 0);
//            data_Time.AddDataField(1, 0);
//            data_Time.AddDataField(1, 0);
//            _DataCodes.Add(data_Time);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000107", 4), _DataCodes.Count - 1);

//            //两套费率电价切换时间
//            data_Time = new DataInfo();
//            data_Time.InitParam(3, true, true);
//            data_Time.AddDataField(5, 0);
//            _DataCodes.Add(data_Time);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000108", 4), _DataCodes.Count - 1);
//            //两套阶梯切换时间
//            data_Time = new DataInfo();
//            data_Time.InitParam(3, true, true);
//            data_Time.AddDataField(5, 0);
//            _DataCodes.Add(data_Time);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000109", 4), _DataCodes.Count - 1);


//            //年时区数p≤14
//            data_Time = new DataInfo();
//            data_Time.InitParam(3, true, true);
//            data_Time.AddDataField(1, 0);
//            _DataCodes.Add(data_Time);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000201", 4), _DataCodes.Count - 1);
//            //日时段表数q≤8
//            data_Time = new DataInfo();
//            data_Time.InitParam(3, true, true);
//            data_Time.AddDataField(1, 0);
//            _DataCodes.Add(data_Time);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000202", 4), _DataCodes.Count - 1);
//            //日时段数(每日切换数) m≤14
//            data_Time = new DataInfo();
//            data_Time.InitParam(3, true, true);
//            data_Time.AddDataField(1, 0);
//            _DataCodes.Add(data_Time);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000203", 4), _DataCodes.Count - 1);
//            //费率数k≤63
//            data_Time = new DataInfo();
//            data_Time.InitParam(3, true, true);
//            data_Time.AddDataField(1, 0);
//            _DataCodes.Add(data_Time);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000204", 4), _DataCodes.Count - 1);
//            //公共假日数n≤254
//            data_Time = new DataInfo();
//            data_Time.InitParam(3, true, true);
//            data_Time.AddDataField(2, 0);
//            _DataCodes.Add(data_Time);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000205", 4), _DataCodes.Count - 1);
//            //谐波分析次数
//            data_Time = new DataInfo();
//            data_Time.InitParam(3, true, true);
//            data_Time.AddDataField(1, 0);
//            _DataCodes.Add(data_Time);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000206", 4), _DataCodes.Count - 1);

//            //阶梯数
//            data_Time = new DataInfo();
//            data_Time.InitParam(3, true, true);
//            data_Time.AddDataField(1, 0);
//            _DataCodes.Add(data_Time);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000207", 4), _DataCodes.Count - 1);

//            //自动循环显示屏数
//            data_Time = new DataInfo();
//            data_Time.InitParam(3, true, true);
//            data_Time.AddDataField(1, 0);
//            _DataCodes.Add(data_Time);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000301", 4), _DataCodes.Count - 1);

//            //每屏显示时间
//            data_Time = new DataInfo();
//            data_Time.InitParam(3, true, true);
//            data_Time.AddDataField(1, 0);
//            _DataCodes.Add(data_Time);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000302", 4), _DataCodes.Count - 1);

//            //显示电能小数位数
//            data_Time = new DataInfo();
//            data_Time.InitParam(3, true, true);
//            data_Time.AddDataField(1, 0);
//            _DataCodes.Add(data_Time);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000303", 4), _DataCodes.Count - 1);

//            //显示功率(最大需量)小数位数
//            data_Time = new DataInfo();
//            data_Time.InitParam(3, true, true);
//            data_Time.AddDataField(1, 0);
//            _DataCodes.Add(data_Time);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000304", 4), _DataCodes.Count - 1);

//            //按键循环显示屏数
//            data_Time = new DataInfo();
//            data_Time.InitParam(3, true, true);
//            data_Time.AddDataField(1, 0);
//            _DataCodes.Add(data_Time);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000305", 4), _DataCodes.Count - 1);

//            //电流互感器变比
//            data_Time = new DataInfo();
//            data_Time.InitParam(3, true, true);
//            data_Time.AddDataField(3, 0);
//            _DataCodes.Add(data_Time);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000306", 4), _DataCodes.Count - 1);
//            //电压互感器变比
//            data_Time = new DataInfo();
//            data_Time.InitParam(3, true, true);
//            data_Time.AddDataField(3, 0);
//            _DataCodes.Add(data_Time);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000307", 4), _DataCodes.Count - 1);

//            //通信地址
//            data_Time = new DataInfo();
//            data_Time.InitParam(3, true, true);
//            data_Time.AddDataField(6, 0);
//            _DataCodes.Add(data_Time);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000401", 4), _DataCodes.Count - 1);

//            //表号
//            data_Time = new DataInfo();
//            data_Time.InitParam(3, true, true);
//            data_Time.AddDataField(6, 0);
//            _DataCodes.Add(data_Time);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000402", 4), _DataCodes.Count - 1);

//            //资产管理编码(ASCII码)
//            data_Time = new DataInfo();
//            data_Time.InitParam(3, true, true);
//            data_Time.AddDataField(32, 0);
//            _DataCodes.Add(data_Time);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000403", 4), _DataCodes.Count - 1);

//            //额定电压(ASCII码)
//            data_Time = new DataInfo();
//            data_Time.InitParam(3, true, false);
//            data_Time.AddDataField(6, 0);
//            _DataCodes.Add(data_Time);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000404", 4), _DataCodes.Count - 1);

//            //额定电流/基本电流(ASCII码)
//            data_Time = new DataInfo();
//            data_Time.InitParam(3, true, false);
//            data_Time.AddDataField(6, 0);
//            _DataCodes.Add(data_Time);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000405", 4), _DataCodes.Count - 1);

//            //最大电流(ASCII码)
//            data_Time = new DataInfo();
//            data_Time.InitParam(3, true, false);
//            data_Time.AddDataField(6, 0);
//            _DataCodes.Add(data_Time);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000406", 4), _DataCodes.Count - 1);
//            //有功准确度等级(ASCII码)
//            data_Time = new DataInfo();
//            data_Time.InitParam(3, true, false);
//            data_Time.AddDataField(4, 0);
//            _DataCodes.Add(data_Time);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000407", 4), _DataCodes.Count - 1);
//            //无功准确度等级(ASCII码)
//            data_Time = new DataInfo();
//            data_Time.InitParam(3, true, false);
//            data_Time.AddDataField(4, 0);
//            _DataCodes.Add(data_Time);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000408", 4), _DataCodes.Count - 1);
//            //电表有功常数
//            data_Time = new DataInfo();
//            data_Time.InitParam(3, true, false);
//            data_Time.AddDataField(3, 0);
//            _DataCodes.Add(data_Time);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000409", 4), _DataCodes.Count - 1);

//            //电表无功常数
//            data_Time = new DataInfo();
//            data_Time.InitParam(3, true, false);
//            data_Time.AddDataField(3, 0);
//            _DataCodes.Add(data_Time);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("0400040A", 4), _DataCodes.Count - 1);
//            //电表型号(ASCII码)
//            data_Time = new DataInfo();
//            data_Time.InitParam(3, true, false);
//            data_Time.AddDataField(10, 0);
//            _DataCodes.Add(data_Time);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("0400040B", 4), _DataCodes.Count - 1);
//            //生产日期(ASCII码)
//            data_Time = new DataInfo();
//            data_Time.InitParam(3, true, false);
//            data_Time.AddDataField(10, 0);
//            _DataCodes.Add(data_Time);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("0400040C", 4), _DataCodes.Count - 1);
//            //协议版本号(ASCII码)
//            data_Time = new DataInfo();
//            data_Time.InitParam(3, true, false);
//            data_Time.AddDataField(16, 0);
//            _DataCodes.Add(data_Time);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("0400040D", 4), _DataCodes.Count - 1);

//            //客户编号
//            data_Time = new DataInfo();
//            data_Time.InitParam(3, true, true);
//            data_Time.AddDataField(6, 0);
//            _DataCodes.Add(data_Time);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("0400040E", 4), _DataCodes.Count - 1);

//            //电表运行状态1
//            for (int m = 0; m < 254; m++)
//            {
//                DataInfo data_RunState1 = new DataInfo();
//                data_RunState1.InitParam(3, true, false);
//                data_RunState1.AddDataField(2, 0);
//                _DataCodes.Add(data_RunState1);
//                _Pointer.AddData(DLTFun.HexStrToBytsDataA("040005" + (m + 1).ToString("X2"), 4), _DataCodes.Count - 1);
//            }
//            //DataInfo data_RunState1 = new DataInfo();
//            //data_RunState1.InitParam(3, true, false);
//            //data_RunState1.AddDataField(2, 0);
//            //_DataCodes.Add(data_RunState1);
//            //_Pointer.AddData(DLTFun.HexStrToBytsDataA("04000501", 4), _DataCodes.Count - 1);
//            //电表运行状态2
//            //DataInfo data_RunState2 = new DataInfo();
//            //data_RunState2.InitParam(3, true, false);
//            //data_RunState2.AddDataField(2, 0);
//            //_DataCodes.Add(data_RunState2);
//            //_Pointer.AddData(DLTFun.HexStrToBytsDataA("04000502", 4), _DataCodes.Count - 1);
//            ////电表运行状态3
//            //DataInfo data_RunState3 = new DataInfo();
//            //data_RunState3.InitParam(3, true, false);
//            //data_RunState3.AddDataField(2, 0);
//            //_DataCodes.Add(data_RunState3);
//            //_Pointer.AddData(DLTFun.HexStrToBytsDataA("04000503", 4), _DataCodes.Count - 1);
//            ////电表运行状态4
//            //DataInfo data_RunState4 = new DataInfo();
//            //data_RunState4.InitParam(3, true, false);
//            //data_RunState4.AddDataField(2, 0);
//            //_DataCodes.Add(data_RunState4);
//            //_Pointer.AddData(DLTFun.HexStrToBytsDataA("04000504", 4), _DataCodes.Count - 1);
//            ////电表运行状态5
//            //DataInfo data_RunState5 = new DataInfo();
//            //data_RunState5.InitParam(3, true, false);
//            //data_RunState5.AddDataField(2, 0);
//            //_DataCodes.Add(data_RunState5);
//            //_Pointer.AddData(DLTFun.HexStrToBytsDataA("04000505", 4), _DataCodes.Count - 1);
//            ////电表运行状态6
//            //DataInfo data_RunState6 = new DataInfo();
//            //data_RunState6.InitParam(3, true, false);
//            //data_RunState6.AddDataField(2, 0);
//            //_DataCodes.Add(data_RunState6);
//            //_Pointer.AddData(DLTFun.HexStrToBytsDataA("04000506", 4), _DataCodes.Count - 1);
//            ////电表运行状态7
//            //DataInfo data_RunState7 = new DataInfo();
//            //data_RunState7.InitParam(3, true, false);
//            //data_RunState7.AddDataField(2, 0);
//            //_DataCodes.Add(data_RunState7);
//            //_Pointer.AddData(DLTFun.HexStrToBytsDataA("04000507", 4), _DataCodes.Count - 1);

//            //当前阶梯电价
//            //DataInfo data_LadderPrice = new DataInfo();
//            //data_LadderPrice.InitParam(3, true, true);
//            //data_LadderPrice.AddDataField(4, 4);
//            //_DataCodes.Add(data_LadderPrice);
//            //_Pointer.AddData(DLTFun.HexStrToBytsDataA("0280000B", 4), _DataCodes.Count - 1);

//            //有功组合方式特征字
//            DataInfo data_RunState7 = new DataInfo();
//            data_RunState7.InitParam(3, true, true);
//            data_RunState7.AddDataField(1, 0);
//            _DataCodes.Add(data_RunState7);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000601", 4), _DataCodes.Count - 1);

//            //无功组合方式1特征字
//            data_RunState7 = new DataInfo();
//            data_RunState7.InitParam(3, true, true);
//            data_RunState7.AddDataField(1, 0);
//            _DataCodes.Add(data_RunState7);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000602", 4), _DataCodes.Count - 1);
//            //无功组合方式2特征字
//            data_RunState7 = new DataInfo();
//            data_RunState7.InitParam(3, true, true);
//            data_RunState7.AddDataField(1, 0);
//            _DataCodes.Add(data_RunState7);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000603", 4), _DataCodes.Count - 1);

//            //调制型红外光口通信速率特征字
//            for (int h = 0; h < 5; h++)
//            {
//                DataInfo Hdata_RunState8 = new DataInfo();
//                Hdata_RunState8.InitParam(3, true, true);
//                Hdata_RunState8.AddDataField(1, 0);
//                _DataCodes.Add(Hdata_RunState8);
//                _Pointer.AddData(DLTFun.HexStrToBytsDataA("040007" + (h + 1).ToString("X2"), 4), _DataCodes.Count - 1);
//            }
//            //周休日特征字
//            data_RunState7 = new DataInfo();
//            data_RunState7.InitParam(3, true, true);
//            data_RunState7.AddDataField(1, 0);
//            _DataCodes.Add(data_RunState7);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000801", 4), _DataCodes.Count - 1);

//            //周休日采用的日时段表号
//            data_RunState7 = new DataInfo();
//            data_RunState7.InitParam(3, true, true);
//            data_RunState7.AddDataField(1, 0);
//            _DataCodes.Add(data_RunState7);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000802", 4), _DataCodes.Count - 1);

//            //负荷记录模式字
//            data_RunState7 = new DataInfo();
//            data_RunState7.InitParam(3, true, true);
//            data_RunState7.AddDataField(1, 0);
//            _DataCodes.Add(data_RunState7);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000901", 4), _DataCodes.Count - 1);
//            //冻结数据模式字
//            data_RunState7 = new DataInfo();
//            data_RunState7.InitParam(3, true, true);
//            data_RunState7.AddDataField(1, 0);
//            _DataCodes.Add(data_RunState7);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000902", 4), _DataCodes.Count - 1);
//            //写表冻结值 ：瞬时冻结
//            DataInfo data_RunState8 = new DataInfo();
//            data_RunState8.InitParam(3, true, true);
//            data_RunState8.AddDataField(1, 0);
//            _DataCodes.Add(data_RunState8);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000903", 4), _DataCodes.Count - 1);
//            //写表冻结值 ：约定冻结数据模式字
//            DataInfo data_RunState9 = new DataInfo();
//            data_RunState9.InitParam(3, true, true);
//            data_RunState9.AddDataField(1, 0);
//            _DataCodes.Add(data_RunState9);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000904", 4), _DataCodes.Count - 1);
//            //写表冻结值 ：整点冻结数据模式字
//            DataInfo data_RunState10 = new DataInfo();
//            data_RunState10.InitParam(3, true, true);
//            data_RunState10.AddDataField(1, 0);
//            _DataCodes.Add(data_RunState10);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000905", 4), _DataCodes.Count - 1);

//            //写表冻结值 ：日冻结
//            DataInfo data_RunState11 = new DataInfo();
//            data_RunState11.InitParam(3, true, true);
//            data_RunState11.AddDataField(1, 0);
//            _DataCodes.Add(data_RunState11);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000906", 4), _DataCodes.Count - 1);

//            //负荷记录起始时间
//            data_RunState7 = new DataInfo();
//            data_RunState7.InitParam(3, true, true);
//            data_RunState7.AddDataField(1, 0);
//            data_RunState7.AddDataField(1, 0);
//            data_RunState7.AddDataField(1, 0);
//            data_RunState7.AddDataField(1, 0);
//            _DataCodes.Add(data_RunState7);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000A01", 4), _DataCodes.Count - 1);

//            //第一套阶梯电价1
//            DataInfo data_OneLadder1 = new DataInfo();
//            data_OneLadder1.InitParam(3, true, true);
//            data_OneLadder1.AddDataField(4, 4);
//            _DataCodes.Add(data_OneLadder1);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04050101", 4), _DataCodes.Count - 1);
//            //第一套阶梯电价2
//            DataInfo data_OneLadder2 = new DataInfo();
//            data_OneLadder2.InitParam(3, true, true);
//            data_OneLadder2.AddDataField(4, 4);
//            _DataCodes.Add(data_OneLadder2);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04050102", 4), _DataCodes.Count - 1);
//            //第一套阶梯电价3
//            DataInfo data_OneLadder3 = new DataInfo();
//            data_OneLadder3.InitParam(3, true, true);
//            data_OneLadder3.AddDataField(4, 4);
//            _DataCodes.Add(data_OneLadder3);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04050103", 4), _DataCodes.Count - 1);
//            //第一套阶梯电价4
//            DataInfo data_OneLadder4 = new DataInfo();
//            data_OneLadder4.InitParam(3, true, true);
//            data_OneLadder4.AddDataField(4, 4);
//            _DataCodes.Add(data_OneLadder4);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04050104", 4), _DataCodes.Count - 1);
//            //第二套阶梯电价1
//            DataInfo data_TwoLadder1 = new DataInfo();
//            data_TwoLadder1.InitParam(3, true, true);
//            data_TwoLadder1.AddDataField(4, 4);
//            _DataCodes.Add(data_TwoLadder1);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04050201", 4), _DataCodes.Count - 1);
//            //第二套阶梯电价2
//            DataInfo data_TwoLadder2 = new DataInfo();
//            data_TwoLadder2.InitParam(3, true, true);
//            data_TwoLadder2.AddDataField(4, 4);
//            _DataCodes.Add(data_TwoLadder2);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04050202", 4), _DataCodes.Count - 1);
//            //第二套阶梯电价3
//            DataInfo data_TwoLadder3 = new DataInfo();
//            data_TwoLadder3.InitParam(3, true, true);
//            data_TwoLadder3.AddDataField(4, 4);
//            _DataCodes.Add(data_TwoLadder3);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04050203", 4), _DataCodes.Count - 1);
//            //第二套阶梯电价4
//            DataInfo data_TwoLadder4 = new DataInfo();
//            data_TwoLadder4.InitParam(3, true, true);
//            data_TwoLadder4.AddDataField(4, 4);
//            _DataCodes.Add(data_TwoLadder4);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04050204", 4), _DataCodes.Count - 1);
//            #endregion ==== 表A.5 参变量数据标识编码表 ====
//            #region ==== 表A.6 安全认证专写数据标识编码表 ====
//            /* 身份认证 */
//            //密文1
//            DataInfo data_Cryp1 = new DataInfo();
//            data_Cryp1.InitParam(3, true, true);
//            data_Cryp1.AddDataField(8, 0);
//            _DataCodes.Add(data_Cryp1);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("07000001", 4), _DataCodes.Count - 1);
//            //随机数1
//            DataInfo data_StochasticNum1 = new DataInfo();
//            data_StochasticNum1.InitParam(3, true, true);
//            data_StochasticNum1.AddDataField(8, 0);
//            _DataCodes.Add(data_StochasticNum1);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("07000002", 4), _DataCodes.Count - 1);
//            //分散因子
//            DataInfo data_Gene = new DataInfo();
//            data_Gene.InitParam(3, true, true);
//            data_Gene.AddDataField(8, 0);
//            _DataCodes.Add(data_Gene);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("07000003", 4), _DataCodes.Count - 1);
//            /* 控制命令密钥更新 */
//            //密钥信息+MAC
//            DataInfo data_CtlKeyInfo = new DataInfo();
//            data_CtlKeyInfo.InitParam(3, true, true);
//            data_CtlKeyInfo.AddDataField(4, 0); //MAC
//            data_CtlKeyInfo.AddDataField(4, 0); //密钥信息
//            _DataCodes.Add(data_CtlKeyInfo);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("07020101", 4), _DataCodes.Count - 1);
//            //控制命令文件线路保护密钥
//            DataInfo data_CtlFileKey = new DataInfo();
//            data_CtlFileKey.InitParam(3, true, true);
//            data_CtlFileKey.AddDataField(32, 0);
//            _DataCodes.Add(data_CtlFileKey);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("07020102", 4), _DataCodes.Count - 1);
//            /* 参数密钥更新 */
//            //密钥信息+MAC
//            DataInfo data_ParamKeyInfo = new DataInfo();
//            data_ParamKeyInfo.InitParam(3, true, true);
//            data_ParamKeyInfo.AddDataField(4, 0); //MAC
//            data_ParamKeyInfo.AddDataField(4, 0); //密钥信息
//            _DataCodes.Add(data_ParamKeyInfo);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("07020201", 4), _DataCodes.Count - 1);
//            //参数更新文件线路保护密钥
//            DataInfo data_ParamFileKey = new DataInfo();
//            data_ParamFileKey.InitParam(3, true, true);
//            data_ParamFileKey.AddDataField(32, 0);
//            _DataCodes.Add(data_ParamFileKey);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("07020202", 4), _DataCodes.Count - 1);
//            /* 远程身份认证密钥更新 */
//            //密钥信息+MAC
//            DataInfo data_CrypKeyInfo = new DataInfo();
//            data_CrypKeyInfo.InitParam(3, true, true);
//            data_CrypKeyInfo.AddDataField(4, 0); //MAC
//            data_CrypKeyInfo.AddDataField(4, 0); //密钥信息
//            _DataCodes.Add(data_CrypKeyInfo);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("07020301", 4), _DataCodes.Count - 1);
//            //远程身份认证密钥
//            DataInfo data_CrypFileKey = new DataInfo();
//            data_CrypFileKey.InitParam(3, true, true);
//            data_CrypFileKey.AddDataField(32, 0);
//            _DataCodes.Add(data_CrypFileKey);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("07020302", 4), _DataCodes.Count - 1);
//            /* 主控密钥更新 */
//            //密钥信息+MAC
//            DataInfo data_ZkKeyInfo = new DataInfo();
//            data_ZkKeyInfo.InitParam(3, true, true);
//            data_ZkKeyInfo.AddDataField(4, 0); //MAC
//            data_ZkKeyInfo.AddDataField(4, 0); //密钥信息
//            _DataCodes.Add(data_ZkKeyInfo);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("07020401", 4), _DataCodes.Count - 1);
//            //远程身份认证密钥
//            DataInfo data_ZkFileKey = new DataInfo();
//            data_ZkFileKey.InitParam(3, true, true);
//            data_ZkFileKey.AddDataField(32, 0);
//            _DataCodes.Add(data_ZkFileKey);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("07020402", 4), _DataCodes.Count - 1);
//            /* 数据回抄 */
//            DataInfo data_EsamFile = new DataInfo();
//            data_EsamFile.InitParam(3, true, true);
//            data_EsamFile.AddDataField(2, 0); //数据长度
//            data_EsamFile.AddDataField(2, 0); //起始地址
//            data_EsamFile.AddDataField(2, 0); //文件标识
//            data_EsamFile.AddDataField(2, 0); //目录标识
//            _DataCodes.Add(data_EsamFile);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("07800101", 4), _DataCodes.Count - 1);
//            #endregion ==== 表A.6 安全认证专写数据标识编码表 ====
//            #region ==== 表A.9 自定义 ====
//            //跳合闸、报警、保电功能控制
//            DataInfo data_ControlFun = new DataInfo();
//            data_ControlFun.InitParam(3, true, true);
//            data_ControlFun.AddDataField(1, 0); //状态
//            data_ControlFun.AddDataField(1, 0); //保留
//            data_ControlFun.AddDataField(6, 0); //有效截止时间
//            _DataCodes.Add(data_ControlFun);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("08000001", 4), _DataCodes.Count - 1);
//            //多功能端子输出信号类别
//            DataInfo data_ControlMult = new DataInfo();
//            data_ControlMult.InitParam(3, true, true);
//            data_ControlMult.AddDataField(1, 0); //脉冲类型
//            _DataCodes.Add(data_ControlMult);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("08000002", 4), _DataCodes.Count - 1);
//            //通讯地址
//            DataInfo data_CommAddr = new DataInfo();
//            data_CommAddr.InitParam(3, true, true);
//            data_CommAddr.AddDataField(6, 0);
//            _DataCodes.Add(data_CommAddr);
//            _Pointer.AddData(DLTFun.HexStrToBytsDataA("08000003", 4), _DataCodes.Count - 1);
//            #endregion ==== 自定义表A.9 ====
//#endif

            #region ---new--

            #region ==== 表A.1 电能量数据标识编码表 ====
            /*组合有功*/
            //总电能
            DataInfo data_ZhTotalEnergy = new DataInfo();
            data_ZhTotalEnergy.InitParam(3, true, false);
            data_ZhTotalEnergy.AddDataField(4, 0);
            _DataCodes.Add(data_ZhTotalEnergy);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("00000000", 4), _DataCodes.Count - 1);

            //定点 瞬时冻结命令
            DataInfo data_RunState16 = new DataInfo();
            data_RunState16.InitParam(3, true, true);
            data_RunState16.AddDataField(4, 0);
            _DataCodes.Add(data_RunState16);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("00000016", 4), _DataCodes.Count - 1);


            //修改密码
            DataInfo data_RunState17 = new DataInfo();
            data_RunState17.InitParam(3, true, true);
            data_RunState17.AddDataField(12, 0);
            _DataCodes.Add(data_RunState17);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("00000018", 4), _DataCodes.Count - 1);

            /* 正向有功 */
            //总电能
            DataInfo data_ZYTotalEnergy = new DataInfo();
            data_ZYTotalEnergy.InitParam(3, true, false);
            data_ZYTotalEnergy.AddDataField(4, 0);
            _DataCodes.Add(data_ZYTotalEnergy);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("00010000", 4), _DataCodes.Count - 1);
            //费率1电能
            DataInfo data_ZY1Energy = new DataInfo();
            data_ZY1Energy.InitParam(3, true, false);
            data_ZY1Energy.AddDataField(4, 0);
            _DataCodes.Add(data_ZY1Energy);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("00010100", 4), _DataCodes.Count - 1);
            //费率2电能
            DataInfo data_ZY2Energy = new DataInfo();
            data_ZY2Energy.InitParam(3, true, false);
            data_ZY2Energy.AddDataField(4, 0);
            _DataCodes.Add(data_ZY2Energy);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("00010200", 4), _DataCodes.Count - 1);
            //费率3电能
            DataInfo data_ZY3Energy = new DataInfo();
            data_ZY3Energy.InitParam(3, true, false);
            data_ZY3Energy.AddDataField(4, 0);
            _DataCodes.Add(data_ZY3Energy);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("00010300", 4), _DataCodes.Count - 1);
            //费率4电能
            DataInfo data_ZY4Energy = new DataInfo();
            data_ZY4Energy.InitParam(3, true, false);
            data_ZY4Energy.AddDataField(4, 0);
            _DataCodes.Add(data_ZY4Energy);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("00010400", 4), _DataCodes.Count - 1);

            //剩余电量
            DataInfo data_SpareEnergy = new DataInfo();
            data_SpareEnergy.InitParam(3, true, false);
            data_SpareEnergy.AddDataField(4, 2);
            _DataCodes.Add(data_ZY4Energy);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("00900100", 4), _DataCodes.Count - 1);

            //剩余金额
            DataInfo data_SpareMoney = new DataInfo();
            data_SpareMoney.InitParam(3, true, false);
            data_SpareMoney.AddDataField(4, 2);
            _DataCodes.Add(data_SpareMoney);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("00900200", 4), _DataCodes.Count - 1);

            /* 正向有功 */
            //总费率需量
            DataInfo data_ZYTotalDemand = new DataInfo();
            data_ZYTotalDemand.InitParam(3, true, false);
            data_ZYTotalDemand.AddDataField(8, 0);
            _DataCodes.Add(data_ZYTotalDemand);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("01010000", 4), _DataCodes.Count - 1);
            //费率1费率需量
            DataInfo data_ZY1Demand = new DataInfo();
            data_ZY1Demand.InitParam(3, true, false);
            data_ZY1Demand.AddDataField(8, 0);
            _DataCodes.Add(data_ZY1Demand);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("01010100", 4), _DataCodes.Count - 1);
            //费率2费率需量
            DataInfo data_ZY2Demand = new DataInfo();
            data_ZY2Demand.InitParam(3, true, false);
            data_ZY2Demand.AddDataField(8, 0);
            _DataCodes.Add(data_ZY2Demand);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("01010200", 4), _DataCodes.Count - 1);
            //费率3费率需量
            DataInfo data_ZY3Demand = new DataInfo();
            data_ZY3Demand.InitParam(3, true, false);
            data_ZY3Demand.AddDataField(8, 0);
            _DataCodes.Add(data_ZY3Demand);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("01010300", 4), _DataCodes.Count - 1);
            //费率4费率需量
            DataInfo data_ZY4Demand = new DataInfo();
            data_ZY4Demand.InitParam(3, true, false);
            data_ZY4Demand.AddDataField(8, 0);
            _DataCodes.Add(data_ZY4Demand);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("01010400", 4), _DataCodes.Count - 1);



            #endregion ==== 表A.1 电能量数据标识编码表 ====
            #region ==== 表A.3 参变量数据标识编码表 ====

            //A相电压
            DataInfo data_Ua = new DataInfo();
            data_Ua.InitParam(3, true, false);
            data_Ua.AddDataField(2, 0);
            _DataCodes.Add(data_Ua);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("02010100", 4), _DataCodes.Count - 1);
            //B相电压
            DataInfo data_Ub = new DataInfo();
            data_Ub.InitParam(3, true, false);
            data_Ub.AddDataField(2, 0);
            _DataCodes.Add(data_Ub);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("02010200", 4), _DataCodes.Count - 1);
            //C相电压
            DataInfo data_Uc = new DataInfo();
            data_Uc.InitParam(3, true, false);
            data_Uc.AddDataField(2, 0);
            _DataCodes.Add(data_Uc);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("02010300", 4), _DataCodes.Count - 1);
            //A相电流
            DataInfo data_Ia = new DataInfo();
            data_Ia.InitParam(3, true, false);
            data_Ia.AddDataField(3, 0);
            _DataCodes.Add(data_Ia);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("02020100", 4), _DataCodes.Count - 1);
            //B相电流
            DataInfo data_Ib = new DataInfo();
            data_Ib.InitParam(3, true, false);
            data_Ib.AddDataField(3, 0);
            _DataCodes.Add(data_Ib);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("02020200", 4), _DataCodes.Count - 1);
            //C相电流
            DataInfo data_Ic = new DataInfo();
            data_Ic.InitParam(3, true, false);
            data_Ic.AddDataField(3, 0);
            _DataCodes.Add(data_Ic);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("02020300", 4), _DataCodes.Count - 1);
            //阶梯电价
            DataInfo data_MeterJtdj = new DataInfo();
            data_MeterJtdj.InitParam(3, true, false);
            data_MeterJtdj.AddDataField(4, 2);
            _DataCodes.Add(data_ZY4Energy);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("0280000B", 4), _DataCodes.Count - 1);

            //日期及星期
            DataInfo data_DateAndWeek = new DataInfo();
            data_DateAndWeek.InitParam(3, true, true);
            data_DateAndWeek.AddDataField(1, 0); //星期
            data_DateAndWeek.AddDataField(1, 0); //日
            data_DateAndWeek.AddDataField(1, 0); //月
            data_DateAndWeek.AddDataField(1, 0); //年
            _DataCodes.Add(data_DateAndWeek);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000101", 4), _DataCodes.Count - 1);
            //时间
            DataInfo data_Time = new DataInfo();
            data_Time.InitParam(3, true, true);
            data_Time.AddDataField(1, 0); //秒
            data_Time.AddDataField(1, 0); //分
            data_Time.AddDataField(1, 0); //时
            _DataCodes.Add(data_Time);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000102", 4), _DataCodes.Count - 1);
            //最大需量周期
            data_Time = new DataInfo();
            data_Time.InitParam(3, true, true);
            data_Time.AddDataField(1, 0);
            _DataCodes.Add(data_Time);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000103", 4), _DataCodes.Count - 1);
            //滑差时间
            data_Time = new DataInfo();
            data_Time.InitParam(3, true, true);
            data_Time.AddDataField(1, 0);
            _DataCodes.Add(data_Time);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000104", 4), _DataCodes.Count - 1);
            // 校表脉冲宽度
            data_Time = new DataInfo();
            data_Time.InitParam(3, true, false);
            data_Time.AddDataField(2, 0);
            _DataCodes.Add(data_Time);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000105", 4), _DataCodes.Count - 1);

            //两套时区表切换时间
            data_Time = new DataInfo();
            data_Time.InitParam(3, true, true);
            data_Time.AddDataField(1, 0);
            data_Time.AddDataField(1, 0);
            data_Time.AddDataField(1, 0);
            data_Time.AddDataField(1, 0);
            data_Time.AddDataField(1, 0);
            _DataCodes.Add(data_Time);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000106", 4), _DataCodes.Count - 1);
            //两套日时段表切换时间
            data_Time = new DataInfo();
            data_Time.InitParam(3, true, true);
            data_Time.AddDataField(1, 0);
            data_Time.AddDataField(1, 0);
            data_Time.AddDataField(1, 0);
            data_Time.AddDataField(1, 0);
            data_Time.AddDataField(1, 0);
            _DataCodes.Add(data_Time);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000107", 4), _DataCodes.Count - 1);

            //两套费率电价切换时间
            data_Time = new DataInfo();
            data_Time.InitParam(3, true, true);
            data_Time.AddDataField(5, 0);
            _DataCodes.Add(data_Time);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000108", 4), _DataCodes.Count - 1);
            //两套阶梯切换时间
            data_Time = new DataInfo();
            data_Time.InitParam(3, true, true);
            data_Time.AddDataField(5, 0);
            _DataCodes.Add(data_Time);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000109", 4), _DataCodes.Count - 1);


            //年时区数p≤14
            data_Time = new DataInfo();
            data_Time.InitParam(3, true, true);
            data_Time.AddDataField(1, 0);
            _DataCodes.Add(data_Time);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000201", 4), _DataCodes.Count - 1);
            //日时段表数q≤8
            data_Time = new DataInfo();
            data_Time.InitParam(3, true, true);
            data_Time.AddDataField(1, 0);
            _DataCodes.Add(data_Time);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000202", 4), _DataCodes.Count - 1);
            //日时段数(每日切换数) m≤14
            data_Time = new DataInfo();
            data_Time.InitParam(3, true, true);
            data_Time.AddDataField(1, 0);
            _DataCodes.Add(data_Time);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000203", 4), _DataCodes.Count - 1);
            //费率数k≤63
            data_Time = new DataInfo();
            data_Time.InitParam(3, true, true);
            data_Time.AddDataField(1, 0);
            _DataCodes.Add(data_Time);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000204", 4), _DataCodes.Count - 1);
            //公共假日数n≤254
            data_Time = new DataInfo();
            data_Time.InitParam(3, true, true);
            data_Time.AddDataField(2, 0);
            _DataCodes.Add(data_Time);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000205", 4), _DataCodes.Count - 1);
            //谐波分析次数
            data_Time = new DataInfo();
            data_Time.InitParam(3, true, true);
            data_Time.AddDataField(1, 0);
            _DataCodes.Add(data_Time);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000206", 4), _DataCodes.Count - 1);

            //阶梯数
            data_Time = new DataInfo();
            data_Time.InitParam(3, true, true);
            data_Time.AddDataField(1, 0);
            _DataCodes.Add(data_Time);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000207", 4), _DataCodes.Count - 1);

            //自动循环显示屏数
            data_Time = new DataInfo();
            data_Time.InitParam(3, true, true);
            data_Time.AddDataField(1, 0);
            _DataCodes.Add(data_Time);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000301", 4), _DataCodes.Count - 1);

            //每屏显示时间
            data_Time = new DataInfo();
            data_Time.InitParam(3, true, true);
            data_Time.AddDataField(1, 0);
            _DataCodes.Add(data_Time);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000302", 4), _DataCodes.Count - 1);

            //显示电能小数位数
            data_Time = new DataInfo();
            data_Time.InitParam(3, true, true);
            data_Time.AddDataField(1, 0);
            _DataCodes.Add(data_Time);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000303", 4), _DataCodes.Count - 1);

            //显示功率(最大需量)小数位数
            data_Time = new DataInfo();
            data_Time.InitParam(3, true, true);
            data_Time.AddDataField(1, 0);
            _DataCodes.Add(data_Time);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000304", 4), _DataCodes.Count - 1);

            //按键循环显示屏数
            data_Time = new DataInfo();
            data_Time.InitParam(3, true, true);
            data_Time.AddDataField(1, 0);
            _DataCodes.Add(data_Time);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000305", 4), _DataCodes.Count - 1);

            //电流互感器变比
            data_Time = new DataInfo();
            data_Time.InitParam(3, true, true);
            data_Time.AddDataField(3, 0);
            _DataCodes.Add(data_Time);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000306", 4), _DataCodes.Count - 1);
            //电压互感器变比
            data_Time = new DataInfo();
            data_Time.InitParam(3, true, true);
            data_Time.AddDataField(3, 0);
            _DataCodes.Add(data_Time);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000307", 4), _DataCodes.Count - 1);

            //通信地址
            data_Time = new DataInfo();
            data_Time.InitParam(3, true, true);
            data_Time.AddDataField(6, 0);
            _DataCodes.Add(data_Time);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000401", 4), _DataCodes.Count - 1);

            //表号
            data_Time = new DataInfo();
            data_Time.InitParam(3, true, true);
            data_Time.AddDataField(6, 0);
            _DataCodes.Add(data_Time);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000402", 4), _DataCodes.Count - 1);

            //资产管理编码(ASCII码)
            data_Time = new DataInfo();
            data_Time.InitParam(3, true, true);
            data_Time.AddDataField(32, 0);
            _DataCodes.Add(data_Time);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000403", 4), _DataCodes.Count - 1);

            //额定电压(ASCII码)
            data_Time = new DataInfo();
            data_Time.InitParam(3, true, false);
            data_Time.AddDataField(6, 0);
            _DataCodes.Add(data_Time);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000404", 4), _DataCodes.Count - 1);

            //额定电流/基本电流(ASCII码)
            data_Time = new DataInfo();
            data_Time.InitParam(3, true, false);
            data_Time.AddDataField(6, 0);
            _DataCodes.Add(data_Time);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000405", 4), _DataCodes.Count - 1);

            //最大电流(ASCII码)
            data_Time = new DataInfo();
            data_Time.InitParam(3, true, false);
            data_Time.AddDataField(6, 0);
            _DataCodes.Add(data_Time);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000406", 4), _DataCodes.Count - 1);
            //有功准确度等级(ASCII码)
            data_Time = new DataInfo();
            data_Time.InitParam(3, true, false);
            data_Time.AddDataField(4, 0);
            _DataCodes.Add(data_Time);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000407", 4), _DataCodes.Count - 1);
            //无功准确度等级(ASCII码)
            data_Time = new DataInfo();
            data_Time.InitParam(3, true, false);
            data_Time.AddDataField(4, 0);
            _DataCodes.Add(data_Time);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000408", 4), _DataCodes.Count - 1);
            //电表有功常数
            data_Time = new DataInfo();
            data_Time.InitParam(3, true, false);
            data_Time.AddDataField(3, 0);
            _DataCodes.Add(data_Time);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000409", 4), _DataCodes.Count - 1);

            //电表无功常数
            data_Time = new DataInfo();
            data_Time.InitParam(3, true, false);
            data_Time.AddDataField(3, 0);
            _DataCodes.Add(data_Time);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("0400040A", 4), _DataCodes.Count - 1);
            //电表型号(ASCII码)
            data_Time = new DataInfo();
            data_Time.InitParam(3, true, false);
            data_Time.AddDataField(10, 0);
            _DataCodes.Add(data_Time);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("0400040B", 4), _DataCodes.Count - 1);
            //生产日期(ASCII码)
            data_Time = new DataInfo();
            data_Time.InitParam(3, true, false);
            data_Time.AddDataField(10, 0);
            _DataCodes.Add(data_Time);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("0400040C", 4), _DataCodes.Count - 1);
            //协议版本号(ASCII码)
            data_Time = new DataInfo();
            data_Time.InitParam(3, true, false);
            data_Time.AddDataField(16, 0);
            _DataCodes.Add(data_Time);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("0400040D", 4), _DataCodes.Count - 1);

            //客户编号
            data_Time = new DataInfo();
            data_Time.InitParam(3, true, true);
            data_Time.AddDataField(6, 0);
            _DataCodes.Add(data_Time);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("0400040E", 4), _DataCodes.Count - 1);

            //运行状态
            //for (int m = 0; m < 254; m++)
            //{
            //    DataInfo data_RunState1 = new DataInfo();
            //    data_RunState1.InitParam(3, true, false);
            //    data_RunState1.AddDataField(2, 0);
            //    _DataCodes.Add(data_RunState1);
            //    _Pointer.AddData(DLTFun.HexStrToBytsDataA("040005" + (m + 1).ToString("X2"), 4), _DataCodes.Count - 1);
            //}
            //电表运行状态1
            DataInfo data_RunState1 = new DataInfo();
            data_RunState1.InitParam(3, true, false);
            data_RunState1.AddDataField(2, 0);
            _DataCodes.Add(data_RunState1);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000501", 4), _DataCodes.Count - 1);
            //电表运行状态2
            DataInfo data_RunState2 = new DataInfo();
            data_RunState2.InitParam(3, true, false);
            data_RunState2.AddDataField(2, 0);
            _DataCodes.Add(data_RunState2);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000502", 4), _DataCodes.Count - 1);
            //电表运行状态3
            DataInfo data_RunState3 = new DataInfo();
            data_RunState3.InitParam(3, true, false);
            data_RunState3.AddDataField(2, 0);
            _DataCodes.Add(data_RunState3);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000503", 4), _DataCodes.Count - 1);
            //电表运行状态4
            DataInfo data_RunState4 = new DataInfo();
            data_RunState4.InitParam(3, true, false);
            data_RunState4.AddDataField(2, 0);
            _DataCodes.Add(data_RunState4);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000504", 4), _DataCodes.Count - 1);
            //电表运行状态5
            DataInfo data_RunState5 = new DataInfo();
            data_RunState5.InitParam(3, true, false);
            data_RunState5.AddDataField(2, 0);
            _DataCodes.Add(data_RunState5);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000505", 4), _DataCodes.Count - 1);
            //电表运行状态6
            DataInfo data_RunState6 = new DataInfo();
            data_RunState6.InitParam(3, true, false);
            data_RunState6.AddDataField(2, 0);
            _DataCodes.Add(data_RunState6);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000506", 4), _DataCodes.Count - 1);
            //电表运行状态7
            DataInfo data_RunState7 = new DataInfo();
            data_RunState7.InitParam(3, true, false);
            data_RunState7.AddDataField(2, 0);
            _DataCodes.Add(data_RunState7);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000507", 4), _DataCodes.Count - 1);


            //有功组合方式特征字
             data_RunState7 = new DataInfo();
            data_RunState7.InitParam(3, true, true);
            data_RunState7.AddDataField(1, 0);
            _DataCodes.Add(data_RunState7);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000601", 4), _DataCodes.Count - 1);

            //无功组合方式1特征字
            data_RunState7 = new DataInfo();
            data_RunState7.InitParam(3, true, true);
            data_RunState7.AddDataField(1, 0);
            _DataCodes.Add(data_RunState7);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000602", 4), _DataCodes.Count - 1);
            //无功组合方式2特征字
            data_RunState7 = new DataInfo();
            data_RunState7.InitParam(3, true, true);
            data_RunState7.AddDataField(1, 0);
            _DataCodes.Add(data_RunState7);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000603", 4), _DataCodes.Count - 1);

            //调制型红外光口通信速率特征字
            for (int h = 0; h < 5; h++)
            {
                data_RunState7 = new DataInfo();
                data_RunState7.InitParam(3, true, true);
                data_RunState7.AddDataField(1, 0);
                _DataCodes.Add(data_RunState7);
                _Pointer.AddData(DLTFun.HexStrToBytsDataA("040007" + (h + 1).ToString("X2"), 4), _DataCodes.Count - 1);
            }
            //周休日特征字
            data_RunState7 = new DataInfo();
            data_RunState7.InitParam(3, true, true);
            data_RunState7.AddDataField(1, 0);
            _DataCodes.Add(data_RunState7);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000801", 4), _DataCodes.Count - 1);

            //周休日采用的日时段表号
            data_RunState7 = new DataInfo();
            data_RunState7.InitParam(3, true, true);
            data_RunState7.AddDataField(1, 0);
            _DataCodes.Add(data_RunState7);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000802", 4), _DataCodes.Count - 1);

            //负荷记录模式字
            data_RunState7 = new DataInfo();
            data_RunState7.InitParam(3, true, true);
            data_RunState7.AddDataField(1, 0);
            _DataCodes.Add(data_RunState7);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000901", 4), _DataCodes.Count - 1);
            //冻结数据模式字
            data_RunState7 = new DataInfo();
            data_RunState7.InitParam(3, true, true);
            data_RunState7.AddDataField(1, 0);
            _DataCodes.Add(data_RunState7);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000902", 4), _DataCodes.Count - 1);
            //写表冻结值 ：瞬时冻结
            DataInfo data_RunState8 = new DataInfo();
            data_RunState8.InitParam(3, true, true);
            data_RunState8.AddDataField(1, 0);
            _DataCodes.Add(data_RunState8);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000903", 4), _DataCodes.Count - 1);
            //写表冻结值 ：约定冻结数据模式字
            DataInfo data_RunState9 = new DataInfo();
            data_RunState9.InitParam(3, true, true);
            data_RunState9.AddDataField(1, 0);
            _DataCodes.Add(data_RunState9);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000904", 4), _DataCodes.Count - 1);
            //写表冻结值 ：整点冻结数据模式字
            DataInfo data_RunState10 = new DataInfo();
            data_RunState10.InitParam(3, true, true);
            data_RunState10.AddDataField(1, 0);
            _DataCodes.Add(data_RunState10);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000905", 4), _DataCodes.Count - 1);

            //写表冻结值 ：日冻结
            DataInfo data_RunState11 = new DataInfo();
            data_RunState11.InitParam(3, true, true);
            data_RunState11.AddDataField(1, 0);
            _DataCodes.Add(data_RunState11);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000906", 4), _DataCodes.Count - 1);

            //负荷记录起始时间
            data_RunState7 = new DataInfo();
            data_RunState7.InitParam(3, true, true);
            data_RunState7.AddDataField(1, 0);
            data_RunState7.AddDataField(1, 0);
            data_RunState7.AddDataField(1, 0);
            data_RunState7.AddDataField(1, 0);
            _DataCodes.Add(data_RunState7);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000A01", 4), _DataCodes.Count - 1);
            for (int u = 0; u < 6; u++)
            {
                data_RunState11 = new DataInfo();
                data_RunState11.InitParam(3, true, true);
                data_RunState11.AddDataField(2, 0);
                _DataCodes.Add(data_RunState11);
                _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000A" + (u + 2).ToString("X2"), 4), _DataCodes.Count - 1);
            }
            for (int l = 0; l < 3; l++)
            {
                data_RunState11 = new DataInfo();
                data_RunState11.InitParam(3, true, true);
                data_RunState11.AddDataField(1, 0);
                data_RunState11.AddDataField(1, 0);
                _DataCodes.Add(data_RunState11);
                _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000B" + (l + 1).ToString("X2"), 4), _DataCodes.Count - 1);
            }
            for (int r = 0; r < 10; r++)
            {
                data_RunState11 = new DataInfo();
                data_RunState11.InitParam(3, true, true);
                data_RunState11.AddDataField(4, 0);
                _DataCodes.Add(data_RunState11);
                _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000C" + (r + 1).ToString("X2"), 4), _DataCodes.Count - 1);
            }

            for (int y = 0; y < 12; y++)
            {
                data_RunState11 = new DataInfo();
                data_RunState11.InitParam(3, true, true);
                data_RunState11.AddDataField(2, 0);
                _DataCodes.Add(data_RunState11);
                _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000D" + (y + 1).ToString("X2"), 4), _DataCodes.Count - 1);
            }
            //正向有功功率上限值
            data_RunState11 = new DataInfo();
            data_RunState11.InitParam(3, true, true);
            data_RunState11.AddDataField(3, 0);
            _DataCodes.Add(data_RunState11);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000E01", 4), _DataCodes.Count - 1);

            //反向有功功率上限值
            data_RunState11 = new DataInfo();
            data_RunState11.InitParam(3, true, true);
            data_RunState11.AddDataField(3, 0);
            _DataCodes.Add(data_RunState11);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000E02", 4), _DataCodes.Count - 1);

            //电压上限值
            data_RunState11 = new DataInfo();
            data_RunState11.InitParam(3, true, true);
            data_RunState11.AddDataField(2, 0);
            _DataCodes.Add(data_RunState11);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000E03", 4), _DataCodes.Count - 1);


            //电压下限值
            data_RunState11 = new DataInfo();
            data_RunState11.InitParam(3, true, true);
            data_RunState11.AddDataField(2, 0);
            _DataCodes.Add(data_RunState11);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000E04", 4), _DataCodes.Count - 1);


            //报警电量1限值
            data_RunState11 = new DataInfo();
            data_RunState11.InitParam(3, true, true);
            data_RunState11.AddDataField(4, 0);
            _DataCodes.Add(data_RunState11);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000F01", 4), _DataCodes.Count - 1);

            //报警电量2限值
            data_RunState11 = new DataInfo();
            data_RunState11.InitParam(3, true, true);
            data_RunState11.AddDataField(4, 0);
            _DataCodes.Add(data_RunState11);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000F02", 4), _DataCodes.Count - 1);
            //囤积电量限值
            data_RunState11 = new DataInfo();
            data_RunState11.InitParam(3, true, true);
            data_RunState11.AddDataField(4, 0);
            _DataCodes.Add(data_RunState11);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000F03", 4), _DataCodes.Count - 1);
            //透支电量限值
            data_RunState11 = new DataInfo();
            data_RunState11.InitParam(3, true, true);
            data_RunState11.AddDataField(4, 0);
            _DataCodes.Add(data_RunState11);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04000F04", 4), _DataCodes.Count - 1);
            //报警金额1限值
            data_RunState11 = new DataInfo();
            data_RunState11.InitParam(3, true, true);
            data_RunState11.AddDataField(4, 0);
            _DataCodes.Add(data_RunState11);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04001001", 4), _DataCodes.Count - 1);
            //报警金额2限值
            data_RunState11 = new DataInfo();
            data_RunState11.InitParam(3, true, true);
            data_RunState11.AddDataField(4, 0);
            _DataCodes.Add(data_RunState11);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04001002", 4), _DataCodes.Count - 1);
            //透支金额限值
            data_RunState11 = new DataInfo();
            data_RunState11.InitParam(3, true, true);
            data_RunState11.AddDataField(4, 0);
            _DataCodes.Add(data_RunState11);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04001003", 4), _DataCodes.Count - 1);
            //囤积金额限值
            data_RunState11 = new DataInfo();
            data_RunState11.InitParam(3, true, true);
            data_RunState11.AddDataField(4, 0);
            _DataCodes.Add(data_RunState11);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04001004", 4), _DataCodes.Count - 1);
            //合闸允许金额限值
            data_RunState11 = new DataInfo();
            data_RunState11.InitParam(3, true, true);
            data_RunState11.AddDataField(4, 0);
            _DataCodes.Add(data_RunState11);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04001005", 4), _DataCodes.Count - 1);

            //电表运行特征字1
            data_RunState11 = new DataInfo();
            data_RunState11.InitParam(3, true, true);
            data_RunState11.AddDataField(4, 0);
            _DataCodes.Add(data_RunState11);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04001101", 4), _DataCodes.Count - 1);


            //写整点冻结起始时间 年月日时分
            DataInfo data_RunState14 = new DataInfo();
            data_RunState14.InitParam(3, true, true);
            data_RunState14.AddDataField(5, 0);
            _DataCodes.Add(data_RunState14);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04001201", 4), _DataCodes.Count - 1);

            //写整点冻结时间间隔 
            DataInfo data_RunState15 = new DataInfo();
            data_RunState15.InitParam(3, true, true);
            data_RunState15.AddDataField(1, 0);
            _DataCodes.Add(data_RunState15);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04001202", 4), _DataCodes.Count - 1);

            //日冻结时间
            data_RunState15 = new DataInfo();
            data_RunState15.InitParam(3, true, true);
            data_RunState15.AddDataField(2, 0);
            _DataCodes.Add(data_RunState15);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04001203", 4), _DataCodes.Count - 1);

            //无线通信在线及信号强弱指示
            data_RunState15 = new DataInfo();
            data_RunState15.InitParam(3, true, true);
            data_RunState15.AddDataField(1, 0);
            _DataCodes.Add(data_RunState15);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04001301", 4), _DataCodes.Count - 1);

            //跳闸延时时间
            DataInfo s4waitTime = new DataInfo();
            s4waitTime.InitParam(3, true, true);
            s4waitTime.AddDataField(2, 0);
            _DataCodes.Add(s4waitTime);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04001401", 4), _DataCodes.Count - 1);
            //合闸延时时间
            DataInfo waitoldTime = new DataInfo();
            waitoldTime.InitParam(3, true, true);
            waitoldTime.AddDataField(2, 0);
            _DataCodes.Add(waitoldTime);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04001402", 4), _DataCodes.Count - 1);
            //第一套时区表数据
            waitoldTime = new DataInfo();
            waitoldTime.InitParam(3, true, true);
            waitoldTime.AddDataField(14, 0);
            waitoldTime.AddDataField(14, 0);
            waitoldTime.AddDataField(14, 0);
            _DataCodes.Add(waitoldTime);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04010000", 4), _DataCodes.Count - 1);

            //第一套第1日时段表数据
            for (int e = 0; e < 8; e++)
            {
                waitoldTime = new DataInfo();
                waitoldTime.InitParam(3, true, true);
                waitoldTime.AddDataField(14, 0);
                waitoldTime.AddDataField(14, 0);
                waitoldTime.AddDataField(14, 0);
                _DataCodes.Add(waitoldTime);
                _Pointer.AddData(DLTFun.HexStrToBytsDataA("040100" + (e + 1).ToString("X2"), 4), _DataCodes.Count - 1);
            }

            //第二套时区表数据
            waitoldTime = new DataInfo();
            waitoldTime.InitParam(3, true, true);
            waitoldTime.AddDataField(14, 0);
            waitoldTime.AddDataField(14, 0);
            waitoldTime.AddDataField(14, 0);
            _DataCodes.Add(waitoldTime);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04020000", 4), _DataCodes.Count - 1);
            //第二套第1日时段表数据
            for (int f = 0; f < 8; f++)
            {
                waitoldTime = new DataInfo();
                waitoldTime.InitParam(3, true, true);
                waitoldTime.AddDataField(14, 0);
                waitoldTime.AddDataField(14, 0);
                waitoldTime.AddDataField(14, 0);
                _DataCodes.Add(waitoldTime);
                _Pointer.AddData(DLTFun.HexStrToBytsDataA("040200" + (f + 1).ToString("X2"), 4), _DataCodes.Count - 1);
            }
            for (int o = 0; o < 254; o++)
            {
                waitoldTime = new DataInfo();
                waitoldTime.InitParam(3, true, true);
                waitoldTime.AddDataField(1, 0);
                waitoldTime.AddDataField(1, 0);
                waitoldTime.AddDataField(1, 0);
                waitoldTime.AddDataField(1, 0);
                _DataCodes.Add(waitoldTime);
                _Pointer.AddData(DLTFun.HexStrToBytsDataA("040300" + (o + 1).ToString("X2"), 4), _DataCodes.Count - 1);
            }
            //自动循环显示
            for (int q = 0; q < 254; q++)
            {
                waitoldTime = new DataInfo();
                waitoldTime.InitParam(3, true, true);
                waitoldTime.AddDataField(4, 0);
                waitoldTime.AddDataField(1, 0);
                _DataCodes.Add(waitoldTime);
                _Pointer.AddData(DLTFun.HexStrToBytsDataA("040401" + (q + 1).ToString("X2"), 4), _DataCodes.Count - 1);
            }
            //按键循环显示
            for (int w = 0; w < 254; w++)
            {
                waitoldTime = new DataInfo();
                waitoldTime.InitParam(3, true, true);
                waitoldTime.AddDataField(4, 0);
                waitoldTime.AddDataField(1, 0);
                _DataCodes.Add(waitoldTime);
                _Pointer.AddData(DLTFun.HexStrToBytsDataA("040402" + (w + 1).ToString("X2"), 4), _DataCodes.Count - 1);
            }



            //第一二套阶梯电价
            for (int x = 0; x < 2; x++)
            {
                for (int k = 0; k < 63; k++)
                {
                    DataInfo feiludianjia = new DataInfo();
                    feiludianjia.InitParam(3, true, true);
                    feiludianjia.AddDataField(4, 0);
                    _DataCodes.Add(feiludianjia);
                    _Pointer.AddData(DLTFun.HexStrToBytsDataA("0405" + (x + 1).ToString("X2") + (k + 1).ToString("X2"), 4), _DataCodes.Count - 1);
                }
            }
            //第一二套阶梯值
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 254; j++)
                {
                    DataInfo jietis = new DataInfo();
                    jietis.InitParam(3, true, true);
                    jietis.AddDataField(4, 0);
                    _DataCodes.Add(jietis);
                    _Pointer.AddData(DLTFun.HexStrToBytsDataA("0406" + i.ToString("X2") + (j + 1).ToString("X2"), 4), _DataCodes.Count - 1);
                }
            }
            //失压事件电压触发上限
            waitoldTime = new DataInfo();
            waitoldTime.InitParam(3, true, true);
            waitoldTime.AddDataField(2, 0);
            _DataCodes.Add(waitoldTime);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04090101", 4), _DataCodes.Count - 1);
            //失压事件电压恢复下限
            waitoldTime = new DataInfo();
            waitoldTime.InitParam(3, true, true);
            waitoldTime.AddDataField(2, 0);
            _DataCodes.Add(waitoldTime);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04090102", 4), _DataCodes.Count - 1);
            //失压事件电流触发下限
            waitoldTime = new DataInfo();
            waitoldTime.InitParam(3, true, true);
            waitoldTime.AddDataField(3, 0);
            _DataCodes.Add(waitoldTime);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04090103", 4), _DataCodes.Count - 1);

            //失压事件判定延时时间
            waitoldTime = new DataInfo();
            waitoldTime.InitParam(3, true, true);
            waitoldTime.AddDataField(1, 0);
            _DataCodes.Add(waitoldTime);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04090104", 4), _DataCodes.Count - 1);

            //欠压事件电压触发上限
            waitoldTime = new DataInfo();
            waitoldTime.InitParam(3, true, true);
            waitoldTime.AddDataField(2, 0);
            _DataCodes.Add(waitoldTime);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04090201", 4), _DataCodes.Count - 1);

            //欠压事件判定延时时间
            waitoldTime = new DataInfo();
            waitoldTime.InitParam(3, true, true);
            waitoldTime.AddDataField(1, 0);
            _DataCodes.Add(waitoldTime);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04090202", 4), _DataCodes.Count - 1);

            //过压事件电压触发下限
            waitoldTime = new DataInfo();
            waitoldTime.InitParam(3, true, true);
            waitoldTime.AddDataField(2, 0);
            _DataCodes.Add(waitoldTime);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04090301", 4), _DataCodes.Count - 1);


            //过压事件判定延时时间
            waitoldTime = new DataInfo();
            waitoldTime.InitParam(3, true, true);
            waitoldTime.AddDataField(1, 0);
            _DataCodes.Add(waitoldTime);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04090302", 4), _DataCodes.Count - 1);

            //断相事件电压触发上限
            waitoldTime = new DataInfo();
            waitoldTime.InitParam(3, true, true);
            waitoldTime.AddDataField(2, 0);
            _DataCodes.Add(waitoldTime);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04090401", 4), _DataCodes.Count - 1);
            //断相事件电流触发上限
            waitoldTime = new DataInfo();
            waitoldTime.InitParam(3, true, true);
            waitoldTime.AddDataField(3, 0);
            _DataCodes.Add(waitoldTime);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04090402", 4), _DataCodes.Count - 1);
            //断相事件判定延时时间
            waitoldTime = new DataInfo();
            waitoldTime.InitParam(3, true, true);
            waitoldTime.AddDataField(1, 0);
            _DataCodes.Add(waitoldTime);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04090403", 4), _DataCodes.Count - 1);

            //电压不平衡率限值
            waitoldTime = new DataInfo();
            waitoldTime.InitParam(3, true, true);
            waitoldTime.AddDataField(2, 0);
            _DataCodes.Add(waitoldTime);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04090501", 4), _DataCodes.Count - 1);

            //电压不平衡率判定延时时间
            waitoldTime = new DataInfo();
            waitoldTime.InitParam(3, true, true);
            waitoldTime.AddDataField(1, 0);
            _DataCodes.Add(waitoldTime);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04090502", 4), _DataCodes.Count - 1);

            //电流不平衡率限值
            waitoldTime = new DataInfo();
            waitoldTime.InitParam(3, true, true);
            waitoldTime.AddDataField(2, 0);
            _DataCodes.Add(waitoldTime);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04090601", 4), _DataCodes.Count - 1);

            //电流不平衡率判定延时时间
            waitoldTime = new DataInfo();
            waitoldTime.InitParam(3, true, true);
            waitoldTime.AddDataField(1, 0);
            _DataCodes.Add(waitoldTime);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04090602", 4), _DataCodes.Count - 1);

            //失流事件电压触发下限
            waitoldTime = new DataInfo();
            waitoldTime.InitParam(3, true, true);
            waitoldTime.AddDataField(2, 0);
            _DataCodes.Add(waitoldTime);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04090701", 4), _DataCodes.Count - 1);
            //失流事件电流触发上限
            waitoldTime = new DataInfo();
            waitoldTime.InitParam(3, true, true);
            waitoldTime.AddDataField(3, 0);
            _DataCodes.Add(waitoldTime);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04090702", 4), _DataCodes.Count - 1);

            //失流事件电流触发下限
            waitoldTime = new DataInfo();
            waitoldTime.InitParam(3, true, true);
            waitoldTime.AddDataField(3, 0);
            _DataCodes.Add(waitoldTime);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04090703", 4), _DataCodes.Count - 1);

            //失流事件判定延时时间
            waitoldTime = new DataInfo();
            waitoldTime.InitParam(3, true, true);
            waitoldTime.AddDataField(1, 0);
            _DataCodes.Add(waitoldTime);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04090704", 4), _DataCodes.Count - 1);

            //过流事件电流触发下限
            waitoldTime = new DataInfo();
            waitoldTime.InitParam(3, true, true);
            waitoldTime.AddDataField(2, 0);
            _DataCodes.Add(waitoldTime);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04090801", 4), _DataCodes.Count - 1);

            //过流事件判定延时时间
            waitoldTime = new DataInfo();
            waitoldTime.InitParam(3, true, true);
            waitoldTime.AddDataField(1, 0);
            _DataCodes.Add(waitoldTime);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04090802", 4), _DataCodes.Count - 1);

            //断流事件电压触发下限
            waitoldTime = new DataInfo();
            waitoldTime.InitParam(3, true, true);
            waitoldTime.AddDataField(2, 0);
            _DataCodes.Add(waitoldTime);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04090901", 4), _DataCodes.Count - 1);
            //断流事件电流触发上限
            waitoldTime = new DataInfo();
            waitoldTime.InitParam(3, true, true);
            waitoldTime.AddDataField(3, 0);
            _DataCodes.Add(waitoldTime);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04090902", 4), _DataCodes.Count - 1);

            //断流事件判定延时时间
            waitoldTime = new DataInfo();
            waitoldTime.InitParam(3, true, true);
            waitoldTime.AddDataField(1, 0);
            _DataCodes.Add(waitoldTime);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04090903", 4), _DataCodes.Count - 1);

            //潮流反向事件有功功率触发下限
            waitoldTime = new DataInfo();
            waitoldTime.InitParam(3, true, true);
            waitoldTime.AddDataField(3, 0);
            _DataCodes.Add(waitoldTime);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04090A01", 4), _DataCodes.Count - 1);
            //潮流反向事件有功功率触发下限
            waitoldTime = new DataInfo();
            waitoldTime.InitParam(3, true, true);
            waitoldTime.AddDataField(1, 0);
            _DataCodes.Add(waitoldTime);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04090A02", 4), _DataCodes.Count - 1);

            //过载事件有功功率触发下限
            waitoldTime = new DataInfo();
            waitoldTime.InitParam(3, true, true);
            waitoldTime.AddDataField(3, 0);
            _DataCodes.Add(waitoldTime);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04090B01", 4), _DataCodes.Count - 1);

            //过载事件判定延时时间
            waitoldTime = new DataInfo();
            waitoldTime.InitParam(3, true, true);
            waitoldTime.AddDataField(1, 0);
            _DataCodes.Add(waitoldTime);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04090B02", 4), _DataCodes.Count - 1);

            //电压考核上限
            waitoldTime = new DataInfo();
            waitoldTime.InitParam(3, true, true);
            waitoldTime.AddDataField(2, 0);
            _DataCodes.Add(waitoldTime);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04090C01", 4), _DataCodes.Count - 1);

            //电压考核下限
            waitoldTime = new DataInfo();
            waitoldTime.InitParam(3, true, true);
            waitoldTime.AddDataField(2, 0);
            _DataCodes.Add(waitoldTime);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04090C02", 4), _DataCodes.Count - 1);
            //有功需量超限事件需量触发下限
            waitoldTime = new DataInfo();
            waitoldTime.InitParam(3, true, true);
            waitoldTime.AddDataField(3, 0);
            _DataCodes.Add(waitoldTime);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04090D01", 4), _DataCodes.Count - 1);

            //无功需量超限事件需量触发下限
            waitoldTime = new DataInfo();
            waitoldTime.InitParam(3, true, true);
            waitoldTime.AddDataField(3, 0);
            _DataCodes.Add(waitoldTime);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04090D02", 4), _DataCodes.Count - 1);

            //需量超限事件判定延时时间
            waitoldTime = new DataInfo();
            waitoldTime.InitParam(3, true, true);
            waitoldTime.AddDataField(1, 0);
            _DataCodes.Add(waitoldTime);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04090D03", 4), _DataCodes.Count - 1);


            //总功率因数超下限阀值
            waitoldTime = new DataInfo();
            waitoldTime.InitParam(3, true, true);
            waitoldTime.AddDataField(2, 0);
            _DataCodes.Add(waitoldTime);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04090E01", 4), _DataCodes.Count - 1);

            //总功率因数超下限判定延时时间
            waitoldTime = new DataInfo();
            waitoldTime.InitParam(3, true, true);
            waitoldTime.AddDataField(1, 0);
            _DataCodes.Add(waitoldTime);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04090E02", 4), _DataCodes.Count - 1);

            //电流严重不平衡限值
            waitoldTime = new DataInfo();
            waitoldTime.InitParam(3, true, true);
            waitoldTime.AddDataField(2, 0);
            _DataCodes.Add(waitoldTime);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04090F01", 4), _DataCodes.Count - 1);

            //电流严重不平衡触发延时时间
            waitoldTime = new DataInfo();
            waitoldTime.InitParam(3, true, true);
            waitoldTime.AddDataField(1, 0);
            _DataCodes.Add(waitoldTime);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("04090F02", 4), _DataCodes.Count - 1);

            //厂家软件版本号(ASCII码)
            //厂家硬件版本号(ASCII码)
            //厂家编号(ASCII码)
            for (int t = 0; t < 3; t++)
            {
                waitoldTime = new DataInfo();
                waitoldTime.InitParam(3, true, false);
                waitoldTime.AddDataField(32, 0);
                _DataCodes.Add(waitoldTime);
                _Pointer.AddData(DLTFun.HexStrToBytsDataA("048000" + (t + 1).ToString("X2"), 4), _DataCodes.Count - 1);
            }
            #endregion ==== 表A.5 参变量数据标识编码表 ====
            #region ==== 表A.6 安全认证专写数据标识编码表 ====
            /* 身份认证 */

            //密文1
            DataInfo data_Cryp1 = new DataInfo();
            data_Cryp1.InitParam(3, true, true);
            data_Cryp1.AddDataField(8, 0); //秒
            _DataCodes.Add(data_Cryp1);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("07000001", 4), _DataCodes.Count - 1);
            //随机数1
            DataInfo data_StochasticNum1 = new DataInfo();
            data_StochasticNum1.InitParam(3, true, true);
            data_StochasticNum1.AddDataField(8, 0); //秒
            _DataCodes.Add(data_StochasticNum1);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("07000002", 4), _DataCodes.Count - 1);
            //费率电价

            //分散因子
            DataInfo data_Gene = new DataInfo();
            data_Gene.InitParam(3, true, true);
            data_Gene.AddDataField(8, 0);
            _DataCodes.Add(data_Gene);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("07000003", 4), _DataCodes.Count - 1);


            //data_Gene = new DataInfo();
            //data_Gene.InitParam(3, true, true);
            //data_Gene.AddDataField(2, 0);
            //_DataCodes.Add(data_Gene);
            //_Pointer.AddData(DLTFun.HexStrToBytsDataA("070001FF", 4), _DataCodes.Count - 1);
            /* 控制命令密钥更新 */
            //密钥信息+MAC
            DataInfo data_CtlKeyInfo = new DataInfo();
            data_CtlKeyInfo.InitParam(3, true, true);
            data_CtlKeyInfo.AddDataField(4, 0); //MAC
            data_CtlKeyInfo.AddDataField(4, 0); //密钥信息
            _DataCodes.Add(data_CtlKeyInfo);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("07020101", 4), _DataCodes.Count - 1);
            //控制命令文件线路保护密钥
            DataInfo data_CtlFileKey = new DataInfo();
            data_CtlFileKey.InitParam(3, true, true);
            data_CtlFileKey.AddDataField(32, 0);
            _DataCodes.Add(data_CtlFileKey);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("07020102", 4), _DataCodes.Count - 1);
            /* 参数密钥更新 */
            //密钥信息+MAC
            DataInfo data_ParamKeyInfo = new DataInfo();
            data_ParamKeyInfo.InitParam(3, true, true);
            data_ParamKeyInfo.AddDataField(4, 0); //MAC
            data_ParamKeyInfo.AddDataField(4, 0); //密钥信息
            _DataCodes.Add(data_ParamKeyInfo);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("07020201", 4), _DataCodes.Count - 1);
            //参数更新文件线路保护密钥
            DataInfo data_ParamFileKey = new DataInfo();
            data_ParamFileKey.InitParam(3, true, true);
            data_ParamFileKey.AddDataField(32, 0);
            _DataCodes.Add(data_ParamFileKey);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("07020202", 4), _DataCodes.Count - 1);
            /* 远程身份认证密钥更新 */
            //密钥信息+MAC
            DataInfo data_CrypKeyInfo = new DataInfo();
            data_CrypKeyInfo.InitParam(3, true, true);
            data_CrypKeyInfo.AddDataField(4, 0); //MAC
            data_CrypKeyInfo.AddDataField(4, 0); //密钥信息
            _DataCodes.Add(data_CrypKeyInfo);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("07020301", 4), _DataCodes.Count - 1);
            //远程身份认证密钥
            DataInfo data_CrypFileKey = new DataInfo();
            data_CrypFileKey.InitParam(3, true, true);
            data_CrypFileKey.AddDataField(32, 0);
            _DataCodes.Add(data_CrypFileKey);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("07020302", 4), _DataCodes.Count - 1);
            /* 主控密钥更新 */
            //密钥信息+MAC
            DataInfo data_ZkKeyInfo = new DataInfo();
            data_ZkKeyInfo.InitParam(3, true, true);
            data_ZkKeyInfo.AddDataField(4, 0); //MAC
            data_ZkKeyInfo.AddDataField(4, 0); //密钥信息
            _DataCodes.Add(data_ZkKeyInfo);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("07020401", 4), _DataCodes.Count - 1);
            //远程身份认证密钥
            DataInfo data_ZkFileKey = new DataInfo();
            data_ZkFileKey.InitParam(3, true, true);
            data_ZkFileKey.AddDataField(32, 0);
            _DataCodes.Add(data_ZkFileKey);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("07020402", 4), _DataCodes.Count - 1);
            /* 数据回抄 */
            DataInfo data_EsamFile = new DataInfo();
            data_EsamFile.InitParam(3, true, true);
            data_EsamFile.AddDataField(2, 0); //数据长度
            data_EsamFile.AddDataField(2, 0); //起始地址
            data_EsamFile.AddDataField(2, 0); //文件标识
            data_EsamFile.AddDataField(2, 0); //目录标识
            _DataCodes.Add(data_EsamFile);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("07800101", 4), _DataCodes.Count - 1);

            #endregion ==== 表A.6 安全认证专写数据标识编码表 ====
            #region ==== 表A.9 自定义 ====
            //跳合闸、报警、保电功能控制
            DataInfo data_ControlFun = new DataInfo();
            data_ControlFun.InitParam(3, true, true);
            data_ControlFun.AddDataField(1, 0); //状态
            data_ControlFun.AddDataField(1, 0); //保留
            data_ControlFun.AddDataField(6, 0); //有效截止时间
            _DataCodes.Add(data_ControlFun);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("08000001", 4), _DataCodes.Count - 1);
            //多功能端子输出信号类别
            DataInfo data_ControlMult = new DataInfo();
            data_ControlMult.InitParam(3, true, true);
            data_ControlMult.AddDataField(1, 0); //脉冲类型
            _DataCodes.Add(data_ControlMult);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("08000002", 4), _DataCodes.Count - 1);
            //通讯地址
            DataInfo data_CommAddr = new DataInfo();
            data_CommAddr.InitParam(3, true, true);
            data_CommAddr.AddDataField(6, 0);
            _DataCodes.Add(data_CommAddr);
            _Pointer.AddData(DLTFun.HexStrToBytsDataA("08000003", 4), _DataCodes.Count - 1);
            #endregion ==== 自定义表A.9 ====

            #endregion
        }
        //初始化数据编码表
        private bool InitDataCode_RF()
        {
            int itemCount = 0;
            string itemKey = ""; //数据标识
            string itemPath = "";
            int fieldCount = 0;
            string fieldPath = "";
            try
            {
                _ConfigFile.Load();
                itemCount = _ConfigFile.GetChildCount("/DataCode", "Item");
                for (int i = 0; i < itemCount; i++)
                {
                    itemPath = String.Format("/DataCode/Item[{0}]", i + 1);
                    itemKey = _ConfigFile.GetAttributeValue(itemPath, "key");
                    DataInfo newData = new DataInfo();
                    newData.InitParam(
                        Convert.ToInt32(_ConfigFile.GetAttributeValue(itemPath, "sort")), //数据类型
                        Convert.ToBoolean(_ConfigFile.GetAttributeValue(itemPath, "isRead")), //是否可读
                        Convert.ToBoolean(_ConfigFile.GetAttributeValue(itemPath, "isWrite"))); //是否可写
                    fieldCount = _ConfigFile.GetChildCount(itemPath, "Field");
                    for (int j = 0; j < fieldCount; j++)
                    {
                        fieldPath = String.Format("{0}/Field[{1}]", itemPath, j + 1);
                        newData.AddDataField(
                            Convert.ToInt32(_ConfigFile.GetAttributeValue(fieldPath, "len")), //数据长度
                            Convert.ToInt32(_ConfigFile.GetAttributeValue(fieldPath, "multiple"))); //放大倍数
                    }
                    _DataCodes.Add(newData);
                    _Pointer.AddData(DLTFun.HexStrToBytsDataA(itemKey, 4), _DataCodes.Count - 1);
                }
                return true;
            }
            catch
            {
                CreateDefault();
                return false;
            }
        }
        //生成默认的文件
        private void CreateDefault() { }
    }
}
