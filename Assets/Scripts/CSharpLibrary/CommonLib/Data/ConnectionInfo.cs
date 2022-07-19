using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Shiftup.CommonLib
{
    public abstract class ConnectionInfo
    {
        public abstract string GetInfo();
        private const char splitter = '|';
        static private byte[] secureKey = null;
        static ConnectionInfo()
        {
            SetSecureKey("rmkey_@#");
        }
        static public void SetSecureKey(string key)
        {
            secureKey = ASCIIEncoding.ASCII.GetBytes(key);
        }
        static public string[] DecryptData(string src)
        {
            DESCryptoServiceProvider rc2 = new DESCryptoServiceProvider();
            rc2.Key = secureKey;
            rc2.IV = secureKey;

            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, rc2.CreateDecryptor(), CryptoStreamMode.Write);

            byte[] data = Convert.FromBase64String(src);

            cs.Write(data, 0, data.Length);
            cs.FlushFinalBlock();

            var payload = Encoding.UTF8.GetString(ms.GetBuffer());

            var buffers = payload.Split(new char[] { splitter }, 2);
            int len = Convert.ToInt32(buffers[0]);

            return buffers[1].Substring(0, len).Split(splitter);
        }
        static public string EncryptData(string[] src)
        {
            string merged = String.Join(splitter.ToString(), src);
            int len = merged.Length;

            string buffer = String.Format("{0}|{1}", len, merged);

            DESCryptoServiceProvider rc2 = new DESCryptoServiceProvider();
            rc2.Key = secureKey;
            rc2.IV = secureKey;

            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, rc2.CreateEncryptor(), CryptoStreamMode.Write);

            byte[] data = Encoding.UTF8.GetBytes(buffer.ToCharArray());

            cs.Write(data, 0, data.Length);
            cs.FlushFinalBlock();

            return Convert.ToBase64String(ms.ToArray());
        }
    }
}
