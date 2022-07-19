using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Shiftup.CommonLib.Data.Attributes;
using Shiftup.CommonLib.Data.Bulk;
using Shiftup.CommonLib.Logger;

using InnerDevToolCommon.Attributes;
using InnerDevToolCommon.Common;
using InnerDevToolCommon.Database;

namespace InnerDevToolCommon.Data
{
    public abstract class RowData : ObjectData
    {
        private const string RowDataClassNameSuffixes = "Data";
        protected Dictionary<string, object> originalData = new Dictionary<string, object>();
        protected IDatabaseStorage storage = null;

        public RowData()
        {
            SetDefaultPropertyValue();
        }

        public virtual object this[string name]
        {
            get
            {
                var property = GetType().GetProperty(name);
                if (property != null)
                {
                    return property.GetValue(this);
                }

                return null;
            }

            set
            {
                base.SetData(new Dictionary<string, object>() { { name, value } });
            }
        }

        public virtual Type this[string name, bool dummyArg]
        {
            get
            {
                var property = GetType().GetProperty(name);
                if (property != null)
                {
                    return property.PropertyType;
                }

                Log.Debug("{0} have not {1} property!!!", GetType().Name, name);

                return null;
            }

            set
            {
                base.SetData(new Dictionary<string, object>() { { name, value } });
            }
        }

        private void SetDefaultPropertyValue()
        {
            var defaultValueAttrInfos = AttributeUtil.SelectAttributeInProperty(GetType(), new AttributeTypeSelector<DefaultValueAttribute>());
            foreach (var info in defaultValueAttrInfos)
            {
                var value = (info.Attribute as DefaultValueAttribute).Value;
                info.Property.SetValue(this, value);

                this.originalData[info.Property.Name] = value;
            }
        }

        public static IEnumerable<string> GetDefaultFieldNames(Type t)
        {
            return t.GetProperties().SelectMany(pi =>
            {
                if (pi.Name == "Item")
                {
                    return Enumerable.Empty<string>();
                }

                var attributes = pi.GetCustomAttributes(true)
                                .Where(attr => attr is BaseAttribute)
                                .Cast<BaseAttribute>()
                                .ToList();

                if (attributes.Any(attr => attr is CommonFieldAttribute || attr is RouteFieldAttribute || attr is NotDatabaseFieldAttribute))
                {
                    return Enumerable.Empty<string>();
                }

                return new List<string>() { pi.Name };
            });
        }

