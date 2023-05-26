using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace CLDC_Comm
{
    #region 类：序列化数据基类
    /// <summary>
    /// 类：序列化数据基类
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    [Serializable()]
    public class SerializationBytes
    {
         
        public SerializationBytes()
        {
        }

        #region 获取对象拷贝
        /// <summary>
        /// 获取对象拷贝
        /// </summary>
        /// <returns></returns>
        public SerializationBytes Copy()
        {
            
            return this;
        }
        #endregion

        #region 序列化、反序列化
        /// <summary>
        /// 序列化
        /// </summary>
        public byte[] GetBytes()
        {
            byte[] binaryDataResult = null;
            MemoryStream memStream = new MemoryStream();
            IFormatter brFormatter = new BinaryFormatter();

            brFormatter.Serialize(memStream, this);
            binaryDataResult = memStream.ToArray();
            memStream.Close();
            memStream.Dispose();
            return binaryDataResult;
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="byPacket"></param>
        /// <returns></returns>
        public static SerializationBytes GetObject(byte[] byPacket)
        {
            MemoryStream memStream = new MemoryStream(byPacket);
            IFormatter brFormatter = new BinaryFormatter();
            SerializationBytes obj;
            try
            {
                obj = (SerializationBytes)brFormatter.Deserialize(memStream);
            }
            catch
            {
                throw new InvalidCastException("序列化对象结构发生变化，请删除缓存文件后再操作");
            }
            memStream.Close();
            memStream.Dispose();
            return obj;
        }
        #endregion

        #region 保存到文件
        /// <summary>
        /// 保存到文件
        /// </summary>
        /// <param name="FileName">文件绝对路径</param>
        /// <returns></returns>
        public bool SaveToFile(string FileName)
        {
            FileStream tmpFs = null;
            try
            {
                tmpFs = File.Open(FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            }
            catch {
                return false;
            }
            byte[] byWrite = this.GetBytes();
            tmpFs.Position = 0;
            tmpFs.Write(byWrite, 0, byWrite.Length);
            tmpFs.Flush();
            tmpFs.Close();
            tmpFs.Dispose();
            return true ;
        }
        #endregion

        #region 从文件读取
        /// <summary>
        /// 从文件读取
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns>失败返回 null</returns>
        public static SerializationBytes ReadFromFile(string FileName)
        {
            if (!File.Exists(FileName)) return null;
            FileStream tmpFs = null;
            try
            {
                tmpFs = File.Open(FileName, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch
            {
                return null;
            }
            byte[] byRead = new byte[tmpFs.Length];
            tmpFs.Read(byRead, 0, byRead.Length);
            tmpFs.Flush();
            tmpFs.Close();
            tmpFs.Dispose();
            try
            {
                return GetObject(byRead);
            }
            catch { }
            return null;
        }
        #endregion

    }

    #endregion
}
