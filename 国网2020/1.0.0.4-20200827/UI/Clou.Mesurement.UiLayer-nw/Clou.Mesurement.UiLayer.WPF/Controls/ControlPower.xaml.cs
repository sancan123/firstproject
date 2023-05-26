using Mesurement.UiLayer.ViewModel;
using Mesurement.UiLayer.ViewModel.Model;
using System.Windows.Controls;

namespace Mesurement.UiLayer.WPF.Controls
{
    /// <summary>
    /// ControlPower.xaml 的交互逻辑
    /// </summary>
    public partial class ControlPower : UserControl
    {
        public ControlPower()
        {
            InitializeComponent();
        }
        private PowerViewModel viewModel
        {
            get { return Resources["PowerViewModel"] as PowerViewModel; }
        }
    }

    public class PowerViewModel : ViewModelBase
    {
        public PowerViewModel()
        {
            PowerParaCommand.CommandAction = (obj) => PowerCommandFactory(obj as string);
        }

        #region 自由升源信息
        private double ua = 100;
        public double Ua
        {
            get { return ua; }
            set { SetPropertyValue(value, ref ua, "Ua"); }
        }
        private double phaseUa;
        public double PhaseUa
        {
            get { return phaseUa; }
            set { SetPropertyValue(value, ref phaseUa, "PhaseUa"); }
        }
        private double ub = 100;
        public double Ub
        {
            get { return ub; }
            set { SetPropertyValue(value, ref ub, "Ub"); }
        }
        private double phaseUb = 240;
        public double PhaseUb
        {
            get { return phaseUb; }
            set { SetPropertyValue(value, ref phaseUb, "PhaseUb"); }
        }
        private double uc = 100;
        public double Uc
        {
            get { return uc; }
            set { SetPropertyValue(value, ref uc, "Uc"); }
        }
        private double phaseUc = 120;
        public double PhaseUc
        {
            get { return phaseUc; }
            set { SetPropertyValue(value, ref phaseUc, "PhaseUc"); }
        }
        private double ia;
        public double Ia
        {
            get { return ia; }
            set { SetPropertyValue(value, ref ia, "Ia"); }
        }
        private double phaseIa;
        public double PhaseIa
        {
            get { return phaseIa; }
            set { SetPropertyValue(value, ref phaseIa, "PhaseIa"); }
        }
        private double ib = 0;
        public double Ib
        {
            get { return ib; }
            set { SetPropertyValue(value, ref ib, "Ib"); }
        }
        private double phaseIb = 240;
        public double PhaseIb
        {
            get { return phaseIb; }
            set { SetPropertyValue(value, ref phaseIb, "PhaseIb"); }
        }
        private double ic = 0;
        public double Ic
        {
            get { return ic; }
            set { SetPropertyValue(value, ref ic, "Ic"); }
        }
        private double phaseIc = 120;
        public double PhaseIc
        {
            get { return phaseIc; }
            set { SetPropertyValue(value, ref phaseIc, "PhaseIc"); }
        }
        private float freq = 50;

        public float Freq
        {
            get { return freq; }
            set { SetPropertyValue(value, ref freq, "Freq"); }
        }

        #endregion

        #region 快速升源信息
        private string connectionMode = "三相四线";
        /// <summary>
        /// 接线方式
        /// </summary>
        public string ConnectionMode
        {
            get { return connectionMode; }
            set { SetPropertyValue(value, ref connectionMode, "ConnectionMode"); }
        }
        private string powerComponent = "合元";
        /// <summary>
        /// 功率元件
        /// </summary>
        public string PowerComponent
        {
            get { return powerComponent; }
            set { SetPropertyValue(value, ref powerComponent, "PowerComponent"); }
        }
        private double un = 57.7;
        /// <summary>
        /// 输出电压
        /// </summary>
        public double Un
        {
            get { return un; }
            set
            {
                if (value < 0)
                {
                    SetPropertyValue(0, ref un, "Un");
                }
                else
                { SetPropertyValue(value, ref un, "Un"); }
            }
        }
        private double iRated = 1.5;
        /// <summary>
        /// 额定电流
        /// </summary>
        public double IRated
        {
            get { return iRated; }
            set { SetPropertyValue(value, ref iRated, "IRated"); }
        }
        private double ibRates = 1.0;
        /// 电流倍数
        /// <summary>
        /// 电流倍数
        /// </summary>
        public double IbRates
        {
            get { return ibRates; }
            set
            {
                if (value < 0)
                {
                    SetPropertyValue(0, ref ibRates, "IbRates");
                }
                else
                {
                    SetPropertyValue(value, ref ibRates, "IbRates");
                }
            }
        }
        private string powerFactor = "1.0";
        /// <summary>
        /// 功率因数
        /// </summary>
        public string PowerFactor
        {
            get { return powerFactor; }
            set { SetPropertyValue(value, ref powerFactor, "PowerFactor"); }
        }
        private float frequency = 50;
        /// 频率
        /// <summary>
        /// 频率
        /// </summary>
        public float Frequency
        {
            get { return frequency; }
            set
            {
                if (value < 0)
                {
                    SetPropertyValue(0, ref frequency, "Frequency");
                }
                else
                {
                    SetPropertyValue(value, ref frequency, "Frequency");
                }
            }
        }

