using System;
using System.Collections.Generic;
using System.Text;
using CLDC_Comm.Enum;
using CLDC_Comm.Utils;

namespace CLDC_DeviceDriver.Drivers.Geny
{
    class GenyUtil
    {

        /// <summary>
        /// 根据 测量方式 和方向 计算 geny标准表 测量方式
        /// </summary>
        /// <param name="clfs"></param>
        /// <param name="glfx"></param>
        /// <returns></returns>
        public static Geny_StdK6DTestType GetStdTestType(Cus_Clfs clfs, Cus_PowerFangXiang glfx)
        {
            Geny_StdK6DTestType k6dTestType = Geny_StdK6DTestType.三相三线_无功;

            if (clfs == Cus_Clfs.单相 || clfs == Cus_Clfs.三相四线)
            {
                if (glfx.ToString().IndexOf("有功", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    k6dTestType = Geny_StdK6DTestType.三相四线_有功;
                }
                if (glfx.ToString().IndexOf("无功", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    k6dTestType = Geny_StdK6DTestType.三相四线_无功;
                }
            }
            else if (clfs == Cus_Clfs.三相三线)
            {
                if (glfx.ToString().IndexOf("有功", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    k6dTestType = Geny_StdK6DTestType.三相三线_有功;
                }
                if (glfx.ToString().IndexOf("无功", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    k6dTestType = Geny_StdK6DTestType.三相三线_无功;
                }
            }
            else
            {
                k6dTestType = Geny_StdK6DTestType.三相四线_有功;
            }

            return k6dTestType;
        }



        /// <summary>
        /// 根据电流值，计算 格林标准表的档位
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static GenyStdMeterCurrentLevel GetCurrentLevel(params float[] values)
        {
            double max = ArrayHelper.Max(values);
            GenyStdMeterCurrentLevel level = GenyStdMeterCurrentLevel.Level_1A;
            if (max <= 1)
            {
                level = GenyStdMeterCurrentLevel.Level_1A;
            }
            else if (max <= 10)
            {
                level = GenyStdMeterCurrentLevel.Level_10A;
            }
            else if (max <= 100)
            {
                level = GenyStdMeterCurrentLevel.Level_100A;
            }
            else
            {
                level = GenyStdMeterCurrentLevel.Level_100A;
            }
            return level;
        }


    }
}
