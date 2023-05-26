using System;
using CLDC_DataCore;
using System.Collections.Generic;
using System.Reflection;
using CLDC_VerifyAdapter.MulitThread;
using CLDC_Comm.BaseClass;
using CLDC_Comm.Enum;
using System.Threading;
using CLDC_DataCore.Const;
using System.Windows.Forms;
using CLDC_DataCore.Function;

namespace CLDC_VerifyAdapter
{
    /// <summary>
    /// 电能表多功能操作类
    /// </summary>
    public class MeterProtocolAdapter : SingletonBase<MeterProtocolAdapter>
    {

        const string meterProtocolDll = "CLDC_MeterProtocol.dll";
        /// <summary>
        /// 电能表多功能操作对象
        /// </summary>
        private CLDC_MeterProtocol.IMeterProtocol[] MeterProtocols = null;
        /// <summary>
        /// 表位数量
        /// </summary>
        private int bwCount = 24;

        /// <summary>
        /// 硬件路数，一般来说，一路硬件将启动一个通讯线程
        /// </summary>
        private int channelCount = 1;

        /// <summary>
        /// 每一通道负载
        /// </summary>
        private int oneChannelMeterCount = 1;

        private CLDC_Encryption.CLEncryption.EncryptionFactory encryptionFactory;

        private CLDC_SafeFileProtocol.ICardControlProtocol[] CardCtrProtocols = null;

        /// <summary>
        /// 加密机操作
        /// </summary>
        public CLDC_Encryption.CLEncryption.Interface.IAmMeterEncryption EncryptionTool
        {
            get;
            private set;
        }

        //string EncryptionType = GlobalUnit.g_SystemConfig.SystemMode.getValue(CLDC_DataCore.Const.Variable.CTC_ENCRYPTION_DEFAULTTYPE);
        //运行标志
        private bool runFlag = false;

        public bool IsWork { get { return runFlag; } }


