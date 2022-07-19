using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
//using System.Windows.Forms;

using Shiftup.CommonLib.Logger;

namespace Shiftup.CommonLib
{
    public static class TypeHelper
    {
        public static bool IsSubsetOf<T>(this IEnumerable<T> a, IEnumerable<T> b)
        {
            return !a.Except(b).Any();
        }

        public static bool EqualAllProperty<T>(T a, T b)
        {
            Type t = typeof(T);

            foreach (PropertyInfo pi in t.GetProperties())
            {
                var property_a = pi.GetValue(a, null);
                var property_b = pi.GetValue(b, null);

                if (property_a.Equals(property_b) == false)
                    return false;
            }
            return true;
        }

        public static object ToConvert(this string str, FieldInfo fi)
        {
            return str.ToConvert(Type.GetTypeCode(fi.FieldType));
        }

        public static object ToConvert(this string str, TypeCode code)
        {
            if (String.IsNullOrWhiteSpace(str) == true)
            {
                switch (code)
                {
                    case TypeCode.Byte:
                        return (Byte)0;

                    case TypeCode.SByte:
                        return (SByte)0;

                    case TypeCode.UInt16:
                        return (UInt16)0;

                    case TypeCode.UInt32:
                        return (UInt32)0;

                    case TypeCode.UInt64:
                        return (UInt64)0;

                    case TypeCode.Int16:
                        return (Int16)0;

                    case TypeCode.Int32:
                        return (Int32)0;

                    case TypeCode.Int64:
                        return (Int64)0;

                    case TypeCode.Decimal:
                        return (Decimal)0;

                    case TypeCode.Double:
                        return (Double)0;

                    case TypeCode.Single:
                        return (Single)0;

                    case TypeCode.String:
                        return String.Empty;

                    default:
                        Log.Warning("데이터 타입({0})을 숫자로 변환할 수 없습니다.", code.ToString());
                        return str;
                }
            }
            else
            {
                switch (code)
                {
                    case TypeCode.Byte:
                        Byte resultByte;
                        if (Byte.TryParse(str, out resultByte))
                        {
                            return resultByte;
                        }
                        else
                        {
                            Log.Warning("데이터 타입({0})을 숫자로 변환할 수 없습니다.", code.ToString());
                            return 0;
                        }
                    //return Convert.ToByte(str);

                    case TypeCode.SByte:
                        SByte resultSByte;
                        if (SByte.TryParse(str, out resultSByte))
                        {
                            return resultSByte;
                        }
                        else
                        {
                            Log.Warning("데이터 타입({0})을 숫자로 변환할 수 없습니다.", code.ToString());
                            return 0;
                        }
                    //return Convert.ToSByte(str);

                    case TypeCode.UInt16:
                        UInt16 resultUInt16;
                        if (UInt16.TryParse(str, out resultUInt16))
                        {
                            return resultUInt16;
                        }
                        else
                        {
                            Log.Warning("데이터 타입({0})을 숫자로 변환할 수 없습니다.", code.ToString());
                            return 0;
                        }
                    //return Convert.ToUInt16(str);

                    case TypeCode.UInt32:
                        UInt32 resultUInt32;
                        if (UInt32.TryParse(str, out resultUInt32))
                        {
                            return resultUInt32;
                        }
                        else
                        {
                            Log.Warning("데이터 타입({0})을 숫자로 변환할 수 없습니다.", code.ToString());
                            return 0;
                        }
                    //return Convert.ToUInt32(str);

                    case TypeCode.UInt64:
                        UInt64 resultUInt64;
                        if (UInt64.TryParse(str, out resultUInt64))
                        {
                            return resultUInt64;
                        }
                        else
                        {
                            Log.Warning("데이터 타입({0})을 숫자로 변환할 수 없습니다.", code.ToString());
                            return 0;
                        }
                    //return Convert.ToUInt64(str);

                    case TypeCode.Int16:
                        Int16 resultInt16;
                        if (Int16.TryParse(str, out resultInt16))
                        {
                            return resultInt16;
                        }
                        else
                        {
                            Log.Warning("데이터 타입({0})을 숫자로 변환할 수 없습니다.", code.ToString());
                            return 0;
                        }
                    //return Convert.ToInt16(str);

                    case TypeCode.Int32:
                        Int32 resultInt32;
                        if (Int32.TryParse(str, out resultInt32))
                        {
                            return resultInt32;
                        }
                        else
                        {
                            Log.Warning("데이터 타입({0})을 숫자로 변환할 수 없습니다.", code.ToString());
                            return 0;
                        }
                    //return Convert.ToInt32(str);

                    case TypeCode.Int64:
                        Int64 resultInt64;
                        if (Int64.TryParse(str, out resultInt64))
                        {
                            return resultInt64;
                        }
                        else
                        {
                            Log.Warning("데이터 타입({0})을 숫자로 변환할 수 없습니다.", code.ToString());
                            return 0;
                        }
                    //return Convert.ToInt64(str);

                    case TypeCode.Decimal:
                        Decimal resultDecimal;
                        if (Decimal.TryParse(str, out resultDecimal))
                        {
                            return resultDecimal;
                        }
                        else
                        {
                            Log.Warning("데이터 타입({0})을 숫자로 변환할 수 없습니다.", code.ToString());
                            return 0;
                        }
                    //return Convert.ToDecimal(str);

                    case TypeCode.Double:
                        Double resultDouble;
                        if (Double.TryParse(str, out resultDouble))
                        {
                            return resultDouble;
                        }
                        else
                        {
                            Log.Warning("데이터 타입({0})을 숫자로 변환할 수 없습니다.", code.ToString());
                            return 0;
                        }
                    //return Convert.ToDouble(str);

                    case TypeCode.Single:
                        Single resultSingle;
                        if (Single.TryParse(str, out resultSingle))
                        {
                            return resultSingle;
                        }
                        else
                        {
                            Log.Warning("데이터 타입({0})을 숫자로 변환할 수 없습니다.", code.ToString());
                            return 0;
                        }
                    //return Convert.ToSingle(str);

                    case TypeCode.String:
                        return str;

                    default:
                        Log.Warning("데이터 타입({0})을 숫자로 변환할 수 없습니다.", code.ToString());
                        return str;
                }
            }
        }

