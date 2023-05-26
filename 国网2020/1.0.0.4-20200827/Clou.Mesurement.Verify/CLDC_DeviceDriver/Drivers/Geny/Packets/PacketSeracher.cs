using System;
using System.Collections.Generic;
using System.Text;
using CLDC_DeviceDriver.Drivers.Geny.Packets.Out;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets
{
    class TypeCompare : IComparer<Type>
    {

        #region IComparer<Type> 成员

        public int Compare(Type x, Type y)
        {
            return x.Name.CompareTo(y.Name);
        }

        #endregion
    }

    /// <summary>
    /// 提供 搜索 所有 格林请求数据包的方法
    /// </summary>
    public class PacketSeracher
    {

        /// <summary>
        /// 列出所有格林请的数据包
        /// </summary>
        /// <returns></returns>
        public static List<Type> GetAllOutPacketType()
        {
            List<Type> types = new List<Type>();
            Type[] ts = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();

            foreach (Type t in ts)
            {
                if (t.BaseType != null && t.BaseType == typeof(GenySendPacket))
                {
                    types.Add(t);
                }
            }
            types.Sort(new TypeCompare());
            return types;
        }
    }
}