        #endregion

        public void PowerOnFree()
        {
            EquipmentData.DeviceManager.PowerOnFree(Ua, Ub, Uc, Ia, Ib, Ic, PhaseUa, PhaseUb, PhaseUc, PhaseIa, PhaseIb, PhaseIc, Freq);
        }

        public void PowerOnQuik()
        {
            //double ib = IRated * IbRates;
            //#region 功率元件
            //int ele = 1;
            //switch (PowerComponent)
            //{
            //    case "合元":
            //        ele = 1;
            //        break;
            //    case "A相":
            //        ele = 2;
            //        break;
            //    case "B相":
            //        ele = 3;
            //        break;
            //    case "C相":
            //        ele = 4;
            //        break;
            //}
            //#endregion
        }

        public void PowerOff()
        {
            EquipmentData.DeviceManager.PowerOff();
        }

        private BasicCommand powerParaCommand;
        /// 控件命令
        /// <summary>
        /// 控件命令
        /// </summary>
        public BasicCommand PowerParaCommand
        {
            get
            {
                if (powerParaCommand == null)
                {
                    powerParaCommand = new BasicCommand();
                }
                return powerParaCommand;
            }
            set { powerParaCommand = value; }
        }
        /// 解析快速参数设置按钮
        /// <summary>
        /// 解析快速参数设置按钮
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private void PowerCommandFactory(string p)
        {
            switch (p)
            {
                #region 接线方式
                case "单相":
                case "三相四线":
                case "三相三线":
                    ConnectionMode = p;
                    break;
                #endregion
                #region 功率元件
                case "合元":
                case "A相":
                case "B相":
                case "C相":
                    PowerComponent = p;
                    break;
                #endregion
                #region 输出电压
                case "0V":
                    Un = 0;
                    break;
                case "57.7V":
                    Un = 57.7;
                    break;
                case "100V":
                    Un = 100;
                    break;
                case "220V":
                    Un = 220;
                    break;
                case "+5%":
                    Un *= 1.05;
                    break;
                case "-5%":
                    Un *= 0.95;
                    break;
                #endregion
                #region 额定电流
                case "0.3A":
                    IRated = 0.3;
                    break;
                case "1.5A":
                    IRated = 1.5;
                    break;
                case "5A":
                    IRated = 5;
                    break;
                case "10A":
                    IRated = 10;
                    break;
                case "20A":
                    IRated *= 20;
                    break;
                case "40A":
                    IRated = 40;
                    break;
                #endregion
                #region 电流倍数
                case "0.01Ib":
                case "0.02Ib":
                case "0.1Ib":
                case "0.2Ib":
                case "Ib":
                case "2Ib":
                    if (p == "Ib")
                    {
                        IbRates = 1;
                    }
                    else
                    {
                        string ratesIb = p.Replace("Ib", "");
                        double temp = IbRates;
                        double.TryParse(ratesIb, out temp);
                        IbRates = temp;
                    }
                    break;
                case "+0.01Ib":
                    IbRates += 0.01;
                    break;
                case "+0.1Ib":
                    IbRates += 0.1;
                    break;
                case "-1%":
                    IbRates -= 0.01;
                    break;
                case "-10%":
                    IbRates -= 0.1;
                    break;
                #endregion
                #region 功率因数
                case "0.25L":
                case "1.0":
                case "0.5L":
                case "0.5C":
                case "0.8L":
                case "0.8C":
                    PowerFactor = p;
                    break;
                #endregion
                #region 频率
                case "+0.1":
                    Frequency += (float)0.1;
                    break;
                case "+1":
                    Frequency += (float)1;
                    break;
                case "-0.1":
                    Frequency -= (float)0.1;
                    break;
                case "-1":
                    Frequency -= (float)1;
                    break;
                case "50Hz":
                    Frequency = 50;
                    break;
                #endregion
            }
        }
    }
}