        public static bool IsNumericType(this Type t)
        {
            switch (Type.GetTypeCode(t))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        public static IEnumerable<string> EnumsToStrings<T>()
        {
            return typeof(T).EnumsToStrings();
        }

        public static IEnumerable<string> EnumsToStrings(this Type t)
        {
            return Enum.GetValues(t).Cast<object>().Select(r => r.ToString());
        }

        /*public static void EnumsToComboBox<T>(this ComboBox comboBox, T defaultValue)
        {
            comboBox.EnumsToComboBox<T>(defaultValue, Enumerable.Empty<T>());
        }

        public static void EnumsToComboBox<T>(this ComboBox comboBox, T defaultValue, IEnumerable<T> excludes)
        {
            comboBox.Items.Clear();
            comboBox.Items.AddRange(EnumsToStrings<T>().Except(excludes.Select(e => e.ToString())).ToArray());
            comboBox.SelectedIndex = Convert.ToInt32(defaultValue);
        }*/
        private static DateTime epochBaseDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        private static DateTimeOffset epochBaseDateTimeOffset = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
        /// <summary>
        /// Converts the given date value to epoch time.
        /// </summary>
        public static long ToEpochTime(this DateTime dateTime)
        {
            var date = dateTime.ToUniversalTime();
            var ticks = date.Ticks - epochBaseDateTime.Ticks;
            var ts = ticks / TimeSpan.TicksPerSecond;
            return ts;
        }

        /// <summary>
        /// Converts the given date value to epoch time.
        /// </summary>
        /*
        public static long ToEpochTime(this DateTimeOffset dateTime)
        {
            var date = dateTime.ToUniversalTime();
            var ticks = date.Ticks - epochBaseDateTimeOffset.Ticks;
            var ts = ticks / TimeSpan.TicksPerSecond;
            return ts;
        }*/

        /// <summary>
        /// Converts the given epoch time to a <see cref="DateTime"/> with <see cref="DateTimeKind.Utc"/> kind.
        /// </summary>
        public static DateTime ToDateTimeFromEpoch(this long intDate)
        {
            var timeInTicks = intDate * TimeSpan.TicksPerSecond;
            return TimeZoneInfo.ConvertTime(epochBaseDateTime.AddTicks(timeInTicks), TimeZoneInfo.Utc, TimeZoneInfo.Local);
        }

        /// <summary>
        /// Converts the given epoch time to a UTC <see cref="DateTimeOffset"/>.
        /// </summary>
        /*
        public static DateTimeOffset ToDateTimeOffsetFromEpoch(this long intDate)
        {
            var timeInTicks = intDate * TimeSpan.TicksPerSecond;

            return TimeZoneInfo.ConvertTime(epochBaseDateTimeOffset.AddTicks(timeInTicks), currentTZI);
        }
        */
    }
}
;