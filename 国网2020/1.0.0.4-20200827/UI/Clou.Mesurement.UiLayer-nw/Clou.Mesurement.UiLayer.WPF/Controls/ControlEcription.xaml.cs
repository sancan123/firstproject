using Mesurement.UiLayer.ViewModel;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Mesurement.UiLayer.WPF.Controls
{
    /// <summary>
    /// ControlEcription.xaml 的交互逻辑
    /// </summary>
    public partial class ControlEcription : UserControl
    {
        
        public ControlEcription()
        {
            InitializeComponent();

            txt_Type.Text = viewModel.strType;
            txt_IP.Text = viewModel.strIP;
            txt_Port.Text = viewModel.intPort.ToString();
        }
        private EcriptionViewModel viewModel
        {
            get { return Resources["EcriptionViewModel"] as EcriptionViewModel; }
        }

       

    }
    public class EcriptionViewModel : ViewModelBase
    {
        List<string> listEcription = new List<string>();
        public EcriptionViewModel()
        {
            listEcription = DAL.Config.ConfigHelper.Instance.GetConfig(DAL.Config.EnumConfigId.加密机配置);
            string[] strEcriptionData = listEcription[0].Split('|');
            _strType = strEcriptionData[0];
            _strIP = strEcriptionData[1];
            _intPort = int.Parse(strEcriptionData[2]);
            _intTimeOut = int.Parse(strEcriptionData[4]);
        }
        #region 方法
        #region 加密机
        public void SouthLink()
        {
            EquipmentData.DeviceManager.SouthLink(strType, strIP, intPort, intTimeOut);
        }
        public void SouthCloseDevice()
        {
            EquipmentData.DeviceManager.SouthCloseDevice();
        }
        public void SouthIdentityAuthentication()
        {
            EquipmentData.DeviceManager.SouthIdentityAuthentication(Flag,  PutDiv, out _OutRand, out _OutEndata);
        }
        public void SouthUserControl()
        {
            PutData = "1A00140730104001";
            EquipmentData.DeviceManager.SouthUserControl(Flag, PutRand, PutDiv, PutEsamNo,
 PutData, out _OutEndata);
        }
        public void SouthParameterUpdate()
        {
            EquipmentData.DeviceManager.SouthParameterUpdate(Flag, PutRand, PutDiv, PutApdu, PutData, out _OutData);
        }
        public void SouthPrice1Update()
        {
            PutApdu = "04D6830484";
            EquipmentData.DeviceManager.SouthPrice1Update(Flag, PutRand, PutDiv, PutApdu, PutData, out _OutData);
        }
        public void SouthPrice2Update()
        {
            PutApdu = "04D6840484";
            EquipmentData.DeviceManager.SouthPrice2Update(Flag, PutRand, PutDiv, PutApdu, PutData, out _OutData);
        }
        public void SouthParameterElseUpdate()
        {
            PutApdu = "04D6880014";
            EquipmentData.DeviceManager.SouthParameterElseUpdate(Flag, PutRand, PutDiv, PutApdu, PutData, out _OutEndata);
        }
        public void SouthIncreasePurse()
        {
            PutData = "000000C800000002112233445566";
            EquipmentData.DeviceManager.SouthIncreasePurse(1, PutRand, PutDiv, PutData, out _OutData);
        }
        public void SouthInitPurse()
        {
            PutData = "000000C8";
            EquipmentData.DeviceManager.SouthInitPurse(0, PutRand, PutDiv, PutData, out _OutData);
        }
        public void SouthKeyUpdateV2()
        {
            PutKeySum = 17;
            PutKeyId = "00010203";
            EquipmentData.DeviceManager.SouthKeyUpdateV2(PutKeySum, PutKeyState, PutKeyId, PutRand, PutDiv, PutEsamNo, out _OutData);
        }
        public void SouthDataClear1()
        {
            PutData = "1900140903111011";
            EquipmentData.DeviceManager.SouthDataClear1(Flag, PutRand, PutDiv, PutData, out _OutData);
        }
        public void SouthInfraredRand()
        {
            EquipmentData.DeviceManager.SouthInfraredRand(out _OutRand1);
        }
        public void SouthInfraredAuth()
        {
            EquipmentData.DeviceManager.SouthInfraredAuth(Flag, PutDiv, PutEsamNo, PutRand1, PutRand1Endata, PutRand2, out _OutRand2Endata);
        }
        public void SouthMacCheck()
        {
            PutData = "11223344";
            EquipmentData.DeviceManager.SouthMacCheck(Flag, PutRand, PutDiv, PutApdu, PutData, PutMac);
        }
        public void SouthMacWrite()
        {
            PutFileID = "17";
            PutData = "1122334455";
            EquipmentData.DeviceManager.SouthMacWrite(Flag, PutRand, PutDiv, PutEsamNo, PutFileID, PutDataBegin, PutData, out _OutData);
        }
        public void SouthEncMacWrite()
        {
            PutFileID = "18";
            EquipmentData.DeviceManager.SouthEncMacWrite(Flag, PutRand, PutDiv, PutEsamNo, PutFileID, PutDataBegin, PutData, out _OutData);
        }
        public void SouthEncForCompare()
        {
            PutKeyId = "06";
            PutData =
"00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";
            EquipmentData.DeviceManager.SouthEncForCompare(PutKeyId, PutDiv, PutData, out _OutData);
        }
        public void SouthDecreasePurse()
        {
            PutData = "000000C8";
            EquipmentData.DeviceManager.SouthDecreasePurse(1, PutRand, PutDiv, PutData, out _OutEndata);
        }
        public void SouthSwitchChargeMode()
        {
            PutData = "020000000500000001";
            EquipmentData.DeviceManager.SouthSwitchChargeMode(1, PutRand, PutDiv, PutData,
  out _OutData);
        }
        #endregion

        #region 读卡器
        public void WINAPI_OpenDevice()
        {
            EquipmentData.DeviceManager.WINAPI_OpenDevice();
        }
        public void WINAPI_ReadUserCardNum()
        {
            EquipmentData.DeviceManager.WINAPI_ReadUserCardNum(out _UserCardNum);
        }
        public void WINAPI_ReadParamPresetCardNum()
        {
            EquipmentData.DeviceManager.WINAPI_ReadParamPresetCardNum(out _ParamPresetCardNum);
        }
        public void WINAPI_CloseDevice()
        {
            EquipmentData.DeviceManager.WINAPI_CloseDevice();
        }
        public void WINAPI_ReadParamPresetCard()
        {
            EquipmentData.DeviceManager.WINAPI_ReadParamPresetCard(out _fileParam, out _fileMoney, out _filePrice1, out _filePrice2, out _cardNum);
        }
        public void WINAPI_MakeParamPresetCard()
        {
            fileParam = "6802001A00000000000000000000000000000000000000000000000000001C16";
            fileMoney = "0000000000000000";
            filePrice1 = "680100C100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000C216";
            filePrice2 = "680100C100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000C216";
            EquipmentData.DeviceManager.WINAPI_MakeParamPresetCard(fileParam, fileMoney, filePrice1, filePrice2);
        }
        public void WINAPI_ReadUserCard()
        {
            EquipmentData.DeviceManager.WINAPI_ReadUserCard(out _fileParam, out _fileMoney, out _filePrice1, out _filePrice2, out _fileReply, out _enfileControl, out _cardNum);
        }
        public void WINAPI_MakeUserCard()
        {
            fileParam = "680300270000000000000000000000000000000000000000000000000000000000000000000000000000002A16";
            fileMoney = "0000000000000000";
            filePrice1 = "680100C100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000C216";
            filePrice2 = "680100C100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000C216";
            fileReply = "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";
            fileControl = "1B00000015250315";// "0000000000000000";////BA44EF16F2C3FFF4D831C7B90FBD60416B3BA5F9
            EquipmentData.DeviceManager.WINAPI_MakeUserCard(fileParam, fileMoney, filePrice1, filePrice2, fileControl);
        }
        #endregion

        #endregion

        #region 属性
        private string _strType = "南网加密机";
        public string strType
        {
            get { return _strType; }
            set { SetPropertyValue(value, ref _strType, "strType"); }
        }
        private string _strIP = "192.168.19.99";
        public string strIP
        {
            get { return _strIP; }
            set { SetPropertyValue(value, ref _strIP, "strIP"); }
        }
        private int _intPort = 8018;
        public int intPort
        {
            get { return _intPort; }
            set { SetPropertyValue(value, ref _intPort, "intPort"); }
        }
        private int _intTimeOut = 30;
        public int intTimeOut
        {
            get { return _intTimeOut; }
            set { SetPropertyValue(value, ref _intTimeOut, "intTimeOut"); }
        }
        private int _Flag = 0;
        public int Flag
        {
            get { return _Flag; }
            set { SetPropertyValue(value, ref _Flag, "Flag"); }
        }
        private string _PutDiv = "0000000000000001";
        public string PutDiv
        {
            get { return _PutDiv; }
            set { SetPropertyValue(value, ref _PutDiv, "PutDiv"); }
        }
        private string _OutRand = "";
        public string OutRand
        {
            get { return _OutRand; }
            set { SetPropertyValue(value, ref _OutRand, "OutRand"); }
        }
        private string _OutEndata = "";
        public string OutEndata
        {
            get { return _OutEndata; }
            set { SetPropertyValue(value, ref _OutEndata, "OutEndata"); }
        }
        private string _PutEsamNo = "0000000000000000";
        public string PutEsamNo
        {
            get { return _PutEsamNo; }
            set { SetPropertyValue(value, ref _PutEsamNo, "PutEsamNo"); }
        }
        private string _PutData = "1A00140730104001";
        public string PutData
        {
            get { return _PutData; }
            set { SetPropertyValue(value, ref _PutData, "PutData"); }
        }
         
        private string _OutEndataout = "";
        public string OutEndataout
        {
            get { return _OutEndataout; }
            set { SetPropertyValue(value, ref _OutEndataout, "OutEndataout"); }
        }
        private string _PutRand = "271789B1";
        public string PutRand
        {
            get { return _PutRand; }
            set { SetPropertyValue(value, ref _PutRand, "PutRand"); }
        }
        private string _PutApdu = "04D6811008";
        public string PutApdu
        {
            get { return _PutApdu; }
            set { SetPropertyValue(value, ref _PutApdu, "PutApdu"); }
        }
        private string _OutData = "";
        public string OutData
        {
            get { return _PutApdu; }
            set { SetPropertyValue(value, ref _OutData, "OutData"); }
        }
        private int _PutKeySum = 17;
        public int PutKeySum
        {
            get { return _PutKeySum; }
            set { SetPropertyValue(value, ref _PutKeySum, "PutKeySum"); }
        }
        private string _PutKeyState = "01";
        public string PutKeyState
        {
            get { return _PutKeyState; }
            set { SetPropertyValue(value, ref _PutKeyState, "PutKeyState"); }
        }
        private string _PutKeyId = "0001020A";
        public string PutKeyId
        {
            get { return _PutKeyId; }
            set { SetPropertyValue(value, ref _PutKeyId, "PutKeyId"); }
        }
        private string _OutRand1 = "";
        public string OutRand1
        {
            get { return _OutRand1; }
            set { SetPropertyValue(value, ref _OutRand1, "OutRand1"); }
        }
        private string _PutRand1 = "271789B1271789B1";
        public string PutRand1
        {
            get { return _PutRand1; }
            set { SetPropertyValue(value, ref _PutRand1, "PutRand1"); }
        }
        private string _PutRand1Endata = "B89467F6980DA078";
        public string PutRand1Endata
        {
            get { return _PutRand1Endata; }
            set { SetPropertyValue(value, ref _PutRand1Endata, "PutRand1Endata"); }
        }
        private string _PutRand2 = "98C1FFAFFA314BEC";
        public string PutRand2
        {
            get { return _PutRand2; }
            set { SetPropertyValue(value, ref _PutRand2, "PutRand2"); }
        }
        private string _OutRand2Endata = "";
        public string OutRand2Endata
        {
            get { return _OutRand2Endata; }
            set { SetPropertyValue(value, ref _OutRand2Endata, "OutRand2Endata"); }
        }
        private string _PutMac = "33445566";
        public string PutMac
        {
            get { return _PutMac; }
            set { SetPropertyValue(value, ref _PutMac, "PutMac"); }
        }
        private string _PutFileID = "17";
        public string PutFileID
        {
            get { return _PutFileID; }
            set { SetPropertyValue(value, ref _PutFileID, "PutFileID"); }
        }
        private string _PutDataBegin = "0000";
        public string PutDataBegin
        {
            get { return _PutDataBegin; }
            set { SetPropertyValue(value, ref _PutDataBegin, "PutDataBegin"); }
        }

        private string _fileParam = "";
        public string fileParam
        {
            get { return _fileParam; }
            set { SetPropertyValue(value, ref _fileParam, "fileParam"); }
        }
        private string _fileMoney = "";
        public string fileMoney
        {
            get { return _fileMoney; }
            set { SetPropertyValue(value, ref _fileMoney, "fileMoney"); }
        }
        private string _filePrice1 = "";
        public string filePrice1
        {
            get { return _filePrice1; }
            set { SetPropertyValue(value, ref _filePrice1, "filePrice1"); }
        }
        private string _filePrice2 = "";
        public string filePrice2
        {
            get { return _filePrice2; }
            set { SetPropertyValue(value, ref _filePrice2, "filePrice2"); }
        }
        private string _cardNum = "";
        public string cardNum
        {
            get { return _cardNum; }
            set { SetPropertyValue(value, ref _cardNum, "cardNum"); }
        }
        private string _fileReply = "";
        public string fileReply
        {
            get { return _fileReply; }
            set { SetPropertyValue(value, ref _fileReply, "fileReply"); }
        }
        private string _enfileControl = "";
        public string enfileControl
        {
            get { return _enfileControl; }
            set { SetPropertyValue(value, ref _enfileControl, "enfileControl"); }
        }
        private string _fileControl = "";
        public string fileControl
        {
            get { return _fileControl; }
            set { SetPropertyValue(value, ref _fileControl, "fileControl"); }
        }
        private string _UserCardNum = "";
        public string UserCardNum
        {
            get { return _UserCardNum; }
            set { SetPropertyValue(value, ref _UserCardNum, "UserCardNum"); }
        }
        private string _ParamPresetCardNum = "";
        public string ParamPresetCardNum
        {
            get { return _ParamPresetCardNum; }
            set { SetPropertyValue(value, ref _ParamPresetCardNum, "ParamPresetCardNum"); }
        }

        #endregion

    }
}
