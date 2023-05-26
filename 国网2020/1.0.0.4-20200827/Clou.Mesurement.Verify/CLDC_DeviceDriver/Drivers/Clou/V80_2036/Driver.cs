using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceDriver.Drivers.Clou.V80_2036
{
    internal class Driver:DeviceDriver.Drivers.IDriver
    {


        #region IDriver 成员

        public MsgCallBack CallBack
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public bool Stop()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool Link()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool UnLink()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool SetHarmonic(int[][] int_XTSwitch, float[][] sng_Value, float[][] sng_Phase)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        
        public bool PowerOff()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool InitWarmUp(Comm.Enum.Cus_PowerFangXiang glfx, ImpluseMode[] im, bool[] IsOnOff)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool InitDuiSeBiao(Comm.Enum.Cus_Clfs clfs, Comm.Enum.Cus_PowerFangXiang glfx, float sng_U, float sng_I, int PulseCount, ImpluseMode[] im, bool[] IsOnOff)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool InitStartUp(Comm.Enum.Cus_Clfs clfs, Comm.Enum.Cus_PowerFangXiang glfx, float sng_Ub, float sng_Ib, float sng_IMax, float sng_xUb_A, float sng_xUb_B, float sng_xUb_C, float sng_xIb_A, float sng_xIb_B, float sng_xIb_C, ImpluseMode[] im, bool[] IsOnOff)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool InitCreeping(Comm.Enum.Cus_Clfs clfs, Comm.Enum.Cus_PowerFangXiang glfx, float sng_Ub, float sng_Ib, float sng_IMax, float sng_xUb_A, float sng_xUb_B, float sng_xUb_C, float sng_xIb_A, float sng_xIb_B, float sng_xIb_C, ImpluseMode[] im, bool[] IsOnOff)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool InitError(Comm.Enum.Cus_Clfs clfs, Comm.Enum.Cus_PowerFangXiang glfx, float[] bcs, int[] quans, int wccs, ImpluseMode[] im, bool[] IsOnOff)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool InitCommTest(bool[] IsOnOff)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool InitTimeAccuracy(bool[] IsOnOff)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool InitTimePeriod(bool[] IsOnOff)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool InitMaxDemand(bool[] IsOnOff)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool InitDemandPeriod(bool[] IsOnOff)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool InitZZ(bool[] IsOnOff, Comm.Enum.Cus_PowerFangXiang glfx, ImpluseMode[] im)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public stError[] ReadWcb()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public stError ReadWcb(int intBwh)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        //public stPowerInfo ReadPowerInfo()
        //{
        //    throw new Exception("The method or operation is not implemented.");
        //}

        public stStdInfo ReadStdInfo()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool ReadTempHuim(ref float sng_temp, ref float sng_huim)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public DateTime ReadGPSTime()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool InitRs485(string[] arBtl)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public byte[] Rs485(byte[] bySend, ref byte[] byRecv, int intBwh)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IDriver 成员


        public bool PowerOn(Comm.Enum.Cus_Clfs clfs, Comm.Enum.Cus_PowerFangXiang glfx, float sng_Ub, float sng_Ib, float sng_IMax, float sng_xUb_A, float sng_xUb_B, float sng_xUb_C, float sng_xIb_A, float sng_xIb_B, float sng_xIb_C, Comm.Enum.Cus_PowerYuanJiang element, float sng_UaPhi, float sng_UbPhi, float sng_UcPhi, float sng_IaPhi, float sng_IbPhi, float sng_IcPhi, float sng_Freq, bool IsDuiBiao, bool IsQianDong, bool bln_IsNxx)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool InitStartUp(Comm.Enum.Cus_Clfs clfs, Comm.Enum.Cus_PowerFangXiang glfx, ImpluseMode[] im, bool[] IsOnOff, int[] startTimes)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool InitCreeping(Comm.Enum.Cus_Clfs clfs, Comm.Enum.Cus_PowerFangXiang glfx, ImpluseMode[] im, bool[] IsOnOff, int[] startTimes)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool InitMaxDemand(bool[] IsOnOff, int xlzqSeconds, int hccs)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool InitDemandPeriod(bool[] IsOnOff, int xlzqSeconds, int hccs)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public stError[] ReadWcb(bool[] IsOnOff, int errTimes)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public stError ReadWcb(int intBwh, int errTimes)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public byte[] Rs485(byte[] bySend, ref byte[] byRecv, int intBwh, bool isNeedReturn)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IDriver 成员


        public bool InitTimePeriod(bool[] IsOnOff, int xlzqSeconds, int hccs)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IDriver 成员


        private bool isShowLog = false;
        public bool ShowLog
        {
            get
            {
                return isShowLog;
            }
            set
            {
                isShowLog = true;
            }
        }

        #endregion
    }
}