        public static string GetDefaultTableName(Type t)
        {
            string typeName = t.Name;
            if (typeName.EndsWith(RowDataClassNameSuffixes))
            {
                typeName = typeName.Remove(typeName.Length - RowDataClassNameSuffixes.Length, RowDataClassNameSuffixes.Length);
            }
            string tableName = string.Concat(typeName.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
            return tableName;
        }

        public static List<InnerTableMeta> GetMetas(Type t)
        {
            var tableFields = new Dictionary<string, List<string>>();
            var commonFieldNames = AttributeUtil.SelectPropertyNames<CommonFieldAttribute>(t);
            var routingFieldInfos = AttributeUtil.SelectAttributeInProperty<RouteFieldAttribute>(t);
            foreach (var routeAttributeInfo in routingFieldInfos)
            {
                var tableName = routeAttributeInfo.Attribute.Name;
                if (!tableFields.ContainsKey(tableName))
                {
                    tableFields.Add(tableName, new List<string>(commonFieldNames));
                }
                tableFields[tableName].Add(routeAttributeInfo.Property.Name);
            }

            var defaultTableName = RowData.GetDefaultTableName(t);
            if (!tableFields.ContainsKey(defaultTableName))
            {
                tableFields.Add(defaultTableName, new List<string>(commonFieldNames));
            }
            var defaultFields = RowData.GetDefaultFieldNames(t);
            tableFields[defaultTableName].AddRange(defaultFields);

            var defaultPrimaryKeys = AttributeUtil.SelectPropertyNames<PrimaryKeyAttribute>(t);
            var joinKeyInfos = AttributeUtil.SelectAttributeInProperty<JoinPrimaryKeyAttribute>(t);
            foreach (var joinKeyInfo in joinKeyInfos)
            {
                var attr = (joinKeyInfo.Attribute as JoinPrimaryKeyAttribute);
                var tableName = attr.TableName;
                if (!tableFields.ContainsKey(tableName))
                {
                    tableFields.Add(tableName, new List<string>(commonFieldNames));
                }
                tableFields[tableName].Add(attr.ColumnName);
            }
            var primaryKeys = defaultPrimaryKeys.Union(joinKeyInfos.Select(keyInfo => (keyInfo.Attribute as JoinPrimaryKeyAttribute).ColumnName));

            var metas = new List<InnerTableMeta>();
            var ak = AttributeUtil.SelectPropertyNames<AutoIncreaseKeyAttribute>(t).ElementAtOrDefault(0);
            if (ak == null)
            {
                ak = String.Empty;
            }

            var exceptionValues = AttributeUtil.SelectAttributeInProperty<ExceptionValueInInsertingAttribute>(t)
                                               .ToDictionary(
                                                    attr => attr.Property.Name, 
                                                    attr => (attr.Attribute as ExceptionValueInInsertingAttribute).ExceptionValue
                                                );
            foreach (var tableField in tableFields)
            {
                var autoKey = tableField.Key == defaultTableName ? ak : String.Empty;
                metas.Add(new InnerTableMeta(tableField.Key,
                    tableField.Value.Where(name => String.IsNullOrWhiteSpace(autoKey) || !autoKey.Equals(name)),
                    tableField.Value.Where(name => primaryKeys.Contains(name)),
                    autoKey,
                    joinKeyInfos.Where(info => (info.Attribute as JoinPrimaryKeyAttribute).TableName.Equals(tableField.Key)),
                    tableField.Key == defaultTableName,
                    exceptionValues)
                );
            }

            return metas;
        }

        public void BuildData(IDictionary<string, object> data, bool ignoreNotExist = true)
        {
            base.SetData(data, ignoreNotExist);
            
            foreach (var pair in data)
            {
                var property = GetType().GetProperty(pair.Key);
                if (property != null)
                {
                    this.originalData[pair.Key] = property.GetValue(this);
                }
            }
        }

        public bool Equals(RowData other)
        {
            if (this.GetType() != other.GetType())
            {
                return false;
            }

            var primaryKeys = RowUtil.GetPrimaryKeys(GetType());
            foreach (string key in primaryKeys)
            {
                var thisValue = this[key];
                var compareValue = other[key];
                if (!thisValue.Equals(compareValue))
                {
                    return false;
                }
            }

            return true;
        }

        public IDictionary<string, object> GetDatabaseValues(bool original = false)
        {
            var notDatabaseFieldNames = AttributeUtil.SelectPropertyNames<NotDatabaseFieldAttribute>(GetType());
            IDictionary<string, object> result;
            if (original)
            {
                result = this.originalData;
            }
            else
            {
                result = GetType().GetProperties()
                .Where(pi => !pi.Name.Equals("Item") && !notDatabaseFieldNames.Contains(pi.Name))
                .ToDictionary(pi => pi.Name, pi => pi.GetValue(this));
            }
            var routePrimaryKeys = AttributeUtil.SelectAttributeInProperty<JoinPrimaryKeyAttribute>(GetType());
            routePrimaryKeys.ToList().ForEach(pkInfo => result[(pkInfo.Attribute as JoinPrimaryKeyAttribute).ColumnName] = pkInfo.Property.GetValue(this));
            return result;
        }

        public bool IsDirty()
        {
            foreach (var pair in this.originalData)
            {
                if (pair.Value != null)
                {
                    if (!pair.Value.Equals(this[pair.Key]))
                    {
                        return true;
                    }
                }
                else
                {
                    if (this[pair.Key] != null)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public virtual bool IsExcept()
        {
            return false;
        }

        public void Restore()
        {
            foreach (var key in this.originalData.Keys)
            {
                var property = GetType().GetProperty(key);
                if (property != null)
                {
                    property.SetValue(this, this.originalData[key]);
                }
            }
        }

        public void RestoreProperty(string name)
        {
            var property = GetType().GetProperty(name);
            if (this.originalData.ContainsKey(name) && property != null)
            {
                property.SetValue(this, this.originalData[property.Name]);
            }
        }
        
        public override string ToString()
        {
            var primaryKeys = AttributeUtil.SelectPropertyNames<PrimaryKeyAttribute>(GetType());
            return String.Join(",", primaryKeys.Select(key => String.Format("{0}:{1}", key, this[key])));
        }
    }
}