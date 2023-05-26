
using System;
using CLDC_DataCore;
using CLDC_DataCore.Const;
using CLDC_DataCore.Struct;
using CLDC_Comm.Enum;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using System.Threading;


namespace CLDC_VerifyAdapter
{

    class PreHeat : VerifyBase
    {
        #region----------���캯��----------
        public PreHeat(object plan) : base(plan) { }
        #endregion

        #region----------�춨����----------
        /// <summary>
        /// ��ʼԤ�ȼ춨,Ԥ�Ȳ���Ҫ�춨����
        /// </summary>
        public override void Verify()
        {
            base.Verify();
            PowerOn();

            string[] verPlan = VerifyPara.Split('|');
            if (verPlan[0] != "0")
            {
                int TIME = int.Parse(verPlan[0]) * 60;
                MessageController.Instance.AddMessage("��ʼԤ�ȣ���ȴ�" + TIME + "��");
                if (Stop) return;

                ShowWaitMessage("���ڵȴ�{0}��,���Ժ�....", 1000 * TIME);

            }
            Helper.EquipHelper.Instance.PowerOff();


        }
        #endregion

        #region ��ʼ���豸����
        /// <summary>
        /// ��ʼ���豸����
        /// </summary>
        /// <returns></returns>
        private bool InitEquipment()
        {

            return true;
        }
        #endregion

        #region ����Դ���
        /// <summary>
        /// ����Դ���
        /// </summary>
        /// <returns>��Դ���</returns>
        //protected override bool PowerOn()
        //{
        //    int firstYaoJianMeterIndex = Helper.MeterDataHelper.Instance.FirstYaoJianMeter;
        //    MeterBasicInfo firstYaoJianMeter = Helper.MeterDataHelper.Instance.Meter(firstYaoJianMeterIndex);
        //    float powerOutI = CLDC_DataCore.Function.Number.GetCurrentByIb(CurPlan.xIb, firstYaoJianMeter.Mb_chrIb);
        //    Cus_PowerYuanJian ele = Cus_PowerYuanJian.H;
        //    //����ǵ��ֻ࣬���AԪ
        //    if (GlobalUnit.IsDan) ele = Cus_PowerYuanJian.A;
        //    MessageController.Instance.AddMessage(string.Format("��ʼ���ƹ���Դ���:{0}V {1}A", GlobalUnit.U, powerOutI));
        //    bool ret = Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U,
        //                          powerOutI,
        //                          (int)ele, (int)CurPlan.PowerFangXiang,
        //                          FangXiangStr + "1.0",
        //                          IsYouGong, false);
        //    MessageController.Instance.AddMessage(string.Format("���ƹ���Դ���:{0}V {1}A {2}", GlobalUnit.U, powerOutI, ret ? "�ɹ�" : "ʧ��"));
        //    return ret;
        //}

        #endregion

        

        #region -----------������д---------
        /// <summary>
        /// ��ǰ��Ŀ���ݽڵ��ֵ��Ԥ�Ȳ���Ҫ�������ݣ���������Ϊ��
        /// </summary>
        protected override string ItemKey
        {
            get { return string.Empty; }
        }

        /// <summary>
        /// ��ǰ��Ŀ�ܽ��ۼ�ֵ��Ԥ�Ȳ���Ҫ�������ݣ���������Ϊ��
        /// </summary>
        protected override string ResultKey
        {
            get { return string.Empty; }
        }
        #endregion
    }
}
