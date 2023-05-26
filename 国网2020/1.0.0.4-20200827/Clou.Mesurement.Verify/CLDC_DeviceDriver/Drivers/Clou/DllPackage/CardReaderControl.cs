using CLDC_Comm.Enum;
using CLDC_DataCore.Const;
using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou.DllPackage
{
   public class CardReaderControl:ABase
    {
       public override string DisPlayName
        {
            get { return "南网读写卡器接口"; }
        }

       public CardReaderControl(string fileName, string className, bool isLocalAssemly)
            : base(fileName, className, isLocalAssemly)
        {
        }

       public void ShowCardReaderConfig()
       {
          
           if (GlobalUnit.CardDllType == Cus_SouthCardDllType.DotNet平台开发)
           {
               object[] paras = null;
               MethodInvoke("ShowCardReaderConfig", paras);
           }
           else
           {
               switch (GlobalUnit.DeviceManufacturers)
               {
                   case Cus_DeviceManufacturers.格宁:
                       API.CardControlAPI_GeNing.ShowCardReaderConfig();
                       break;
                   default:
                       break;
               }
           }
       }

       /// <summary>
       /// 初始化 连接设备、初始化通信端口
       /// </summary>
       /// <returns></returns>
       public int InitCard()
       {
           int result = 1;
           if (GlobalUnit.CardDllType == Cus_SouthCardDllType.DotNet平台开发)
           {
               object[] paras = null;
               return MethodInvoke("InitCard", paras);
           }
           else
           {
               switch (GlobalUnit.DeviceManufacturers)
               {
                   case Cus_DeviceManufacturers.格宁:
                       result = API.CardControlAPI_GeNing.InitCard();
                       break;
                   default:
                       break;
               }
           }
           return result;
       }

       /// <summary>
       /// 复位读写卡器，完成自身寻卡等
       /// </summary>
       /// <returns></returns>
       public int ResetCard(int meterIndex)
       {
           int result = 1;
           if (GlobalUnit.CardDllType == Cus_SouthCardDllType.DotNet平台开发)
           {
               object[] paras = new object[] { meterIndex };
               result = MethodInvoke("ResetCard", paras);
           }
           else
           {
               switch (GlobalUnit.DeviceManufacturers)
               {
                   case Cus_DeviceManufacturers.格宁:
                       result = API.CardControlAPI_GeNing.ResetCard();
                       break;
                   default:
                       break;
               }
           }
           return result;
       }

       /// <summary>
       /// 启动、停止读写卡器工作
       /// </summary>
       /// <returns></returns>
       public int SwitchCardState(int workFlag)
       {
           int result = 1;
           if (GlobalUnit.CardDllType == Cus_SouthCardDllType.DotNet平台开发)
           {
               object[] paras = new object[] { workFlag };
               result = MethodInvoke("SwitchCardState", paras);
           }
           else
           {
               switch (GlobalUnit.DeviceManufacturers)
               {
                   case Cus_DeviceManufacturers.格宁:
                       result = API.CardControlAPI_GeNing.SwitchCardState(workFlag);
                       break;
                   default:
                       break;
               }
           }
           return result;
       }

       /// <summary>
       /// 发送APDU指令到卡，并返回APDU指令
       /// </summary>
       /// <param name="meterIndex">表位号</param>
       /// <param name="sendFrame">发送报文</param>
       /// <param name="outTime">超时时间</param>
       /// <param name="RecvFrame">返回报文</param>
       /// <returns></returns>
       public int SendToCard(int meterIndex, string sendFrame, int outTime, ref string RecvFrame)
       {
           int result = 1;
           if (GlobalUnit.CardDllType == Cus_SouthCardDllType.DotNet平台开发)
           {
               object[] paras = new object[] { meterIndex, sendFrame, outTime, RecvFrame };
               result = MethodInvoke("SendToCard", paras);
               if (paras[3] is string)
               {
                   RecvFrame = paras[3].ToString();
               }
           }
           else
           {
               switch (GlobalUnit.DeviceManufacturers)
               {
                   case Cus_DeviceManufacturers.格宁:
                       result = API.CardControlAPI_GeNing.SendToCard(meterIndex, sendFrame, outTime, ref RecvFrame);
                       break;
                   default:
                       break;
               }
           }
           return result;
       }
    }
}
