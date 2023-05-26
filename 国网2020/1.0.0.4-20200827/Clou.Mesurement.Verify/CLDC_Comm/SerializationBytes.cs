using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace CLDC_Comm
{
    #region �ࣺ���л����ݻ���
    /// <summary>
    /// �ࣺ���л����ݻ���
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    [Serializable()]
    public class SerializationBytes
    {
         
        public SerializationBytes()
        {
        }

        #region ��ȡ���󿽱�
        /// <summary>
        /// ��ȡ���󿽱�
        /// </summary>
        /// <returns></returns>
        public SerializationBytes Copy()
        {
            
            return this;
        }
        #endregion

        #region ���л��������л�
        /// <summary>
        /// ���л�
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
        /// �����л�
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
                throw new InvalidCastException("���л�����ṹ�����仯����ɾ�������ļ����ٲ���");
            }
            memStream.Close();
            memStream.Dispose();
            return obj;
        }
        #endregion

        #region ���浽�ļ�
        /// <summary>
        /// ���浽�ļ�
        /// </summary>
        /// <param name="FileName">�ļ�����·��</param>
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

        #region ���ļ���ȡ
        /// <summary>
        /// ���ļ���ȡ
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns>ʧ�ܷ��� null</returns>
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
