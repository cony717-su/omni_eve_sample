using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Network
{
    public static class Crypt
    {
        static string s_strResult1 = "";
        static string s_strResult2 = "";

        /*------ Base64 Encoding Table ------*/
        static readonly char[] MimeBase64 = {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H',
            'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P',
            'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X',
            'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f',
            'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n',
            'o', 'p', 'q', 'r', 's', 't', 'u', 'v',
            'w', 'x', 'y', 'z', '0', '1', '2', '3',
            '4', '5', '6', '7', '8', '9', '+', '/'
        };
        
        public static string EncryptWithBase64(string c_szSrc, string c_szKey)
        {
            if (c_szSrc.Length == 0 || c_szKey.Length == 0)
            {
                return "";
            }
            
            s_strResult1 = c_szSrc;
            s_strResult2 = "";

            byte[] encryption = Encoding.UTF8.GetBytes(c_szSrc);
            int keyLength = c_szKey.Length;
            for (int i = 0; i < c_szSrc.Length; ++i)
            {
                encryption[i] = (byte)(c_szSrc[i] ^ c_szKey[i % keyLength]);
            }

            base64_encode(encryption, c_szSrc.Length, out s_strResult2);
            return s_strResult2;
        }

        private static int base64_encode(byte[] text, int numBytes, out string pEncodedText)
        {
            byte[] input = {0, 0, 0};
            byte[] output = {0, 0, 0, 0};

            int size = (4 * (numBytes / 3)) + (numBytes % 3 != 0 ? 4 : 0) + 1;

            DebugManager.Log($"size: {size}, numBytes: {numBytes}");
            pEncodedText = "";
            for (int i = 0; i < numBytes; i++)
            {
                int index = i % 3;
                input[index] = text[i];
                
                if (index == 2 || i == numBytes - 1)
                {
                    output[0] = (byte)((input[0] & 0xFC) >> 2);
                    output[1] = (byte)(((input[0] & 0x3) << 4) | ((input[1] & 0xF0) >> 4));
                    output[2] = (byte)(((input[1] & 0xF) << 2) | ((input[2] & 0xC0) >> 6));
                    output[3] = (byte)(input[2] & 0x3F);
                    
                    pEncodedText += MimeBase64[output[0]];
                    pEncodedText += MimeBase64[output[1]];
                    pEncodedText += index == 0? '=' : MimeBase64[output[2]];
                    pEncodedText += index <  2? '=' : MimeBase64[output[3]];
                    
                    input[0] = input[1] = input[2] = 0;
                }
            }

            for (int i = 0; i < size - numBytes; i++)
            {
                //pEncodedText += " ";
            }
            return size;
        }
    }
    
}