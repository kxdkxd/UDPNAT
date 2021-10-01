using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace UDPNATCLIENT
{
    public class FileUtils
    {

        public static byte[] GetFileData(string fileName, long startPosition, int length)
        {
            byte[] data;
            using (FileStream fileRead = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                data = new byte[length];
                fileRead.Seek(startPosition, SeekOrigin.Begin);
                fileRead.Read(data, 0, length);
                fileRead.Close();
                fileRead.Dispose();
            }
            return data;
        }

        public static long GetFileSize(string sFullName)
        {
            long lSize = 0;
            if (File.Exists(sFullName))
                lSize = new FileInfo(sFullName).Length;
            return lSize;
        }

        public static void write2File(string filePathName, byte[] bytes)

        {
            if (File.Exists(filePathName))
            {
                File.Delete(filePathName);//直接覆盖文件
            }
            FileStream stream = new FileStream(filePathName, FileMode.Create);
            stream.Write(bytes, 0, bytes.Length);
            stream.Flush();
            stream.Close();
        }

        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }
    }
    public class Crc16
    {
        const ushort polynomial = 0xA001;
        static readonly ushort[] table = new ushort[256];

        public static ushort ComputeChecksum(byte[] bytes)
        {
            ushort crc = 0;
            for (int i = 0; i < bytes.Length; ++i)
            {
                byte index = (byte)(crc ^ bytes[i]);
                crc = (ushort)((crc >> 8) ^ table[index]);
            }
            return crc;
        }

        static Crc16()
        {
            ushort value;
            ushort temp;
            for (ushort i = 0; i < table.Length; ++i)
            {
                value = 0;
                temp = i;
                for (byte j = 0; j < 8; ++j)
                {
                    if (((value ^ temp) & 0x0001) != 0)
                    {
                        value = (ushort)((value >> 1) ^ polynomial);
                    }
                    else
                    {
                        value >>= 1;
                    }
                    temp >>= 1;
                }
                table[i] = value;
            }
        }
    }
}
