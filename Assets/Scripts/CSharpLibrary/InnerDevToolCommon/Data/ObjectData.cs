using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Shiftup.CommonLib.Logger;

namespace InnerDevToolCommon.Data
{
    public class ObjectData
    {
        public virtual void SetData(IDictionary<string, Object> data, bool ignoreNotExist = true)
        {
            foreach (var kvp in data)
            {
                try
                {
                    var pi = GetType().GetProperty(kvp.Key);
                    if (pi == null)
                        continue;

                    if (pi.PropertyType == typeof(bool))
                    {
                        int v = Convert.ToInt32(kvp.Value);
                        pi.SetValue(this, (v != 0) ? true : false);
                    }
                    else if (pi.PropertyType == typeof(byte))
                    {
                        if (kvp.Value == System.DBNull.Value)
                        {
                            pi.SetValue(this, 0);
                        }
                        else
                        {
                            pi.SetValue(this, Convert.ToByte(kvp.Value));
                        }
                    }
                    else if (pi.PropertyType == typeof(double))
                    {
                        if (kvp.Value == System.DBNull.Value)
                        {
                            pi.SetValue(this, 0.0);
                        }
                        else
                        {
                            pi.SetValue(this, Convert.ToDouble(kvp.Value));
                        }
                    }
                    else if (pi.PropertyType == typeof(int))
                    {
                        if (kvp.Value == System.DBNull.Value)
                        {
                            pi.SetValue(this, 0);
                        }
                        else
                        {
                            pi.SetValue(this, Convert.ToInt32(kvp.Value));
                        }
                    }
                    else if (pi.PropertyType == typeof(uint))
                    {
                        if (kvp.Value == System.DBNull.Value)
                        {
                            pi.SetValue(this, (uint)0);
                        }
                        else
                        {
                            pi.SetValue(this, Convert.ToUInt32(kvp.Value));
                        }
                    }
                    else if (pi.PropertyType == typeof(UInt64))
                    {
                        if (kvp.Value == System.DBNull.Value)
                        {
                            pi.SetValue(this, (UInt64)0);
                        }
                        else
                        {
                            pi.SetValue(this, Convert.ToUInt64(kvp.Value));
                        }
                    }
                    else if (pi.PropertyType == typeof(long))
                    {
                        if (kvp.Value == System.DBNull.Value)
                        {
                            pi.SetValue(this, (long)0);
                        }
                        else
                        {
                            pi.SetValue(this, Convert.ToInt64(kvp.Value));
                        }
                    }
                    else if (pi.PropertyType == typeof(ulong))
                    {
                        if (kvp.Value == System.DBNull.Value)
                        {
                            pi.SetValue(this, (ulong)0);
                        }
                        else
                        {
                            pi.SetValue(this, Convert.ToUInt64(kvp.Value));
                        }
                    }
                    else if (pi.PropertyType == typeof(DateTime))
                    {
                        pi.SetValue(this, Convert.ToDateTime(kvp.Value));
                    }
                    else if (pi.PropertyType == typeof(DateTime?))
                    {
                        DateTime value;
                        if (kvp.Value != null && DateTime.TryParse(kvp.Value.ToString(), out value))
                        {
                            pi.SetValue(this, value);
                        }
                        else
                        {
                            pi.SetValue(this, null);
                        }
                    }
                    else
                    {
                        pi.SetValue(this, kvp.Value);
                    }
                }
                catch (Exception e)
                {
                    if (ignoreNotExist == false)
                    {
                        Console.WriteLine(e.Message);
                        Console.WriteLine("Key {0}, Value {1}", kvp.Key, kvp.Value.ToString());
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine(e.Message);
                    }
                }
            }
        }
    }
}