        public void Stop()
        {
            runFlag = false;
        }
        /// <summary>
        /// 电能表
        /// </summary>
        public MeterProtocolAdapter()
        {
        }
        #region 设置脉冲端子    //zhengrubin-20190920
        public bool[] SetPulseCom(byte pulseType)
        {
            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate(int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                arrRet[pos] = SetPulseCom(pulseType, pos);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            return arrRet;
        }

        public bool SetPulseCom(byte pulseType, int pos)
        {
            if (MeterProtocols[pos] == null) return false;
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return true;
            return MeterProtocols[pos].SetPulseCom(pulseType);
        }



        #endregion

        #region 蓝牙连接
        public bool[] ConnectBlueTooth(string[] strAddress_MAC)
        {
            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate(int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                arrRet[pos] = ConnectBlueTooth(strAddress_MAC[pos],pos);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            return arrRet;
        }

        public bool ConnectBlueTooth(string strAddress_MAC, int pos)
        {
            if (MeterProtocols[pos] == null) return false;
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return true;
            return MeterProtocols[pos].ConnectBlueTooth(strAddress_MAC);
        }



        #endregion
        /// <summary>
        /// 以免浪费
        /// </summary>
        public void SetBwCount(int bws)
        {
            bwCount = bws;
            MeterProtocols = new CLDC_MeterProtocol.IMeterProtocol[bws];
            encryptionFactory = new CLDC_Encryption.CLEncryption.EncryptionFactory();
            EncryptionTool = encryptionFactory.CreateEncryptionControler();
            if (EncryptionTool.Status.Length != bws)
            {
                EncryptionTool.Status = new int[bws];
            }
            //if (GlobalUnit.g_CUS.DnbData.GetFirstYaoJianMeterBwh() != -1 && "是" == GlobalUnit.g_SystemConfig.SystemMode.getValue(Variable.CTC_ENCRYPTION_MACHINEISAUTOLINK) && !Helper.MeterDataHelper.Instance.Meter(GlobalUnit.g_CUS.DnbData.GetFirstYaoJianMeterBwh()).DgnProtocol.HaveProgrammingkey)
            //{
                //Thread thShowData = new Thread(EncryptionLink);
                //thShowData.IsBackground = true;
                //thShowData.Name = "EncryptionLink";
                //thShowData.Start();
            //}

            CardCtrProtocols = new CLDC_SafeFileProtocol.ICardControlProtocol[bws];
        }

        void EncryptionLink()
        {
            EncryptionTool.Link();
        }
        /// <summary>
        /// 初始化被检表协议
        /// </summary>
        /// <param name="dgnProtocols">电能表协议数组</param>
        public void Initialize(CLDC_DataCore.Model.DgnProtocol.DgnProtocolInfo[] dgnProtocols, string[] meterAddr, string[] meterAddr_MAC)
        {
            //创建协议对象            
            int currentChannelID = 0;
            string currentChannelPortName = string.Empty;
            CLDC_MeterProtocol.MeterProtocolManager.Instance.Initialize();
            channelCount = CLDC_MeterProtocol.MeterProtocolManager.Instance.GetChannelCount();
            oneChannelMeterCount = bwCount / channelCount;
            if (oneChannelMeterCount == 0)//TODO:每条总线负载
            {
                MessageController.Instance.AddMessage("485通道错误，配置485通道数！（通道数最大等于表位数，不能超过表位数。）", 6, 2);

                return;
                //oneChannelMeterCount = 1;
            }
            //更新线程管理对象
            MulitThreadManager.Instance.MaxThread = channelCount;
            MulitThreadManager.Instance.MaxTaskCountPerThread = oneChannelMeterCount;

            for (int i = 0; i < dgnProtocols.Length; i++)
            {
                if (dgnProtocols[i] == null)
                {
                    continue;
                }
                CLDC_MeterProtocol.IMeterProtocol newInterface = CreateInstance(dgnProtocols[i].ClassName);
                if (newInterface != null)
                {
                    newInterface.SetProtocol(dgnProtocols[i]);   //设置方案内容
                    newInterface.SetMeterAddress(meterAddr[i]);  //设置电表地址
                    newInterface.SetMeterAddress_MAC(meterAddr_MAC[i]);  //设置电表蓝牙地址
                    currentChannelID = GetChannelByPos(i + 1);
                    currentChannelPortName = CLDC_MeterProtocol.MeterProtocolManager.Instance.GetChannelPortName(currentChannelID);
                    newInterface.SetPortName(currentChannelPortName);//通讯端口
                }
                else
                {
                    MessageController.Instance.AddMessage(string.Format("第{0}表位创建多功能协议对象失败，没有找到与对{1}应的多功能协议", i + 1, dgnProtocols[i].ClassName), 6, 1);
                }
                MeterProtocols[i] = newInterface;
            }
            //读卡器
            CLDC_SafeFileProtocol.CardControlManager.Instance.Initialize();
            int CardchannelCount = CLDC_SafeFileProtocol.CardControlManager.Instance.GetChannelCount();
            if (CardchannelCount == 0)
            { }
            else
            {
                for (int i = 0; i < bwCount; i++)
                {
                    CLDC_SafeFileProtocol.Protocols.WatchDataW2160 newInterface = new CLDC_SafeFileProtocol.Protocols.WatchDataW2160();
                    if (newInterface != null)
                    {
                        currentChannelPortName = CLDC_SafeFileProtocol.CardControlManager.Instance.GetChannelPortName(i + 1);
                        newInterface.SetPortName(currentChannelPortName);//通讯端口
                    }
                    else
                    {
                        MessageController.Instance.AddMessage(string.Format("第{0}表位创建读卡器协议对象失败.", i + 1), 6, 1);
                    }
                    CardCtrProtocols[i] = newInterface;
                }
            }
        }

        /// <summary>
        /// 通过反射创建电能表协议对象
        /// </summary>
        /// <param name="className">多功能协议类</param>
        /// <returns>IMeterProtocol</returns>
        private CLDC_MeterProtocol.IMeterProtocol CreateInstance(string className)
        {
            try
            {
                string dllPath = CLDC_DataCore.Function.File.GetPhyPath(meterProtocolDll);
                string classFullName = string.Format("{0}.{1}", "CLDC_MeterProtocol.Protocols", className);
                Assembly asm = Assembly.LoadFile(dllPath);
                object obj = asm.CreateInstance(classFullName);//Activator.CreateInstance(asm.GetType(),className);
                return obj as CLDC_MeterProtocol.IMeterProtocol;
            }
            catch { }
            return null;
        }

        /// <summary>
        /// 通过表位计算通道号
        /// </summary>
        /// <param name="pos">表位号</param>
        /// <returns>通道号，下标从1开始</returns>
        private int GetChannelByPos(int pos)
        {
            pos--;
            int currentChannel = pos / oneChannelMeterCount;
            currentChannel++;
            return currentChannel;
        }

        /// <summary>
        /// 等待所有线程完成
        /// </summary>
        private void WaitWorkDone()
        {
            while (true)
            {
                if (!runFlag) break;
                if (MulitThreadManager.Instance.IsWorkDone())
                {
                    runFlag = false;
                    break;
                }
                System.Threading.Thread.Sleep(300);
            }
        }

        #region 试验功能

        

        #region 读取电量
        /// <summary>
        /// 读取电量
        /// </summary>
        /// <param name="energyType">电量功率类型0=P+ 1=P- 2=Q+ 3=Q- 4=Q1 5=Q2 6=Q3 6=Q4</param>
        /// <param name="tariffType">费率类型,当电量功率类型大于3时本参数无效</param>
        /// <returns>读取到的各表位电量</returns>
        public float[] ReadEnergy(byte energyType, byte tariffType)
        {
            float[] arrRet = new float[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate(int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                arrRet[pos] = ReadEnergy(energyType, tariffType, pos);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            return arrRet;
        }


        /// <summary>
        /// 读取指定表位的电表电量
        /// </summary>
        /// <param name="energyType"></param>
        /// <param name="tariffType"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public float ReadEnergy(byte energyType, byte tariffType, int pos)
        {
            if (MeterProtocols[pos] == null) return 0F;
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return 0F;
            return MeterProtocols[pos].ReadEnergy(energyType, tariffType);
        }

        /// <summary>
        /// 读取所有费率电量
        /// </summary>
        /// <param name="energyType">电量功率类型</param>
        /// <returns>电量列表</returns>
        public Dictionary<int, float[]> ReadEnergy(byte energyType)
        {
            Dictionary<int, float[]> arrRet = new Dictionary<int, float[]>();
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate(int pos)
            {
                if (!runFlag) return;
                float[] tmpValue = ReadEnergy(energyType, (byte)pos);
                arrRet.Add(pos, tmpValue);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            Console.WriteLine("已完成:ReadEnergy");
            return arrRet;
        }
        /// <summary>
        /// 读取所有费率电量
        /// </summary>
        /// <param name="energyType">电量功率类型</param>
        /// <returns>电量列表</returns>
        public Dictionary<int, float[]> ReadEnergys(byte energyType, int int_FreezeTimes)
        {
            Dictionary<int, float[]> arrRet = new Dictionary<int, float[]>();
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate(int pos)
            {
                if (!runFlag) return;
                float[] tmpValue = ReadEnergys(energyType, int_FreezeTimes, pos);
                if (arrRet.ContainsKey(pos))
                {
                    arrRet.Remove(pos);
                }

                arrRet.Add(pos, tmpValue);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            Console.WriteLine("已完成:ReadEnergy");
            return arrRet;
        }
        public float[] ReadEnergys(byte energyType, int int_FreezeTimes, int pos)
        {
            if (MeterProtocols[pos] == null) return new float[0];
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return new float[0];
            try
            {
                return MeterProtocols[pos].ReadEnergys(energyType, int_FreezeTimes);
            }
            catch
            {
                ;
            }
            return new float[0];
        }

        #endregion

        #region 读日期时间
        public DateTime[] ReadDateTime()
        {
            DateTime[] arrRet = new DateTime[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate(int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                arrRet[pos] = ReadDateTime(pos);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            return arrRet;
        }

        public DateTime ReadDateTime(int pos)
        {
            if (MeterProtocols[pos] == null) return DateTime.Now;
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return DateTime.Now;
            return MeterProtocols[pos].ReadDateTime();
        }
        #endregion

        #region 读通信地址
        /// <summary>
        /// 读取电表地址
        /// </summary>
        /// <returns>电表地址</returns>
        public string[] ReadAddress()
        {
            string[] arrAddress = new string[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate(int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                arrAddress[pos] = ReadAddress(pos);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            return arrAddress;
        }

        /// <summary>
        /// 读取指定表位置的电表
        /// </summary>
        /// <param name="pos">表号</param>
        /// <returns></returns>
        public string ReadAddress(int pos)
        {
            if (MeterProtocols[pos] == null) return string.Empty;
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return string.Empty;
            return MeterProtocols[pos].ReadAddress();
        }
        #endregion
        

        #region 读取数据：指定ID

        /// <summary>
        /// 读取数据（字符型，数据项）
        /// </summary>
        /// <param name="str_ID">标识符,2个字节</param>
        /// <param name="int_Len">数据长度(字节数)</param>
        /// <returns></returns>
        public string[] ReadLoadRecord(string str_ID, int int_Len, string strItem)
        {
            string[] arrRet = new string[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate(int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                arrRet[pos] = ReadLoadRecordByPos(str_ID, int_Len, strItem, pos);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            return arrRet;
        }

        public string ReadLoadRecordByPos(string str_ID, int int_Len, string strItem, int pos)
        {
            if (MeterProtocols[pos] == null) return string.Empty;
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return string.Empty;
            return MeterProtocols[pos].ReadData(str_ID, int_Len, strItem);
        }
        /// <summary>
        /// 读取数据（数据型，数据项）
        /// </summary>
        /// <param name="str_ID">标识符,2个字节</param>
        /// <param name="int_Len">数据长度(字节数)</param>
        /// <param name="int_Dot">小数位</param>
        /// <returns>返回数据</returns>
        public float[] ReadData(string str_ID, int int_Len, int int_Dot)
        {
            float[] arrRet = new float[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate(int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                arrRet[pos] = ReadData(str_ID, int_Len, int_Dot, pos);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            return arrRet;
        }

        /// <summary>
        /// 读取数据（数据型，数据项）
        /// </summary>
        /// <param name="str_ID">标识符,2个字节</param>
        /// <param name="int_Len">数据长度(字节数)</param>
        /// <param name="int_Dot">小数位</param>
        /// <returns>返回数据</returns>
        public string[] ReadData(string sendData)
        {
            string[] arrRet = new string[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate(int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                arrRet[pos] = ReadDatas(sendData, pos);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            return arrRet;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="str_ID"></param>
        /// <param name="int_Len"></param>
        /// <param name="int_Dot"></param>
        /// <param name="pos">start zero</param>
        /// <returns></returns>
        public float ReadData(string str_ID, int int_Len, int int_Dot, int pos)
        {
            if (MeterProtocols[pos] == null) return 0F;
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return 0F;
            return MeterProtocols[pos].ReadData(str_ID, int_Len, int_Dot);
        }

                                            
        public string ReadDatas(string sendData, int pos)
        {
            if (MeterProtocols[pos] == null) return "0F";
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return "0F";
            return MeterProtocols[pos].ReadData(sendData);
        }

    
        /// <summary>
        /// 读取数据（字符型，数据项）
        /// </summary>
        /// <param name="str_ID">标识符,2个字节</param>
        /// <param name="int_Len">数据长度(字节数)</param>
        /// <returns></returns>
        public string[] ReadDataWithCode(string str_ID, int int_Len, out string[] RevStrData)
        {
            string[] recData = new string[bwCount];
            string[] arrRet = new string[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate(int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                arrRet[pos] = ReadDataByPosWithCode(str_ID, int_Len, pos, out recData[pos]);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            RevStrData = recData;
            return arrRet;
        }

        public string ReadDataByPosWithCode(string str_ID, int int_Len, int pos, out string RevStrData)
        {
            RevStrData = "";
            if (MeterProtocols[pos] == null) return string.Empty;
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return string.Empty;
            return MeterProtocols[pos].ReadData(str_ID, int_Len, out RevStrData);
        }

        /// <summary>
        /// 读取数据（字符型，数据项）
        /// </summary>
        /// <param name="str_ID">标识符,2个字节</param>
        /// <param name="int_Len">数据长度(字节数)</param>
        /// <returns></returns>
        public string[] ReadData(string str_ID, int int_Len)
        {
            string[] arrRet = new string[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate(int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                arrRet[pos] = ReadDataByPos(str_ID, int_Len, pos);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            return arrRet;
        }

        public string ReadDataByPos(string str_ID, int int_Len, int pos)
        {
            if (MeterProtocols[pos] == null) return string.Empty;
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return string.Empty;
            return MeterProtocols[pos].ReadData(str_ID, int_Len);
        }

        #endregion

        

        #region 写数据+
        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="str_ID">标识符</param>
        /// <param name="byt_Value">写入数据</param>
        /// <returns></returns>
        public bool[] WriteData(string str_ID, byte[] byt_Value)
        {
            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate(int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                arrRet[pos] = WriteData(str_ID, byt_Value, pos);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            return arrRet;
        }
        public bool WriteData(string str_ID, byte[] byt_Value, int pos)
        {
            if (MeterProtocols[pos] == null) return false;
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return true;
            bool bln_Rst = false;

            bln_Rst = MeterProtocols[pos].WriteData(str_ID, byt_Value);

            return bln_Rst;
        }

        /// <summary>
        /// 写数据(字符型，数据项)
        /// </summary>
        /// <param name="str_ID">标识符,两个字节</param>
        /// <param name="int_Len">数据长度(块中每项字节数)</param>
        /// <param name="str_Value">写入数据</param>
        /// <returns></returns>
        public bool[] WriteArrData(string str_ID, int int_Len, string[] str_Value)
        {
            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate(int pos)
            {
                if (!runFlag) return;
                arrRet[pos] = WriteData(str_ID, int_Len, str_Value[pos], pos);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            return arrRet;
        }

        /// <summary>
        /// 写数据(字符型，数据项)
        /// </summary>
        /// <param name="str_ID">标识符,两个字节</param>
        /// <param name="int_Len">数据长度(块中每项字节数)</param>
        /// <param name="str_Value">写入数据</param>
        /// <returns></returns>
        public bool[] WriteData(string str_ID, int int_Len, string str_Value)
        {
            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate(int pos)
            {
                if (!runFlag) return;
                arrRet[pos] = WriteData(str_ID, int_Len, str_Value, pos);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            return arrRet;
        }
        public bool WriteData(string str_ID, int int_Len, string str_Value, int pos)
        {
            if (MeterProtocols[pos] == null) return false;
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return true;
            bool blnWriteType = false;

            bool bln_Rst = false;
            if (Helper.MeterDataHelper.Instance.Meter(pos).DgnProtocol.IsSouthEncryption) //南网费控
            {
                //身份认证
                //判断参数更新文件
                //处理数据明文
                //取得密文
                //组帧发送电表
                if (str_ID == "04001503")//三类数据
                {
                    bln_Rst = MeterProtocols[pos].WriteData(str_ID, int_Len, str_Value);
                }
                else if (CheckStrIDType(str_ID) == 2)//二类数据
                {


                    string rand1 = "";
                    string rand2 = "";
                    string esamNo = "";
                    string[] PutData = new string[this.bwCount];
                    string[] DataCode = new string[bwCount];
                    string[] strData = new string[bwCount];
                    string[] strID = new string[bwCount];
                    int Fag = SouthCheckIdentity(pos, out rand1, out rand2, out esamNo); // 检查密钥状态
                    if (Fag <= 1)
                    {
                        string str_div = Helper.MeterDataHelper.Instance.Meter(pos)._Mb_MeterNo;
                        for (int i = 0; i < bwCount; i++)
                        {
                            strID[i] = str_ID;
                            strData[i] = str_ID + str_Value;
                        }
                        bln_Rst = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(Convert.ToInt32(pos), Fag, rand2, strData[pos], strID[pos]);
                    }
                }
                else//一类数据
                {
                    string rand1 = "";
                    string rand2 = "";
                    string esamNo = "";
                    string[] PutData = new string[this.bwCount];
                    string[] DataCode = new string[bwCount];
                    string[] strData = new string[bwCount];
                    string[] strID = new string[bwCount];
                    int Fag = SouthCheckIdentity(pos, out rand1, out rand2, out esamNo); // 检查密钥状态
                    if (Fag <= 1)
                    {
                        string str_div = Helper.MeterDataHelper.Instance.Meter(pos)._Mb_MeterNo;
                        for (int i = 0; i < bwCount; i++)
                        {
                            strID[i] = str_ID;
                            strData[i] = str_ID + str_Value;
                        }
                        bln_Rst = MeterProtocolAdapter.Instance.SouthParameterUpdate(Convert.ToInt32(pos), Fag, rand2, "", strID[pos], strData[pos]);
                    }
                }
            }
            else
            {
                bln_Rst = MeterProtocols[pos].WriteData(str_ID, int_Len, str_Value);
            }
            return bln_Rst;
        }

                    /// <summary>
        /// 参变量分类
        /// </summary>
        /// <param name="str_ID">协议标识</param>
        /// <returns>1类,2类,3类</returns>
        private int CheckStrIDType(string str_IDs)
        {
            int tp = 0;
            string str_ID = str_IDs.ToUpper();
            if (DicIDType.ContainsKey(str_ID))
            {
                tp = 1;
            }
            else if (str_ID.IndexOf("040501", 0) > 0)
            {
                tp = 1;
            }
            else if (str_ID.IndexOf("040502", 0) > 0)
            {
                tp = 1;
            }
            else
            {
                tp = 2;
            }
            return tp;
        }
        #endregion

        
        #endregion

        /// <summary>
        /// 打开RS485通断状态
        /// </summary>
        /// <param name="pos">表位号,0-bwCount.0xFF为全部打开0xFE为全部关闭</param>
        /// <returns></returns>
        private bool OpenCommSwitch(int pos)
        {
            //bool[] openStatus = new bool[bwCount];
            //int channelID = pos / oneChannelMeterCount;
            //int startPos = pos * oneChannelMeterCount;
            //int endPos = startPos + oneChannelMeterCount;
            //for (int i = startPos; i < endPos; i++)
            //{
            //    if (i == pos || pos == 0xFF)
            //        openStatus[pos] = true;
            //}
            //return Helper.EquipHelper.Instance.InitPara_CommTest(openStatus);
            return true;
        }

                /// <summary>
        /// 参变量分类字典。key 协议标识 value 1类2类3类
        /// </summary>
        private Dictionary<string, int> DicIDType = new Dictionary<string, int> { 
        {"04000108",1},{"04000109",1},{"04000306",1},{"04000307",1},
        {"04000402",1},{"0400040E",1},{"04001001",1},{"04001002",1},
        {"040501XX",1},{"040502XX",1},{"040604FF",1},{"040605FF",1},
        };

        /// <summary>
        /// 数组翻转
        /// </summary>
        /// <param name="dData"></param>
        /// <returns></returns>
        private byte[] bytesReserve(byte[] bData)
        {
            if (bData.Length <= 0 || bData == null)
            {
                return null;
            }
            byte[] tdata = new byte[bData.Length];
            for (int i = bData.Length - 1; i >= 0; i--)
            {
                tdata[bData.Length - 1 - i] = bData[i];
            }
            return tdata;
        }


        /// <summary>
        /// HEX字符串转字节数组,并反转
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        private byte[] stringToByte(string hexString)
        {
            int DLen = hexString.Length / 2;
            byte[] DataFrame = new byte[DLen];
            for (int i = DLen - 1; i >= 0; i--)
            {
                string bb = hexString.Substring(i * 2, 2);
                DataFrame[DLen - 1 - i] = byte.Parse(bb, System.Globalization.NumberStyles.HexNumber);
            }
            return DataFrame;

        }



        #region 南网
        public bool SouthLink()
        {
            string str_message = "";
            //int rst = EncryptionTool.SouthOpenDevice("南网加密机", "192.168.19.99", 8018, 30, out str_message);
            int rst = EncryptionTool.SouthOpenDevice(GlobalUnit.ENCRYPTION_MACHINE_TYPE, GlobalUnit.ENCRYPTION_MACHINE_IP, Convert.ToInt32(GlobalUnit.ENCRYPTION_MACHINE_PORT), Convert.ToInt32(GlobalUnit.ENCRYPTION_MACHINE_OUTTIME), out str_message);
            return rst == 0;
        }
        #region 检查身份认证状态并认证
        /// <summary>
        /// 身份认证
        /// </summary>
        /// <param name="strRand1"></param>
        /// <param name="strRand2"></param>
        /// <param name="strEsamNo"></param>
        /// <returns></returns>
        public int[] SouthCheckIdentity(out string[] strRand1,out  string[] strRand2,out string[] strEsamNo)
        {
            string[] strRand1Tmp = new string[bwCount];
            string[] strRand2Tmp = new string[bwCount];
            string[] strEsamNoTmp = new string[bwCount];
            int[] iFlag = new int[bwCount];

          
            MessageController.Instance.AddMessage("正在检查密钥状态,请稍候....");
            string[] MyStatus = MeterProtocolAdapter.Instance.ReadData("04000508", 4);
            Thread.Sleep(1000);
            for (int i = 0; i < bwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(MyStatus[i]))
                {
                    if (MyStatus[i].EndsWith("1FFFF"))
                    {
                        iFlag[i] = 1;
                    }
                    else
                    {
                        iFlag[i] = 0;
                    }
                }
                else
                {
                    iFlag[i] = 0;
                }
            }

            MessageController.Instance.AddMessage("正在身份认证,请稍候....");
            bool[] bResult = SouthIdentity(iFlag, out strRand1Tmp, out strRand2Tmp, out strEsamNoTmp);
            Thread.Sleep(1000);
           // string[] MyStatus1 = MeterProtocolAdapter.Instance.ReadData("00010000", 4);
            strRand1 = strRand1Tmp;
            strRand2 = strRand2Tmp;
            strEsamNo = strEsamNoTmp;
            return iFlag;
        }

        /// <summary>
        /// 单表位身份认证
        /// </summary>
        /// <param name="strRand1"></param>
        /// <param name="strRand2"></param>
        /// <param name="strEsamNo"></param>
        /// <returns></returns>
        public int SouthCheckIdentity(int pos,out string strRand1, out  string strRand2, out string strEsamNo)
        {
            int IdentityType = 0;
            string strRand1Tmp = "";
            string strRand2Tmp = "";
            string strEsamNoTmp = "";

            bool GyResult = SouthIdentity(pos, 0, out strRand1Tmp, out strRand2Tmp, out strEsamNoTmp);
            for (int i = 0; i < bwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (GyResult)
                {
                    IdentityType = 0;
                }
                else
                {
                    bool SyResult = SouthIdentity(pos, 1, out strRand1Tmp, out strRand2Tmp, out strEsamNoTmp);
                    if (SyResult)
                    {
                        IdentityType = 1;
                    }
                    else
                    {
                        IdentityType = 2;
                    }
                }
            }
            strRand1 = strRand1Tmp;
            strRand2 = strRand2Tmp;
            strEsamNo = strEsamNoTmp;
            return IdentityType;
        }
        #endregion

        #region 进行蓝牙请求并认证
        /// <summary>
        /// 进行蓝牙请求并认证
        /// </summary>
        public void SouthCheckBlueToothIdentity()
        {
            string[] strRand1 = new string[bwCount];
            string[] strRand2 = new string[bwCount];
            string[] strEsamNo = new string[bwCount];
            string[] strERand1 = new string[bwCount];
            string[] strERand2 = new string[bwCount];
            int[] iFlag = new int[bwCount];

            if (GlobalUnit.g_CommunType == Cus_CommunType.通讯蓝牙)
            {
                MessageController.Instance.AddMessage("正在进行红外认证查询,请稍候....");
                bool[] bln_Rst1 = MeterProtocolAdapter.Instance.SouthInfraredRand(out strEsamNo, out strRand1, out strERand1, out strRand2);

                MessageController.Instance.AddMessage("正在进行公钥红外认证,请稍候....");
                bool[] bln_Rst2 = MeterProtocolAdapter.Instance.SouthInfraredAuth(iFlag, strEsamNo, strRand1, strERand1, strRand2, out strERand2);

                MessageController.Instance.AddMessage("正在进行私钥红外认证,请稍候....");
                Common.Memset(ref iFlag, 1);
                bool[] bln_Rst3 = MeterProtocolAdapter.Instance.SouthInfraredAuth(iFlag, strEsamNo, strRand1, strERand1, strRand2, out strERand2);
            }
        }
        #endregion


        #region 检查交互终端身份认证状态并认证
        public int[] SouthCheckTerminalIdentity(out string[] strRand1, out string[] strRand2, out string[] strEsamNo)
        {
            int[] IdentityType = new int[bwCount];
            string[] strRand1Tmp = new string[bwCount];
            string[] strRand2Tmp = new string[bwCount];
            string[] strEsamNoTmp = new string[bwCount];
            int[] iFlag = new int[bwCount];
            for (int i = 0; i < bwCount; i++)
            {
                iFlag[i] = 0;
            }

            bool[] GyResult = SouthIdentityAuthentication (iFlag,out strRand1Tmp, out strRand2Tmp, out strEsamNoTmp);
            for (int i = 0; i < bwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (GyResult[i])
                {
                    IdentityType[i] = 0;
                }
                else
                {
                    bool SyResult = SouthIdentityAuthentication(i, 1,out strRand1Tmp[i], out strRand2Tmp[i], out strEsamNoTmp[i]);
                    if (SyResult)
                    {
                        IdentityType[i] = 1;
                    }
                    else
                    {
                        IdentityType[i] = 2;
                    }
                }
            }
            strRand1 = strRand1Tmp;
            strRand2 = strRand2Tmp;
            strEsamNo = strEsamNoTmp;
            return IdentityType;
        }
        #endregion

        #region 身份认证
        /// <summary>
        /// 安全认证
        /// </summary>
        /// <param name="pos">表位号</param>
        /// <param name="str_Rand">随机数</param>
        /// <param name="str_Endata">密文</param>
        /// <param name="m_str_Div">分散因子</param>
        /// <returns></returns>
        public bool IdentityAuthentication(int pos, string str_Rand, string str_Endata, string m_str_Div, out byte[] rand2, out byte[] esamNo)
        {
            rand2 = new byte[4];
            esamNo = new byte[8];
            if (MeterProtocols[pos] == null) return false;
            str_Rand = str_Rand.PadLeft(16, '0');
            str_Endata = str_Endata.PadLeft(16, '0');
            //if (is_N2013_2007[pos] == false)
            //{
            m_str_Div = m_str_Div.PadLeft(16, '0');
            //}
            //else
            //{
            //    m_str_Div = "0000000000000001";
            //}
            byte[] data = new byte[32];
            byte[] code = bytesReserve(new byte[] { 0x07, 0x00, 0x00, 0xFF });
            byte[] oper = new byte[4];
            byte[] Rand = stringToByte(str_Rand);
            byte[] Endata = stringToByte(str_Endata);
            byte[] Div = stringToByte(m_str_Div);
            Array.Copy(code, 0, data, 0, 4);
            Array.Copy(oper, 0, data, 4, 4);
            Array.Copy(Endata, 0, data, 8, Endata.Length);
            Array.Copy(Rand, 0, data, 16, Rand.Length);
            Array.Copy(Div, 0, data, 24, Div.Length);
            bool seqela = false;
            byte[] revdata = null;
            if (MeterProtocols[pos].UpdateRemoteEncryptionCommand(0x03, data, ref seqela, ref revdata))
            {
                if (revdata.Length != 16)
                {
                    return false;
                }
                byte[] r2 = new byte[4];
                byte[] eNo = new byte[8];
                Array.Copy(revdata, 4, r2, 0, 4);
                Array.Copy(revdata, 8, eNo, 0, 8);
                rand2 = bytesReserve(r2); //随机数2
                esamNo = bytesReserve(eNo);//ESAM 序列号
                return true;
            }
            return false;
        }

        /// <summary>
        /// 身份认证
        /// </summary>
        /// <param name="Flag">0=公钥，1=私钥</param>
        /// <param name="rand1">加密机传出的随机数1</param>
        /// <param name="rand2">所有表位电能表传出的随机数2</param>
        /// <param name="esamNo">所有表位电能表esam号</param>
        /// <returns></returns>
        public bool[] SouthIdentity(int[] Flag, out string[] rand1, out string[] rand2, out string[] esamNo)
        {
            string[] rand2Tmps = new string[bwCount];
            string[] rand1Tmps = new string[bwCount];
            string[] esamNoTmps = new string[bwCount];
            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate (int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                if (!GlobalUnit.IsDemo)
                    arrRet[pos] = SouthIdentity(pos, Flag[pos], out rand1Tmps[pos], out rand2Tmps[pos], out esamNoTmps[pos]);
                else
                    arrRet[pos] = true;

            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            rand2 = rand2Tmps;
            rand1 = rand1Tmps;
            esamNo = esamNoTmps;
            return arrRet;
        }
        /// <summary>
        /// 单表位身份认证
        /// </summary>
        /// <param name="pos">表位号</param>
        /// <param name="rand1">加密机传出的随机数1</param>
        /// <param name="Outrand2">电能表传出的随机数2</param>
        /// <param name="OutesamNo">电能表esam号</param>
        /// <returns></returns>
        public bool SouthIdentity(int pos, int Flag, out string rand1, out string Outrand2, out string OutesamNo)
        {
            rand1 = "";
            Outrand2 = "";
            OutesamNo = "";
            if (MeterProtocols[pos] == null) return false;
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return true;
            byte[] rand2 = new byte[4];
            byte[] esamNo = new byte[8];

            string str_rand = "";
            string str_endata = "";
            string str_message = "";
            string str_Div = "0000" + Helper.MeterDataHelper.Instance.Meter(pos)._Mb_MeterNo;
            int rst = EncryptionTool.SouthIdentityAuthentication(Flag, str_Div, out str_rand, out str_endata, out str_message);
            if (rst == 0)
            {
                rand1 = str_rand;
                bool rst2 = IdentityAuthentication(pos, str_rand, str_endata, str_Div, out rand2, out esamNo);
                Outrand2 = BitConverter.ToString(rand2).Replace("-", "");
                OutesamNo = BitConverter.ToString(esamNo).Replace("-", "");
                return rst2;
            }
            return false;
        }
        #endregion

        #region 交互终端身份认证
        /// <summary>
        /// 交互终端和身份认证
        /// </summary>
        /// <param name="rand2">所有表位电能表传出的随机数</param>
        /// <param name="esamNo">所有表位电能表esam号</param>
        /// <returns></returns>
        public bool[] SouthIdentityAuthentication(int[] Flag, out string[] rand1, out string[] rand2, out string[] esamNo)
        {
            string[] rand1Tmps = new string[bwCount];
            string[] rand2Tmps = new string[bwCount];
            string[] esamNoTmps = new string[bwCount];
            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate (int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                if (!GlobalUnit.IsDemo)
                    arrRet[pos] = SouthIdentityAuthentication(pos, Flag[pos],out rand1Tmps[pos], out rand2Tmps[pos], out esamNoTmps[pos]);
                else
                    arrRet[pos] = true;

            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            rand1 = rand1Tmps;
            rand2 = rand2Tmps;
            esamNo = esamNoTmps;
            return arrRet;
        }
        /// <summary>
        /// 单表位身份认证
        /// </summary>
        /// <param name="pos">表位号</param>
        /// <param name="Outrand2">电能表传出的随机数</param>
        /// <param name="OutesamNo">电能表esam号</param>
        /// <returns></returns>
        public bool SouthIdentityAuthentication(int pos, int Flag, out string rand1, out string Outrand2, out string OutesamNo)
        {
            rand1 = "";
            Outrand2 = "";
            OutesamNo = "";
            byte[] rand2 = new byte[4];
            byte[] esamNo = new byte[8];

            string str_rand = "";
            string str_endata = "";
            string str_message = "";
            string str_Div = "0000" + Helper.MeterDataHelper.Instance.Meter(pos)._Mb_MeterNo;
            int rst = EncryptionTool.SouthIdentityAuthentication(Flag, str_Div, out str_rand, out str_endata, out str_message);
            if (rst == 0)
            {
                rand1 = str_rand;
                bool rst2 = SouthTerminalIdentityAuthentication(pos, str_rand, str_endata, str_Div, out rand2, out esamNo);
                Outrand2 = BitConverter.ToString(rand2).Replace("-", "");
                OutesamNo = BitConverter.ToString(esamNo).Replace("-", "");
                return rst2;
            }
            return false;
        }
        /// <summary>
        /// 交互终端安全认证
        /// </summary>
        /// <param name="pos">表位号</param>
        /// <param name="str_Rand">随机数</param>
        /// <param name="str_Endata">密文</param>
        /// <param name="m_str_Div">分散因子</param>
        /// <returns></returns>
        private bool SouthTerminalIdentityAuthentication(int pos, string str_Rand, string str_Endata, string m_str_Div, out byte[] rand2, out byte[] esamNo)
        {
            rand2 = new byte[4];
            esamNo = new byte[8];
            if (MeterProtocols[pos] == null) return false;
            str_Rand = str_Rand.PadLeft(16, '0');
            str_Endata = str_Endata.PadLeft(16, '0');

            m_str_Div = m_str_Div.PadLeft(16, '0');

            byte[] data = new byte[32];
            byte[] code = bytesReserve(new byte[] { 0x07, 0xA0, 0x01, 0xFF });
            byte[] oper = new byte[4];
            byte[] Rand = stringToByte(str_Rand);
            byte[] Endata = stringToByte(str_Endata);
            byte[] Div = stringToByte(m_str_Div);
            Array.Copy(code, 0, data, 0, 4);
            Array.Copy(oper, 0, data, 4, 4);
            Array.Copy(Endata, 0, data, 8, Endata.Length);
            Array.Copy(Rand, 0, data, 16, Rand.Length);
            Array.Copy(Div, 0, data, 24, Div.Length);
            bool seqela = false;
            byte[] revdata = null;
            if (MeterProtocols[pos].UpdateRemoteEncryptionCommand(0x03, data, ref seqela, ref revdata))
            {
                if (revdata.Length != 25)
                {
                    return false;
                }
                byte[] r2 = new byte[4];
                byte[] eNo = new byte[8];
                Array.Copy(revdata, 4, r2, 0, 4);
                Array.Copy(revdata, 8, eNo, 0, 8);
                rand2 = bytesReserve(r2); //随机数2
                esamNo = bytesReserve(eNo);//ESAM 序列号
                return true;
            }
            return false;
        }
        #endregion

        #region 远程控制
        /// <summary>
        /// 远程拉闸、合闸、报警等控制数据计算。
        /// </summary>
        /// <param name="Flag"></param>
        /// <param name="Rand2"></param>
        /// <param name="EsamNo"></param>
        /// <param name="strData"></param>
        /// <returns></returns>
        public bool[] SouthUserControl(int[] Flag, string[] Rand2, string[] EsamNo, string[] strData)
        {
            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate (int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                if (!GlobalUnit.IsDemo)
                    arrRet[pos] = SouthUserControl(pos, Flag[pos], Rand2[pos], EsamNo[pos], strData[pos]);
                    
                else
                    arrRet[pos] = true;


            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();

            return arrRet;
        }
        public bool SouthUserControl(int pos, int Flag, string Rand2, string EsamNo, string strData)
        {
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return true;
            string str_EndData;
            string str_Message = "";
            string PutDiv = "0000" + Helper.MeterDataHelper.Instance.Meter(pos)._Mb_MeterNo;
            int bReturn = EncryptionTool.SouthUserControl(Flag, Rand2, PutDiv, EsamNo, strData, out str_EndData, out str_Message);

            if (bReturn == 0)
            {
                if (UserControl(pos, str_EndData))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="str_Endata"></param>
        /// <returns></returns>
        public bool UserControl(int pos, string str_Endata)
        {
            byte[] oper = new byte[4];
            byte[] byt_Endata = stringToByte(str_Endata);
            List<byte> frameData = new List<byte>();
            frameData.AddRange(oper);
            frameData.AddRange(byt_Endata);

            bool seqela = false;
            byte[] revdata = null;

            if (MeterProtocols[pos].UpdateRemoteEncryptionCommand(0x1C, frameData.ToArray(), ref seqela, ref revdata))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 远程拉闸、合闸、报警等控制数据计算。
        /// </summary>
        /// <param name="Flag"></param>
        /// <param name="Rand2"></param>
        /// <param name="EsamNo"></param>
        /// <param name="strData">下发的明文</param>
        /// <param name="strData">返回的密文</param>
        /// <returns></returns>
        public bool[] SouthUserControl(int[] Flag, string[] Rand2, string[] EsamNo, string[] strData,out string[] strOutEndData)
        {
            string [] strOutEndDataTmp = new string[bwCount];
            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate(int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                if (!GlobalUnit.IsDemo)
                    arrRet[pos] = SouthUserControl(pos, Flag[pos], Rand2[pos], EsamNo[pos], strData[pos],out strOutEndDataTmp[pos]);
                else
                    arrRet[pos] = true;


            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            strOutEndData = strOutEndDataTmp;
            return arrRet;
        }
        public bool SouthUserControl(int pos, int Flag, string Rand2, string EsamNo, string strData, out string str_EndData)
        {
            string str_EndDataTmp = "";
            str_EndData = str_EndDataTmp;
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return true;
            string str_Message = "";
            string PutDiv = "0000" + Helper.MeterDataHelper.Instance.Meter(pos)._Mb_MeterNo;
            int bReturn = EncryptionTool.SouthUserControl(Flag, Rand2, PutDiv, EsamNo, strData, out str_EndDataTmp, out str_Message);

            if (bReturn == 0)
            {
                str_EndData = str_EndDataTmp;
                return true;
            }
            return false;
        }
        #endregion

        #region 参数信息更新
        /// <summary>
        /// 用于参数信息计算。
        /// </summary>
        /// <param name="Flag"></param>
        /// <param name="Rand2"></param>
        /// <param name="PutApdu"></param>
        /// <param name="str_ID"></param>
        /// <param name="strData"></param>
        /// <returns></returns>
        public bool[] SouthParameterUpdate(int[] Flag, string[] Rand2, string[] PutApdu, string[] str_ID, string[] strData)
        {
            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate (int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                if (!GlobalUnit.IsDemo)
                    arrRet[pos] = SouthParameterUpdate(pos, Flag[pos], Rand2[pos], PutApdu[pos], str_ID[pos], strData[pos]);
                else
                    arrRet[pos] = true;


            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();

            return arrRet;
        }
        public bool SouthParameterUpdate(int pos, int Flag, string Rand2, string PutApdu, string str_ID, string strData)
        {
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return true;
            string OutData;
            string str_Message = "";
            string PutDiv = "0000" + Helper.MeterDataHelper.Instance.Meter(pos)._Mb_MeterNo;
            int bReturn = EncryptionTool.SouthParameterUpdate(Flag, Rand2, PutDiv, PutApdu, strData, out OutData, out str_Message);

            if (bReturn == 0)
            {
                string str_Tmp = OutData.Substring(OutData.Length - 8, 8);
                string str_m_data = str_Tmp + OutData.Substring(0, OutData.Length - 8);
                if (str_ID == "070001FF")
                {
                    List<byte> frameData = new List<byte>();
                    byte[] code = stringToByte(str_ID);
                    byte[] data = stringToByte(str_m_data);
                    byte[] recData = null;
                    bool sequela = false;

                    byte[] oper = new byte[4];
                    frameData.AddRange(code);
                    frameData.AddRange(oper);
                    frameData.AddRange(data);
                    if (MeterProtocols[pos].UpdateRemoteEncryptionCommand(0x03, frameData.ToArray(), ref sequela, ref recData))
                    {
                        return true;
                    }
                }
                else
                {
                    if (MeterProtocols[pos].WriteData(str_ID, str_m_data.Length / 2, str_m_data))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 用于参数信息计算。返回数据
        /// </summary>
        /// <param name="Flag"></param>
        /// <param name="Rand2"></param>
        /// <param name="PutApdu"></param>
        /// <param name="str_ID"></param>
        /// <param name="strData"></param>
        /// <returns></returns>
        public bool[] SouthParameterUpdate(int[] Flag, string[] Rand2, string[] PutApdu, string[] strData, out string[] strRevData,out string[] strRevMac)
        {
            string[] OutData = new string[bwCount];
            string[] OutMac = new string[bwCount];
            strRevData = OutData;
            strRevMac = OutMac;
            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate (int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                if (!GlobalUnit.IsDemo)
                    arrRet[pos] = SouthParameterUpdate(pos, Flag[pos], Rand2[pos], PutApdu[pos], strData[pos],out OutData[pos], out OutMac[pos]);
                else
                    arrRet[pos] = true;

            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            strRevData = OutData;
            return arrRet;
        }
        public bool SouthParameterUpdate(int pos, int Flag, string Rand2, string PutApdu, string strData, out string strRevData, out string strRevMac)
        {
            string OutData = "";
            string OutMac = "";
            strRevData = OutData;
            strRevMac = OutMac;
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return true;
            string str_Message = "";
            string PutDiv = "0000" + Helper.MeterDataHelper.Instance.Meter(pos)._Mb_MeterNo;
            int bReturn = EncryptionTool.SouthParameterUpdate(Flag, Rand2, PutDiv, PutApdu, strData, out OutData, out str_Message);
            if (bReturn == 0)
            {
                strRevData = OutData.Substring(0, OutData.Length - 8) ;
                strRevMac = OutData.Substring(OutData.Length - 8, 8);
                return true;
            }
            return false;
        }
        #endregion

        #region 数据回抄
        /// <summary>
        /// 数据回抄
        /// </summary>
        /// <param name="Flag"></param>
        /// <param name="Rand1"></param>
        /// <param name="str_RevCode">数据回抄标识</param>
        /// <param name="OutRevData"></param>
        /// <param name="OutMac"></param>
        /// <returns></returns>
        public bool[] SouthMacCheck(int[] Flag, string[] Rand1, string[] str_RevCode, out string[] OutRevData, out string[] OutMac)
        {
            string[] OutRevDataTmp = new string[bwCount];
            string[] OutMacTmp = new string[bwCount];
            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate (int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                if (!GlobalUnit.IsDemo)
                    arrRet[pos] = SouthMacCheck(pos, Flag[pos], Rand1[pos], str_RevCode[pos], out OutRevDataTmp[pos], out OutMacTmp[pos]);
                else
                    arrRet[pos] = true;


            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            OutRevData = OutRevDataTmp;
            OutMac = OutMacTmp;
            return arrRet;
        }
        /// <summary>
        /// 数据回抄
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="Flag"></param>
        /// <param name="Rand1"></param>
        /// <param name="str_RevCode">数据回抄标识共8字节</param>
        /// <param name="OutRevData"></param>
        /// <param name="OutMac"></param>
        /// <returns></returns>
        public bool SouthMacCheck(int pos, int Flag, string Rand1, string str_RevCode, out string OutRevData, out string OutMac)
        {
            OutRevData = "";
            OutMac = "";
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return true;
            string str_Message = "";
            string PutDiv = "0000" + Helper.MeterDataHelper.Instance.Meter(pos)._Mb_MeterNo;
            byte[] cmd = bytesReserve(new byte[] { 0x07, 0x80, 0x01, 0xFF });
            byte[] oper = new byte[4];
            byte[] rCode = stringToByte(str_RevCode);    //回抄数据标识
            List<byte> frameData = new List<byte>();
            frameData.AddRange(cmd);
            frameData.AddRange(oper);
            frameData.AddRange(rCode);
            bool seqela = false;
            byte[] revdata = null;
            if (MeterProtocols[pos].UpdateRemoteEncryptionCommand(0x03, frameData.ToArray(), ref seqela, ref revdata))
            {
                byte[] kinfo = new byte[revdata.Length - 8];
                byte[] mac = new byte[4];
                Array.Copy(revdata, 4, kinfo, 0, revdata.Length - 8);
                Array.Copy(revdata, revdata.Length - 4, mac, 0, 4);
                kinfo = bytesReserve(kinfo);
                mac = bytesReserve(mac);
                string str_Mac = BitConverter.ToString(mac).Replace("-", "");
                string strRData = BitConverter.ToString(kinfo).Replace("-", "");
                string strOnlyData = strRData.Substring(0, strRData.Length - 16);
                int LC = strOnlyData.Length / 2 + 12;
                string strIndex = str_RevCode.Substring(10, 2);
                string PutApdu = "04D686" + strIndex + LC.ToString("X2");

                //int result = EncryptionTool.SouthMacCheck(Flag, Rand1.PadRight(8, '0').Substring(0, 8), PutDiv, PutApdu, strOnlyData, str_Mac, out str_Message);
                //if (0 == result)  暂不校验 电价文件及返写文件校验不过
                {
                    OutRevData = strOnlyData;
                    OutMac = str_Mac;
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region 数据回抄 购电金额和购电次数
        /// <summary>
        /// 数据回抄
        /// </summary>
        /// <param name="Flag"></param>
        /// <param name="Rand1"></param>
        /// <param name="str_RevCode">数据回抄标识</param>
        /// <param name="OutRevData"></param>
        /// <param name="OutMac1">加密机返回的mac</param>
        /// <param name="OutMac2">数据返回的mac 返回格式=购电次数+mac+剩余金额</param>
        /// <param name="OutRevCount">剩余金额</param>
        /// <param name="OutRevMoney">购电次数</param>
        /// <returns></returns>
        public bool[] SouthMacCheck(int[] Flag, string[] Rand1, string[] str_RevCode,out string[] OutMac1, out string[] OutMac2, out string[] OutRevCount,out string[] OutRevMoney)
        {
            string[] OutMac1Tmp = new string[bwCount];
            string[] OutMac2Tmp = new string[bwCount];
            string[] OutRevCountTmp = new string[bwCount];
            string[] OutRevMoneyTmp = new string[bwCount];
            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate (int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                if (!GlobalUnit.IsDemo)
                    arrRet[pos] = SouthMacCheck(pos, Flag[pos], Rand1[pos], str_RevCode[pos], out OutMac1Tmp[pos], out OutMac2Tmp[pos], out OutRevCountTmp[pos], out OutRevMoneyTmp[pos]);
                else
                    arrRet[pos] = true;


            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            OutRevCount = OutRevCountTmp;
            OutRevMoney = OutRevMoneyTmp;
            OutMac1 = OutMac1Tmp;
            OutMac2 = OutMac2Tmp;
            return arrRet;
        }
        /// <summary>
        /// 数据回抄
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="Flag"></param>
        /// <param name="Rand1"></param>
        /// <param name="str_RevCode">数据回抄标识</param>
        /// <param name="OutRevData"></param>
        /// <param name="OutMac1">加密机返回的mac</param>
        /// <param name="OutMac2">数据返回的mac 返回格式=购电次数+mac+剩余</param>
        /// <param name="OutRevCount">剩余金额</param>
        /// <param name="OutRevMoney">购电次数</param>
        /// <returns></returns>
        public bool SouthMacCheck(int pos, int Flag, string Rand1, string str_RevCode, out string OutMac1, out string OutMac2, out string OutRevCount, out string OutRevMoney)
        {

            OutMac1 = "";
            OutMac2 = "";
            OutRevCount = "";
            OutRevMoney = "";
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return true;
            
            string PutDiv = "0000" + Helper.MeterDataHelper.Instance.Meter(pos)._Mb_MeterNo;
            byte[] cmd = bytesReserve(new byte[] { 0x07, 0x80, 0x01, 0xFF });
            byte[] oper = new byte[4];
            byte[] rCode = stringToByte(str_RevCode);    //回抄数据标识
            List<byte> frameData = new List<byte>();
            frameData.AddRange(cmd);
            frameData.AddRange(oper);
            frameData.AddRange(rCode);
            bool seqela = false;
            byte[] revdata = null;
            if (MeterProtocols[pos].UpdateRemoteEncryptionCommand(0x03, frameData.ToArray(), ref seqela, ref revdata))
            {
                byte[] kinfo = new byte[revdata.Length - 8];
                byte[] mac = new byte[4];
                Array.Copy(revdata, 4, kinfo, 0, revdata.Length - 8);
                Array.Copy(revdata, revdata.Length - 4, mac, 0, 4);
                kinfo = bytesReserve(kinfo);
                mac = bytesReserve(mac);
                string str_Mac = BitConverter.ToString(mac).Replace("-", "");
                string strRData = BitConverter.ToString(kinfo).Replace("-", "");
                string strOnlyData = strRData.Substring(0, strRData.Length - 16);
                int LC = 4 + 12;
                string strIndex = str_RevCode.Substring(10, 2);
                string PutApdu = "04D686" + strIndex + LC.ToString("X2");
                OutMac1 = str_Mac;
                OutMac2 = DxString(strOnlyData.Substring(8, 8));
                //strOnlyData = strOnlyData.Substring(16,8);

                //int result = EncryptionTool.SouthMacCheck(Flag, Rand1.PadRight(8, '0').Substring(0, 8), PutDiv, PutApdu, strOnlyData, OutMac2, out str_Message);
                //if (0 == result)
                {
                    OutRevCount = strOnlyData.Substring(0, 8);
                    OutRevMoney = strOnlyData.Substring(16, 8);
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region 费控模式切换
        /// <summary>
        /// 用于费控模式切换，仅正式密钥状态下可以做此操作。
        /// </summary>
        /// <param name="rand2"></param>
        /// <param name="strData"></param>
        /// <returns></returns>
        public bool[] SouthSwitchChargeMode(int[] Flag, string[] rand2, string[] strData)
        {
            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate (int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                if (!GlobalUnit.IsDemo)
                    arrRet[pos] = SouthSwitchChargeMode(pos,Flag[pos], rand2[pos], strData[pos]);
                else
                    arrRet[pos] = true;

            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();

            return arrRet;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="Rand2"></param>
        /// <param name="strData">包含费控模式状态字 + 购电金额 + 购电次数, 金额、次数均为HEX码；</param>
        /// <returns></returns>
        public bool SouthSwitchChargeMode(int pos, int Flag, string Rand2, string strData)
        {
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return true;
            string str_endata = "";
            string str_message = "";
            string str_Div = "0000" + Helper.MeterDataHelper.Instance.Meter(pos)._Mb_MeterNo;
            int rst = EncryptionTool.SouthSwitchChargeMode(Flag, Rand2, str_Div, strData, out str_endata, out str_message);
            if (rst == 0)
            {
                byte[] cmd = bytesReserve(new byte[] { 0x07, 0xA0, 0x06, 0xFF });
                byte[] oper = new byte[4];
                //费控模式状态字+MAC1+购电金额+购电次数+MAC2
                byte[] byt_m_data = new byte[str_endata.Length / 2 + 8];
                Array.Copy(stringToByte(str_Div), 0, byt_m_data, 0, 8);
                Array.Copy((stringToByte(str_endata.Substring(0, 2))), 0, byt_m_data, 8, 1);
                Array.Copy((stringToByte(str_endata.Substring(2, 8))), 0, byt_m_data, 9, 4);
                Array.Copy((stringToByte(str_endata.Substring(10, 8))), 0, byt_m_data, 13, 4);
                Array.Copy((stringToByte(str_endata.Substring(18, 8))), 0, byt_m_data, 17, 4);
                Array.Copy((stringToByte(str_endata.Substring(26, 8))), 0, byt_m_data, 21, 4);
                List<byte> frameData = new List<byte>();
                frameData.AddRange(cmd);
                frameData.AddRange(oper);
                frameData.AddRange(byt_m_data);
                bool seqela = false;
                byte[] revdata = null;
                if (MeterProtocols[pos].UpdateRemoteEncryptionCommand(0x03, frameData.ToArray(), ref seqela, ref revdata))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region    记录信息文件1更新
        /// <summary>
        /// 记录信息文件1更新
        /// </summary>
        /// <param name="PutRand">4字节随机数，电表身份认证成功后返回；</param>
        /// <param name="PutEsamNo">8字节电表安全模块序列号；</param>
        /// <param name="PutFileID">1字节文件标识；</param>
        /// <param name="PutDataBegin">2字节起始字节；</param>
        /// <param name="PutData"> 明文数据,文件最大0x95字节；</param>
        /// <returns></returns>
        public bool[] SouthMacWrite(int[] Flag, string[] PutRand, string[] PutEsamNo, string[] PutFileID, string[] PutDataBegin, string[] PutData)
        {

            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate (int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                arrRet[pos] = SouthMacWrite(pos, Flag[pos], PutRand[pos], PutEsamNo[pos], PutFileID[pos], PutDataBegin[pos], PutData[pos]);

            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            return arrRet;
        }


        /// <summary>
        /// 记录信息文件1更新
        /// </summary>
        /// <param name="pos">表位号</param>
        /// <param name="PutRand">4字节随机数，电表身份认证成功后返回；</param>
        /// <param name="PutEsamNo">8字节电表安全模块序列号；</param>
        /// <param name="PutFileID">1字节文件标识；</param>
        /// <param name="PutDataBegin">2字节起始字节；</param>
        /// <param name="PutData"> 明文数据,文件最大0x95字节；</param>
        /// <returns></returns>
        public bool SouthMacWrite(int pos, int Flag, string PutRand, string PutEsamNo, string PutFileID, string PutDataBegin, string PutData)
        {
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return true;
            string str_Message = "";
            List<byte> frameData = new List<byte>();
            byte[] code = bytesReserve(new byte[] { 0x07, 0x00, 0x04, 0xFF });
            string str_Div = "0000" + Helper.MeterDataHelper.Instance.Meter(pos)._Mb_MeterNo;

            string OutData = "";
            int result = EncryptionTool.SouthMacWrite(Flag, PutRand, str_Div, PutEsamNo, PutFileID, PutDataBegin, PutData, out OutData, out str_Message);

            bool seqela = false;
            byte[] revdata = null;
            if (result == 0)
            {
                byte[] oper = new byte[4];
                byte[] byt_m_data = new byte[OutData.Length / 2];
                Array.Copy((stringToByte(OutData.Substring(0, OutData.Length - 8))), 0, byt_m_data, 0, byt_m_data.Length - 4);
                Array.Copy((stringToByte(OutData.Substring(OutData.Length - 8, 8))), 0, byt_m_data, byt_m_data.Length - 4, 4);

                frameData.AddRange(code);
                frameData.AddRange(oper);
                frameData.AddRange(byt_m_data);

                if (MeterProtocols[pos].UpdateRemoteEncryptionCommand(0x03, frameData.ToArray(), ref seqela, ref revdata))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 记录信息文件2更新
        /// </summary>
        /// <param name="PutRand">4字节随机数，电表身份认证成功后返回；</param>
        /// <param name="PutEsamNo">8字节电表安全模块序列号；</param>
        /// <param name="PutFileID">1字节文件标识；</param>
        /// <param name="PutDataBegin">2字节起始字节；</param>
        /// <param name="PutData"> 明文数据,文件最大0x95字节；</param>
        /// <returns></returns>
        public bool[] SouthEncMacWrite(int Flag, string[] PutRand, string[] PutEsamNo, string[] PutFileID, string[] PutDataBegin, string[] PutData)
        {

            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate (int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                arrRet[pos] = SouthEncMacWrite(pos, Flag, PutRand[pos], PutEsamNo[pos], PutFileID[pos], PutDataBegin[pos], PutData[pos]);

            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            return arrRet;
        }


        /// <summary>
        /// 记录信息文件2更新
        /// </summary>
        /// <param name="pos">表位号</param>
        /// <param name="PutRand">4字节随机数，电表身份认证成功后返回；</param>
        /// <param name="PutEsamNo">8字节电表安全模块序列号；</param>
        /// <param name="PutFileID">1字节文件标识；</param>
        /// <param name="PutDataBegin">2字节起始字节；</param>
        /// <param name="PutData"> 明文数据,文件最大0x95字节；</param>
        /// <returns></returns>
        public bool SouthEncMacWrite(int pos, int Flag, string PutRand, string PutEsamNo, string PutFileID, string PutDataBegin, string PutData)
        {
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return true;
            string str_Message = "";
            List<byte> frameData = new List<byte>();
            byte[] code = bytesReserve(new byte[] { 0x07, 0x00, 0x05, 0xFF });
            string str_Div = "0000" + Helper.MeterDataHelper.Instance.Meter(pos)._Mb_MeterNo;

            string OutData = "";
            int result = EncryptionTool.SouthEncMacWrite(Flag, PutRand, str_Div, PutEsamNo, PutFileID, PutDataBegin, PutData, out OutData, out str_Message);

            bool seqela = false;
            byte[] revdata = null;
            if (result == 0)
            {
                byte[] oper = new byte[4];
                byte[] byt_m_data = new byte[OutData.Length / 2];
                Array.Copy((stringToByte(OutData.Substring(0, OutData.Length - 8))), 0, byt_m_data, 0, byt_m_data.Length - 4);
                Array.Copy((stringToByte(OutData.Substring(OutData.Length - 8, 8))), 0, byt_m_data, byt_m_data.Length - 4, 4);

                frameData.AddRange(code);
                frameData.AddRange(oper);
                frameData.AddRange(byt_m_data);

                if (MeterProtocols[pos].UpdateRemoteEncryptionCommand(0x03, frameData.ToArray(), ref seqela, ref revdata))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 程序比对数据计算
        /// </summary>
        /// <param name="PutKeyid">1字节密钥索引，本套件中支持的密钥索引为05-0a，通信规约中与函数中输入索引需统一</param>
        /// <param name="PutData">比对数据块，64字节</param>
        /// <returns></returns>
        public bool[] SouthEncForCompare(string[] PutKeyid, string[] PutData)
        {
            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate (int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                arrRet[pos] = SouthEncForCompare(pos, PutKeyid[pos], PutData[pos]);

            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            return arrRet;
        }

        /// <summary>
        /// 程序比对数据计算
        /// </summary>
        /// <param name="pos">表位号</param>
        /// <param name="PutKeyid">1字节密钥索引，本套件中支持的密钥索引为05-0a，通信规约中与函数中输入索引需统一</param>
        /// <param name="PutData">比对数据块，64字节</param>
        /// <returns></returns>
        public bool SouthEncForCompare(int pos, string PutKeyid, string PutData)
        {
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return true;
            string str_Message = "";
            List<byte> frameData = new List<byte>();
            byte[] code = bytesReserve(new byte[] { 0x07, 0x80, 0x02, 0xFF });
            string str_Div = "0000" + Helper.MeterDataHelper.Instance.Meter(pos)._Mb_MeterNo;

            string OutData = "";
            int result = EncryptionTool.SouthEncForCompare(PutKeyid, str_Div, PutData, out OutData, out str_Message);

            bool seqela = false;
            byte[] revdata = null;
            if (result == 0)
            {
                byte[] oper = new byte[4];
                byte[] Endata = stringToByte(OutData);
                frameData.AddRange(code);
                frameData.AddRange(oper);
                frameData.AddRange(Endata);

                if (MeterProtocols[pos].UpdateRemoteEncryptionCommand(0x03, frameData.ToArray(), ref seqela, ref revdata))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 钱包退费计算
        /// </summary>
        /// <param name="PutRand">输入的随机数,字符型,4字节，电表身份认证成功后返回</param>
        /// <param name="PutData">输入的4字节退费金额,字符型，HEX码</param>
        /// <returns></returns>
        public bool[] SouthDecreasePurse(string[] PutRand, string[] PutData)
        {
            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate (int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                arrRet[pos] = SouthDecreasePurse(pos, PutRand[pos], PutData[pos]);

            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            return arrRet;
        }

        /// <summary>
        /// 钱包退费计算
        /// </summary>
        /// <param name="pos">表位号</param>
        /// <param name="PutRand">输入的随机数,字符型,4字节，电表身份认证成功后返回</param>
        /// <param name="PutData">输入的4字节退费金额,字符型，HEX码</param>
        /// <returns></returns>
        public bool SouthDecreasePurse(int pos, string PutRand, string PutData)
        {
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return true;
            string str_Message = "";
            List<byte> frameData = new List<byte>();
            string code = "04001006";
            string str_Div = "0000" + Helper.MeterDataHelper.Instance.Meter(pos)._Mb_MeterNo;

            string OutData = "";
            int result = EncryptionTool.SouthDecreasePurse(1, PutRand, str_Div, PutData, out OutData, out str_Message);

            if (result == 0)
            {
                byte[] byt_m_data = new byte[OutData.Length / 2];
                Array.Copy((stringToByte(OutData.Substring(0, OutData.Length - 8))), 0, byt_m_data, 0, byt_m_data.Length - 4);
                Array.Copy((stringToByte(OutData.Substring(OutData.Length - 8, 8))), 0, byt_m_data, byt_m_data.Length - 4, 4);
                return MeterProtocols[pos].WriteData(code, byt_m_data);
            }
            return false;
        }
        #endregion

        #region 当前套电价参数
        /// <summary>
        /// 当前套电价参数
        /// </summary>
        /// <param name="Flag">表示电表密钥状态，整型，0: 测试密钥状态；1: 正式密钥状态</param>
        /// <param name="PutRand">表示输入的随机数,字符型,4字节，电表身份认证成功后返回</param>
        /// <param name="PutApdu">写电表安全模块命令头，字符型，5字节</param>
        /// <param name="PutData">表示输入的当前套电价参数明文,字符型</param>
        /// <returns></returns>
        public bool[] SouthPrice1Update(int[] Flag,string[] PutApdu, string[] PutRand,string[] str_ID, string[] PutData)
        {

            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate (int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                arrRet[pos] = SouthPrice1Update(Flag[pos],PutApdu[pos], PutRand[pos],str_ID[pos], PutData[pos], pos);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            return arrRet;
        }

        public bool SouthPrice1Update(int Flag,string PutApdu, string PutRand,string str_ID ,string PutData, int pos)
        {
            if (MeterProtocols[pos] == null) return false;
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return true;

            //表示输入的分散因子,字符型,8字节，“0000”+表号</param>
            string PutDiv = "0000" + Helper.MeterDataHelper.Instance.Meter(pos)._Mb_MeterNo;

            //定义输出的数据和MAC
            string OutData = "";
            string message = "";
            int result = EncryptionTool.SouthPrice1Update(Flag, PutRand, PutDiv, PutApdu, PutData, out OutData, out message);

            List<byte> frameData = new List<byte>();
            if (result == 0)
            {
                byte[] byt_m_data = new byte[OutData.Length / 2];
                for (int i = 0; i < (byt_m_data.Length - 4) / 4; i++)
                {
                    Array.Copy((stringToByte(OutData.Substring(0 + i * 8, 8))), 0, byt_m_data, 0 + i * 4, 4);
                }

                Array.Copy((stringToByte(OutData.Substring(OutData.Length - 8, 8))), 0, byt_m_data, byt_m_data.Length - 4, 4);
                if (MeterProtocols[pos].WriteRatesPrice(str_ID, byt_m_data))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region 备用套电价参数
        /// <summary>
        /// 备用套电价参数
        /// </summary>
        /// <param name="Flag">表示电表密钥状态，整型，0: 测试密钥状态；1: 正式密钥状态</param>
        /// <param name="PutRand">表示输入的随机数,字符型,4字节，电表身份认证成功后返回</param>
        /// <param name="PutApdu">写电表安全模块命令头，字符型，5字节</param>
        /// <param name="PutData">表示输入的当前套电价参数明文,字符型</param>
        /// <returns></returns>
        public bool[] SouthPrice2Update(int[] Flag,string[] PutApdu, string[] PutRand, string[] str_ID,string[] PutData)
        {

            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate (int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                arrRet[pos] = SouthPrice2Update(Flag[pos],PutApdu[pos], PutRand[pos],str_ID[pos], PutData[pos], pos);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            return arrRet;
        }

        public bool SouthPrice2Update(int Flag,string PutApdu, string PutRand,string str_ID, string PutData, int pos)
        {
            if (MeterProtocols[pos] == null) return false;
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return true;

            //表示输入的分散因子,字符型,8字节，“0000”+表号</param>
            string PutDiv = "0000" + Helper.MeterDataHelper.Instance.Meter(pos)._Mb_MeterNo;

            //定义输出的数据和MAC
            string OutData = "";
            string message = "";
            int result = EncryptionTool.SouthPrice2Update(Flag, PutRand, PutDiv, PutApdu, PutData, out OutData, out message);

            List<byte> frameData = new List<byte>();
            if (result == 0)
            {
                byte[] byt_m_data = new byte[OutData.Length / 2];
                if (str_ID == "04000109")
                {
                    Array.Copy((stringToByte(OutData.Substring(0, 10))), 0, byt_m_data, 0, 5);
                    Array.Copy((stringToByte(OutData.Substring(10, 8))), 0, byt_m_data, 5, 4);
                }
                else if ((str_ID == "04060AFF" ||str_ID == "04060BFF") && OutData.Length == 148)
                {
                    for (int i = 0; i < 6; i++) //阶梯值1-6  24字节
                    {
                        Array.Copy((stringToByte(OutData.Substring(0 + i * 8, 8))), 0, byt_m_data, 0 + i * 4, 4);
                    }
                    for (int i = 6; i < 13; i++) // 阶梯电价1-7 28字节
                    {
                        Array.Copy((stringToByte(OutData.Substring(0 + i * 8, 8))), 0, byt_m_data, 0 + i * 4, 4);
                    }
                    for (int i = 0; i < 7; i++)  //结算日1-6  18字节
                    {
                        Array.Copy((stringToByte(OutData.Substring(0 + i * 6 + 104, 6))), 0, byt_m_data, 0 + i * 3 + 52, 3);
                    }

                }
                else
                {
                    for (int i = 0; i < (byt_m_data.Length - 4) / 4; i++)
                    {
                        Array.Copy((stringToByte(OutData.Substring(0 + i * 8, 8))), 0, byt_m_data, 0 + i * 4, 4);
                    }
                }
                Array.Copy((stringToByte(OutData.Substring(OutData.Length - 8, 8))), 0, byt_m_data, byt_m_data.Length - 4, 4);
                if (MeterProtocols[pos].WriteRatesPrice(str_ID, byt_m_data))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region 二类参数更新
        /// <summary>
        /// 二类参数更新
        /// </summary>
        /// <param name="Flag">表示电表密钥状态，整型，0: 测试密钥状态；1: 正式密钥状态</param>
        /// <param name="PutRand">表示输入的随机数,字符型,4字节，电表身份认证成功后返回</param>
        /// <param name="PutData">表示输入的二类参数明文,字符型</param>         
        /// <param name="Datacode">数据标识</param>
        /// <returns></returns>
        public bool[] SouthParameterElseUpdate(int[] Flag, string[] PutRand, string[] PutData, string[] Datacode)
        {

            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate (int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                arrRet[pos] = SouthParameterElseUpdate(pos,Flag[pos], PutRand[pos], PutData[pos], Datacode[pos]);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            return arrRet;
        }

        public bool SouthParameterElseUpdate( int pos,int Flag, string PutRand, string PutData, string Datacode)
        {
            if (MeterProtocols[pos] == null) return false;
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return true;

            //表示输入的分散因子,字符型,8字节，“0000”+表号</param>
            string PutDiv ="0000"+ Helper.MeterDataHelper.Instance.Meter(pos)._Mb_MeterNo;

            //定义输出的数据和MAC
            string OutData = "";
            string message = "";
            string PutApdu = "";
            List<byte> frameData = new List<byte>();
            byte[] code = stringToByte(Datacode);
            
            byte LC = 0x14;
            int intByteLength = PutData.Length / 2;
            if ((intByteLength + 3) % 16 == 0)
            {
                LC = Convert.ToByte(((intByteLength + 3) / 16) * 16 + 4);
            }
            else
            {
                int a =(intByteLength + 3) / 16;
                LC = Convert.ToByte(((intByteLength + 3) / 16 + 1) * 16 + 4);
            }
            byte p1 = 0x88;
            int ModData = code[2] % 5;
            switch (ModData)
            {
                case 1:
                    p1 = 0x89;
                    break;
                case 2:
                    p1 = 0x90;
                    break;
                case 3:
                    p1 = 0x91;
                    break;
                case 4:
                    p1 = 0x92;
                    break;
                default:
                    break;
            }

            PutApdu = "04D6" + p1.ToString("X") + "00" + LC.ToString("X");
            int result = EncryptionTool.SouthParameterElseUpdate(Flag, PutRand, PutDiv, PutApdu, PutData, out OutData, out message);


            if (result == 0)
            {
                byte[] byt_m_data = new byte[OutData.Length / 2];
                Array.Copy((stringToByte(OutData.Substring(0, OutData.Length - 8))), 0, byt_m_data, 0, byt_m_data.Length - 4);
                Array.Copy((stringToByte(OutData.Substring(OutData.Length - 8, 8))), 0, byt_m_data, byt_m_data.Length - 4, 4);
                return MeterProtocols[pos].WriteData(Datacode, byt_m_data);
            }
            return false;
        }

        /// <summary>
        /// 二类参数更新 传错误的Mac 用于mac攻击
        /// </summary>
        /// <param name="Flag">表示电表密钥状态，整型，0: 测试密钥状态；1: 正式密钥状态</param>
        /// <param name="PutRand">表示输入的随机数,字符型,4字节，电表身份认证成功后返回</param>
        ///// <param name="PutDiv">表示输入的分散因子,字符型,8字节，“0000”+表号</param>        
        /// <param name="PutData">表示输入的二类参数明文,字符型</param>         
        /// <param name="Datacode">数据标识</param>
        /// <returns></returns>
        public bool[] SouthParameterElseUpdateByMac(int[] Flag, string[] PutRand, string[] PutData, string[] Datacode)
        {

            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate (int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                arrRet[pos] = SouthParameterElseUpdateByMac(pos,Flag[pos], PutRand[pos], PutData[pos], Datacode[pos]);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            return arrRet;
        }
        

        public bool SouthParameterElseUpdateByMac(int pos,int Flag, string PutRand, string PutData, string Datacode)
        {
            if (MeterProtocols[pos] == null) return false;
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return true;

            //表示输入的分散因子,字符型,8字节，“0000”+表号</param>
            string PutDiv = "0000" + Helper.MeterDataHelper.Instance.Meter(pos)._Mb_MeterNo;

            //定义输出的数据和MAC
            string OutData = "";
            string message = "";
            string PutApdu = "";
            List<byte> frameData = new List<byte>();
            byte[] code = stringToByte(Datacode);
            
            byte LC = 0x14;
            int intByteLength = PutData.Length / 2;
            if ((intByteLength + 3) % 16 == 0)
            {
                LC = Convert.ToByte(((intByteLength + 3) / 16) * 16 + 4);
            }
            else
            {
                LC = Convert.ToByte(((intByteLength + 3) / 16 + 1) * 16 + 4);
            }
            byte p1 = 0x88;
            int ModData = code[2] % 5;
            switch (ModData)
            {
                case 1:
                    p1 = 0x89;
                    break;
                case 2:
                    p1 = 0x90;
                    break;
                case 3:
                    p1 = 0x91;
                    break;
                case 4:
                    p1 = 0x92;
                    break;
                default:
                    break;
            }

            PutApdu = "04D6" + p1.ToString("X") + "00" + LC.ToString("X");
            int result = EncryptionTool.SouthParameterElseUpdate(Flag, PutRand, PutDiv, PutApdu, PutData, out OutData, out message);


            if (result == 0)
            {
                byte[] bMacTmp = new byte[4] { 0x00, 0x00, 0x00, 0x00 };
                byte[] byt_m_data = new byte[OutData.Length / 2];
                Array.Copy((stringToByte(OutData.Substring(0, OutData.Length - 8))), 0, byt_m_data, 0, byt_m_data.Length - 4);
                Array.Copy(bMacTmp, 0, byt_m_data, byt_m_data.Length - 4, 4);
                return MeterProtocols[pos].WriteDataByMac(Datacode, byt_m_data);
            }
            return false;
        }
        #endregion

        #region 远程钱包开户/充值
        /// <summary>
        /// 用于远程钱包开户/充值，仅正式密钥状态下可以做此操作
        /// </summary>
        /// <param name="OpertionType">操作类型，0：开户； 1：充值  </param>
        /// <param name="Flag">表示电表密钥状态，整型，0: 测试密钥状态；1: 正式密钥状态</param>
        /// <param name="PutRand">表示输入的随机数,字符型,4字节，电表身份认证成功后返回</param>
        /// <param name="PutData">表示输入的参数明文,包含：购电金额+购电次数+客户编号，共9字节，金额、次数均为HEX码</param>
        /// <returns></returns>
        public bool[] SouthIncreasePurse(int OpertionType, int[] Flag, string[] PutRand, string[] PutData, out string[] strRevDataF)
        {
            string[] recData = new string[bwCount];
            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            strRevDataF = recData;
            MulitThreadManager.Instance.DoWork = delegate (int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                if (Flag[pos] == 1)
                {
                    arrRet[pos] = SouthIncreasePurse(OpertionType, Flag[pos], PutRand[pos], PutData[pos], pos, out recData[pos]);
                }
                else
                {
                    arrRet[pos] = false;
                }
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            strRevDataF = recData;
            return arrRet;
        }

        public bool SouthIncreasePurse(int OpertionType, int Flag, string PutRand, string PutData, int pos, out string strRevDataF)
        {
            string strOutErr = "";
            strRevDataF = "";
            if (MeterProtocols[pos] == null) return false;
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return true;

            //表示输入的分散因子,字符型,8字节，“0000”+表号</param>
            string PutDiv = "0000" + Helper.MeterDataHelper.Instance.Meter(pos)._Mb_MeterNo;

            //购电金额+购电次数+MAC1+客户编号+MAC2
            string OutData = "";
            string message = "";
            int result = EncryptionTool.SouthIncreasePurse(Flag, PutRand, PutDiv, PutData, out OutData, out message);

            List<byte> frameData = new List<byte>();
            byte[] code = bytesReserve(new byte[] { 0x07, 0x01, 0x01, 0xFF });
            if (OpertionType == 1)
            {
                code = bytesReserve(new byte[] { 0x07, 0x01, 0x02, 0xFF });
            }
            byte[] recData = null;
            bool sequela = false;
            if (result == 0)
            {
                byte[] oper = new byte[4];
                byte[] byt_m_data = new byte[OutData.Length / 2];
                Array.Copy((stringToByte(OutData.Substring(0, 8))), 0, byt_m_data, 0, 4);
                Array.Copy((stringToByte(OutData.Substring(8, 8))), 0, byt_m_data, 4, 4);
                Array.Copy((stringToByte(OutData.Substring(16, 8))), 0, byt_m_data, 8, 4);
                Array.Copy((stringToByte(OutData.Substring(24, 12))), 0, byt_m_data, 12, 6);
                Array.Copy((stringToByte(OutData.Substring(36, 8))), 0, byt_m_data, 18, 4);
                frameData.AddRange(code);
                frameData.AddRange(oper);
                frameData.AddRange(byt_m_data);
                if (MeterProtocols[pos].UpdateRemoteEncryptionCommand(0x03, frameData.ToArray(), ref sequela, ref recData))
                {
                    return true;
                }
                else
                {
                    strOutErr = BitConverter.ToString(recData).Replace("-", "");
                    strOutErr = DxString(strOutErr);
                }
            }
            strRevDataF = strOutErr;
            return false;
        }

        #endregion

        #region 钱包初始化
        /// <summary>
        /// 所有表位的钱包初始化
        /// </summary>
        /// <param name="Flag">表示电表密钥状态，整型，0: 测试密钥状态；</param>
        /// <param name="rand2">表示输入的随机数,字符型,4字节，电表身份认证成功后返回；</param>
        /// <param name="strData">表示输入的数据明文，包含预置金额，4字节，HEX码；</param>
        /// <returns></returns>
        public bool[] SouthInitPurse(int[] Flag, string[] rand2, string[] strData)
        {
            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate (int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                if (!GlobalUnit.IsDemo)
                {
                    arrRet[pos] = SouthInitPurse(pos, Flag[pos], rand2[pos], strData[pos]);
                }
                else
                {
                    arrRet[pos] = true;
                }
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();

            return arrRet;
        }
        /// <summary>
        /// 单个表位的钱包初始化
        /// </summary>
        /// <param name="pos">表位号</param>
        /// <param name="Flag">表示电表密钥状态，整型，0: 测试密钥状态；</param>
        /// <param name="Rand2">表示输入的随机数,字符型,4字节，电表身份认证成功后返回；</param>
        /// <param name="strData">表示输入的数据明文，包含预置金额，4字节，HEX码；</param>
        /// <returns>操作结果</returns>
        public bool SouthInitPurse(int pos, int Flag, string Rand2, string strData)
        {
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return true;
            string str_endata = "";
            string str_message = "";
            string str_Div = "0000" + Helper.MeterDataHelper.Instance.Meter(pos)._Mb_MeterNo;
            int rst = EncryptionTool.SouthInitPurse(Flag, Rand2, str_Div, strData, out str_endata, out str_message);
            if (rst == 0)
            {
                //str_endata==预置金额+MAC1+“00000000”+MAC2
                byte[] cmd = bytesReserve(new byte[] { 0x07, 0x01, 0x03, 0xFF });
                byte[] oper = new byte[4];
                byte[] byt_m_data = new byte[str_endata.Length / 2];
                Array.Copy((stringToByte(str_endata.Substring(0, 8))), 0, byt_m_data, 0, 4);
                Array.Copy((stringToByte(str_endata.Substring(8, 8))), 0, byt_m_data, 4, 4);
                Array.Copy((stringToByte(str_endata.Substring(16, 8))), 0, byt_m_data, 8, 4);
                Array.Copy((stringToByte(str_endata.Substring(24, 8))), 0, byt_m_data, 12, 4);
                List<byte> frameData = new List<byte>();
                frameData.AddRange(cmd);
                frameData.AddRange(oper);
                frameData.AddRange(byt_m_data);
                bool seqela = false;
                byte[] revdata = null;
                if (MeterProtocols[pos].UpdateRemoteEncryptionCommand(0x03, frameData.ToArray(), ref seqela, ref revdata))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region 钱包初始化下发购电次数
        /// <summary>
        /// 所有表位的钱包初始化
        /// </summary>
        /// <param name="Flag">表示电表密钥状态，整型，0: 测试密钥状态；</param>
        /// <param name="rand2">表示输入的随机数,字符型,4字节，电表身份认证成功后返回；</param>
        /// <param name="strData">表示输入的数据明文，包含预置金额，4字节，HEX码；</param>
        /// <param name="strGdCount">表示购电次数</param>
        /// <returns></returns>
        public bool[] SouthInitPurse(int[] Flag, string[] rand2, string[] strData, string[] strGdCount)
        {
            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate (int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                if (!GlobalUnit.IsDemo)
                {
                    arrRet[pos] = SouthInitPurse(pos, Flag[pos], rand2[pos], strData[pos], strGdCount[pos]);
                }
                else
                {
                    arrRet[pos] = true;
                }
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();

            return arrRet;
        }
        /// <summary>
        /// 单个表位的钱包初始化
        /// </summary>
        /// <param name="pos">表位号</param>
        /// <param name="Flag">表示电表密钥状态，整型，0: 测试密钥状态；</param>
        /// <param name="Rand2">表示输入的随机数,字符型,4字节，电表身份认证成功后返回；</param>
        /// <param name="strData">表示输入的数据明文，包含预置金额，4字节，HEX码；</param>
        /// <param name="strGdCount">表示购电次数</param>
        /// <returns>操作结果</returns>
        public bool SouthInitPurse(int pos, int Flag, string Rand2, string strData, string strGdCount)
        {
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return true;
            string str_endata = "";
            string str_message = "";
            string str_Div = "0000" + Helper.MeterDataHelper.Instance.Meter(pos)._Mb_MeterNo;
            int rst = EncryptionTool.SouthInitPurse(Flag, Rand2, str_Div, strData, out str_endata, out str_message);
            if (rst == 0)
            {
                //str_endata==预置金额+MAC1+“00000000”+MAC2
                byte[] cmd = bytesReserve(new byte[] { 0x07, 0x01, 0x03, 0xFF });
                byte[] oper = new byte[4];
                byte[] byt_m_data = new byte[str_endata.Length / 2];
                Array.Copy((stringToByte(str_endata.Substring(0, 8))), 0, byt_m_data, 0, 4);
                Array.Copy((stringToByte(str_endata.Substring(8, 8))), 0, byt_m_data, 4, 4);
                Array.Copy((stringToByte(strGdCount)), 0, byt_m_data, 8, 4);
                Array.Copy((stringToByte(str_endata.Substring(24, 8))), 0, byt_m_data, 12, 4);
                List<byte> frameData = new List<byte>();
                frameData.AddRange(cmd);
                frameData.AddRange(oper);
                frameData.AddRange(byt_m_data);
                bool seqela = false;
                byte[] revdata = null;
                if (MeterProtocols[pos].UpdateRemoteEncryptionCommand(0x03, frameData.ToArray(), ref seqela, ref revdata))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region 密钥更新或恢复
        /// <summary>
        /// 所有表位密钥更新
        /// </summary>
        /// <param name="keyState">密钥恢复用00，密钥更新用01</param>
        /// <param name="keyCount">要更新的密钥条数</param>
        /// <param name="Flag">表示电表密钥状态，整型，0: 测试密钥状态；</param>
        /// <param name="Rand2">表示输入的随机数,字符型,4字节，电表身份认证成功后返回；</param>
        /// <param name="esamNo">8字节电表安全模块序列号；</param>
        /// <returns>操作结果</returns>
        public bool[] SouthKeyUpdateV2(string keyState, int keyCount,string[] rand2, string[] esamNo)
        {
            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate (int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                if (!GlobalUnit.IsDemo)
                {
                    arrRet[pos] = SouthKeyUpdateV2(pos, keyState, keyCount, rand2[pos], esamNo[pos]);
                }
                else
                {
                    arrRet[pos] = true;
                }
            };
            MulitThreadManager.Instance.Start(500);
            //等所有返回
            WaitWorkDone();

            return arrRet;
        }
        /// <summary>
        /// 单表位密钥更新
        /// </summary>
        /// <param name="pos">表位号</param>
        /// <param name="keyState">密钥恢复用00，密钥更新用01</param>
        /// <param name="keyCount">要更新的密钥条数</param>
        /// <param name="Flag">表示电表密钥状态，整型，0: 测试密钥状态；</param>
        /// <param name="Rand2">表示输入的随机数,字符型,4字节，电表身份认证成功后返回；</param>
        /// <param name="esamNo">8字节电表安全模块序列号；</param>
        /// <returns>操作结果</returns>
        public bool SouthKeyUpdateV2(int pos, string keyState, int keyCount,string rand2, string esamNo)
        {
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return true;
            string msg = "";
            string str_OutData = "";
            string str_Div = "0000" + Helper.MeterDataHelper.Instance.Meter(pos)._Mb_MeterNo;

            List<string> keyIdList = new List<string>();
            for (int i = 0; 4 * i < keyCount; i++)
            {
                string idTemp = "";
                for (int j = 0; j < 4; j++)
                {
                    if (4 * i + j < keyCount)
                    {
                        idTemp = idTemp + (4 * i + j).ToString("X2");
                    }
                }
                keyIdList.Add(idTemp);
            }
            string[] keyID = keyIdList.ToArray();

            byte[] oper = new byte[4];
            byte[] cmd3 = new byte[] { 0x07, 0x03, 0x01, 0xFF };
            byte[] cmd = bytesReserve(cmd3);
            int iResult = 0;

            bool bln_rst = true;
            for (int i = 0; i < keyID.Length; i++)
            {
                //用私钥认证
                int intReturn = EncryptionTool.SouthKeyUpdateV2(keyCount, keyState, keyID[i], rand2, str_Div, esamNo, out str_OutData, out msg);
                if (intReturn == 0)
                {
                    string str_Mac = (str_OutData.Substring(str_OutData.Length - 8, 8));//288
                    string str_m_tmp = str_OutData.Substring(0, str_OutData.Length - 8);
                    string str_Keyinfo1 = "";

                    for (int j = 0; j < keyID[i].Length / 2; j++)
                    {
                        string strm_t = str_m_tmp.Substring(j * 72, 72);
                        str_Keyinfo1 += DxString(strm_t.Substring(8, 64)) + DxString(strm_t.Substring(0, 8));
                    }
                    List<byte> frameData = new List<byte>();
                    frameData.AddRange(cmd);
                    frameData.AddRange(oper);
                    frameData.AddRange(stringToByte(DxString(str_Keyinfo1)));
                    frameData.AddRange(stringToByte(str_Mac));
                    //密钥清零 .或更新
                    Thread.Sleep(300);
                    bool seqela = false;
                    byte[] revdata = null;
                    if (MeterProtocols[pos].UpdateRemoteEncryptionCommand(0x03, frameData.ToArray(), ref seqela, ref revdata))
                    {
                    }
                    else
                    {
                        iResult++;
                    }
                }
                else
                {
                    //MessageBox.Show("第" + (pos + 1) + "表位调用秘钥更新函数出错,返回值：" + intReturn + ",异常消息" + msg + "。密钥状态：" + keyState
                    //    + ",密钥编号：" + keyID[i] + "" + ",随机数：" + rand2 + ",表号：" + str_Div + ",Esam序列号：" + esamNo + "。");
                    return false;
                }
            }
            if (iResult == 0)
            {
                return bln_rst;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 所有表位密钥更新 16条私钥 1条公钥
        /// </summary>
        /// <param name="keyState">密钥恢复用00，密钥更新用01</param>
        /// <param name="keyCount">要更新的密钥条数</param>
        /// <param name="Flag">表示电表密钥状态，整型，0: 测试密钥状态；</param>
        /// <param name="Rand2">表示输入的随机数,字符型,4字节，电表身份认证成功后返回；</param>
        /// <param name="esamNo">8字节电表安全模块序列号；</param>
        /// <returns>操作结果</returns>
        public bool[] SouthKeyUpdateOfficialAndTest(string keyState, int keyCount, string[] rand2, string[] esamNo)
        {
            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate(int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                if (!GlobalUnit.IsDemo)
                {
                    arrRet[pos] = SouthKeyUpdateOfficialAndTest(pos, keyState, keyCount, rand2[pos], esamNo[pos]);
                }
                else
                {
                    arrRet[pos] = true;
                }
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();

            return arrRet;
        }
        /// <summary>
        /// 单表位密钥更新
        /// </summary>
        /// <param name="pos">表位号</param>
        /// <param name="keyState">密钥恢复用00，密钥更新用01</param>
        /// <param name="keyCount">要更新的密钥条数</param>
        /// <param name="Flag">表示电表密钥状态，整型，0: 测试密钥状态；</param>
        /// <param name="Rand2">表示输入的随机数,字符型,4字节，电表身份认证成功后返回；</param>
        /// <param name="esamNo">8字节电表安全模块序列号；</param>
        /// <returns>操作结果</returns>
        public bool SouthKeyUpdateOfficialAndTest(int pos, string keyState, int keyCount, string rand2, string esamNo)
        {
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return true;
            string msg = "";
            string str_OutData = "";
            string str_Div = "0000" + Helper.MeterDataHelper.Instance.Meter(pos)._Mb_MeterNo;

            List<string> keyIdList = new List<string>();
            for (int i = 0; 4 * i < keyCount; i++)
            {
                string idTemp = "";
                for (int j = 0; j < 4; j++)
                {
                    if (4 * i + j < keyCount)
                    {
                        idTemp = idTemp + (4 * i + j).ToString("X2");
                    }
                }
                keyIdList.Add(idTemp);
            }
            string[] keyID = keyIdList.ToArray();

            byte[] oper = new byte[4];
            byte[] cmd3 = new byte[] { 0x07, 0x03, 0x01, 0xFF };
            byte[] cmd = bytesReserve(cmd3);

            bool bln_rst = true;
            for (int i = 0; i < keyID.Length; i++)
            {
                int intReturn = 0;
                if (i == 4)
                {
                    //用公钥认证
                   keyState = "00";
                   intReturn = EncryptionTool.SouthKeyUpdateV2(keyCount, keyState, keyID[i], rand2, str_Div, esamNo, out str_OutData, out msg);
                }
                else
                {
                    //用私钥认证
                   intReturn = EncryptionTool.SouthKeyUpdateV2(keyCount, keyState, keyID[i], rand2, str_Div, esamNo, out str_OutData, out msg);
                }
                if (intReturn == 0)
                {
                    string str_Mac = (str_OutData.Substring(str_OutData.Length - 8, 8));//288
                    string str_m_tmp = str_OutData.Substring(0, str_OutData.Length - 8);
                    string str_Keyinfo1 = "";

                    for (int j = 0; j < keyID[i].Length / 2; j++)
                    {
                        string strm_t = str_m_tmp.Substring(j * 72, 72);
                        str_Keyinfo1 += DxString(strm_t.Substring(8, 64)) + DxString(strm_t.Substring(0, 8));
                    }
                    List<byte> frameData = new List<byte>();
                    frameData.AddRange(cmd);
                    frameData.AddRange(oper);
                    frameData.AddRange(stringToByte(DxString(str_Keyinfo1)));
                    frameData.AddRange(stringToByte(str_Mac));
                    //密钥清零 .或更新
                    Thread.Sleep(100);
                    bool seqela = false;
                    byte[] revdata = null;
                    if (MeterProtocols[pos].UpdateRemoteEncryptionCommand(0x03, frameData.ToArray(), ref seqela, ref revdata))
                    {
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return bln_rst;
        }

        /// <summary>
        /// 所有表位密钥更新 编号非法
        /// </summary>
        /// <param name="keyState">密钥恢复用00，密钥更新用01</param>
        /// <param name="keyCount">要更新的密钥条数</param>
        /// <param name="Flag">表示电表密钥状态，整型，0: 测试密钥状态；</param>
        /// <param name="Rand2">表示输入的随机数,字符型,4字节，电表身份认证成功后返回；</param>
        /// <param name="esamNo">8字节电表安全模块序列号；</param>
        /// <returns>操作结果</returns>
        public bool[] SouthKeyUpdateV2DisorderedKey(string keyState, int keyCount, string[] rand2, string[] esamNo)
        {
            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate(int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                if (!GlobalUnit.IsDemo)
                {
                    arrRet[pos] = SouthKeyUpdateV2DisorderedKey(pos, keyState, keyCount, rand2[pos], esamNo[pos]);
                }
                else
                {
                    arrRet[pos] = true;
                }
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();

            return arrRet;
        }
        /// <summary>
        /// 单表位密钥更新
        /// </summary>
        /// <param name="pos">表位号</param>
        /// <param name="keyState">密钥恢复用00，密钥更新用01</param>
        /// <param name="keyCount">要更新的密钥条数</param>
        /// <param name="Flag">表示电表密钥状态，整型，0: 测试密钥状态；</param>
        /// <param name="Rand2">表示输入的随机数,字符型,4字节，电表身份认证成功后返回；</param>
        /// <param name="esamNo">8字节电表安全模块序列号；</param>
        /// <returns>操作结果</returns>
        public bool SouthKeyUpdateV2DisorderedKey(int pos, string keyState, int keyCount, string rand2, string esamNo)
        {
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return true;
            string msg = "";
            string str_OutData = "";
            string str_Div = "0000" + Helper.MeterDataHelper.Instance.Meter(pos)._Mb_MeterNo;

            List<string> keyIdList = new List<string>();
            for (int i = 0; 4 * i < keyCount; i++)
            {
                string idTemp = "";
                for (int j = 0; j < 4; j++)
                {
                    if (4 * i + j < keyCount)
                    {
                        idTemp = idTemp + (4 * i + j).ToString("X2");
                    }
                }
                keyIdList.Add(idTemp);
            }
            string[] keyID = keyIdList.ToArray();

            byte[] oper = new byte[4];
            byte[] cmd3 = new byte[] { 0x07, 0x03, 0x01, 0xFF };
            byte[] cmd = bytesReserve(cmd3);

            bool bln_rst = true;
            for (int i = 0; i < keyID.Length; i++)
            {
                //用私钥认证
                int intReturn = EncryptionTool.SouthKeyUpdateV2(keyCount, keyState, keyID[i], rand2, str_Div, esamNo, out str_OutData, out msg);
                if (intReturn == 0)
                {
                    string str_Mac = (str_OutData.Substring(str_OutData.Length - 8, 8));//288
                    string str_m_tmp = str_OutData.Substring(0, str_OutData.Length - 8);
                    string str_Keyinfo1 = "";

                    for (int j = 0; j < keyID[i].Length / 2; j++)
                    {
                        string strm_t = str_m_tmp.Substring(j * 72, 72);
                        //返回的4个字节数据 将后两个字节和前两个字节对调
                        str_Keyinfo1 += DxString(strm_t.Substring(12, 4)) + DxString(strm_t.Substring(8, 4)) + DxString(strm_t.Substring(16, 64)) + DxString(strm_t.Substring(0, 8));
                    }
                    List<byte> frameData = new List<byte>();
                    frameData.AddRange(cmd);
                    frameData.AddRange(oper);
                    frameData.AddRange(stringToByte(DxString(str_Keyinfo1)));
                    frameData.AddRange(stringToByte(str_Mac));
                    //密钥清零 .或更新
                    Thread.Sleep(100);
                    bool seqela = false;
                    byte[] revdata = null;
                    if (MeterProtocols[pos].UpdateRemoteEncryptionCommand(0x03, frameData.ToArray(), ref seqela, ref revdata))
                    {
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return bln_rst;
        }

        /// <summary>
        /// 按字节反转
        /// </summary>
        /// <param name="str_Keyinfo1"></param>
        /// <returns></returns>
        private static string DxString(string str_Keyinfo1)
        {
            int Len = str_Keyinfo1.Length / 2;
            string DxStr = "";
            for (int i = 0; i < Len; i++)
            {
                DxStr = str_Keyinfo1.Substring(i * 2, 2) + DxStr;
            }
            return DxStr;
        }
        #endregion

        #region 清零
        /// <summary>
        /// 所有表位的电表清零
        /// </summary>
        /// <param name="Flag">表示电表密钥状态，整型，0: 测试密钥状态；</param>
        /// <param name="rand2">表示输入的随机数,字符型,4字节，电表身份认证成功后返回；</param>
        /// <returns></returns>
        public bool[] SouthDataClear1(int[] Flag, string[] Rand2)
        {
            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate (int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                if (!GlobalUnit.IsDemo)
                {
                    arrRet[pos] = SouthDataClear1(pos, Flag[pos], Rand2[pos]);
                }
                else
                {
                    arrRet[pos] = true;
                }
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();

            return arrRet;
        }
        /// <summary>
        /// 单个表位的电能表清零
        /// </summary>
        /// <param name="pos">表位号</param>
        /// <param name="Flag">表示电表密钥状态，整型，0: 测试密钥状态；1: 正式密钥状态；</param>
        /// <param name="Rand2">表示输入的随机数,字符型,4字节，电表身份认证成功后返回；</param>
        /// <returns>操作结果</returns>
        public bool SouthDataClear1(int pos, int Flag, string Rand2)
        {
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return true;
            //清零数据明文，8字节；
            //R1~R8:R1=1AH,R2保留(默认为00H)
            //R3~R8代表命令有效截止时间(格式YYMMDDhhmmss)
            string strData = "1A00" + DateTime.Now.AddSeconds(30).ToString("yyMMddHHmmss");
            string str_endata = "";
            string str_message = "";
            string str_Div = "0000" + Helper.MeterDataHelper.Instance.Meter(pos)._Mb_MeterNo;
            int rst = EncryptionTool.SouthDataClear1(Flag, Rand2, str_Div, strData, out str_endata, out str_message);
            if (rst == 0)
            {
                string strEndata = "";

                byte[] byt_m_data = new byte[str_endata.Length / 2];
                Array.Copy((stringToByte(str_endata.Substring(0, str_endata.Length))), 0, byt_m_data, 0, byt_m_data.Length);

                strEndata = BitConverter.ToString(byt_m_data).Replace("-", "");
                return MeterProtocols[pos].ClearEnergy(strEndata);
            }
            return false;
        }
        #endregion

        #region 红外认证查询
        /// <summary>
        /// 所有表位的红外认证查询
        /// </summary>
        /// <param name="Flag">表示电表密钥状态，整型，0: 测试密钥状态；</param>
        /// <param name="OutesamNo">电能表esam号</param>
        /// <param name="enRand1">电能表传出的随机数1密文</param>
        /// <param name="Outrand2">电能表传出的随机数</param>
        /// <returns></returns>
        public bool[] SouthInfraredRand(out string[] OutesamNo, out string[] Rand1, out string[] enRand1, out string[] Outrand2)
        {
            Rand1 = new string[bwCount];
            OutesamNo = new string[bwCount];
            enRand1 = new string[bwCount];
            Outrand2 = new string[bwCount];
            //out参数不能传入lamda表达式,定义一些参数进行中转
            string[] arrOutesamNo = new string[bwCount];
            string[] arrRand1 = new string[bwCount];
            string[] arrenRand1 = new string[bwCount];
            string[] arrOutrand2 = new string[bwCount];

            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate (int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                if (!GlobalUnit.IsDemo)
                {
                    arrRet[pos] = SouthInfraredRand(pos, out arrOutesamNo[pos],out arrRand1[pos], out arrenRand1[pos], out arrOutrand2[pos]);
                }
                else
                {
                    arrRet[pos] = true;
                }
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            Rand1 = arrRand1;
            OutesamNo = arrOutesamNo;
            enRand1 = arrenRand1;
            Outrand2 = arrOutrand2;
            return arrRet;
        }
        /// <summary>
        /// 单表位红外认证查询
        /// </summary>
        /// <param name="pos">表位号</param>
        /// <param name="OutesamNo">电能表esam号</param>
        /// <param name="enRand1">电能表传出的随机数1密文</param>
        /// <param name="Outrand2">电能表传出的随机数</param>
        /// <returns></returns>
        public bool SouthInfraredRand(int pos, out string OutesamNo, out string Rand1, out string enRand1, out string Outrand2)
        {
            Rand1 = "";
            enRand1 = "";
            Outrand2 = "";
            OutesamNo = "";
            string Rand1Tmp = "";
            string enRand1Tmp = "";
            string Outrand2Tmp = "";
            string OutesamNoTmp = "";
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return true;

            string strOutEn="";
            string rand1 = "";
            string message = "";
            if (EncryptionTool.SouthInfraredRand(out Rand1Tmp, out message) == 0)
            {
                byte[] cmd = bytesReserve(new byte[] { 0x07, 0x80, 0x03, 0xFF });
                byte[] oper = new byte[4];
                byte[] rCode = stringToByte(Rand1Tmp); 
                List<byte> frameData = new List<byte>();
                frameData.AddRange(cmd);
                frameData.AddRange(oper);
                frameData.AddRange(rCode);
                bool seqela = false;
                byte[] revdata = null;
                if (MeterProtocols[pos].UpdateRemoteEncryptionCommand(0x03, frameData.ToArray(), ref seqela, ref revdata))
                {
                    strOutEn = BitConverter.ToString(bytesReserve(revdata)).Replace("-", "");
                    if (strOutEn.Length >= 68)
                    {
                        OutesamNoTmp = strOutEn.Substring(32, 16);
                        enRand1Tmp = strOutEn.Substring(16, 16);
                        Outrand2Tmp = strOutEn.Substring(0, 16);

                        OutesamNo = OutesamNoTmp;
                        Rand1 = Rand1Tmp;
                        enRand1 = enRand1Tmp;
                        Outrand2 = Outrand2Tmp;
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion

        #region 红外认证
        public bool[] SouthInfraredAuth(int[] Flag, string[] esamNo, string[] rand1, string[] enRand1,string[] rand2,out string[] enRand2 )
        {
            bool[] arrRet = new bool[bwCount];
            string[] enRand2Tmp = new string[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate (int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                if (!GlobalUnit.IsDemo)
                {
                    arrRet[pos] = SouthInfraredAuth(pos, Flag[pos], esamNo[pos], rand1[pos], enRand1[pos], rand2[pos], out enRand2Tmp[pos]);
                }
                else
                {
                    arrRet[pos] = true;
                }
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            enRand2 = enRand2Tmp;
            return arrRet;
        }
        /// <summary>
        /// 单表位红外认证
        /// </summary>
        /// <param name="pos">表位号</param>
        /// <param name="OutesamNo">电能表返回esam号</param>
        /// <param name="rand1">加密机传出的随机数1</param>
        /// <param name="enRand1">电能表返回的随机数1密文</param>
        /// <param name="rand2">电能表传出的随机数2</param>
        /// <param name="enRand2">加密机传出的随机数2密文</param>
        /// <returns></returns>
        public bool SouthInfraredAuth(int pos, int Flag, string esamNo, string rand1, string enRand1,string rand2,out string enRand2 )
        {

            enRand2 = "";
            string enRand2Tmp = "";
            string strOutEn = "";

            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return true;
            
            string str_Div = "0000" + Helper.MeterDataHelper.Instance.Meter(pos)._Mb_MeterNo;
            string message = "";

            if (EncryptionTool.SouthInfraredAuth(Flag, str_Div, esamNo, rand1, enRand1, rand2, out enRand2Tmp, out message) == 0)
            {
               byte[] cmd = bytesReserve(new byte[] { 0x07, 0x00, 0x03, 0xFF });   //红外认证指令
               byte[] oper = new byte[4];
               byte[] rCode = stringToByte(enRand2Tmp);
                List<byte> frameData = new List<byte>();
                frameData.AddRange(cmd);
                frameData.AddRange(oper);
                frameData.AddRange(rCode);
                bool seqela = false;
                byte[] revdata = null;
                if (MeterProtocols[pos].UpdateRemoteEncryptionCommand(0x03, frameData.ToArray(), ref seqela, ref revdata))
                {
                    enRand2 = enRand2Tmp;
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region 不带数据域03指令（例如：身份认证失效）
        /// <summary>
        /// 不带数据域03指令（例如：身份认证失效）
        /// </summary>
        /// <param name="Datacode">数据标识</param>
        /// <returns></returns>
        public bool[] SouthCmdNoData(string Datacode)
        {

            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate (int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                arrRet[pos] = SouthCmdNoData(pos, Datacode);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            return arrRet;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="Datacode"></param>
        /// <returns></returns>
        public bool SouthCmdNoData(int pos, string Datacode)
        {
            if (MeterProtocols[pos] == null) return false;
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return true;

            List<byte> frameData = new List<byte>();
            byte[] code = stringToByte(Datacode);
            byte[] recData = null;
            bool sequela = false;

            {
                byte[] oper = new byte[4];
                frameData.AddRange(code);
                frameData.AddRange(oper);
                if (MeterProtocols[pos].UpdateRemoteEncryptionCommand(0x03, frameData.ToArray(), ref sequela, ref recData))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region 带数据域03指令读
        /// <summary>
        /// 带数据域03指令
        /// </summary>
        /// <param name="Datacode">数据标识</param>
        /// <returns></returns>
        public bool[] SouthCmdData(string Datacode, string[] strData, out string[] RevStrData)
        {
            string[] recData = new string[bwCount];
            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            RevStrData = new string[bwCount];
            MulitThreadManager.Instance.DoWork = delegate (int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                 arrRet[pos] = SouthCmdData(pos, Datacode, strData[pos], out recData[pos]);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            RevStrData = recData;
            return arrRet;
        }
        /// <summary>
        /// 单表位带数据域03指令
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="Datacode"></param>
        /// <returns></returns>
        public bool SouthCmdData(int pos, string Datacode, string strData, out string RevStrData)
        {
            
            RevStrData = "";
            if (MeterProtocols[pos] == null) return false;
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return true;

            List<byte> frameData = new List<byte>();
            byte[] code = stringToByte(Datacode);
            byte[] data = stringToByte(DxString(strData));
            byte[] recData = null;
            bool sequela = false;

            {
                byte[] oper = new byte[4];
                frameData.AddRange(code);
                frameData.AddRange(oper);
                frameData.AddRange(data);
                if (MeterProtocols[pos].UpdateRemoteEncryptionCommandByTerminal(0x03, frameData.ToArray(), ref sequela, ref recData))
                {
                    if (Datacode == "07A002FF" || Datacode == "07A003FF" || Datacode == "07A004FF")
                    {
                        try
                        {
                            RevStrData = BitConverter.ToString(recData).Replace("-", "");
                            
                            if (RevStrData.Length >= 118)
                            {
                                //返回格式= 数据标识+返写文件状态+返写信息文件+ MAC
                                //返回出去= 返写信息文件+ MAC
                                RevStrData = DxString(RevStrData.Substring(10, 100))  + DxString(RevStrData.Substring(RevStrData.Length - 8, 8));
                            }
                            return true;
                        }
                        catch (Exception)
                        {

                            return false;
                        }
                    }
                    else
                    {
                        RevStrData = BitConverter.ToString(recData).Replace("-", "");
                        return true;
                    }
                }
                else
                {
                    if (Datacode == "07A002FF")
                    {

                        RevStrData = DxString(BitConverter.ToString(recData).Replace("-", ""));
                        if (RevStrData.Length == 4)
                        {
                            RevStrData = RevStrData.Substring(RevStrData.Length - 4, 4);
                        }
                        else if (RevStrData.Length == 114)
                        {
                            //返回来的格式= 数据标识+返写文件状态+返写信息文件+ MAC
                            //返回出去= 返写信息文件+ MAC + AA +SERR
                            RevStrData = RevStrData.Substring(8, 100) + RevStrData.Substring(0, 8) + RevStrData.Substring(108, 6);
                        }
                        return false;
                    }
                }
            }
            return false;
        }

        #endregion

        #region 交互终端下发参数文件、当前套电价文件、备用套电价文件

        /// <summary>
        /// 交互终端下发参数文件
        /// </summary>
        /// <param name="CardNo"></param>
        /// <param name="strRand2"></param>
        /// <param name="CardType"></param>
        /// <param name="strParaInfo"></param>
        /// <param name="strRevData"></param>
        /// <returns></returns>
        public bool[] SouthTerminalSendParam(string[] CardNo, string CardType, string[] strParaInfo,out string[] strRevData)
        {
            bool[] arrRet = new bool[bwCount];
            string[] strRevDataTmp = new string[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate(int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                arrRet[pos] = SouthTerminalSendParam(pos, CardNo[pos], CardType, strParaInfo[pos], out strRevDataTmp[pos]);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            strRevData = strRevDataTmp;
            return arrRet;

        }

        /// <summary>
        /// 交互终端下发参数文件
        /// </summary>
        /// <param name="CardNo"></param>
        /// <param name="strRand2"></param>
        /// <param name="CardType"></param>
        /// <param name="strParaInfo"></param>
        /// <param name="strRevData"></param>
        /// <returns></returns>
        public bool SouthTerminalSendParam(int pos, string CardNo, string CardType,string ParaInfo,out string strRevData)
        {
            string strDataTmp = "";
            if (string.IsNullOrEmpty(CardNo))
            {
                CardNo = "00".PadLeft(16, '0');
            }
            if (ParaInfo.Length < 146)
            {
                ParaInfo.PadLeft(146, '0');
            }
            string BuyMoney = ParaInfo.Substring(0,8);
            string BuyCount = ParaInfo.Substring(8, 8);
            string strMac1 = ParaInfo.Substring(16, 8);
            string strMac2 = ParaInfo.Substring(24, 8);
            string ParaFile = ParaInfo.Substring(32, 90);
            string strMac3 = ParaInfo.Substring(122, 8);
            string strMac4 = ParaInfo.Substring(130, 8);
            string strRand1 = ParaInfo.Substring(138, 8);

            string strData = DxString(CardNo) + DxString(strRand1) + DxString(CardType) + DxString(BuyMoney) + DxString(BuyCount) + DxString(strMac1) + DxString(strMac2) + DxString(ParaFile) + DxString(strMac3) + DxString(strMac4);
            bool Result = MeterProtocolAdapter.Instance.SouthCmdData(pos, "07A002FF", strData, out strDataTmp);
            strRevData = strDataTmp;
            return Result;
        }


        /// <summary>
        /// 交互终端下发当前套电价文件
        /// </summary>
        /// <param name="paraFile">当前套电价文件</param>
        /// <param name="Mac1"></param>
        /// <returns></returns>
        public bool[] SouthTerminalSendPrice1(string[] strPrice1, out string[] strRevData)
        {
            strRevData = new string[bwCount];
            string[] strRevDataTmp = new string[bwCount];
            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate (int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                arrRet[pos] = SouthTerminalSendPrice1(pos, strPrice1[pos], out strRevDataTmp[pos]);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            strRevData = strRevDataTmp;
            return arrRet;
        }

        /// <summary>
        /// 交互终端下发当前套电价文件
        /// </summary>
        /// <param name="pos">表位</param>
        /// <param name="paraFile">当前套电价文件</param>
        /// <param name="Mac1"></param>
        /// <returns></returns>
        public bool SouthTerminalSendPrice1(int pos, string strPrice1, out string strRevData)
        {
            strRevData = "";
            string RevDataTmp = null;
            if (strPrice1.Length >= 406)
            {
                string strData = DxString(strPrice1.Substring(0, strPrice1.Length - 8)) + DxString(strPrice1.Substring(strPrice1.Length - 8, 8));
                bool result = MeterProtocolAdapter.Instance.SouthCmdData(pos, "07A003FF", strData, out RevDataTmp);
                strRevData = RevDataTmp;
                return result;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 交互终端下发备用套电价文件
        /// </summary>
        /// <param name="paraFile">备用套电价文件</param>
        /// <param name="Mac1"></param>
        /// <returns></returns>
        public bool[] SouthTerminalSendPrice2(string[] strPrice2, out string[] strRevData)
        {
            strRevData = new string[bwCount];
            string[] strRevDataTmp = new string[bwCount];
            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate (int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                arrRet[pos] = SouthTerminalSendPrice2(pos, strPrice2[pos],out strRevDataTmp[pos]);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            strRevData = strRevDataTmp;
            return arrRet;
        }

        /// <summary>
        /// 交互终端下发备用套电价文件
        /// </summary>
        /// <param name="pos">表位</param>
        /// <param name="paraFile">备用套电价文件</param>
        /// <param name="Mac1"></param>
        /// <returns></returns>
        public bool SouthTerminalSendPrice2(int pos, string strPrice2, out string strRevData)
        {
            strRevData = "";
            string RevDataTmp = null;
            if (strPrice2.Length >= 406)
            {
                string strData = DxString(strPrice2.Substring(0, strPrice2.Length - 8)) + DxString(strPrice2.Substring(strPrice2.Length - 8, 8));
                bool result = MeterProtocolAdapter.Instance.SouthCmdData(pos, "07A004FF", strData, out RevDataTmp);
                strRevData = RevDataTmp;
                return result;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 交互终端下发合闸复电
        /// </summary>
        /// <param name="paraFile">合闸复电命令密文</param>
        /// <param name="Mac1"></param>
        /// <returns></returns>
        public bool[] SouthTerminalSendHzfd(string[] paraFile)
        {
            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate(int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                arrRet[pos] = SouthTerminalSendHzfd(pos, paraFile[pos]);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            return arrRet;
        }

        /// <summary>
        /// 交互终端下发合闸复电
        /// </summary>
        /// <param name="pos">表位</param>
        /// <param name="paraFile">合闸复电密文</param>
        /// <param name="Mac1"></param>
        /// <returns></returns>
        public bool SouthTerminalSendHzfd(int pos, string paraFile)
        {
            string bytRevData = null;
            if (paraFile.Length >= 40)
            {
                string strData = DxString(paraFile.Substring(0, 32)) + DxString(paraFile.Substring(32, 8));
                bool result = MeterProtocolAdapter.Instance.SouthCmdData(pos, "07A005FF", strData, out bytRevData);
                return result;
            }
            else
            {
                return false;
            }
        }


        #endregion

        #region 寻卡命令

        /// <summary>
        /// 寻卡命令 0=开启寻卡，1=结束寻卡
        /// </summary>
        /// <param name="StartOrEnd">0=开启寻卡，1=结束寻卡</param>
        /// <returns></returns>
        public bool[] SouthFindCard(int StartOrEnd)
        {
            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate(int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                if (!GlobalUnit.IsDemo)
                    arrRet[pos] = SouthFindCard(pos,StartOrEnd);
                else
                    arrRet[pos] = true;
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();

            return arrRet;
        }
        public bool SouthFindCard(int pos,int StartOrEnd)
        {
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return true;
            return FindCard(pos,StartOrEnd);
        }

        /// <summary>
        /// 寻卡
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="str_Endata">有效时间截止时间</param>
        /// <param name="StartOrEnd">0=开启电能表寻卡，1=结束电能表寻卡</param>
        /// <returns></returns>
        public bool FindCard(int pos, int StartOrEnd)
        {
            List<byte> frameData = new List<byte>();
            byte[] oper = new byte[4];
            string strDate = DateTime.Now.AddMinutes(50).ToString("yyMMddHHmmss");
            frameData.AddRange(oper);
            if (StartOrEnd == 1)
            {
                frameData.AddRange(stringToByte("1B"));
            }
            else
            {
                frameData.AddRange(stringToByte("1A"));
            }
            frameData.AddRange(stringToByte("00"));
            byte[] byt_Endate = stringToByte(strDate);
            frameData.AddRange(byt_Endate);
            bool seqela = false;
            byte[] revdata = null;

            if (MeterProtocols[pos].UpdateRemoteEncryptionCommand(0x09, frameData.ToArray(), ref seqela, ref revdata))
            {
                return true;
            }
            return false;
        }

        #endregion

        #region 读卡器
        #region Reset Card
        /// <summary>
        /// 复位卡
        /// </summary>
        /// <returns></returns>
        public bool[] SouthResetCard()
        {
            bool[] arrRet = new bool[bwCount];
            //if (GlobalUnit.g_Dev_CommunType == Cus_CommunType.南网通讯DLL)
            //{
            //    arrRet[0] = SouthResetCard(0);
            //}
            //else
            {
                runFlag = true;
                MulitThreadManager.Instance.DoWork = delegate(int pos)
                {
                    if (!runFlag) return;
                    if (!OpenCommSwitch(pos)) return;
                    if (!GlobalUnit.IsDemo)
                    {
                        arrRet[pos] = SouthResetCard(pos);
                    }
                    else
                    {
                        arrRet[pos] = true;
                    }
                };
                MulitThreadManager.Instance.Start();
                //等所有返回
                WaitWorkDone();
            }
            return arrRet;
        }
        public bool SouthResetCard(int pos)
        {
            //if (GlobalUnit.g_Dev_CommunType != Cus_CommunType.南网通讯DLL)
            //{
            //    if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return true;
            //}
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return false;
            if (CardCtrProtocols[pos].ResetDevice(pos+1) == 0)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region Read UserCardNum
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cardNum"></param>
        /// <returns></returns>
        public bool[] SouthReadUserCardNum(out string[] cardNum)
        {
            cardNum = new string[bwCount];
            string[] cardNumTmp = new string[bwCount];

            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate (int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                if (!GlobalUnit.IsDemo)
                {
                    arrRet[pos] = SouthReadUserCardNum(pos, out cardNumTmp[pos]);
                }
                else
                {
                    arrRet[pos] = true;
                }
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();

            cardNum = cardNumTmp;
            return arrRet;
        }
        public bool SouthReadUserCardNum(int pos, out string cardNum)
        {
            cardNum = "";
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return true;
            string Tmp = "";
            if (CardCtrProtocols[pos].ReadUserCardNum(out Tmp) == 0)
            {
                cardNum = Tmp;
                return true;
            }
            return false;
        }
        #endregion

        #region Read UserCard
        public bool[] SouthReadUserCard( out string[] fileParamData, out string[] fileMoneyData, out string[] filePrice1Data, out string[] filePrice2Data, out string[] fileReplyData, out string[] enfileControlData)
        {
            fileParamData = new string[bwCount];
            fileMoneyData = new string[bwCount];
            filePrice1Data = new string[bwCount];
            filePrice2Data = new string[bwCount];
            fileReplyData = new string[bwCount];
            enfileControlData = new string[bwCount];

            string[] fileParamDataTmp = new string[bwCount];
            string[] fileMoneyDataTmp = new string[bwCount];
            string[] filePrice1DataTmp = new string[bwCount];
            string[] filePrice2DataTmp = new string[bwCount];
            string[] fileReplyDataTmp = new string[bwCount];
            string[] enfileControlDataTmp = new string[bwCount];

            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate (int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                if (!GlobalUnit.IsDemo)
                {
                    arrRet[pos] = SouthReadUserCard(pos,out fileParamDataTmp[pos], out fileMoneyDataTmp[pos], out filePrice1DataTmp[pos], out filePrice2DataTmp[pos], out fileReplyDataTmp[pos], out enfileControlDataTmp[pos]);
                }
                else
                {
                    arrRet[pos] = true;
                }
            };
            MulitThreadManager.Instance.Start(500);
            //等所有返回
            WaitWorkDone();

            fileParamData = fileParamDataTmp;
            fileMoneyData = fileMoneyDataTmp;
            filePrice1Data = filePrice1DataTmp;
            filePrice2Data = filePrice2DataTmp;
            fileReplyData = fileReplyDataTmp;
            enfileControlData = enfileControlDataTmp;
            return arrRet;
        }
        public bool SouthReadUserCard(int pos, out string fileParamData, out string fileMoneyData, out string filePrice1Data, out string filePrice2Data, out string fileReplyData, out string enfileControlData)
        {
            fileParamData = "";
            fileMoneyData = "";
            filePrice1Data = "";
            filePrice2Data = "";
            fileReplyData = "";
            enfileControlData = "";
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return true;

            if (CardCtrProtocols[pos].ReadUserCard(out fileParamData, out fileMoneyData, out filePrice1Data, out filePrice2Data, out fileReplyData, out enfileControlData) == 0)
            {
                return true;
            }
            return false;
        }




        #endregion


        #region ReadUserCardMAC
        public bool[] SouthReadUserCardMAC(string[] strRand, out string[] fileParamData, out string[] fileMoneyData, out string[] filePrice1Data, out string[] filePrice2Data, out string[] enfileControlData)
        {
            fileParamData = new string[bwCount];
            fileMoneyData = new string[bwCount];
            filePrice1Data = new string[bwCount];
            filePrice2Data = new string[bwCount];
            enfileControlData = new string[bwCount];

            string[] fileParamDataTmp = new string[bwCount];
            string[] fileMoneyDataTmp = new string[bwCount];
            string[] filePrice1DataTmp = new string[bwCount];
            string[] filePrice2DataTmp = new string[bwCount];
            string[] enfileControlDataTmp = new string[bwCount];

            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate(int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                if (!GlobalUnit.IsDemo)
                {
                    arrRet[pos] = SouthReadUserCardMAC(pos,strRand[pos], out fileParamDataTmp[pos], out fileMoneyDataTmp[pos], out filePrice1DataTmp[pos], out filePrice2DataTmp[pos], out enfileControlDataTmp[pos]);
                }
                else
                {
                    arrRet[pos] = true;
                }
            };
            MulitThreadManager.Instance.Start(500);
            //等所有返回
            WaitWorkDone();

            fileParamData = fileParamDataTmp;
            fileMoneyData = fileMoneyDataTmp;
            filePrice1Data = filePrice1DataTmp;
            filePrice2Data = filePrice2DataTmp;
            enfileControlData = enfileControlDataTmp;
            return arrRet;
        }
        public bool SouthReadUserCardMAC(int pos,string strRand, out string fileParamData, out string fileMoneyData, out string filePrice1Data, out string filePrice2Data, out string enfileControlData)
        {
            fileParamData = "";
            fileMoneyData = "";
            filePrice1Data = "";
            filePrice2Data = "";
            enfileControlData = "";
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return true;

            if (CardCtrProtocols[pos].ReadUserCardMAC(strRand, out fileParamData, out fileMoneyData, out filePrice1Data, out filePrice2Data, out enfileControlData) == 0)
            {
                return true;
            }
            return false;
        }




        #endregion

        #region Write UserCard
        public bool[] SouthWriteUserCard(string[] fileParam, string[] fileMoney, string[] filePrice1, string[] filePrice2, string[] fileReply,string[] fileControl)
        {
            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate (int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                if (!GlobalUnit.IsDemo)
                {
                    arrRet[pos] = SouthWriteUserCard(pos, fileParam[pos], fileMoney[pos], filePrice1[pos], filePrice2[pos], fileReply[pos], fileControl[pos]);
                }
                else
                {
                    arrRet[pos] = true;
                }
            };
            MulitThreadManager.Instance.Start(1500);
            //等所有返回
            WaitWorkDone();

            return arrRet;
        }
        public bool SouthWriteUserCard(int pos, string fileParam, string fileMoney, string filePrice1, string filePrice2, string fileReply,string fileControl)
        {
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return true;

            if (CardCtrProtocols[pos].WriteUserCard(fileParam, fileMoney, filePrice1, filePrice2, fileReply, fileControl) == 0)
            {
                return true;
            }
            return false;
        }


        #endregion

        #region Read ParamCardNum
        public bool[] SouthReadParamPresetCardNum(out string[] cardNum)
        {
            cardNum = new string[bwCount];
            string[] cardNumTmp = new string[bwCount];

            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate (int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                if (!GlobalUnit.IsDemo)
                {
                    arrRet[pos] = SouthReadParamPresetCardNum(pos, out cardNumTmp[pos]);
                }
                else
                {
                    arrRet[pos] = true;
                }
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();

            cardNum = cardNumTmp;
            return arrRet;
        }
        public bool SouthReadParamPresetCardNum(int pos, out string cardNum)
        {
            cardNum = "";
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return true;

            if (CardCtrProtocols[pos].ReadParamPresetCardNum(out cardNum) == 0)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region Read ParamCard
        public bool[] SouthReadParamPresetCard(string[] fileParam, string[] fileMoney, string[] filePrice1, string[] filePrice2)
        {
            fileParam = new string[bwCount];
            fileMoney = new string[bwCount];
            filePrice1 = new string[bwCount];
            filePrice2 = new string[bwCount];
            string[] fileParamTmp = new string[bwCount];
            string[] fileMoneyTmp = new string[bwCount];
            string[] filePrice1Tmp = new string[bwCount];
            string[] filePrice2Tmp = new string[bwCount];

            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate (int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                if (!GlobalUnit.IsDemo)
                {
                    arrRet[pos] = SouthReadParamPresetCard(pos, out fileParamTmp[pos], out fileMoneyTmp[pos], out filePrice1Tmp[pos], out filePrice2Tmp[pos]);
                }
                else
                {
                    arrRet[pos] = true;
                }
            };
            MulitThreadManager.Instance.Start(1000);
            //等所有返回
            WaitWorkDone();

            fileParam = fileParamTmp;
            fileMoney = fileMoneyTmp;
            filePrice1 = filePrice1Tmp;
            filePrice2 = filePrice2Tmp;
            return arrRet;
        }
        public bool SouthReadParamPresetCard(int pos, out string fileParam, out string fileMoney, out string filePrice1, out string filePrice2)
        {
            fileParam = "";
            fileMoney = "";
            filePrice1 = "";
            filePrice2 = "";
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return true;

            if (CardCtrProtocols[pos].ReadParamPresetCard(out fileParam, out fileMoney, out filePrice1, out filePrice2) == 0)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region Write ParamCard
        public bool[] SouthWriteParamPresetCard(string[] fileParam, string[] fileMoney, string[] filePrice1, string[] filePrice2)
        {
            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate (int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                if (!GlobalUnit.IsDemo)
                {
                    arrRet[pos] = SouthWriteParamPresetCard(pos, fileParam[pos], fileMoney[pos], filePrice1[pos], filePrice2[pos]);
                }
                else
                {
                    arrRet[pos] = true;
                }
            };
            MulitThreadManager.Instance.Start(1000);
            //等所有返回
            WaitWorkDone();

            return arrRet;
        }
        public bool SouthWriteParamPresetCard(int pos, string fileParam, string fileMoney, string filePrice1, string filePrice2)
        {
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return true;

            if (CardCtrProtocols[pos].WriteParamPresetCard(fileParam, fileMoney, filePrice1, filePrice2) == 0)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region Write UserCardReply
        public bool[] SouthWriteUserCardReply(string[] fileReply)
        {
            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate(int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                if (!GlobalUnit.IsDemo)
                {
                    arrRet[pos] = SouthWriteUserCardReply(pos, fileReply[pos]);
                }
                else
                {
                    arrRet[pos] = true;
                }
            };
            MulitThreadManager.Instance.Start(1000);
            //等所有返回
            WaitWorkDone();

            return arrRet;
        }
        public bool SouthWriteUserCardReply(int pos,string fileReply)
        {
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return true;
            if (string.IsNullOrEmpty(fileReply)) return false;
            if (CardCtrProtocols[pos].WriteUserCardReply(fileReply) == 0)
            {
                return true;
            }
            return false;
        }


        #endregion

        #region Read TerminalToCardInfo
        public bool[] SouthReadTerminalToCardInfo(string[] strRand2,string[] strUserCardNo,out string[] ParaInfo)
        {
            ParaInfo = new string[bwCount];
            string[] ParaInfoTmp = new string[bwCount]; 
            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate(int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                if (!GlobalUnit.IsDemo)
                {
                    arrRet[pos] = SouthReadTerminalToCardInfo(pos, strRand2[pos], strUserCardNo[pos], out ParaInfoTmp[pos]);
                }
                else
                {
                    arrRet[pos] = true;
                }
            };
            MulitThreadManager.Instance.Start(500);
            //等所有返回
            WaitWorkDone();
            ParaInfo = ParaInfoTmp;
            return arrRet;
        }
        public bool SouthReadTerminalToCardInfo(int pos, string strRand2, string strUserCardNo, out string ParaInfo)
        {
            ParaInfo = "";
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return true;
            string ParaInfoTmp = "";
            if (CardCtrProtocols[pos].ReadTerminalToCardInfo(strRand2, strUserCardNo, out ParaInfoTmp) == 0)
            {
                ParaInfo = ParaInfoTmp;
                return true;
            }
            ParaInfo = ParaInfoTmp;
            return false;
        }


        #endregion

        #region Write UserCardReplyPrice
        public bool[] SouthUserCardReplyPrice2(string[] fileReplyPrice2)
        {
            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate(int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                if (!GlobalUnit.IsDemo)
                {
                    arrRet[pos] = SouthUserCardReplyPrice2(pos, fileReplyPrice2[pos]);
                }
                else
                {
                    arrRet[pos] = true;
                }
            };
            MulitThreadManager.Instance.Start(500);
            //等所有返回
            WaitWorkDone();

            return arrRet;
        }
        public bool SouthUserCardReplyPrice2(int pos, string fileReplyPrice2)
        {
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return true;
            if (string.IsNullOrEmpty(fileReplyPrice2)) return false;
            if (CardCtrProtocols[pos].WriteUserCardReplyPrice2(fileReplyPrice2) == 0)
            {
                return true;
            }
            return false;
        }


        #endregion

        #region Read TerminalToCardPrice1
        public bool[] SouthReadTerminalToCardPrice1(string[] strRand2, string[] strUserCardNo, out string[] Price1)
        {
            Price1 = new string[bwCount];
            string[] Price1Tmp = new string[bwCount];
            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate(int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                if (!GlobalUnit.IsDemo)
                {
                    arrRet[pos] = SouthReadTerminalToCardPrice1(pos, strRand2[pos], strUserCardNo[pos], out Price1Tmp[pos]);
                }
                else
                {
                    arrRet[pos] = true;
                }
            };
            MulitThreadManager.Instance.Start(500);
            //等所有返回
            WaitWorkDone();
            Price1 = Price1Tmp;
            return arrRet;
        }
        public bool SouthReadTerminalToCardPrice1(int pos, string strRand2, string strUserCardNo, out string Price1)
        {
            Price1 = "";
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return true;
            string Price1Tmp = "";
            if (CardCtrProtocols[pos].ReadTerminalToCardPrice1(strRand2, strUserCardNo, out Price1Tmp) == 0)
            {
                Price1 = Price1Tmp;
                return true;
            }
            Price1 = Price1Tmp;
            return false;
        }


        #endregion

        #region Read TerminalToCardPrice2
        public bool[] SouthReadTerminalToCardPrice2(string[] strRand2, string[] strUserCardNo, out string[] Price2)
        {
            Price2 = new string[bwCount];
            string[] Price2Tmp = new string[bwCount];
            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate(int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                if (!GlobalUnit.IsDemo)
                {
                    arrRet[pos] = SouthReadTerminalToCardPrice2(pos, strRand2[pos], strUserCardNo[pos], out Price2Tmp[pos]);
                }
                else
                {
                    arrRet[pos] = true;
                }
            };
            MulitThreadManager.Instance.Start(1000);
            //等所有返回
            WaitWorkDone();
            Price2 = Price2Tmp;
            return arrRet;
        }
        public bool SouthReadTerminalToCardPrice2(int pos, string strRand2, string strUserCardNo, out string Price2)
        {
            Price2 = "";
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return true;
            string Price2Tmp = "";
            if (CardCtrProtocols[pos].ReadTerminalToCardPrice2(strRand2, strUserCardNo, out Price2Tmp) == 0)
            {
                Price2 = Price2Tmp;
                return true;
            }
            Price2 = Price2Tmp;
            return false;
        }


        #endregion

        #endregion

        #region 清空需量+
        /// <summary>
        /// 清理需量
        /// </summary>
        /// <returns></returns>
        public bool[] ClearDemand()
        {
            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate(int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                if (!GlobalUnit.IsDemo)
                 arrRet[pos] = ClearDemand(pos);
               else
                    arrRet[pos] = true;
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            return arrRet;
        }
        /// <summary>
        /// 单表位清空需量
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool ClearDemand(int pos)
        {
            if (MeterProtocols[pos] == null) return false;
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return true;
            bool bln_Rst = false;
  
                return MeterProtocols[pos].ClearDemand();
            
       
        }
        #endregion

        #region 清空电量+
        public bool[] ClearEnergy()
        {
            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate(int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                if (!GlobalUnit.IsDemo)
                    arrRet[pos] = ClearEnergy(pos);
                else
                    arrRet[pos] = true;
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            return arrRet;
        }
        /// <summary>
        /// 单表位电表清零
        /// </summary>
        /// <param name="pos">表位索引号，从0开始</param>
        /// <returns></returns>
        public bool ClearEnergy(int pos)
        {
            if (MeterProtocols[pos] == null) return false;
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn)
            {
                return true;
            }
            bool bln_Rst = false;
            //取密文 EncryptionTool
            if (Helper.MeterDataHelper.Instance.Meter(pos).DgnProtocol.IsSouthEncryption) //南网费控
            {
                string rand1 = "";
                string rand2 = "";
                string esamNo = "";

                int Fag = SouthCheckIdentity(pos, out rand1, out rand2, out esamNo); // 检查密钥状态
                if (Fag <= 1)
                {
                    string str_div = Helper.MeterDataHelper.Instance.Meter(pos)._Mb_MeterNo;

                    if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return true;
                    //清零数据明文，8字节；
                    //R1~R8:R1=1AH,R2保留(默认为00H)
                    //R3~R8代表命令有效截止时间(格式YYMMDDhhmmss)
                    string strData = "1A00" + DateTime.Now.AddSeconds(30).ToString("yyMMddHHmmss");
                    string str_endata = "";
                    string str_message = "";
                    string str_Div = "0000" + Helper.MeterDataHelper.Instance.Meter(pos)._Mb_MeterNo;
                    int rst = EncryptionTool.SouthDataClear1(Fag, rand2, str_Div, strData, out str_endata, out str_message);
                    if (rst == 0)
                    {
                        string strEndata = "";

                        byte[] byt_m_data = new byte[str_endata.Length / 2];
                        Array.Copy((stringToByte(str_endata.Substring(0, str_endata.Length))), 0, byt_m_data, 0, byt_m_data.Length);

                        strEndata = BitConverter.ToString(byt_m_data).Replace("-", "");
                        bln_Rst = MeterProtocols[pos].ClearEnergy(strEndata);
                    }

                }
            }
            else
            {
                bln_Rst = MeterProtocols[pos].ClearEnergy();
            }
            return bln_Rst;
        }
        #endregion

        #region 内部安全认证
        private bool PriIdentity(int pos, out byte[] rand2, out byte[] esamNo)
        {
            rand2 = new byte[4];
            esamNo = new byte[8];

            string str_rand = "";
            string str_endata = "";
           // string str_Div = Helper.MeterDataHelper.Instance.Meter(pos)._Mb_MeterNo;
            string str_Div = "0000"+Helper.MeterDataHelper.Instance.Meter(pos)._Mb_MeterNo;

            if (EncryptionTool.Status[pos] == 0 && Helper.MeterDataHelper.Instance.Meter(pos).DgnProtocol.HaveProgrammingkey)
            {
                str_Div = "0000000000000001";
            }
           
              string strMsg = "";
             
            int intRst = MeterProtocolAdapter.Instance.EncryptionTool.SouthIdentityAuthentication(0, str_Div, out str_rand, out str_endata, out strMsg);
            MessageController.Instance.AddMessage(string.Format("调用{0}返回{1},出参：{3},信息:{2}.", "SouthIdentityAuthentication", intRst, strMsg, "OutRand=" + str_rand + ",OutEndata=" + str_endata), 7, 0);
            if (intRst == 0)
            {

                bool rst2 = IdentityAuthentication(pos, str_rand, str_endata, str_Div, out rand2, out esamNo);
                return rst2;
           }
           return false;
        }
        //私钥认证zxr
        private bool PriSiYaoIentify(int pos, out byte[] rand2, out byte[] esamNo)
        {
            rand2 = new byte[4];
            esamNo = new byte[8];

            string str_rand = "";
            string str_endata = "";
            string smg = "";
            string str_Div = Helper.MeterDataHelper.Instance.Meter(pos).Mb_chrAddr;
            if (EncryptionTool.Status[pos] == 0 && Helper.MeterDataHelper.Instance.Meter(pos).DgnProtocol.HaveProgrammingkey)
            {
                str_Div = "0000000000000001";
            }
            
            int rst = EncryptionTool.SouthIdentityAuthentication(1, str_Div, out str_rand, out str_endata,out smg);
            if (rst == 0)
            {

                bool rst2 = IdentityAuthentication(pos, str_rand, str_endata, str_Div, out rand2, out esamNo);
                return rst2;
            }
            return false;
        }

        #endregion

        #region 读取需量
        /// <summary>
        /// 读取需量
        /// </summary>
        /// <param name="energyType">功率类型</param>
        /// <param name="tariffType">费率类型</param>
        /// <returns>读取到的需量值</returns>
        public float[] ReadDemand(byte energyType, byte tariffType)
        {
            float[] arrRet = new float[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate(int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                arrRet[pos] = ReadDemand(energyType, tariffType, pos);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            return arrRet;
        }



        /// <summary>
        /// 读取指定表位的需量
        /// </summary>
        /// <param name="energyType"></param>
        /// <param name="tariffType"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public float ReadDemand(byte energyType, byte tariffType, int pos)
        {
            if (MeterProtocols[pos] == null) return -1F;
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return 0F;
            return MeterProtocols[pos].ReadDemand(energyType, tariffType);
        }

        /// <summary>
        /// 读取需量[所有费率]
        /// </summary>
        /// <param name="energyType"></param>
        /// <returns></returns>
        public Dictionary<int, float[]> ReadDemands(byte energyType, int int_FreezeTimes)
        {
            Dictionary<int, float[]> arrRet = new Dictionary<int, float[]>();
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate(int pos)
            {
                if (!runFlag) return;
                float[] tmpValue = ReadDemands(energyType, int_FreezeTimes, pos);
                arrRet.Add(pos, tmpValue);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            return arrRet;
        }


        /// <summary>
        /// 读取指定表位的需量
        /// </summary>
        /// <param name="energyType"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public float[] ReadDemands(byte energyType, int int_FreezeTimes, int pos)
        {
            if (MeterProtocols[pos] == null) return new float[0];
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return new float[0];
            return MeterProtocols[pos].ReadDemands(energyType, int_FreezeTimes);
        }
        #endregion

        #region 设置冻结模式字
        public bool[] WritePatternWord(int type, string str_DateTime)
        {
            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate(int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                //arrRet[pos] = MeterProtocols[pos].WritePatternWord(type, str_DateTime);
                arrRet[pos] = WritePatternWord(type, str_DateTime, pos);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            return arrRet;
        }

        /// <summary>
        /// 写冻结模式字（包含不带和带编程键的被检表）
        /// </summary>
        /// <param name="type">冻结类型[2~6]</param>
        /// <param name="str_DateTime">冻结模式字</param>
        /// <param name="pos">表位号[0,n]</param>
        /// <returns></returns>
        private bool WritePatternWord(int type, string str_DateTime, int pos)
        {
            string str_ID = "0400090" + type.ToString();      //取出整点冻结数据模式字
            if (MeterProtocols[pos] == null) return false;
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return true;
            //二类数据
            bool bln_Rst = false;
            if (Helper.MeterDataHelper.Instance.Meter(pos).DgnProtocol.IsSouthEncryption) //南网费控
            {

                string rand1 = "";
                string rand2 = "";
                string esamNo = "";
                string[] PutData = new string[this.bwCount];
                string[] DataCode = new string[bwCount];
                string[] strData = new string[bwCount];
                string[] strID = new string[bwCount];

                if (SouthIdentity(pos, 0, out rand1, out rand2, out esamNo) == true)
                {
                    string str_div = Helper.MeterDataHelper.Instance.Meter(pos)._Mb_MeterNo;

                    for (int i = 0; i < bwCount; i++)
                    {
                        strID[i] = str_ID;
                        strData[i] = str_ID + str_DateTime;
                    }
                    bln_Rst = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(Convert.ToInt32(pos),0, rand2, strData[pos], strID[pos]);
                }
            }
            else
            {
                bln_Rst = MeterProtocols[pos].WritePatternWord(type, str_DateTime);
            }
            return bln_Rst;
        }
        //public bool WriteFreezeInterval(string str_DateTime, int pos)
        //{
        //    bool bPatternWord = false;
        //    if (MeterProtocols[pos] == null) return bPatternWord;
        //    if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return bPatternWord;
        //    bPatternWord = MeterProtocols[pos].WriteFreezeInterval(str_DateTime);
        //    return bPatternWord;
        //}
        #endregion

        #region 读取冻结模式字
        public string[] ReadPatternWord(int int_PatternType)
        {
            string[] arrRet = new string[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate(int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                arrRet[pos] = ReadPatternWord(int_PatternType, pos);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            return arrRet;
        }

        public string ReadPatternWord(int int_PatternType, int pos)
        {
            string str_PatternWord = "";
            if (MeterProtocols[pos] == null) return str_PatternWord;
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return str_PatternWord;
            MeterProtocols[pos].ReadPatternWord(1, int_PatternType, ref str_PatternWord);
            return str_PatternWord;
        }
        #endregion

        #region 设置整点冻结间隔
        public bool[] WriteFreezeInterval(int type, string str_DateTime)
        {
            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate(int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                arrRet[pos] = WriteFreezeInterval(type, str_DateTime, pos);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            return arrRet;
        }

        public bool WriteFreezeInterval(int type, string str_DateTime, int pos)
        {
            bool bPatternWord = false;
            if (MeterProtocols[pos] == null) return bPatternWord;
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return true;
            bool bln_Rst = false;
            //取密文 EncryptionTool
            if (!Helper.MeterDataHelper.Instance.Meter(pos).DgnProtocol.HaveProgrammingkey)
            {
                string str_ID = "";
                int length = 5;
                int int_FreezeType = type;
                if (int_FreezeType == 1)
                    str_ID = "04000106";
                else if (int_FreezeType == 2)
                    str_ID = "04000107";
                else if (int_FreezeType == 3)
                    str_ID = "04000108";
                else if (int_FreezeType == 4)
                    str_ID = "04000109";
                else if (int_FreezeType == 5)
                    str_ID = "04001201";
                else if (int_FreezeType == 6)
                {
                    str_ID = "04001202";
                    length = 1;
                }
                bln_Rst = WriteData(str_ID, length, str_DateTime, pos);
            }
            else
            {
                bln_Rst = MeterProtocols[pos].WriteFreezeInterval(type, str_DateTime);
            }
            return bln_Rst;
        }
        #endregion

        #region 读取冻结时间间隔
        public string[] ReadFreezeInterval()
        {
            string[] arrRet = new string[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate(int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                arrRet[pos] = ReadFreezeInterval(1, pos);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            return arrRet;
        }

        public string ReadFreezeInterval(int int_Type, int pos)
        {
            string str_FTime = "";
            if (MeterProtocols[pos] == null) return str_FTime;
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return str_FTime;
            MeterProtocols[pos].ReadFreezeInterval(int_Type, ref str_FTime);
            return str_FTime;
        }
        #endregion

        #region 读取上一次冻结时间
        public string[] ReadFreezeTime(int int_FreezeType)
        {
            string[] arrRet = new string[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate(int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                arrRet[pos] = ReadFreezeTime(int_FreezeType, pos);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            return arrRet;
        }

        public string ReadFreezeTime(int int_FreezeType, int pos)
        {
            string strReturnData = "";
            if (MeterProtocols[pos] == null) return strReturnData;
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return strReturnData;
            MeterProtocols[pos].ReadFreezeTime(int_FreezeType, ref strReturnData);
            return strReturnData;
        }
        #endregion

        #region 冻结命令
        /// <summary>
        /// 冻结命令
        /// </summary>
        /// <param name="str_DateHour">冻结时间，MMDDhhmm(月.日.时.分)</param>
        /// <returns></returns>
        public bool[] FreezeCmd(string str_DateHour)
        {

            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate(int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                arrRet[pos] = FreezeCmd(str_DateHour, pos);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            return arrRet;
        }

        public bool FreezeCmd(string strID, int pos)
        {
            if (MeterProtocols[pos] == null) return false;
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return true;
            return MeterProtocols[pos].FreezeCmd(strID);
        }
        #endregion

        #region 读取特殊电量
        public Dictionary<int, float[]> ReadSpecialEnergy(int int_DLType, int int_Times)
        {
            Dictionary<int, float[]> arrRet = new Dictionary<int, float[]>();
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate(int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;

                float[] tmpValue = ReadSpecialEnergy(int_DLType, int_Times, pos);
                arrRet.Add(pos, tmpValue);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            return arrRet;
        }

        public float[] ReadSpecialEnergy(int int_DLType, int int_Times, int pos)
        {
            float[] fReturn = new float[0];
            if (MeterProtocols[pos] == null) return fReturn;
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return fReturn;
            MeterProtocols[pos].ReadSpecialEnergy(1, int_DLType, int_Times, ref fReturn);
            return fReturn;
        }
        #endregion

        #region 设置时间

        public bool[] WriteDateTime(string newTime)
        {
            bool[] arrRet = new bool[bwCount];
            string str_MeterTime1 = newTime;
            string[] strID1 = new string[bwCount];
            string[] strData1 = new string[bwCount];
            string[] strSetData1 = new string[bwCount];
            int[] iFlag1 = new int[bwCount];
            string[] strShowData1 = new string[bwCount];
            string[] strCode1 = new string[bwCount];
            string[] strRand11 = new string[bwCount];//随机数
            string[] strRand21 = new string[bwCount];//随机
            string[] strEsamNo1 = new string[bwCount];//Esam序列号
            iFlag1 = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand11, out strRand21, out strEsamNo1);
            for (int i = 0; i < bwCount; i++)
            {
                strCode1[i] = "0400010C";
                strSetData1[i] = str_MeterTime1.Substring(0, 6) + "0" + (int)DateTime.Now.DayOfWeek;
                strSetData1[i] += str_MeterTime1.Substring(6, 6);
                strShowData1[i] = str_MeterTime1;
                strData1[i] = strCode1[i] + strSetData1[i];
            }
            arrRet = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag1, strRand21, strData1, strCode1);
            return arrRet;
        }

        public bool[] WriteDateTimeByMW(string newTime)
        {
            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate(int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                arrRet[pos] = WriteDateTimeByMW(newTime,pos);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            return arrRet;
        }

        /// <summary>
        /// 明文写时间
        /// </summary>
        /// <param name="newTime"></param>
        /// <returns></returns>
        public bool WriteDateTimeByMW(string newTime, int pos)
        {
           
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return true;
            //二类数据
            bool bln_Rst = false;
             bln_Rst = MeterProtocols[pos].WriteDateTime(newTime);
            
            return bln_Rst;
        }



        #endregion

        #region 设置切换时间
        public bool[] WriteSwitchTime(int int_SwitchType, string str_Time)
        {
            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate(int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                arrRet[pos] = WriteSwitchTime(int_SwitchType, str_Time, pos);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            return arrRet;
        }

        public bool WriteSwitchTime(int int_SwitchType, string str_Time, int pos)
        {
            bool bPatternWord = false;
            if (MeterProtocols[pos] == null) return bPatternWord;
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return bPatternWord;

            if (Helper.MeterDataHelper.Instance.Meter(pos).DgnProtocol.IsSouthEncryption) //南网费控
            {
                string rand1 = "";
                string rand2 = "";
                string esamNo = "";
                string[] PutData = new string[this.bwCount];
                string[] DataCode = new string[bwCount];
                string[] strData = new string[bwCount];
                string[] strID = new string[bwCount];
                string str_ID = "";
                if (int_SwitchType == 1)
                {
                    str_ID = "04000106"; //两套时区切换时间
                }
                else
                {
                    str_ID = "04000107"; //两套日时段表切换时间
                }
                int Fag = SouthCheckIdentity(pos, out rand1, out rand2, out esamNo); // 检查密钥状态
                if (Fag <= 1)
                {
                    string str_div = Helper.MeterDataHelper.Instance.Meter(pos)._Mb_MeterNo;
                    if (str_Time == "9999999999")
                    {
                    }
                    else
                    {
                        DateTime dte_DateTime = new DateTime(Convert.ToInt16(str_Time.Substring(0, 2)),
                                                     Convert.ToInt16(str_Time.Substring(2, 2)),
                                                     Convert.ToInt16(str_Time.Substring(4, 2)),
                                                     Convert.ToInt16(str_Time.Substring(6, 2)),
                                                     Convert.ToInt16(str_Time.Substring(8, 2)),
                                                      Convert.ToInt16(str_Time.Substring(10, 2)));

                        int int_SysWeekday = (int)dte_DateTime.DayOfWeek;
                        str_Time = dte_DateTime.ToString("yyMMddHHmm");
                    }
                    for (int i = 0; i < bwCount; i++)
                    {
                        strID[i] = str_ID;
                        strData[i] = str_ID + str_Time;// dte_DateTime.ToString("yyMMddHHmm");

                    }
                    bPatternWord = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(Convert.ToInt32(pos),Fag, rand2, strData[pos], strID[pos]);
                }
                else
                {
                }
            }
            else
            {

                bPatternWord = MeterProtocols[pos].WriteFreezeInterval(int_SwitchType, str_Time);
            }
            return bPatternWord;
        }
        #endregion

        #region 广播校时
        /// <summary>
        /// 广播校时
        /// </summary>
        /// <param name="newTime">新时间</param>
        public void BroadCastTime(DateTime newTime)
        {
            //打开所有通讯通道
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate(int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(0xFF)) return;
                MeterProtocols[pos].BroadcastTime(newTime);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();

        }

        /// <summary>
        /// 广播校时(点对点)
        /// </summary>
        /// <param name="newTime">新时间</param>
        public void BroadCastTimeByPoint(DateTime newTime)
        {
            //打开所有通讯通道
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate(int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(0xFF)) return;
                MeterProtocols[pos].BroadcastTimeByPoint(newTime);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();

        }

        #endregion

        #region 设置跳闸心跳帧
        public bool[] SetBreakRelayTime(int Time)
        {
            bool[] arrRet = new bool[bwCount];

            //打开所有通讯通道
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate(int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(0xFF)) return;
               arrRet[pos] = MeterProtocols[pos].SetBreakRelayTime(Time);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            return arrRet;
        }


        #endregion

        #region 清除事件记录
        public bool[] ClearEventLog(string strID)
        {
            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate(int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                arrRet[pos] = ClearEventLog(strID, pos);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            return arrRet;
        }


        public bool ClearEventLog(string strID, int pos)
        {
            if (MeterProtocols[pos] == null) return false;
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return true;

            bool bln_Rst = false;
            //取密文 EncryptionTool
           if (Helper.MeterDataHelper.Instance.Meter(pos).DgnProtocol.IsSouthEncryption) //南网费控
            {
                string rand1 = "";
                string rand2 = "";
                string esamNo = "";

                int Fag = SouthCheckIdentity(pos, out rand1, out rand2, out esamNo); // 检查密钥状态
                if (Fag <= 1)
                {
                    string str_div = Helper.MeterDataHelper.Instance.Meter(pos)._Mb_MeterNo;

                    if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return true;
                    //清零数据明文，8字节；
                    //R1~R8:R1=1AH,R2保留(默认为00H)
                    //R3~R8代表命令有效截止时间(格式YYMMDDhhmmss)
                    string strData = "1B00" + DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss") + strID;
                    string str_endata = "";
                    string str_message = "";
                    string str_Div = "0000" + Helper.MeterDataHelper.Instance.Meter(pos)._Mb_MeterNo;
                    int rst = EncryptionTool.SouthDataClear1(Fag, rand2, str_Div, strData, out str_endata, out str_message);
                    if (rst == 0)
                    {
                        string strEndata = "";

                        byte[] byt_m_data = new byte[str_endata.Length / 2];
                        Array.Copy((stringToByte(str_endata.Substring(0, str_endata.Length))), 0, byt_m_data, 0, byt_m_data.Length);

                        strEndata = BitConverter.ToString(byt_m_data).Replace("-", "");
                        bln_Rst = MeterProtocols[pos].ClearEventLog(strID, strEndata);
                    }

                }
            }
            else
            {
                return MeterProtocols[pos].ClearEventLog(strID);
            }
            return bln_Rst;
        }
        #endregion


        #region 蓝牙认证查询
        /// <summary>
        /// 所有表位的蓝牙认证查询
        /// </summary>
        /// <param name="Flag">表示电表密钥状态，整型，0: 测试密钥状态；</param>
        /// <param name="OutesamNo">电能表esam号</param>
        /// <param name="enRand1">电能表传出的随机数1密文</param>
        /// <param name="Outrand2">电能表传出的随机数</param>
        /// <returns></returns>
        public bool[] SouthInBlueRand(out string[] OutesamNo, out string[] Rand1, out string[] enRand1, out string[] Outrand2)
        {
            Rand1 = new string[bwCount];
            OutesamNo = new string[bwCount];
            enRand1 = new string[bwCount];
            Outrand2 = new string[bwCount];
            //out参数不能传入lamda表达式,定义一些参数进行中转
            string[] arrOutesamNo = new string[bwCount];
            string[] arrRand1 = new string[bwCount];
            string[] arrenRand1 = new string[bwCount];
            string[] arrOutrand2 = new string[bwCount];

            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate(int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                if (!GlobalUnit.IsDemo)
                {
                    arrRet[pos] = SouthInBlueRand(pos, out arrOutesamNo[pos], out arrRand1[pos], out arrenRand1[pos], out arrOutrand2[pos]);
                }
                else
                {
                    arrRet[pos] = true;
                }
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            Rand1 = arrRand1;
            OutesamNo = arrOutesamNo;
            enRand1 = arrenRand1;
            Outrand2 = arrOutrand2;
            return arrRet;
        }
        /// <summary>
        /// 单表位蓝牙认证查询
        /// </summary>
        /// <param name="pos">表位号</param>
        /// <param name="OutesamNo">电能表esam号</param>
        /// <param name="enRand1">电能表传出的随机数1密文</param>
        /// <param name="Outrand2">电能表传出的随机数</param>
        /// <returns></returns>
        public bool SouthInBlueRand(int pos, out string OutesamNo, out string Rand1, out string enRand1, out string Outrand2)
        {
            Rand1 = "";
            enRand1 = "";
            Outrand2 = "";
            OutesamNo = "";
            string Rand1Tmp = "";
            string enRand1Tmp = "";
            string Outrand2Tmp = "";
            string OutesamNoTmp = "";
            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return true;

            string strOutEn = "";
            string rand1 = "";
            string message = "";
            if (EncryptionTool.SouthInfraredRand(out Rand1Tmp, out message) == 0)
            {
                byte[] cmd = bytesReserve(new byte[] { 0x07, 0x80, 0x01, 0xFF });
                byte[] oper = new byte[4];
                byte[] rCode = stringToByte(Rand1Tmp);
                List<byte> frameData = new List<byte>();
                frameData.AddRange(cmd);
                frameData.AddRange(oper);
                frameData.AddRange(rCode);
                bool seqela = false;
                byte[] revdata = null;
                if (MeterProtocols[pos].UpdateRemoteEncryptionCommand(0x03, frameData.ToArray(), ref seqela, ref revdata))
                {
                    strOutEn = BitConverter.ToString(bytesReserve(revdata)).Replace("-", "");
                    if (strOutEn.Length >= 68)
                    {
                        OutesamNoTmp = strOutEn.Substring(32, 16);
                        enRand1Tmp = strOutEn.Substring(16, 16);
                        Outrand2Tmp = strOutEn.Substring(0, 16);

                        OutesamNo = OutesamNoTmp;
                        Rand1 = Rand1Tmp;
                        enRand1 = enRand1Tmp;
                        Outrand2 = Outrand2Tmp;
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion

        #region 蓝牙认证
        public bool[] SouthInBlueAuth(int[] Flag, string[] esamNo, string[] rand1, string[] enRand1, string[] rand2, out string[] enRand2)
        {
            bool[] arrRet = new bool[bwCount];
            string[] enRand2Tmp = new string[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate(int pos)
            {
                if (!runFlag) return;
                if (!OpenCommSwitch(pos)) return;
                if (!GlobalUnit.IsDemo)
                {
                    arrRet[pos] = SouthInBlueAuth(pos, Flag[pos], esamNo[pos], rand1[pos], enRand1[pos], rand2[pos], out enRand2Tmp[pos]);
                }
                else
                {
                    arrRet[pos] = true;
                }
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            enRand2 = enRand2Tmp;
            return arrRet;
        }
        /// <summary>
        /// 单表位蓝牙认证
        /// </summary>
        /// <param name="pos">表位号</param>
        /// <param name="OutesamNo">电能表返回esam号</param>
        /// <param name="rand1">加密机传出的随机数1</param>
        /// <param name="enRand1">电能表返回的随机数1密文</param>
        /// <param name="rand2">电能表传出的随机数2</param>
        /// <param name="enRand2">加密机传出的随机数2密文</param>
        /// <returns></returns>
        public bool SouthInBlueAuth(int pos, int Flag, string esamNo, string rand1, string enRand1, string rand2, out string enRand2)
        {

            enRand2 = "";
            string enRand2Tmp = "";
            string strOutEn = "";

            if (!Helper.MeterDataHelper.Instance.Meter(pos).YaoJianYn) return true;

            string str_Div = "0000" + Helper.MeterDataHelper.Instance.Meter(pos)._Mb_MeterNo;
            string message = "";

            if (EncryptionTool.SouthInfraredAuth(Flag, str_Div, esamNo, rand1, enRand1, rand2, out enRand2Tmp, out message) == 0)
            {
                byte[] cmd = bytesReserve(new byte[] { 0x07, 0x80, 0x06, 0xFF });   //蓝牙认证指令
                byte[] oper = new byte[4];
                byte[] rCode = stringToByte(enRand2Tmp);
                List<byte> frameData = new List<byte>();
                frameData.AddRange(cmd);
                frameData.AddRange(oper);
                frameData.AddRange(rCode);
                bool seqela = false;
                byte[] revdata = null;
                if (MeterProtocols[pos].UpdateRemoteEncryptionCommand(0x03, frameData.ToArray(), ref seqela, ref revdata))
                {
                    enRand2 = enRand2Tmp;
                    return true;
                }
            }

            return false;
        }
        #endregion
        #endregion



    }
}
