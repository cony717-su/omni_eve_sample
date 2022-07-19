using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Shiftup.CommonLib.Logger;

using InnerDevTool.Data.Game;

namespace InnerDevTool
{
    public static class Util
    {
        private static string[] attributeNames = { UNKNOWN_TYPE, "���Ӽ�", "�ҼӼ�", "��Ӽ�", "���Ӽ�", "�ϼӼ�" };
        private static string[] roleNames = { UNKNOWN_TYPE, "������", "������", "ġ����", "������", "������", "����ġ", "��ȭ���", "�Ѱ赹��" };
        private static Dictionary<int, string> itemCategoryNames = new Dictionary<int, string>() {
                                                        {1, "����"},
                                                        {2, "��"},
                                                        {3, "�Ǽ��縮"},
                                                        {4, "���"},
                                                        {5, "��������"},
                                                        {6, "Ƽ��"},
                                                        {8, "�ҿ�ī��Ÿ"},
                                                        {11, "��������"},
                                                        {12, "������"},
                                                        {13, "�Ǽ��縮����" },
                                                        {18, "�ҿ��ʷο�" },
                                                        {101001, "����" },
                                                        {101011, "�������" },
                                                        {101021, "�������ġ" },
                                                    };

        public static readonly string UNKNOWN_TYPE = "Unknown";

        public static string DateTimeToString(DateTime datetime)
        {
            return datetime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static string DateTimeToString(DateTime? datetime)
        {
            if (datetime == null)
            {
                return "0000-00-00 00:00:00";
            }

            return datetime.Value.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static string GetCharacterAttributeName(int attribute)
        {
            if (attributeNames.Length <= attribute)
            {
                return UNKNOWN_TYPE;
            }

            return attributeNames[attribute];
        }

        public static string GetCharacterRoleName(int role)
        {
            if (roleNames.Length <= role)
            {
                return UNKNOWN_TYPE;
            }

            return roleNames[role];
        }

        public static string GetItemCategoryNames(int category)
        {
            if (!itemCategoryNames.ContainsKey(category))
            {
                return UNKNOWN_TYPE;
            }

            return itemCategoryNames[category];
        }

        public static string GetShardingName(string tableName, ulong nfguid)
        {
            int shardKey = Convert.ToInt32(nfguid % 32);
            return String.Format("{0}_{1}", tableName, shardKey);
        }

        public static DateTime Now()
        {
            return DateTime.Now;
        }

        public static int ParseInt(string str, int defaultValue = 0)
        {
            try
            {
                int result;
                if (!Int32.TryParse(str, out result))
                {
                    return defaultValue;
                }

                return result;
            }
            catch (Exception e)
            {
                Log.Warning(e.ToString());
            }

            return defaultValue;
        }

        public static int ParseInt(object obj)
        {
            try
            {
                return Convert.ToInt32(obj.ToString());
            }
            catch (Exception e)
            {
                Log.Warning(e.ToString());
            }

            return 0;
        }

        public static long ParseLong(string str)
        {
            try
            {
                long result;
                if (!Int64.TryParse(str, out result))
                {
                    return 0;
                }

                return result;
            }
            catch (Exception e)
            {
                Log.Warning(e.ToString());
            }

            return (long)0;
        }

        public static double ParseDouble(string str)
        {
            try
            {
                double result;
                if (!Double.TryParse(str, out result))
                {
                    return 0;
                }

                return result;
            }
            catch (Exception e)
            {
                Log.Warning(e.ToString());
            }

            return (double)0;
        }

        public static DateTime? StrToDateTime(string str)
        {
            if (str == "0000-00-00 00:00:00")
            {
                return null;
            }

            return Convert.ToDateTime(str);
        }
    }
}