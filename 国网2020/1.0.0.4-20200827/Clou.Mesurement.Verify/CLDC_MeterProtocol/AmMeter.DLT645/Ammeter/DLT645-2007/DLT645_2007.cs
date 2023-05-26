/*-----------------------------------------------------------------------------------
 * Copyright(C) 2011 深圳市科陆软件有限公司电测事业部
 * 文件名: DLT645_2007.cs
 * 文件功能描述: DL/T645-2007电能表协议
 * 创建标识: ShiHe 20110316
 * 修改标识:
 * 修改描述:
 *-----------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Text;
using CLDC.CLAT.Framework.Struct;
using CLDC_MeterProtocol.Ammeter.DLT645.Comm.Struct;
using CLDC_MeterProtocol.Ammeter.DLT645.Comm.Class;
using CLDC_MeterProtocol.Ammeter.DLT645.Comm.Const;
using CLDC_MeterProtocol.Ammeter.DLT645;

namespace CLDC_MeterProtocol.Ammeter.DLT6452007
{
    /// <summary>
    /// DL/T645-2007电能表协议
    /// </summary>
    public class DLT645_2007 : ProtocolBase, IAmmeterFun
    {
        private EmMeterType _MeterType;    //表类型
        private IDictionary _Dictionary;   //数据字典
        private IEncrypt _DataEncrypt;     //加密
        private byte _SKeyState;           //密钥状态[0-生产密钥 1-交易密钥]
        private byte[] _HandleCode;        //操作代码
        private byte[] _PassWord;          //密码
        private byte _PwClass;           //密码等级
        private string _DataCode;          //数据标识
        private bool _IsUseDeposit;        //是否开启缓存
        private FrameDeposit _Deposit;     //帧数据缓存
        private object _SyncLock = new object(); //锁
        /// <summary>
        /// 
        /// </summary>
        public DLT645_2007()
        {
            _Dictionary = new DLT645_2007_DataCode();
            _MeterType = EmMeterType.ExpControl;
            _Deposit = new FrameDeposit(10);
            _HandleCode = new byte[4];
            _PassWord = new byte[3];
            _IsUseDeposit = false;
            _SKeyState = 0;
            _DataEncrypt = null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="encrypt"></param>
        public DLT645_2007(IEncrypt encrypt)
        {
            _Dictionary = new DLT645_2007_DataCode();
            _MeterType = EmMeterType.ExpControl;
            _Deposit = new FrameDeposit(10);
            _HandleCode = new byte[4];
            _PassWord = new byte[3];
            _IsUseDeposit = false;
            _SKeyState = 0;
            _DataEncrypt = encrypt;
        }

        #region ---- 公共属性 ----
        /// <summary>
        /// 操作者代码
        /// </summary>
        public string HandleCode
        {
            get
            {
                return DLTFun.BytsToHexStrData(_HandleCode);
            }
            set
            {
                Array.Copy(DLTFun.HexStrToBytsData(value, 4), 0, _HandleCode, 0, 4);
            }
        }
        /// <summary>
        /// 密码
        /// </summary>
        public string PassWord
        {
            get
            {
                byte[] TmpPwArray = { _PwClass };
                return DLTFun.BytsToHexStrData(TmpPwArray) + DLTFun.BytsToHexStrData(_PassWord);
            }
            set
            {
                Array.Copy(DLTFun.HexStrToBytsData(value, 3), 0, _PassWord, 0, 3);
                _PwClass = DLTFun.HexStrToBytsData(value, 4)[3];
            }
        }
        /// <summary>
        /// 是否使用缓存
        /// </summary>
        public bool IsUseDeposit
        {
            get
            {
                return _IsUseDeposit;
            }
            set
            {
                _IsUseDeposit = value;
            }
        }
        /// <summary>
        /// 密钥状态[0-生产密钥 1-交易密钥]
        /// </summary>
        public byte SecretKeyState
        {
            get
            {
                return _SKeyState;
            }
            set
            {
                _SKeyState = value;
            }
        }
        /// <summary>
        /// 电能表类型
        /// </summary>
        public EmMeterType MeterType
        {
            get
            {
                return _MeterType;
            }
            set
            {
                _MeterType = value;
            }
        }
        /// <summary>
        /// 数据加密
        /// </summary>
        public IEncrypt DataEncrypt
        {
            get
            {
                return _DataEncrypt;
            }
            set
            {
                _DataEncrypt = value;
            }
        }
        #endregion ---- 公共属性 ----

        #region ---- 功能接口 ----
        #region ---- 读电能表数据 ----
        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <param name="dataCode">数据标识</param>
        /// <returns>帧长度</returns>
        public int ReadData(out byte[] frameData, string commAddr, string dataCode)
        {
            return AskFrame(out frameData, commAddr, dataCode, EmFunCode.ReadData, 0);
        }
        /// <summary>
        /// 读取通讯地址
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <returns>帧长度</returns>
        public int GetCommAddr(out byte[] frameData)
        {
            return AskFrame(out frameData, String.Intern("AAAAAAAAAAAA"), "", EmFunCode.ReadCommAddr, 0);
        }

        /// <summary>
        /// 写数据 - 2011-8-18 陈大伟
        /// </summary>
        /// <param name="frameData">返回帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <param name="dataCode">数据标识</param>
        /// <param name="strWriteData">写入数据</param>
        /// <returns>帧长度</returns>
        public int WriteData(out byte[] frameData, string commAddr, string dataCode, string strWriteData, int dataSort)
        {

            //1 准备
            int frameLen = 0;
            int intArrLen = 0;
            frameData = new byte[1];
            try
            {

                if (!(dataCode.Length == 8 || dataCode.Length == 2))//数据长度标示为8 ,2
                    return 0;
                //2 转换写入数据格式
                byte[] aryData;

                strWriteData = strWriteData.Length % 2 != 0 ? "0" + strWriteData : strWriteData;
                intArrLen = strWriteData.Length / 2;

                aryData = new byte[intArrLen];

                if (strWriteData.Length == 24 && (dataCode == "18" || dataCode == "00000018"))//密码修改
                {//DIODI1DI2DI3＋PAOP0OP1OP2O＋PANP0NP1NP2N  ：写入正常顺序数据格式：数据标示，老密码 ，新密码
                    //DIO DI1 DI2 DI3
                    for (int i = 0; i < 4; i++)
                    {
                        aryData[4 - i - 1] = Convert.ToByte(strWriteData.Substring(i * 2, 2), 16);
                    }

                    //老密码权限 PA
                    aryData[4] = Convert.ToByte(strWriteData.Substring(8, 2), 16);
                    //存放密码：OP0OP1OP2O
                    for (int i = 5; i < 8; i++)
                    {
                        aryData[8 - i - 1 + 5] = Convert.ToByte(strWriteData.Substring(i * 2, 2), 16);
                    }
                    //新密码权限
                    aryData[8] = Convert.ToByte(strWriteData.Substring(16, 2), 16);
                    for (int i = 9; i < 12; i++)
                    {
                        aryData[12 - i - 1 + 9] = Convert.ToByte(strWriteData.Substring(i * 2, 2), 16);
                    }
                }
                else
                {
                    for (int i = 0; i < intArrLen; i++)
                    {
                        aryData[intArrLen - i - 1] = Convert.ToByte(strWriteData.Substring(i * 2, 2), 16);
                    }
                }
                //3 组帧
                List<DataInfo> lstDatas = _Dictionary.Search(dataCode);

                if (lstDatas.Count > 0)
                {
                    lock (_SyncLock)
                    {
                        lstDatas[0].AryData = aryData;
                        frameLen = SearchFrame(out frameData, dataCode, commAddr, EmFunCode.WriteData, dataSort);
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }
            return frameLen;
        }
        /// <summary>
        /// 读取各费率电能及总电能
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <param name="enyAspect">电能方向[0-组合 1-正向 2-反向]</param>
        /// <param name="enyType">电能类型[0-视在 1-有功 2-无功]</param>
        /// <returns>帧长度</returns>
        public int GetFullEnergy(out byte[] frameData, string commAddr, byte enyAspect, byte enyType)
        {
            return GetFullEnergy(out frameData, commAddr, enyAspect, enyType, 0);
        }
        //backNum：帧序号
        public int GetFullEnergy(out byte[] frameData, string commAddr, byte enyAspect, byte enyType, byte backNum)
        {
            frameData = new byte[0];
            byte bytTemp = 0;
            string dataCode = "";
            if (enyAspect == 0 && enyType == 1) //组合有功
                bytTemp = 0x00;
            else if (enyAspect == 0 && enyType == 2) //组合无功
                bytTemp = 0x03;
            else if (enyAspect == 1 && enyType == 1) //正向有功
                bytTemp = 0x01;
            else if (enyAspect == 1 && enyType == 0) //正向视在
                bytTemp = 0x09;
            else if (enyAspect == 2 && enyType == 1) //反向有功
                bytTemp = 0x02;
            else if (enyAspect == 2 && enyType == 0) //反向视在
                bytTemp = 0x0A;
            dataCode = String.Format(String.Intern("00{0}FF00"), bytTemp.ToString("X2"));
            //dataCode = String.Format(String.Intern("00{0}0000"), bytTemp.ToString("X2"));
            if (backNum > 0)
                return SearchFrame(out frameData, dataCode, commAddr, EmFunCode.ReadBackData, EmPwdGrade._Grade_0, backNum);
            else
                return SearchFrame(out frameData, dataCode, commAddr, EmFunCode.ReadData, 0);
        }
        public int GetFunllDemand(out byte[] frameData, string commAddr, byte DmdAspect, byte DmdType)
        {
            return GetFullDemand(out frameData, commAddr, DmdAspect, DmdType, 0);
        }
        public int GetFullDemand(out byte[] frameData, string commAddr, byte DmdAspect, byte DmdType, byte backNum)
        {
            frameData = new byte[0];
            byte bytTemp = 0;
            string dataCode = "";

            if (DmdAspect == 1 && DmdType == 1) //正向有功总
                bytTemp = 0x01;
            else if (DmdAspect == 2 && DmdType == 1) //反向有功总
                bytTemp = 0x02;
            else if (DmdAspect == 0 && DmdType == 2) //组合无功1总
                bytTemp = 0x03;
            else if (DmdAspect == 0 && DmdType == 3) //组合无功2总
                bytTemp = 0x04;
            else if (DmdAspect == 1 && DmdType == 0) //正向视在总
                bytTemp = 0x09;
            else if (DmdAspect == 2 && DmdType == 0) //反向视在总
                bytTemp = 0x0A;
            dataCode = String.Format(String.Intern("01{0}FF00"), bytTemp.ToString("X2"));
            if (backNum > 0)
                return SearchFrame(out frameData, dataCode, commAddr, EmFunCode.ReadBackData, EmPwdGrade._Grade_0, backNum);
            else
                return SearchFrame(out frameData, dataCode, commAddr, EmFunCode.ReadData, 0);
        }
        /// <summary>
        /// 读取各相电压
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <returns>帧长度</returns>
        public int GetFullUValue(out byte[] frameData, string commAddr)
        {
            return AskFrame(out frameData, commAddr, "0201FF00", EmFunCode.ReadData, 0);
        }
        /// <summary>
        /// 读取各相电流
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <returns>帧长度</returns>
        public int GetFullIValue(out byte[] frameData, string commAddr)
        {
            return AskFrame(out frameData, commAddr, "0202FF00", EmFunCode.ReadData, 0);
        }
        /// <summary>
        /// 读取剩余金额
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <returns>帧长度</returns>
        public int GetSpareMoney(out byte[] frameData, string commAddr)
        {
            return AskFrame(out frameData, commAddr, "00900200", EmFunCode.ReadData, 0);
        }
        /// <summary>
        /// 读取剩余电量
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <returns>帧长度</returns>
        public int GetSpareEnergy(out byte[] frameData, string commAddr)
        {
            return AskFrame(out frameData, commAddr, "00900100", EmFunCode.ReadData, 0);
        }
        /// <summary>
        /// 读取年时区数
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <returns>帧长度</returns>
        public int GetTimeZoneCount(out byte[] frameData, string commAddr)
        {
            frameData = new byte[0];
            return 0;
        }
        /// <summary>
        /// 读取日时段表数
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <returns>帧长度</returns>
        public int GetDayPeriodListCount(out byte[] frameData, string commAddr)
        {
            frameData = new byte[0];
            return 0;
        }
        /// <summary>
        /// 读取日时段数
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <returns>帧长度</returns>
        public int GetDayPeriodCount(out byte[] frameData, string commAddr)
        {
            frameData = new byte[0];
            return 0;
        }
        /// <summary>
        /// 读取费率数
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <returns>帧长度</returns>
        public int GetTariffCount(out byte[] frameData, string commAddr)
        {
            frameData = new byte[0];
            return 0;
        }
        /// <summary>
        /// 读取阶梯数
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <returns>帧长度</returns>
        public int GetLadderCount(out byte[] frameData, string commAddr)
        {
            frameData = new byte[0];
            return 0;
        }
        /// <summary>
        /// 读取阶梯电价
        /// </summary>
        /// <param name="frameData">帧数据 0280000B</param>
        /// <param name="commAddr">通讯地址</param>
        /// <param name="tzListID">年时区表编号（范围：1-2）</param>
        /// <param name="ladderID">阶梯编号（范围：1-63）</param>
        /// <returns>帧长度</returns>
        public int GetLadderPrice(out byte[] frameData, string commAddr, byte tzListID, byte ladderID)
        {
            frameData = new byte[0];
            return 0;
        }

        /// <summary>
        /// 读当前阶梯电价 0280000B XXXX.XXXX  元/kWh
        /// </summary>
        /// <param name="frameData"></param>
        /// <param name="commAddr"></param>
        /// <returns></returns>
        public int GetCurrPrice(out byte[] frameData, string commAddr)
        {
            return AskFrame(out frameData, commAddr, "0280000B", EmFunCode.ReadData, 0);
        }

        /// <summary>
        /// 读取两套阶梯切换时间
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <returns>帧长度</returns>
        public int GetLadderSwitchTime(out byte[] frameData, string commAddr)
        {
            frameData = new byte[0];
            return 0;
        }
        /// <summary>
        /// 读取时区表数据
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <param name="tzListID">年时区表编号（范围：1-2）</param>
        /// <returns>帧长度</returns>
        public int GetTimeZoneList(out byte[] frameData, string commAddr, byte tzListID)
        {
            frameData = new byte[0];
            return 0;
        }
        /// <summary>
        /// 读取日时段表数据
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <param name="tzListID">年时区表编号（范围：1-2）</param>
        /// <param name="dpListID">日时段表编号（范围：1-8）</param>
        /// <returns>帧长度</returns>
        public int GetDayPeriodList(out byte[] frameData, string commAddr, byte tzListID, byte dpListID)
        {
            frameData = new byte[0];
            return 0;
        }
        /// <summary>
        /// 获取电能表运行状态
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <returns>帧长度</returns>
        public int GetRunState(out byte[] frameData, string commAddr)
        {
            return AskFrame(out frameData, commAddr, "040005FF", EmFunCode.ReadData, 0);
        }
        //runCode：运行状态字（0-数据块）
        public int GetRunState(out byte[] frameData, string commAddr, byte runCode)
        {
            string dataCode = "";
            if (runCode == 0) //数据块
                dataCode = String.Intern("040005FF");
            else
                dataCode = String.Format("040005{0}", runCode.ToString("X2"));
            return AskFrame(out frameData, commAddr, dataCode, EmFunCode.ReadData, 0);
        }
        /// <summary>身份认证指令
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <param name="cryptograph1">密文1</param>
        /// <param name="sticNumber1">随机数1(主站)</param>
        /// <param name="sptGene">分散因子</param>
        /// <returns>帧长度</returns>
        public int StatusAtic(out byte[] frameData, string commAddr, string cryptograph1, string sticNumber1, string sptGene)
        {
            int frameLen = 0;
            frameData = new byte[0];
            string dataCode = String.Intern("070000FF");
            List<DataInfo> lstDatas = _Dictionary.Search(dataCode);
            if (lstDatas.Count > 0)
            {
                lock (_SyncLock)
                {
                    lstDatas[0].AryData = DLTFun.HexStrToBytsData(cryptograph1, 8); //密文
                    lstDatas[1].AryData = DLTFun.HexStrToBytsData(sticNumber1, 8); //随机数
                    lstDatas[2].AryData = DLTFun.HexStrToBytsData(sptGene, 8); //分散因子
                    frameLen = AskFrame(out frameData, commAddr, dataCode, EmFunCode.SafetyAtic, 0);
                }
            }
            return frameLen;
        }
        /// <summary>回抄ESAM模块中文件数据
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <param name="fileType">文件类型[0-密钥文件 2-参数信息文件 5-密钥信息文件 6-运行信息文件 7-控制命令文件]</param>
        /// <returns>帧长度</returns>
        public int GetEsamFile(out byte[] frameData, string commAddr, byte fileType)
        {
            int frameLen = 0;
            short dataLen = 0; //数据长度
            short startAddr = 0; //起始地址
            string fileCode = ""; //文件标识
            frameData = new byte[0];
            string dataCode = String.Intern("078001FF");
            List<DataInfo> lstDatas = _Dictionary.Search(dataCode);
            if (lstDatas.Count > 0)
            {
                lock (_SyncLock)
                {
                    byte[] bytsTemp = new byte[8];
                    switch (fileType)
                    {
                        case 6: //运行信息文件
                            startAddr = 0;
                            dataLen = 4;
                            fileCode = "0006";
                            break;
                    }
                    Array.Copy(BitConverter.GetBytes(dataLen), 0, bytsTemp, 0, 2); //数据长度
                    Array.Copy(BitConverter.GetBytes(startAddr), 0, bytsTemp, 2, 2); //起始地址
                    Array.Copy(DLTFun.HexStrToBytsData(fileCode, 2), 0, bytsTemp, 4, 2); //文件标识
                    Array.Copy(DLTFun.HexStrToBytsData("DF01", 2), 0, bytsTemp, 6, 2); //目录标识
                    lstDatas[0].AryData = bytsTemp;
                    frameLen = AskFrame(out frameData, commAddr, dataCode, EmFunCode.SafetyAtic, 0);
                }
            }
            return frameLen;
        }


        #endregion ---- 读电能表数据 ----

        #region ---- 写电能表数据 ---
        /// <summary>设置时间指令
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <param name="ctyTime">时间</param>
        /// <returns>帧长度</returns>
        public int SetTime(out byte[] frameData, string commAddr, DateTime ctyTime, int dataSort)
        {
            int frameLen = 0;
            frameData = new byte[0];
            string dataCode = String.Intern("04000102");
            byte[] aryData = new byte[3];
            aryData[0] = Convert.ToByte(ctyTime.Second.ToString(), 16); //秒
            aryData[1] = Convert.ToByte(ctyTime.Minute.ToString(), 16); //分
            aryData[2] = Convert.ToByte(ctyTime.Hour.ToString(), 16); //时
            List<DataInfo> lstDatas = _Dictionary.Search(dataCode);
            if (lstDatas.Count > 0)
            {
                lock (_SyncLock)
                {
                    lstDatas[0].AryData = aryData;
                    frameLen = SearchFrame(out frameData, dataCode, commAddr, EmFunCode.WriteData, dataSort);
                }
            }
            return frameLen;
        }
        /// <summary>设置日期和星期指令
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <param name="ctyDate">日期及星期</param>
        /// <returns>帧长度</returns>
        public int SetDateAndWeek(out byte[] frameData, string commAddr, DateTime ctyDate, int dataSort)
        {
            int frameLen = 0;
            frameData = new byte[0];
            string dataCode = String.Intern("04000101");
            byte[] aryData = new byte[4];
            aryData[0] = Convert.ToByte(((int)ctyDate.DayOfWeek).ToString(), 16); //星期
            aryData[1] = Convert.ToByte(ctyDate.Day.ToString(), 16); //日
            aryData[2] = Convert.ToByte(ctyDate.Month.ToString(), 16); //月
            aryData[3] = Convert.ToByte((ctyDate.Year % 100).ToString(), 16); //年
            List<DataInfo> lstDatas = _Dictionary.Search(dataCode);
            if (lstDatas.Count > 0)
            {
                lock (_SyncLock)
                {
                    lstDatas[0].AryData = aryData;
                    frameLen = SearchFrame(out frameData, dataCode, commAddr, EmFunCode.WriteData, EmPwdGrade._Grade_2, dataSort);
                }
            }
            return frameLen;
        }

        /// <summary>跳合闸延时时间
        /// </summary>
        /// <param name="frameData"></param>
        /// <param name="commAddr"></param>
        /// <param name="ctyTime"></param>
        /// <returns></returns>
        public int SetPullSwitchTime(out byte[] frameData, string commAddr, byte cltype, byte times, int dataSort)
        {
            int frameLen = 0;
            frameData = new byte[0];
            string dataCode;
            if (cltype == 0)//跳闸延时时间 
                dataCode = String.Intern("04001401");
            else
                dataCode = String.Intern("04001402");
            byte[] aryData = new byte[2];
            aryData = DLTFun.HexStrToBytsData(times.ToString(), 2);
            List<DataInfo> lstDatas = _Dictionary.Search(dataCode);
            if (lstDatas.Count > 0)
            {
                lock (_SyncLock)
                {
                    lstDatas[0].AryData = aryData;
                    frameLen = SearchFrame(out frameData, dataCode, commAddr, EmFunCode.WriteData, dataSort);
                }
            }
            return frameLen;
        }
        /// <summary>设置阶梯电量
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <param name="tzListID">年时区表编号（范围：1-2）</param>
        /// <param name="ladderID">阶梯编号（范围：1-63）</param>
        /// <param name="enyValue">电量值（kWh）</param>
        /// <returns>帧长度</returns>
        public int SetLadderEnergy(out byte[] frameData, string commAddr, byte tzListID, byte ladderID, float enyValue)
        {
            frameData = new byte[0];
            return 0;
        }
        /// <summary>设置费率电价
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <param name="tzListID">年时区表编号（范围：1-2）</param>
        /// <param name="tariffID">费率编号（范围：1-63）</param>
        /// <param name="prcValue">电价（元）</param>
        /// <returns>帧长度</returns>
        public int SetTariffPrice(out byte[] frameData, string commAddr, byte tzListID, byte tariffID, float prcValue, int dataSort)
        {
            int frameLen = 0;
            frameData = new byte[0];
            string dataCode = String.Format("0405{0}{1}", tzListID.ToString("X2"), tariffID.ToString("X2"));
            List<DataInfo> lstDatas = _Dictionary.Search(dataCode);
            byte[] aryData = DLTFun.HexStrToBytsData(((prcValue * 10000) + ""), 4);
            if (lstDatas.Count > 0)
            {
                lock (_SyncLock)
                {
                    lstDatas[0].AryData = aryData;
                    frameLen = SearchFrame(out frameData, dataCode, commAddr, EmFunCode.WriteData, dataSort);
                }
            }
            return frameLen;
        }
        /// <summary>设置年时区数
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <param name="count">时区数（范围：1-14）</param>
        /// <returns>帧长度</returns>
        public int SetTimeZoneCount(out byte[] frameData, string commAddr, byte count)
        {
            frameData = new byte[0];
            return 0;
        }
        /// <summary>设置日时段表数
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <param name="count">日时段表数（范围：1-8）</param>
        /// <returns>帧长度</returns>
        public int SetDayPeriodListCount(out byte[] frameData, string commAddr, byte count)
        {
            frameData = new byte[0];
            return 0;
        }
        /// <summary>设置日时段数
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <param name="count">日时段数（范围：1-14）</param>
        /// <returns>帧长度</returns>
        public int SetDayPeriodCount(out byte[] frameData, string commAddr, byte count)
        {
            frameData = new byte[0];
            return 0;
        }
        /// <summary>设置费率数
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <param name="count">费率数（范围：1-63）</param>
        /// <returns>帧长度</returns>
        public int SetTariffCount(out byte[] frameData, string commAddr, byte count)
        {
            frameData = new byte[0];
            return 0;
        }
        /// <summary>设置阶梯数
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <param name="count">阶梯数（范围：1-63）</param>
        /// <returns>帧长度</returns>
        public int SetLadderCount(out byte[] frameData, string commAddr, byte count)
        {
            frameData = new byte[0];
            return 0;
        }
        /// <summary>设置年时区表数据
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <param name="tzListID">年时区表编号（范围：1-2）</param>
        /// <param name="tZones">年时区数据列表（L=年时区数；范围：1-14）</param>
        /// <returns>帧长度</returns>
        public int SetTimeZoneList(out byte[] frameData, string commAddr, byte tzListID, List<StTimeZone> tZones)
        {
            frameData = new byte[0];
            return 0;
        }
        /// <summary>设置日时段表数据
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <param name="tzListID">年时区表编号（范围：1-2）</param>
        /// <param name="dpListID">日时段表编号（范围：1-8）</param>
        /// <param name="dPeriods">日时段数据列表（L=日时段数；范围：1-14）</param>
        /// <returns>帧长度</returns>
        public int SetDayPeriodList(out byte[] frameData, string commAddr, byte tzListID, byte dpListID, List<StDayPeriod> dPeriods)
        {
            frameData = new byte[0];
            return 0;
        }

        /// <summary>校准准备指令
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <returns>帧长度</returns>
        public int GetAdjustPrepare(out byte[] frameData, string commAddr)
        {
            byte[] dataRegion = new byte[14];
            dataRegion[0] = 0xF2;
            dataRegion[1] = 0xFF;
            dataRegion[2] = (byte)EmPwdGrade._Grade_0; //权限
            Array.Copy(_PassWord, 0, dataRegion, 3, 3); //密码
            DataRegionDis(dataRegion, true); //+33H
            return GetFrameData(out frameData, DLTFun.HexStrToBytsData(commAddr, 6), 0x04, dataRegion);
        }
        /// <summary>校准电能表指令
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <param name="adjParam">校准参数</param>
        /// <returns>帧长度</returns>
        public int GetAdjustMeter(out byte[] frameData, string commAddr, StAdjustParam adjParam)
        {
            byte[] dataRegion = new byte[72];
            dataRegion[0] = 0xF0;
            dataRegion[1] = 0xFF;
            dataRegion[2] = (byte)EmPwdGrade._Grade_0; //权限
            Array.Copy(_PassWord, 0, dataRegion, 3, 3); //密码
            Array.Copy(BitConverter.GetBytes(adjParam.PulseCst_P), 0, dataRegion, 6, 4);
            Array.Copy(BitConverter.GetBytes(adjParam.PulseCst_Q), 0, dataRegion, 10, 4);
            Array.Copy(BitConverter.GetBytes(adjParam.HFConst), 0, dataRegion, 14, 2);
            Array.Copy(BitConverter.GetBytes((short)(adjParam.Basic_U * 10)), 0, dataRegion, 16, 2);
            Array.Copy(BitConverter.GetBytes((short)(adjParam.Basic_I * 1000)), 0, dataRegion, 18, 2);
            Array.Copy(BitConverter.GetBytes((int)(adjParam.StdPower_U * 1000)), 0, dataRegion, 20, 4);
            Array.Copy(BitConverter.GetBytes((int)(adjParam.StdPower_I1 * 10000)), 0, dataRegion, 24, 4);
            Array.Copy(BitConverter.GetBytes((int)(adjParam.StdPower_I2 * 10000)), 0, dataRegion, 28, 4);
            Array.Copy(BitConverter.GetBytes((int)(adjParam.StdPower_P1 * 100000)), 0, dataRegion, 32, 4);
            Array.Copy(BitConverter.GetBytes((int)(adjParam.StdPower_P2 * 100000)), 0, dataRegion, 36, 4);
            Array.Copy(BitConverter.GetBytes(adjParam.I1Offset), 0, dataRegion, 40, 4);
            Array.Copy(BitConverter.GetBytes(adjParam.I2Offset), 0, dataRegion, 44, 4);
            Array.Copy(BitConverter.GetBytes(adjParam.Power1Offset), 0, dataRegion, 48, 4);
            Array.Copy(BitConverter.GetBytes(adjParam.Power2Offset), 0, dataRegion, 52, 4);
            Array.Copy(BitConverter.GetBytes(adjParam.Phase1Repair), 0, dataRegion, 56, 4);
            Array.Copy(BitConverter.GetBytes(adjParam.Phase2Repair), 0, dataRegion, 60, 4);
            Array.Copy(BitConverter.GetBytes(adjParam.Q1Repair), 0, dataRegion, 64, 4);
            Array.Copy(BitConverter.GetBytes(adjParam.Q2Repair), 0, dataRegion, 68, 4);
            DataRegionDis(dataRegion, true); //+33H
            return GetFrameData(out frameData, DLTFun.HexStrToBytsData(commAddr, 6), 0x04, dataRegion);
        }

        /// <summary>控制电能表状态
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <param name="ctlType">状态[0-跳闸 1-合闸 2-报警 3-报警解除 4-保电 5-保电解除]</param>
        /// <param name="alyTime">有效截止时间</param>
        /// <returns>帧长度</returns>
        public int ControlState(out byte[] frameData, string commAddr, byte ctlType, DateTime alyTime)
        {
            int frameLen = 0;
            frameData = new byte[0];
            string dataCode = String.Intern("08000001");
            byte[] aryData = new byte[8];
            switch (ctlType)
            {
                case 0: //跳闸
                    aryData[0] = 0x1A;
                    break;
                case 1: //合闸
                    aryData[0] = 0x1B;
                    break;
                case 2: //报警
                    aryData[0] = 0x2A;
                    break;
                case 3: //报警解除
                    aryData[0] = 0x2B;
                    break;
                case 4: //保电
                    aryData[0] = 0x3A;
                    break;
                case 5: //保电解除
                    aryData[0] = 0x3B;
                    break;
                case 6://华立清零
                    aryData[0] = 0xEA;
                    break;
                case 7://华立需量清零
                    aryData[0] = 0xEB;
                    break;
            }
            aryData[2] = Convert.ToByte(alyTime.Second.ToString(), 16); //秒
            aryData[3] = Convert.ToByte(alyTime.Minute.ToString(), 16); //分钟
            aryData[4] = Convert.ToByte(alyTime.Hour.ToString(), 16); //时
            aryData[5] = Convert.ToByte(alyTime.Day.ToString(), 16); //日
            aryData[6] = Convert.ToByte(alyTime.Month.ToString(), 16); //月
            aryData[7] = Convert.ToByte((alyTime.Year % 100).ToString(), 16); //年
            List<DataInfo> lstDatas = _Dictionary.Search(dataCode);
            if (lstDatas.Count > 0)
            {
                //lock (_SyncLock)
                //{
                lstDatas[0].AryData = aryData;
                frameLen = AskFrame(out frameData, commAddr, dataCode, EmFunCode.ControlFun, 0);
                //}
            }
            return frameLen;
        }
        /// <summary>控制多功能端子输出
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <param name="pulseType">脉冲类型0-时钟秒脉冲 1-需量周期 2-时段投切</param>
        /// <returns>帧长度</returns>
        public int ControlMultPulse(out byte[] frameData, string commAddr, byte pulseType)
        {
            byte[] dataRegion = new byte[1];
            dataRegion[0] = pulseType;
            DataRegionDis(dataRegion, true); //+33H
            return GetFrameData(out frameData, DLTFun.HexStrToBytsData(commAddr, 6), 0x1D, dataRegion);
        }
        /// <summary>控制电表清零
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <returns>帧长度</returns>  //Yonsion  Edit 
        public int ControlMeterClear(out byte[] frameData, string commAddr)
        {
            string DataCode = "";
            int frameLen = 0;
            frameData = new byte[0];
            lock (_SyncLock)
            {
                frameLen = SearchFrame(out frameData, DataCode, commAddr, EmFunCode.MeterClear, 0);
            }
            return frameLen;
        }
        /// <summary>最大需量指令清零
        /// </summary>
        /// <param name="frameData">数据帧</param>
        /// <param name="commAddr">通讯地址</param>
        /// <returns></returns>   //Yonsion  Edit 
        public int ControlMaxDemandClear(out byte[] frameData, string commAddr)
        {
            string DataCode = "";
            int frameLen = 0;
            frameData = new byte[0];
            lock (_SyncLock)
            {
                frameLen = SearchFrame(out frameData, DataCode, commAddr, EmFunCode.MaxDemandClear, 0);
            }
            return frameLen;
        }
        #endregion ---- 写电能表数据 ----

        /// <summary>回抄ESAM模块中密钥信息文件数据
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <param name="fileType">文件类型[0-密钥文件 2-参数信息文件 5-密钥信息文件 6-运行信息文件 7-控制命令文件]</param>
        /// <returns>帧长度</returns>
        public byte[] GetEsamFileData(string commAddr)
        {
            return WeaveDataFrame("03", "078001FF", commAddr, "DF01000600000004", true, false, false);
        }
        ///----------
        /// <summary>更新密钥  1
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <param name="keyType">密钥类型[1-控制命令密钥 2-参数密钥 3-远程身份认证密钥 4-主控密钥]</param>
        /// <param name="keyInfo">密钥信息</param>
        /// <param name="macAddr">MAC地址</param>
        /// <param name="keyData">密钥数据</param>
        /// <returns>帧长度</returns>
        public int UpdateKey(out byte[] frameData, string commAddr, byte keyType, string keyInfo, string macAddr, string keyData)
        {
            int frameLen = 0;
            frameData = new byte[0];
            string dataCode = String.Format(String.Intern("0702{0}FF"), keyType.ToString("X2"));
            List<DataInfo> lstDatas = _Dictionary.Search(dataCode);
            if (lstDatas.Count > 0)
            {
                lock (_SyncLock)
                {
                    byte[] bytsTemp = new byte[8];
                    Array.Copy(HexStrToBytsData(macAddr, 4), 0, bytsTemp, 0, 4); //MAC
                    Array.Copy(HexStrToBytsData(keyInfo, 4), 0, bytsTemp, 4, 4); //密钥信息
                    lstDatas[0].AryData = bytsTemp;
                    lstDatas[1].AryData = HexStrToBytsData(keyData, 32); //文件线路保护密钥或远程身份认证密钥
                    frameLen = AskFrame(out frameData, commAddr, dataCode, EmFunCode.SafetyAtic, 0);
                }
            }
            return frameLen;
        }
        /// <summary>
        /// 密钥清零  1
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <param name="macAddr">MAC地址</param>
        /// <param name="keyData">密钥数据</param>
        /// <returns>帧长度</returns>
        public int ClearKey(out byte[] frameData, string commAddr, string macAddr, string keyData)
        {
            int frameLen = 0;
            frameData = new byte[0];
            string dataCode = String.Intern("070201FF");
            List<DataInfo> lstDatas = _Dictionary.Search(dataCode);
            if (lstDatas.Count > 0)
            {
                lock (_SyncLock)
                {
                    byte[] bytsTemp = new byte[8];
                    Array.Copy(HexStrToBytsData("00000000", 4), 0, bytsTemp, 0, 4); //密钥信息
                    Array.Copy(HexStrToBytsData(macAddr, 4), 0, bytsTemp, 4, 4); //MAC
                    lstDatas[0].AryData = bytsTemp;
                    lstDatas[1].AryData = HexStrToBytsData(keyData, 32); //文件线路保护密钥
                    frameLen = AskFrame(out frameData, commAddr, dataCode, EmFunCode.SafetyAtic, 0);
                }
            }
            return frameLen;
        }
        /// <summary>
        /// 组帧(密钥下装)
        /// </summary>
        /// <param name="keyNo">下装密钥类型</param>
        /// <param name="keyInfo">密钥信息</param>
        /// <param name="macData">MAC</param>
        /// <param name="keyData">密钥数据</param>
        /// <returns>密钥下装命令帧</returns>
        public byte[] WeaveFrame_UpdateKey(int keyNo, string commAddr, string keyInfo, string macData, string keyData)
        {
            string _DataDomain = string.Format("{0:S}{1:S}|{2:S}", keyInfo, macData, keyData); //数据域
            lock (_SyncLock)
            {
                switch (keyNo)
                {
                    case 3: //参数更新密钥
                        return WeaveDataFrame("03", "070202FF", commAddr, _DataDomain, true, false, true);
                    case 1: //身份认证密钥
                        return WeaveDataFrame("03", "070203FF", commAddr, _DataDomain, true, false, true);
                    case 0: //主控密钥
                        return WeaveDataFrame("03", "070204FF", commAddr, _DataDomain, true, false, true);
                    default:
                        return WeaveDataFrame("03", "070201FF", commAddr, _DataDomain, true, false, true);
                }
            }
        }
        /// <summary>
        /// 组合数据帧
        /// </summary>
        /// <param name="cmdCode">命令控制码</param>
        /// <param name="dataCode">数据标识符</param>
        /// <param name="dataDomain">数据域</param>
        /// <param name="isUserCode">是否带操作者代码</param>
        /// <param name="isPwd">是否带密码</param>
        /// <param name="isBlock">是否多块数据(是则数据dataDomain将使用|分割)</param>
        /// <returns>命令帧</returns>
        private byte[] WeaveDataFrame(string cmdCode, string dataCode, string commAddr, string dataDomain,
            bool isUserCode, bool isPwd, bool isBlock)
        {
            string tmpDataCode = "";    //数据标识DI3 DI2 DI1 DI0
            string tmpPassword = "";    //密级+密码PA P0 P1 P2
            string tmpUserCode = "";    //操作者代码C3 C2 C1 C0
            string tmpDataDomain = "";  //数据域
            string cmdText = string.Format("68{0:S}68{1:S}", TurnoverData(commAddr),
                FormatData(cmdCode, 2)/*命令码2位(1个字节)*/);
            if (dataCode.Trim() != "") //数据标识符
                tmpDataCode = TurnoverDataAfterAdd33H(dataCode);

            if (dataDomain.Trim() != "") //数据域具体内容组合
            {
                if (isBlock)//如果是块,则需要分割(因为传进来的块要求用管道符号分割)
                {
                    string[] tmpData = dataDomain.Split('|');
                    for (int i = 0; i < tmpData.Length; i++)
                    {
                        tmpDataDomain += TurnoverDataAfterAdd33H(tmpData[i]);
                    }
                }
                else//不是块,则数据域就直接+0x33翻转
                {
                    tmpDataDomain = TurnoverDataAfterAdd33H(dataDomain);
                }
            }

            if (isUserCode) //如果数据域中包括操作者代码
                tmpUserCode = TurnoverDataAfterAdd33H("12345678");

            //if (isPwd)  //如果数据域中包括密码
            //    tmpPassword = TurnoverDataAfterAdd33H(_WriteLvl) + TurnoverDataAfterAdd33H(_WritePwd);

            dataDomain = tmpDataCode + tmpPassword + tmpUserCode + tmpDataDomain;//组合数据域
            int halfLength = (int)dataDomain.Length / 2; //数据域长度
            cmdText += string.Format("{0:X2}{1:S}", halfLength, dataDomain); //数据域
            cmdText += string.Format("{0:X2}16", GetCmdFrameCS(cmdText));  //加入校验码

            return FormatHexToBinary(ref cmdText, 4); //返回数据帧
        }

        /// <summary>
        /// 组帧(智能表状态查询)
        /// </summary>
        /// <returns>智能表状态查询命令帧</returns>
        public byte[] WeaveFrame_CheckupState(string commAddr)
        {
            //功能: 请求获取电能表状态
            //控制码: 0x03
            //数据域长度：L =04H(数据标识)+04H(操作者代码)
            //DI3 DI2 DI1 DI0: 07 80 02 FF
            //帧格式: 68 A0 A1 A2 A3 A4 A5 68 03 L DIO DI1 DI2 DI3 C0 C1 C2 C3 N1 .. Nm CS 16
            return WeaveDataFrame("03", "078102FF", commAddr, "", true, false, false);
        }



        /// <summary>
        /// 翻转数据
        /// </summary>
        /// <param name="sourceData">待翻转的数据</param>
        /// <returns>翻转后的数据</returns>
        public static string TurnoverData(string sourceData)
        {
            if ((sourceData.Length % 2) != 0)
            {
                sourceData = string.Format("0{0:S}", sourceData);
            }
            string resultData = "";
            int halfLength = sourceData.Length / 2;
            for (int i = 0; i < halfLength; i++)
            {
                resultData = string.Format("{0:S}{1:S}", sourceData.Substring(i * 2, 2), resultData);
            }
            return resultData;
        }
        /// <summary>
        /// 格式化数据
        /// </summary>
        /// <param name="sourceData">待格式化的数据</param>
        /// <param name="nLength">需要截取的长度</param>
        /// <returns>格式化后的数据</returns>
        public static string FormatData(string sourceData, int nLength)
        {
            if (nLength > sourceData.Length)
            {
                int appendLength = nLength - sourceData.Length;
                for (int i = 0; i < appendLength; i++)
                {
                    sourceData = string.Format("0{0:S}", sourceData);
                }
                return sourceData;
            }
            else if (nLength == sourceData.Length)
            {
                return sourceData;
            }
            else
            {
                return sourceData.Substring(sourceData.Length - nLength);
            }
        }

        /// <summary>
        /// 加0x33并翻转数据
        /// </summary>
        /// <param name="sourceData">待加0x33并翻转的16进制数据</param>
        /// <returns>加0x33并翻转后的数据</returns>
        public static string TurnoverDataAfterAdd33H(string sourceData)
        {
            if (sourceData.Length % 2 != 0)
            {
                sourceData = string.Format("0{0:S}", sourceData);
            }
            string resultData = "";
            int halfLength = sourceData.Length / 2;
            for (int i = 0; i < halfLength; i++)
            {
                byte _HexByte = (byte)(Convert.ToByte(sourceData.Substring(i * 2, 2), 16) + 0x33); //加0x33操作
                resultData = string.Format("{0:X2}{1:S}", _HexByte, resultData);//翻转组包
            }
            return resultData;
        }

        /// <summary>
        /// 十六进制字符串转化为二进制字节数组
        /// </summary>
        /// <param name="sourceData">待转化的十六进制字符串</param>
        /// <param name="FECount">唤醒符(FE)填充个数</param>
        /// <returns>转化后的二进制字节数组</returns>
        public static byte[] FormatHexToBinary(ref string sourceData, int FECount)
        {
            for (int i = 0; i < FECount; i++)
            {
                sourceData = string.Format("FE{0:S}", sourceData);
            }
            int halfLength = sourceData.Length / 2;
            byte[] binaryBytes = new byte[halfLength];
            for (int i = 0; i < halfLength; i++)
            {
                binaryBytes[i] = Convert.ToByte(sourceData.Substring(i * 2, 2), 16);
            }
            return binaryBytes;
        }

        /// <summary>
        /// 校验数据帧(各字节求和)
        /// </summary>
        /// <param name="sourceData">待求校验和的16进制命令帧字符串</param>
        /// <returns>数据帧的校验和</returns>
        public static byte GetCmdFrameCS(string sourceData)
        {
            if (sourceData.Length % 2 != 0)
            {
                sourceData = string.Format("0{0:S}", sourceData);
            }
            byte tmp_HexByte = 0x00;
            int halfLength = sourceData.Length / 2;
            for (int i = 0; i < halfLength; i++)
            {
                tmp_HexByte = (byte)(Convert.ToByte(sourceData.Substring(i * 2, 2), 16) + tmp_HexByte);
            }
            return tmp_HexByte;
        }


        /// <summary>
        /// 调整数组的长度
        /// </summary>
        /// <param name="origArray">原数组</param>
        /// <param name="desiredSize">调整后的长度</param>
        /// <returns>调整后的数组</returns>
        public static Array Redim(Array origArray, int desiredSize)
        {
            Array newArray = null;
            if (origArray != null)
            {
                Type type = origArray.GetType().GetElementType(); //元素类型
                newArray = Array.CreateInstance(type, desiredSize);
                Array.Copy(origArray, 0, newArray, 0, Math.Min(origArray.Length, desiredSize));
            }
            return newArray;
        }
        /// <summary>
        /// 16进制字符串转字节数组
        ///     【"123" => {0x23,0x01}】
        /// </summary>
        /// <param name="strData">16进制字符串</param>
        /// <param name="aryLen">数组长度</param>
        /// <returns>字节数组</returns>
        public static byte[] HexStrToBytsData(string strData, int aryLen)
        {
            byte[] bytsData = new byte[aryLen];
            string tempStrData = AdjustmentStrSize(strData, aryLen * 2, true); //调整长度
            for (int i = 0; i < aryLen; i++)
            {
                bytsData[aryLen - 1 - i] = Convert.ToByte(tempStrData.Substring(i * 2, 2), 16);
            }
            return bytsData;
        }
        /// <summary>
        /// 调整字符串大小
        /// </summary>
        /// <param name="origStr">待调字符串</param>
        /// <param name="desiredSize">调整的大小</param>
        /// <param name="isForward">是否前移</param>
        /// <returns>调整后的字符串</returns>
        public static string AdjustmentStrSize(string origStr, int desiredSize, bool isForward)
        {
            string strData = "";
            if (origStr.Length < desiredSize)
            {
                strData = origStr;
                for (int i = 0; i < desiredSize - origStr.Length; i++)
                {
                    if (isForward) //"123" => "0123"
                        strData = String.Intern("0") + strData;
                    else //"123" => "1230"
                        strData += String.Intern("0");
                }
            }
            else
            {
                if (isForward) //"0123" => "123"
                    strData = origStr.Substring(origStr.Length - desiredSize);
                else //"1230" => "123"
                    strData = origStr.Substring(0, desiredSize);
            }
            return strData;
        }

        ///----------


        #region ---- 解析电能表数据 ----

        //1 ESAM中数据解析

        /// <summary>
        /// 拆帧(智能表状态查询 ESAM:intType=1 密钥信息4 intType=2 客户编号6 intType=3 MAC4 intType=4 购电次数（ESAM内）4 intType=5 MAC4  intType=6 剩余金额（ESAM内）4 陈大伟 2011-8-14)
        /// </summary>
        /// <param name="intType">intType=1 密钥信息4 intType=2 客户编号6 intType=3 MAC4 intType=4 购电次数（ESAM内）4 intType=5 MAC4  intType=6 剩余金额（ESAM内）4</param>
        /// <param name="cmdFrame">待解析的命令帧</param>
        /// <param name="stateCode">返回的数据值</param>
        /// <param name="frameText">返回的报文字符串</param>
        /// <param name="errText">错误信息字提示信息</param>
        /// <returns>[true-成功, false-失败]</returns>
        public bool ParseFrame_CheckupState(int intType, byte[] cmdFrame, ref string strRetDat, out string frameText,
            out string errText)
        {
            //正常应答 0x83(1000 0011):  68 A0 A1 A2 A3 A4 A5 68 83 L DIO DI1 DI2 DI3 N1 ... Nm CS 16
            //异常应答 0xC3(1100 0011):  68 A0 A1 A2 A3 A4 A5 68 C3 02 XX YY CS 16 (XX YY: 安全认证错误信息字，2字节)
            byte bytCode = 0; //控制码
            StControlCode ctlCode;
            byte[] dataRegion;
            string strData = "";
            byte[] data = new byte[0];
            frameText = ""; strRetDat = ""; errText = "解析失败";

            ParseFrameData(cmdFrame, out dataRegion, ref bytCode);
            DataRegionDis(dataRegion, false); //-33H
            ctlCode = StControlCode.GetObject(bytCode);
            if (ctlCode.AnsState != 0)
                return false;

            if (dataRegion.Length >= 26)
            {
                data = new byte[26];
                Array.Copy(dataRegion, 4, data, 0, data.Length);

                strData = "";
                for (int i = 0; i < data.Length; i++)
                    strData = data[i].ToString("X2") + strData;

                strRetDat = "";
                if (strData.Length > 51)
                {
                    switch (intType)
                    {
                        case 1:
                            strRetDat = strData.Substring(0, 8); //密钥信息
                            break;
                        case 2:
                            strRetDat = strData.Substring(8, 12); //客户编号
                            break;
                        case 3:
                            strRetDat = strData.Substring(20, 8); //MAC
                            break;
                        case 4:
                            strRetDat = strData.Substring(28, 8); //购电次数（ESAM内）
                            break;
                        case 5:
                            strRetDat = strData.Substring(36, 8); //MAC
                            break;
                        case 6:
                            strRetDat = strData.Substring(44, 8); //剩余金额（ESAM内）
                            break;
                        default://返回所有数据
                            strRetDat = strData;
                            break;
                    }
                }

            }


            errText = "解析成功";
            return true;
        }
        /// <summary>
        /// 解析ESAM模块中运行信息文件的数据--1
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="fileData">文件数据</param>
        /// <param name="macAddr">MAC地址</param>
        /// <returns>True-解析成功；False-解析失败</returns>
        public bool ParseEsamRunFileDataN(byte[] frameData, ref StEsamRunFile fileData, ref string macAddr)
        {
            byte bytCode = 0; //控制码
            StControlCode ctlCode;
            byte[] dataRegion;
            byte[] byts_Customer = new byte[6];
            byte[] byts_MeterNum = new byte[6];
            ParseFrameData(frameData, out dataRegion, ref bytCode);
            DataRegionDis(dataRegion, false); //-33H
            ctlCode = StControlCode.GetObject(bytCode);
            if (ctlCode.AnsState != 0)
                return false;
            if (ctlCode.FunCode != EmFunCode.SafetyAtic)
                return false;
            fileData.KeyVersion = dataRegion[12]; //密钥版本号
            return true;
        }
        /// <summary>
        /// 解析ESAM模块中运行信息文件的数据
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="strData">密钥信息</param>
        /// <param name="macAddr">MAC地址</param>
        /// <returns>True-解析成功；False-解析失败</returns>
        public bool ParseEsamRunFileData(byte[] frameData, ref string strData, ref string macAddr)
        {
            byte bytCode = 0; //控制码
            StControlCode ctlCode;
            byte[] dataRegion;
            byte[] byts_Customer = new byte[6];
            byte[] byts_MeterNum = new byte[6];

            byte[] data = new byte[0];

            ParseFrameData(frameData, out dataRegion, ref bytCode);
            DataRegionDis(dataRegion, false); //-33H
            ctlCode = StControlCode.GetObject(bytCode);
            if (ctlCode.AnsState != 0)
                return false;
            if (ctlCode.FunCode != EmFunCode.SafetyAtic)
                return false;
            //fileData.KeyVersion = dataRegion[12]; //密钥版本号
            if (dataRegion.Length > 15)
            {
                data = new byte[8];
                Array.Copy(dataRegion, 12, data, 0, data.Length);

                strData = "";
                for (int i = 0; i < data.Length; i++)
                    strData = data[i].ToString("X2") + strData;
            }
            return true;
        }
        /// <summary>
        /// 解析ESAM模块中运行信息文件的数据
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="fileData">文件数据</param>
        /// <param name="macAddr">MAC地址</param>
        /// <returns>True-解析成功；False-解析失败</returns>
        public bool ParseEsamRunFile(byte[] frameData, ref StEsamRunFile fileData, ref string macAddr)
        {
            byte bytCode = 0; //控制码
            StControlCode ctlCode;
            byte[] dataRegion;
            byte[] byts_Customer = new byte[6];
            byte[] byts_MeterNum = new byte[6];
            ParseFrameData(frameData, out dataRegion, ref bytCode);
            DataRegionDis(dataRegion, false); //-33H
            ctlCode = StControlCode.GetObject(bytCode);
            if (ctlCode.AnsState != 0)
                return false;
            if (ctlCode.FunCode != EmFunCode.SafetyAtic)
                return false;
            fileData.KeyVersion = dataRegion[12]; //密钥版本号
            return true;
        }

        /// <summary>
        /// 拆帧(密钥下装)
        /// </summary>
        /// <param name="keyNo">下装密钥类型</param>
        /// <param name="cmdFrame">待解析的命令帧</param>
        /// <param name="frameText">返回的报文字符串</param>
        /// <param name="errText">错误信息字提示信息</param>
        /// <returns>[true-成功, false-失败]</returns>
        public bool ParseFrame_UpdateKey(int keyNo, byte[] cmdFrame, out string frameText, out string errText)
        {
            byte bytCode = 0; //控制码
            StControlCode ctlCode;
            byte[] dataRegion;
            byte[] byts_Customer = new byte[6];
            byte[] byts_MeterNum = new byte[6];
            byte[] data = new byte[0];
            string strData = "";
            frameText = "";
            errText = "";

            ParseFrameData(cmdFrame, out dataRegion, ref bytCode);
            DataRegionDis(dataRegion, false); //-33H
            ctlCode = StControlCode.GetObject(bytCode);
            if (ctlCode.AnsState != 0)
                return false;

            string dataCode = "";
            switch (keyNo)
            {
                case 2://EmKeyType.RemoteControlKey: //远程控制密钥
                    dataCode = "070201FF";
                    break;
                case 3://EmKeyType.ParamUpdateKey: //参数更新密钥
                    dataCode = "070202FF";
                    break;
                case 1://EmKeyType.IdentityKey: //身份认证密钥
                    dataCode = "070203FF";
                    break;
                case 0://EmKeyType.ControlKey: //主控密钥
                    dataCode = "070204FF";
                    break;
            }
            if (dataRegion.Length > 3)
            {
                data = new byte[4];
                Array.Copy(dataRegion, 0, data, 0, 4);

                strData = "";
                for (int i = 0; i < data.Length; i++)
                    strData = data[i].ToString("X2") + strData;
            }


            if (strData.ToUpper() != dataCode)
            {
                errText = "返回数据不一致";
                return false;
            }
            return true;
        }

        //2 电表信息解析

        /// <summary>
        /// 解析读取数据返回的数据域
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="errCode">故障编码[0-无错误]</param>
        /// <returns>解析失败，返回空数组</returns>
        public byte[] ParseDataRegion(byte[] frameData, ref int errCode)
        {
            byte[] dataRegion;
            byte[] data = new byte[0];
            byte bytCtlCode = 0;
            errCode = 0;
            ParseFrameData(frameData, out dataRegion, ref bytCtlCode);
            DataRegionDis(dataRegion, false); //-33H
            StControlCode objCtlCode = StControlCode.GetObject(bytCtlCode);
            if (objCtlCode.AnsState == 0) //正常应答
            {
                switch (objCtlCode.FunCode)
                {
                    case EmFunCode.ReadData:
                        #region ==== 读数据 ====
                        if (dataRegion.Length - 4 > 0)
                        {
                            data = new byte[dataRegion.Length - 4];
                            Array.Copy(dataRegion, 4, data, 0, data.Length);
                        }
                        #endregion ==== 读数据 ====
                        break;
                }
            }
            return data;
        }
        /// <summary>
        /// 解析随机数2和EASM序列号
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="sticNumber2">随机数2(电能表)</param>
        /// <param name="salNumber">ESAM序列号</param>
        /// <returns>True-解析成功；False-解析失败</returns>
        public bool ParseSticNumAndEsam(byte[] frameData, ref string sticNumber2, ref string salNumber)
        {
            byte[] dataRegion;
            byte bytCode = 0; //控制码
            byte[] byts_SticNum2 = new byte[4];
            byte[] byts_SalNum = new byte[8];
            ParseFrameData(frameData, out dataRegion, ref bytCode);
            DataRegionDis(dataRegion, false); //-33H
            StControlCode ctlCode = StControlCode.GetObject(bytCode);
            if (ctlCode.AnsState != 0)
                return false;
            if (ctlCode.FunCode != EmFunCode.SafetyAtic)
                return false;
            Array.Copy(dataRegion, 4, byts_SticNum2, 0, 4); //随机数2
            Array.Copy(dataRegion, 8, byts_SalNum, 0, 8); //ESAM序列号
            sticNumber2 = DLTFun.BytsToHexStrData(byts_SticNum2);
            salNumber = DLTFun.BytsToHexStrData(byts_SalNum);
            return true;
        }
        /// <summary>
        /// 解析通讯地址
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="errCode">错误编码</param>
        /// <returns>通讯地址</returns>
        public string ParseCommAddr(byte[] frameData, ref int errCode)
        {
            lock (_SyncLock)
            {
                if ((errCode = (byte)AnsFrame(frameData)) == 0)
                {
                    List<DataInfo> lstDatas1 = _Dictionary.Search("08000003");
                    if (lstDatas1.Count != 0)
                        return DLTFun.BytsToHexStrData(lstDatas1[0].AryData);
                }
            }
            return "";
        }
        /// <summary>
        /// 解析各费率电能及总电能
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="errCode">错误编码</param>
        /// <returns>解析失败，返回空列表；（下标为0表示总电能；L=费率数+1；）</returns>
        public List<float> ParseFullEnergy(byte[] frameData, ref int errCode)
        {
            List<float> lstEnergys = new List<float>(); //电能值列表
            float sfEnergy = 0.0F;
            lock (_SyncLock)
            {
                if ((errCode = (byte)AnsFrame(frameData)) == 0)
                {
                    List<DataInfo> lstDatas = _Dictionary.Search(_DataCode);
                    foreach (DataInfo data in lstDatas)
                    {
                        sfEnergy = Convert.ToSingle(DLTFun.BytsToHexStrData(data.AryData)) / 100;
                        lstEnergys.Add(sfEnergy);
                    }
                }
            }
            return lstEnergys;
        }
        /// <summary>
        /// 解析各费率需量及总需量
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="errCode">错误编码</param>
        /// <returns>解析失败，返回空列表；（下标为0表示总电能；L=费率数+1；）</returns>
        public List<float> ParseFullDemand(byte[] frameData, ref int errCode)
        {
            List<float> lstDemands = new List<float>(); //需量值列表
            float sfDemand = 0.0F;
            string str_Datetime = "";
            lock (_SyncLock)
            {
                if ((errCode = (byte)AnsFrame(frameData)) == 0)
                {
                    List<DataInfo> lstDatas = _Dictionary.Search(_DataCode);
                    foreach (DataInfo data in lstDatas)
                    {
                        sfDemand = Convert.ToSingle(DLTFun.BytsToHexStrData(data.AryData)) / 10000;
                        byte[] A_Bmd = new byte[3];
                        byte[] A_DT = new byte[5];
                        Array.Copy(data.AryData, 0, A_Bmd, 0, 3);
                        Array.Copy(data.AryData, 3, A_DT, 0, 5);
                        Array.Reverse(A_Bmd);
                        Array.Reverse(A_DT);
                        sfDemand = Convert.ToSingle(BitConverter.ToString(A_Bmd).Replace("-", "")) / 10000;
                        str_Datetime = BitConverter.ToString(A_DT).Replace("-", "");
                        lstDemands.Add(sfDemand);
                    }
                }
                //byte[] Bsf = new byte[4];
                //byte[] B_Data = new byte[8];
                //if (frameData[8] == 0xD1)
                //{

                //}
                //{
                //    Array.Copy(frameData, 10, Bsf, 0, 4);
                //    Array.Reverse(Bsf);
                //    for (int i = 0; i < Bsf.Length; i++)
                //    {
                //        Bsf[i] = (byte)((int)Bsf[i] - 0x33);
                //    }
                //    string strBsf = BitConverter.ToString(Bsf).ToString().Replace("-", "");
                //    switch (strBsf)
                //    {
                //        case "0101FF00":
                //            int n = 0;
                //            while (true)
                //            {
                //                if (frameData[14 + n * 8 + 1] == 0x16) break;
                //                Array.Copy(frameData, 14 + n * 8, B_Data, 0, 8);
                //                for (int i = 0; i < B_Data.Length; i++)
                //                {
                //                    B_Data[i] = (byte)((int)B_Data[i] - 0x33);
                //                }
                //                byte[] B_Demand = new byte[3];
                //                byte[] B_DateTime = new byte[5];
                //                Array.Copy(B_Data, 0, B_Demand, 0, 3);
                //                Array.Copy(B_Data, 3, B_DateTime, 0, 5);
                //                Array.Reverse(B_Demand);
                //                Array.Reverse(B_DateTime);
                //                string str_Bmd = BitConverter.ToString(B_Demand).Replace("-", "");
                //                string str_DT = BitConverter.ToString(B_DateTime).Replace("-", "");
                //                sfDemand = Convert.ToSingle(str_Bmd) / 10000;
                //                lstDemands.Add(sfDemand);
                //                n++;
                //            }
                //            break;
                //    }
                //}
            }
            return lstDemands;
        }
        /// <summary>
        /// 解析剩余金额
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="errCode">错误编码</param>
        /// <returns>剩余金额（单位：元）</returns>
        public float ParseSpareMoney(byte[] frameData, ref int errCode)
        {
            lock (_SyncLock)
            {
                if ((errCode = (byte)AnsFrame(frameData)) == 0)
                {
                    List<DataInfo> lstDatas1 = _Dictionary.Search("00900200");
                    if (lstDatas1.Count != 0)
                        return Convert.ToSingle(DLTFun.BytsToHexStrData(lstDatas1[0].AryData)) / 100;
                }
            }
            return 0.0F;
        }

        //-----
        /// <summary>
        /// 解析阶梯电价  陈大伟 2011-8-14
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="errCode">错误编码</param>
        /// <returns>当前阶梯电价 XXXX.XXXX 元/kWh</returns>
        public float CurrJtdjMoney(byte[] frameData, ref int errCode)
        {
            lock (_SyncLock)
            {
                if ((errCode = (byte)AnsFrame(frameData)) == 0)
                {
                    List<DataInfo> lstDatas1 = _Dictionary.Search("0280000B");
                    if (lstDatas1.Count != 0)
                        return Convert.ToSingle(DLTFun.BytsToHexStrData(lstDatas1[0].AryData)) / 10000;
                }
            }
            return 0.0F;
        }
        //---
        /// <summary>
        /// 解析剩余电量
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="errCode">错误编码</param>
        /// <returns>剩余电量（单位：kWh）</returns>
        public float ParseSpareEnergy(byte[] frameData, ref int errCode)
        {
            lock (_SyncLock)
            {
                if ((errCode = (byte)AnsFrame(frameData)) == 0)
                {
                    List<DataInfo> lstDatas1 = _Dictionary.Search("00900100");
                    if (lstDatas1.Count != 0)
                        return Convert.ToSingle(DLTFun.BytsToHexStrData(lstDatas1[0].AryData)) / 100;
                }
            }
            return 0.0F;
        }
        /// <summary>
        /// 解析时区表数据
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="errCode">错误编号</param>
        /// <returns>解析失败，返回空列表；（L=年时区数；）</returns>
        public List<StTimeZone> ParseTimeZoneList(byte[] frameData, ref int errCode)
        {
            return null;
        }
        /// <summary>
        /// 解析日时段表数据
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="errCode">错误编号</param>
        /// <returns>解析失败，返回空列表；（L=日时段数；）</returns>
        public List<StDayPeriod> ParseDayPeriodList(byte[] frameData, ref int errCode)
        {
            return null;
        }
        /// <summary>
        /// 解析电能表运行状态
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="runState">运行状态</param>
        /// <param name="errCode">错误编码</param>
        /// <returns>True-解析成功；False-解析失败</returns>
        public bool ParseRunState(byte[] frameData, ref StMeterRunState runState, ref int errCode)
        {
            return ParseRunState(frameData, 0, ref runState, ref errCode);
        }
        //runCode：运行状态字（0-数据块）
        public bool ParseRunState(byte[] frameData, byte runCode, ref StMeterRunState runState, ref int errCode)
        {
            lock (_SyncLock)
            {
                if ((errCode = (byte)AnsFrame(frameData)) == 0)
                {
                    List<DataInfo> lstDatas = _Dictionary.Search(_DataCode);//搜索不到的
                    if (runCode == 0) //数据块
                    {
                        for (int i = 0; i < 7; i++)//7个运行状态字
                        {
                            switch (i)
                            {
                                case 0:
                                    break;
                                case 1:
                                    break;
                                case 2: //电表运行状态3
                                    runState.SupplyType = (byte)((lstDatas[i].AryData[0] & 0x02 | lstDatas[i].AryData[0] & 0x04) >> 1); //供电方式
                                    runState.ProgLicense = (byte)((lstDatas[i].AryData[0] >> 3) & 0x01); //编程许可
                                    runState.RelayState = (byte)((lstDatas[i].AryData[0] >> 4) & 0x01); //继电器状态
                                    runState.RelayCmdState = (byte)((lstDatas[i].AryData[0] >> 6) & 0x01); //继电器命令状态
                                    runState.AlarmState = (byte)((lstDatas[i].AryData[0] >> 7) & 0x01); //预跳闸报警状态
                                    break;
                                case 3:
                                    break;
                                case 4:
                                    break;
                                case 5:
                                    break;
                                case 6:
                                    break;
                            }
                        }
                    }
                    else
                    {
                        switch (runCode)
                        {
                            case 1:
                                break;
                            case 2:
                                break;
                            case 3://运行状态字3
                                runState.SupplyType = (byte)((lstDatas[0].AryData[0] & 0x02 | lstDatas[0].AryData[0] & 0x04) >> 1); //供电方式
                                runState.ProgLicense = (byte)((lstDatas[0].AryData[0] >> 3) & 0x01); //编程许可
                                runState.RelayState = (byte)((lstDatas[0].AryData[0] >> 4) & 0x01); //继电器状态
                                runState.RelayCmdState = (byte)((lstDatas[0].AryData[0] >> 6) & 0x01); //继电器命令状态
                                runState.AlarmState = (byte)((lstDatas[0].AryData[0] >> 7) & 0x01); //预跳闸报警状态
                                break;
                            case 4:
                                break;
                            case 5:
                                break;
                            case 6:
                                break;
                        }
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// 解析设参结果
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="errCode">错误编码</param>
        /// <returns>True-设参成功；False-设参失败</returns>
        public bool ParseSetResult(byte[] frameData, ref int errCode)
        {
            errCode = AnsFrame(frameData);
            return (errCode == 0);
        }
        #endregion ---- 解析电能表数据 ----
        #endregion ---- 功能接口 ----

        /// <summary>
        /// 主站命令帧
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <param name="dataCode">数据标识</param>
        /// <param name="funCode">功能码</param>
        /// <returns>帧长度；失败返回故障编码</returns>
        private int AskFrame(out byte[] frameData, string commAddr, string dataCode, EmFunCode funCode, int dataSort)
        {
            return AskFrame(out frameData, commAddr, dataCode, funCode, EmPwdGrade._Grade_0, dataSort);
        }
        //pwdGrade：密码权限
        private int AskFrame(out byte[] frameData, string commAddr, string dataCode, EmFunCode funCode,
            EmPwdGrade pwdGrade, int dataSort)
        {
            return AskFrame(out frameData, commAddr, dataCode, funCode, pwdGrade, 0, dataSort);
        }
        //backNum：帧序号（读后续数据使用）
        private int AskFrame(out byte[] frameData, string commAddr, string dataCode, EmFunCode funCode,
            EmPwdGrade pwdGrade, byte backNum, int dataSort)
        {
            List<DataInfo> lstDatas;
            int s4Cursor = 0;
            byte[] crypData; //密文数据
            byte[] macAddr = new byte[4]; //MAC地址
            byte[] aryData = new byte[255];
            byte[] dataRegion = new byte[0]; //数据域
            frameData = new byte[0];
            try
            {
                //System.Threading.Monitor.Enter(_SyncLock);
                switch (funCode)
                {
                    case EmFunCode.ReadData: //数据域：数据标识
                        #region ---- 读数据 ----
                        dataRegion = DLTFun.HexStrToBytsData(dataCode, 4);
                        #endregion ---- 读数据 ----
                        break;
                    case EmFunCode.ReadBackData: //数据域：数据标识+帧序号
                        #region ---- 读后续数据 ----
                        dataRegion = new byte[5];
                        Array.Copy(DLTFun.HexStrToBytsData(dataCode, 4), 0, dataRegion, 0, 4);
                        dataRegion[4] = backNum; //帧序号
                        #endregion ---- 读后续数据 ----
                        break;
                    case EmFunCode.WriteData: //数据域：数据标识+密码+操作者代码+数据
                        #region ---- 写数据 ----
                        lstDatas = _Dictionary.Search(dataCode); //搜索
                        if (lstDatas.Count == 0)
                            return FaultCode.FC_LACKDATA;
                        for (int i = 0; i < lstDatas.Count; i++)
                        {
                            if (dataSort == 0)
                            {
                                dataSort = lstDatas[i].DataSort;
                            }
                            Array.Copy(lstDatas[i].AryData, 0, aryData, s4Cursor, lstDatas[i].ArySize);
                            s4Cursor += lstDatas[i].ArySize;
                        }
                        aryData = (byte[])DLTFun.Redim(aryData, s4Cursor); //调整长度
                        switch (dataSort)
                        {
                            case 1: //明文+MAC
                                pwdGrade = EmPwdGrade._Grade_99;
                                if (_DataEncrypt == null)
                                    return FaultCode.FC_NOHAVENCRYPT;
                                if (_DataEncrypt.Encrypt(_SKeyState, commAddr, 1, aryData, out crypData, ref macAddr))
                                {
                                    aryData = (byte[])DLTFun.Redim(aryData, aryData.Length + 4);
                                    //Array.Copy(aryData, 0, aryData, 0, aryData.Length); //明文
                                    Array.Copy(macAddr, 0, aryData, aryData.Length - 4, 4); //MAC地址
                                }
                                else
                                    return FaultCode.FC_ENCRYPTFAULT;
                                break;
                            case 2: //密文+MAC
                                pwdGrade = EmPwdGrade._Grade_98;
                                if (_DataEncrypt == null)
                                    return FaultCode.FC_NOHAVENCRYPT;
                                if (_DataEncrypt.Encrypt(_SKeyState, commAddr, 2, aryData, dataCode, out crypData, ref macAddr))
                                {
                                    aryData = (byte[])DLTFun.Redim(aryData, crypData.Length + 4);
                                    Array.Copy(crypData, 0, aryData, 0, crypData.Length); //密文
                                    Array.Copy(macAddr, 0, aryData, crypData.Length, 4); //MAC地址
                                }
                                else
                                    return FaultCode.FC_ENCRYPTFAULT;
                                break;
                            case 3: //明文
                                //pwdGrade = EmPwdGrade._Grade_0;
                                break;
                        }
                        switch (dataCode)//陈大伟 2011-8-19 数据处理 这里借用数据标示传递写冻结命令的16
                        {
                            //case "15"://7.5	写通信地址 
                            //    break ;
                            case "16":
                            case "00000016"://冻结：mm hh DD MM(68 a0...a5 68 16 04 mm hh DD MM CS 16)
                                dataRegion = new byte[aryData.Length];
                                Array.Copy(aryData, 0, dataRegion, 0, aryData.Length);
                                funCode = EmFunCode.WriteFreeze;
                                break;
                            //case "17"://
                            //    break;
                            //case "18"://
                            //    break;
                            //case "19"://
                            //    break;
                            //case "1A"://
                            //    break;
                            //case "1B"://
                            //    break;
                            //case "1C"://
                            //    break;
                            //case "1D"://
                            //    break;

                            default:

                                dataRegion = new byte[12 + aryData.Length];
                                Array.Copy(DLTFun.HexStrToBytsData(dataCode, 4), 0, dataRegion, 0, 4);
                                dataRegion[4] = (byte)pwdGrade;
                                Array.Copy(_PassWord, 0, dataRegion, 5, 3);
                                Array.Copy(_HandleCode, 0, dataRegion, 8, 4);
                                Array.Copy(aryData, 0, dataRegion, 12, aryData.Length);

                                break;

                        }
                        #endregion ---- 写数据 ----
                        break;
                    case EmFunCode.MeterClear: //数据域：密码+操作者代码
                        #region ---- 电表清零 ----
                        dataRegion = new byte[8];
                        dataRegion[0] = (byte)pwdGrade;
                        Array.Copy(_PassWord, 0, dataRegion, 1, 3);
                        Array.Copy(_HandleCode, 0, dataRegion, 4, 4);
                        #endregion ---- 电表清零 ----
                        break;
                    case EmFunCode.MaxDemandClear:  //最大需量清零
                        dataRegion = new byte[8];
                        dataRegion[0] = (byte)pwdGrade;
                        Array.Copy(_PassWord, 0, dataRegion, 1, 3);
                        Array.Copy(_HandleCode, 0, dataRegion, 4, 4);
                        break;
                    case EmFunCode.ControlFun: //数据域：密码+操作者代码+数据
                        #region ---- 拉合闸、报警、保电控制 ----
                        lstDatas = _Dictionary.Search("08000001");
                        if (lstDatas.Count == 0)
                            return FaultCode.FC_LACKDATA;
                        Array.Copy(lstDatas[0].AryData, 0, aryData, 0, lstDatas[0].ArySize);
                        aryData = (byte[])DLTFun.Redim(aryData, lstDatas[0].ArySize); //调整长度
                        switch (_MeterType)
                        {
                            case EmMeterType.ExpControl: //密文
                                pwdGrade = EmPwdGrade._Grade_98;
                                if (_DataEncrypt == null)
                                    return FaultCode.FC_NOHAVENCRYPT;
                                if (_DataEncrypt.Encrypt(_SKeyState, commAddr, 0, aryData, out crypData, ref macAddr))
                                {
                                    aryData = (byte[])DLTFun.Redim(aryData, crypData.Length);
                                    Array.Copy(crypData, 0, aryData, 0, crypData.Length); //密文
                                }
                                else
                                    return FaultCode.FC_ENCRYPTFAULT;
                                break;
                            case EmMeterType.General: //明文
                                pwdGrade = EmPwdGrade._Grade_2;
                                break;
                        }
                        dataRegion = new byte[8 + aryData.Length];
                        dataRegion[0] = (byte)pwdGrade;
                        Array.Copy(_PassWord, 0, dataRegion, 1, 3);
                        Array.Copy(_HandleCode, 0, dataRegion, 4, 4);
                        Array.Copy(aryData, 0, dataRegion, 8, aryData.Length);
                        /*
                        dataRegion = new byte[8 + aryData.Length];
                        Array.Copy(_PassWord, 0, dataRegion, 0, 3);
                        dataRegion[3] = (byte)pwdGrade;
                        Array.Copy(_HandleCode, 0, dataRegion, 4, 4);
                        Array.Copy(aryData, 0, dataRegion, 8, aryData.Length);
                         */
                        #endregion ---- 拉合闸、报警、保电控制 ----
                        break;

                    case EmFunCode.SafetyAtic:
                        #region ---- 安全认证 ----

                        lstDatas = _Dictionary.Search(dataCode); //搜索
                        for (int i = 0; i < lstDatas.Count; i++)
                        {
                            dataSort = lstDatas[i].DataSort;
                            Array.Copy(lstDatas[i].AryData, 0, aryData, s4Cursor, lstDatas[i].ArySize);
                            s4Cursor += lstDatas[i].ArySize;
                        }
                        aryData = (byte[])DLTFun.Redim(aryData, s4Cursor); //调整长度
                        dataRegion = new byte[8 + aryData.Length];
                        Array.Copy(DLTFun.HexStrToBytsData(dataCode, 4), 0, dataRegion, 0, 4);
                        Array.Copy(_HandleCode, 0, dataRegion, 4, 4);
                        Array.Copy(aryData, 0, dataRegion, 8, aryData.Length);
                        #endregion ---- 安全认证 ----
                        break;
                    default:

                        break;
                }
                DataRegionDis(dataRegion, true); //+33H
                StControlCode ctlCode = new StControlCode();
                ctlCode.FrameType = 0;
                ctlCode.AnsState = 0;
                ctlCode.HavBackData = false;
                ctlCode.FunCode = funCode;
                return GetFrameData(out frameData, DLTFun.HexStrToBytsData(commAddr, 6), ctlCode.GetCode(), dataRegion);
            }
            catch { return 0; }
            finally
            {
                //System.Threading.Monitor.Exit(_SyncLock);
            }
        }
        /// <summary>
        /// 从帧应答帧
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <returns>0-解析成功；其他-解析失败</returns>
        private int AnsFrame(byte[] frameData)
        {
            int s4Cursor = 0; //游标
            int faultCode = 0;
            byte bytCtlCode = 0; //控制码
            List<DataInfo> lstDatas;
            byte[] dataRegion = new byte[0]; //数据域
            byte[] bytsErrCode = new byte[4];
            byte[] bytsDataCode = new byte[4]; //数据标识
            try
            {
                //System.Threading.Monitor.Enter(_SyncLock);
                ParseFrameData(frameData, out dataRegion, ref bytCtlCode);
                DataRegionDis(dataRegion, false); //-33H
                StControlCode objCtlCode = StControlCode.GetObject(bytCtlCode);
                if (objCtlCode.AnsState == 0) //正常应答
                {
                    switch (objCtlCode.FunCode)
                    {
                        case EmFunCode.ReadData:
                            #region ==== 读数据 ====
                            byte[] byts_Data;
                            Array.Copy(dataRegion, 0, bytsDataCode, 0, 4);
                            _DataCode = DLTFun.BytsToHexStrData(bytsDataCode);
                            s4Cursor += 4;
                            lstDatas = _Dictionary.Search(_DataCode);
                            if (lstDatas.Count == 0)
                                return FaultCode.FC_LACKDATA;
                            foreach (DataInfo data in lstDatas)
                            {
                                byts_Data = new byte[data.ArySize];
                                Array.Copy(dataRegion, s4Cursor, byts_Data, 0, data.ArySize);
                                data.AryData = byts_Data;
                                s4Cursor += data.ArySize;
                            }
                            #endregion ==== 读数据 ====
                            break;
                        case EmFunCode.ReadCommAddr:
                            #region ==== 读通讯地址 ====
                            lstDatas = _Dictionary.Search("08000003");
                            if (lstDatas.Count == 0)
                                return FaultCode.FC_LACKDATA;
                            lstDatas[0].AryData = dataRegion;
                            #endregion ==== 读通讯地址 ====
                            break;
                    }
                    return 0;
                }
                #region ---- 故障编码 ----
                if (objCtlCode.FunCode != EmFunCode.SafetyAtic)
                {
                    bytsErrCode[0] = dataRegion[0];
                    bytsErrCode[1] = 0;
                    bytsErrCode[2] = 0;
                    bytsErrCode[3] = 0;
                }
                else //认证错误
                {
                    bytsErrCode[0] = 0;
                    bytsErrCode[1] = 0;
                    bytsErrCode[2] = dataRegion[0];
                    bytsErrCode[3] = dataRegion[1];
                }
                faultCode = BitConverter.ToInt32(bytsErrCode, 0);
                #endregion ---- 故障编码 ----
                return GetFaultCode(faultCode);
            }
            catch { return -1; }
            finally
            {
                //System.Threading.Monitor.Exit(_SyncLock);
            }
        }
        /// <summary>
        /// 搜索缓存区数据
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="dataCode">数据标识</param>
        /// <param name="commAddr">通讯地址</param>
        /// <returns>帧长度</returns>
        private int SearchDeposit(out byte[] frameData, string dataCode, string commAddr)
        {
            int frameLen = 0;
            frameData = new byte[0];
            int s4Index = _Deposit.FindCode(dataCode);
            if (s4Index >= 0)
            {
                frameData = _Deposit[s4Index];
                frameLen = _Deposit[s4Index].Length;
                UpdateFrame(frameData, DLTFun.HexStrToBytsData(commAddr, 6));
            }
            return frameLen;
        }
        /// <summary>
        /// 搜索帧数据
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="dataCode">数据标识</param>
        /// <param name="commAddr">通讯地址</param>
        /// <param name="funCode">功能码</param>
        /// <returns>帧长度</returns>
        private int SearchFrame(out byte[] frameData, string dataCode, string commAddr, EmFunCode funCode, int dataSort)
        {
            EmPwdGrade PwdGrade = (EmPwdGrade)_PwClass;
            return SearchFrame(out frameData, dataCode, commAddr, funCode, PwdGrade, dataSort);
        }
        private int SearchFrame(out byte[] frameData, string dataCode, string commAddr, EmFunCode funCode, EmPwdGrade pwdGrade, int dataSort)
        {
            return SearchFrame(out frameData, dataCode, commAddr, funCode, pwdGrade, 0, dataSort);
        }
        private int SearchFrame(out byte[] frameData, string dataCode, string commAddr, EmFunCode funCode, EmPwdGrade pwdGrade, byte backNum, int dataSort)
        {
            int frameLen = 0;
            if (_IsUseDeposit)
            {
                frameLen = SearchDeposit(out frameData, dataCode, commAddr);
                if (frameLen <= 0)
                {
                    frameLen = AskFrame(out frameData, commAddr, dataCode, funCode, pwdGrade, backNum);
                    _Deposit.AddFrameData(dataCode, frameData);
                }
                else
                {
                    UpdateFrame(frameData, DLTFun.HexStrToBytsData(commAddr, 6));
                }
            }
            else
                frameLen = AskFrame(out frameData, commAddr, dataCode, funCode, pwdGrade, backNum, dataSort);
            return frameLen;
        }
        //处理数据域
        private void DataRegionDis(byte[] dataRegion, bool isAsk)
        {
            for (int i = 0; i < dataRegion.Length; i++)
            {
                if (isAsk)
                    dataRegion[i] += 0x33;
                else
                    dataRegion[i] -= 0x33;
            }
        }
        //获取故障编码
        private int GetFaultCode(int faultCode)
        {
            return faultCode;
        }
    }//DLT645_2007(class)

    /// <summary>
    /// 功能码
    /// </summary>
    public enum EmFunCode
    {
        /// <summary>
        /// 保留
        /// </summary>
        Null = 0,
        /// <summary>
        /// 安全认证
        /// </summary>
        SafetyAtic = 0x03,
        /// <summary>
        /// 校表
        /// </summary>
        AdjustMeter = 0x04,
        /// <summary>
        /// 广播校时
        /// </summary>
        BrdctAdjustTime = 0x08,
        /// <summary>
        /// 读数据
        /// </summary>
        ReadData = 0x11,
        /// <summary>
        /// 读后续数据
        /// </summary>
        ReadBackData = 0x12,
        /// <summary>
        /// 读通讯地址
        /// </summary>
        ReadCommAddr = 0x13,
        /// <summary>
        /// 写数据
        /// </summary>
        WriteData = 0x14,
        /// <summary>
        /// 写通讯地址
        /// </summary>
        WriteCommAddr = 0x15,

        //--陈大伟 2011-8-19
        /// <summary>
        /// 冻结电能表数据
        /// </summary>
        WriteFreeze = 0x16,
        /// <summary>
        /// 7.8	更改通信速率
        /// </summary>
        WriteSL = 0x17,
        /// <summary>
        /// 7.9	修改密码
        /// </summary>
        WriteMiMa = 0x18,

        //---上面新增加类型

        /// <summary>
        /// 最大需量清零
        /// </summary>
        MaxDemandClear = 0x19,
        /// <summary>
        /// 电表清零
        /// </summary>
        MeterClear = 0x1A,
        /// <summary>
        /// 跳合闸、报警、保电
        /// </summary>
        ControlFun = 0x1C,
        /// <summary>
        /// 多功能端子输出
        /// </summary>
        ControlMult = 0x1D,
    }//EmFunCode(enum)

    /// <summary>
    /// 密码权限
    /// </summary>
    public enum EmPwdGrade
    {
        /* 00~09，数字越大权限越低  */
        _Grade_0 = 0,
        _Grade_2 = 0x02,
        _Grade_4 = 0x04,

        /// <summary>
        /// 明文+MAC
        /// </summary>
        _Grade_98 = 0x98,
        /// <summary>
        /// 密文+MAC
        /// </summary>
        _Grade_99 = 0x99,
    }//EmPwdGrade(enum)

    /// <summary>
    /// 表类型
    /// </summary>
    public enum EmMeterType
    {
        /// <summary>
        /// 费控表
        /// </summary>
        ExpControl,
        /// <summary>
        /// 普通表
        /// </summary>
        General,
    }//EmMeterType(enum)

    /// <summary>
    /// 控制码结构
    /// </summary>
    public struct StControlCode
    {
        /// <summary>
        /// 功能码
        /// </summary>
        public EmFunCode FunCode;
        /// <summary>
        /// 有无后续数据[True-有后续数据]
        /// </summary>
        public bool HavBackData;
        /// <summary>
        /// 帧类型[0-主站命令帧 1-从站应答帧]
        /// </summary>
        public byte FrameType;
        /// <summary>
        /// 从站应答状态[0-正常应答 1-异常应答]
        /// </summary>
        public byte AnsState;

        /// <summary>
        /// 获取控制码
        /// </summary>
        /// <returns>控制码</returns>
        public byte GetCode()
        {
            return (byte)((byte)(FrameType << 7) | (byte)(AnsState << 6) | (byte)(HavBackData ? 0x20 : 0) | (byte)FunCode);
        }
        /// <summary>
        /// 获取控制码结构
        /// </summary>
        /// <param name="bytCode">控制码</param>
        /// <returns>控制码结构</returns>
        public static StControlCode GetObject(byte bytCode)
        {
            StControlCode ctlCode = new StControlCode();
            ctlCode.FrameType = (byte)((bytCode & 0x80) >> 7);
            ctlCode.AnsState = (byte)((bytCode & 0x40) >> 6);
            ctlCode.HavBackData = ((bytCode & 0x20) >> 5 == 1) ? true : false;
            ctlCode.FunCode = (EmFunCode)(bytCode & 0x1F);
            return ctlCode;
        }
    }//StControlCode(struct)

    /// <summary>
    /// 工作类型
    /// </summary>
    public enum EmWorkType
    {
        /// <summary>
        /// 普通
        /// </summary>
        General = 0,
        /// <summary>
        /// 载波
        /// </summary>
        Carrier = 1,
        /// <summary>
        /// 红外
        /// </summary>
        Infrared = 2,
    }
}
