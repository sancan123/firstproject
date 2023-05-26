using CLDC_Comm.Enum;
using CLDC_DataCore.Const;
using CLDC_Encryption.CLEncryption.Interface;

namespace CLDC_Encryption.CLEncryption
{
    public class EncryptionFactory
    {
        /// <summary>
        /// 创建一个费控项目控制器工厂
        /// </summary>
        /// <param name="VerifyType">多功能试验类型</param>
        /// <returns></returns>
        public IAmMeterEncryption CreateEncryptionControler(Cus_EncryptionMachineType EncryptionType)
        {
            IAmMeterEncryption curControler = null;
            switch (EncryptionType)
            {
                case Cus_EncryptionMachineType.南网加密机:
                    {
                        curControler = new SouthGridEncryption();
                        break;
                    }
                default:
                    {
                        curControler = new SouthGridEncryption();
                        break;
                    }
            }
            return curControler;
        }

        public IAmMeterEncryption CreateEncryptionControler()
        {
            IAmMeterEncryption curControler = null;
            string EncryptionType = GlobalUnit.ENCRYPTION_MACHINE_TYPE;
            
            if (EncryptionType == Cus_EncryptionMachineType.南网加密机.ToString())
            {
                curControler = new SouthGridEncryption();
            }
            else
            {
                curControler = new SouthGridEncryption();
            }
            return curControler;
        }
    }
}
