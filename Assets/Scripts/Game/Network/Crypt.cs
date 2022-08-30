using System;
using System.Linq;
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
            return size;
        }
        
        /*------ Base64 Decoding Table ------*/
        static readonly int[] DecodeMimeBase64 = {
            -1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,  /* 00-0F */
            -1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,  /* 10-1F */
            -1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,62,-1,-1,-1,63,  /* 20-2F */
            52,53,54,55,56,57,58,59,60,61,-1,-1,-1,-1,-1,-1,  /* 30-3F */
            -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9,10,11,12,13,14,  /* 40-4F */
            15,16,17,18,19,20,21,22,23,24,25,-1,-1,-1,-1,-1,  /* 50-5F */
            -1,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,  /* 60-6F */
            41,42,43,44,45,46,47,48,49,50,51,-1,-1,-1,-1,-1,  /* 70-7F */
            -1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,  /* 80-8F */
            -1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,  /* 90-9F */
            -1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,  /* A0-AF */
            -1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,  /* B0-BF */
            -1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,  /* C0-CF */
            -1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,  /* D0-DF */
            -1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,  /* E0-EF */
            -1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1   /* F0-FF */
        };
        
        public static string DecodeBase64WithCrypt(string c_szSrc, string c_szKey)
        {
            if (c_szSrc.Length == 0 || c_szKey.Length == 0)
            {
                return "";
            }
            
            s_strResult1 = "";
            s_strResult2 = "";

            int size = c_szSrc.Length;
            byte[] description = null;
            size = base64_decode(c_szSrc.ToCharArray(), out description, size * 10);

            int keyLength = c_szKey.Length;
            byte[] result = new byte[size]; 
            for (int i = 0; i < size; ++i)
            {
                result[i] = (byte)(description[i] ^ c_szKey[i % keyLength]);
            }

            s_strResult2 = Encoding.Default.GetString(result);

            result[5] = (byte)'\0';
            return s_strResult2;
        }

        static int base64_decode(char[] text, out byte[]dst, int numBytes)
        {
            int space_idx = 0, phase;
            int d, prev_d = 0;
            byte c;
            space_idx = 0;
            phase = 0;

            dst = new byte[numBytes];

            for (int i = 0; i < text.Length; i++) {
                d = DecodeMimeBase64[(int)(text[i])];
                if ( d != -1 ) {
                    switch ( phase ) {
                        case 0:
                            ++phase;
                            break;
                        case 1:
                            c = (byte)( ( prev_d << 2 ) | ( ( d & 0x30 ) >> 4 ) );
                            if (space_idx < numBytes)
                            {
                                dst[space_idx++] = c;
                            }
                            ++phase;
                            break;
                        case 2:
                            c = (byte)( ( ( prev_d & 0xf ) << 4 ) | ( ( d & 0x3c ) >> 2 ) );
                            if (space_idx < numBytes)
                            {
                                dst[space_idx++] = c;
                            }
                            ++phase;
                            break;
                        case 3:
                            c = (byte)( ( ( prev_d & 0x03 ) << 6 ) | d );
                            if (space_idx < numBytes)
                            {
                                dst[space_idx++] = c;
                            }
                            phase = 0;
                            break;
                    }
                    prev_d = d;
                }
            }
            return space_idx;
        }
    }
    
}