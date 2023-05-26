using System;
using System.Collections.Generic;
using System.Text;

namespace MeterProtocol.Packet.EDMI
{
    public enum EDMICmdCode
    {
        CST_BYT_ACK = 0x06,                   //<ACK>
        CST_BYT_CAN = 0x18,                   //<CAN>
        //CST_BYT_STX = 0x02,                   //<STX>
        // CST_BYT_ETX = 0x03,                   //<ETX>
        CST_BYT_EXE = 0x45,                   //<E>扩展指令“E”  ascll=&H45
        CST_BYT_READ = 0x52,                  //<R>指令“R”      ascll=&H52
        CST_BYT_WRITE = 0x57,                 //<W>命令
        CST_BYT_LOGON = 0x4c,                 //<L>命令
        CST_BYT_EXIT = 0x58,                  //<X>
        CST_BYT_INFO = 0x49,                  //<I>

    }
}
