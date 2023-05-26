namespace Mesurement.UiLayer.ViewModel.Monitor
{
    /// 标准表信息
    /// <summary>
    /// 标准表信息
    /// </summary>
    public class StdInfoViewModel : ViewModelBase
    {
        /// 是否为单相表
        /// <summary>
        /// 是否为单相表
        /// </summary>
        public bool IsSingle
        {
            get
            {
                return EquipmentData.Equipment.EquipmentType == "单相台" ;
            }
        }
        /// 是否为三相
        /// <summary>
        /// 是否为三相
        /// </summary>
        public bool IsThree
        {
            get { return !IsSingle; }
        }
        

        private float ua;
        /// <summary>
        /// A相电压
        /// </summary>
        public float Ua
        {
            get { return ua; }
            set { SetPropertyValue(value, ref ua, "Ua"); }
        }

        private float ub ;
        /// <summary>
        /// B相电压
        /// </summary>
        public float Ub
        {
            get { return ub; }
            set { SetPropertyValue(value, ref  ub, "Ub"); }
        }

        private float uc ;
        /// <summary>
        /// C相电压
        /// </summary>
        public float Uc
        {
            get { return uc; }
            set { SetPropertyValue(value, ref uc, "Uc"); }
        }

        private float ia;
        /// <summary>
        /// A相电流
        /// </summary>
        public float Ia
        {
            get { return ia; }
            set { SetPropertyValue(value, ref ia, "Ia"); }
        }

        private float ib;
        /// <summary>
        /// B相电流
        /// </summary>
        public float Ib
        {
            get { return ib; }
            set { SetPropertyValue(value, ref  ib, "Ib"); }
        }

        private float ic;
        /// <summary>
        /// C相电流
        /// </summary>
        public float Ic
        {
            get { return ic; }
            set { SetPropertyValue(value, ref  ic, "Ic"); }
        }
        private float phaseUa;
        /// <summary>
        /// Ua相位
        /// </summary>
        public float PhaseUa
        {
            get { return phaseUa; }
            set { SetPropertyValue(value, ref  phaseUa, "PhaseUa"); }
        }

        private float phaseUb ;
        /// <summary>
        /// <summary>
        /// Ub相位
        /// </summary>
        public float PhaseUb
        {
            get { return phaseUb; }
            set { SetPropertyValue(value, ref  phaseUb, "PhaseUb"); }
        }

        private float phaseUc ;
        /// <summary>
        /// Uc相位
        /// </summary>
        public float PhaseUc
        {
            get { return phaseUc; }
            set { SetPropertyValue(value, ref  phaseUc, "PhaseUc"); }
        }

        private float phaseIa;
        /// <summary>
        /// Ia相位
        /// </summary>
        public float PhaseIa
        {
            get { return phaseIa; }
            set { SetPropertyValue(value, ref  phaseIa, "PhaseIa"); }
        }

        private float phaseIb ;
        /// <summary>
        /// Ib相位
        /// </summary>
        public float PhaseIb
        {
            get { return phaseIb; }
            set { SetPropertyValue(value, ref  phaseIb, "PhaseIb"); }
        }

        private float phaseIc ;
        /// <summary>
        /// Ic相位
        /// </summary>
        public float PhaseIc
        {
            get { return phaseIc; }
            set { SetPropertyValue(value, ref  phaseIc, "PhaseIc"); }
        }

        private float phaseA;
        /// <summary>
        /// A相相位
        /// </summary>
        public float PhaseA
        {
            get { return phaseA; }
            set { SetPropertyValue(value, ref  phaseA, "PhaseA"); }
        }

        private float phaseb ;
        /// <summary>
        /// B相相位
        /// </summary>
        public float PhaseB
        {
            get { return phaseb; }
            set { SetPropertyValue(value, ref  phaseb, "PhaseB"); }
        }

        private float phaseC ;
        /// <summary>
        /// C相相位
        /// </summary>
        public float PhaseC
        {
            get { return phaseC; }
            set { SetPropertyValue(value, ref  phaseC, "PhaseC"); }
        }

        private float pf ;
        /// <summary>
        /// 功率因数
        /// </summary>
        public float PF
        {
            get { return pf; }
            set { SetPropertyValue(value, ref  pf, "PF"); }
        }

        private float p ;
        /// <summary>
        /// 功率
        /// </summary>
        public float P
        {
            get { return p; }
            set { SetPropertyValue(value, ref  p, "P"); }
        }

        private float pa ;
        /// A相功率
        /// <summary>
        /// A相功率
        /// </summary>
        public float Pa
        {
            get { return pa; }
            set { SetPropertyValue(value, ref pa, "Pa"); }
        }
        private float pb;
        /// B相功率
        /// <summary>
        /// B相功率
        /// </summary>
        public float Pb
        {
            get { return pb; }
            set { SetPropertyValue(value, ref pb, "Pb"); }
        }
        private float pc;

        public float Pc
        {
            get { return pc; }
            set { SetPropertyValue(value, ref pc, "Pc"); }
        }
        

        private float q ;
        /// <summary>
        /// 无功功率
        /// </summary>
        public float Q
        {
            get { return q; }
            set { SetPropertyValue(value, ref  q, "Q"); }
        }
        private float qa;

        public float Qa
        {
            get { return qa; }
            set { SetPropertyValue(value, ref qa, "Qa"); }
        }
        private float qb;

        public float Qb
        {
            get { return qb; }
            set { SetPropertyValue(value, ref qb, "Qb"); }
        }
        private float qc;

        public float Qc
        {
            get { return qc; }
            set { SetPropertyValue(value, ref qc, "Qc"); }
        }
        
        private float freq;
        /// <summary>
        /// 频率
        /// </summary>
        public float Freq
        {
            get { return freq; }
            set { SetPropertyValue(value, ref  freq, "Freq"); }
        }

        private float energy ;
        /// <summary>
        /// 电量
        /// </summary>
        public float Energy
        {
            get { return energy; }
            set { SetPropertyValue(value, ref  energy, "Energy"); }
        }

        private float energyReactive ;
        /// <summary>
        /// 无功电量
        /// </summary>
        public float EnergyReactive
        {
            get { return energyReactive; }
            set { SetPropertyValue(value, ref  energyReactive, "EnergyReactive"); }
        }

        private float s ;
        /// <summary>
        /// 视在功率
        /// </summary>
        public float S
        {
            get { return s; }
            set { SetPropertyValue(value, ref  s, "S"); }
        }
        private float sa;

        public float Sa
        {
            get { return sa; }
            set { SetPropertyValue(value, ref sa, "Sa"); }
        }
        private float sb;

        public float Sb
        {
            get { return sb; }
            set { SetPropertyValue(value, ref sb, "Sb"); }
        }
        private float sc;

        public float Sc
        {
            get { return sc; }
            set { SetPropertyValue(value, ref sc, "Sc"); }
        }
    }
}
