using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MySql.Data;
using MySql.Data.MySqlClient;

using Shiftup.CommonLib.Data.Bulk;
using Shiftup.CommonLib.Logger;

using InnerDevToolCommon;
using InnerDevToolCommon.Data;
using InnerDevToolCommon.Database;

namespace InnerDevToolCommon.Common
{
    public abstract class UserDataManager : IDatabaseStorage
    {
        protected List<string> deletedTables = new List<string>();
        protected ulong nfguid { get; set; }

        public UserDataManager(ulong nfguid)
        {
            this.nfguid = nfguid;

            initAllUserProperty();
        }

        public bool IsDirty()
        {
            return this.deletedTables.Count > 0 || GetType().GetProperties().Select(p => (p.GetValue(this) as IStorage)).Where(store => store != null).Any(store => store.IsDirty());
        }

        protected delegate void CallbackDelegate(IDictionary<string, object> data);

        protected delegate void ReadStaticDataExceptionDelegate(Exception e);

        private void initAllUserProperty()
        {
            var properties = GetType().GetProperties();
            foreach (var property in properties)
            {
                if (property.PropertyType.IsGenericType)
                {
                    var propertyGenericType = property.PropertyType.GetGenericTypeDefinition();
                    if (propertyGenericType == typeof(UserTable<>) || propertyGenericType == typeof(UserSingleRow<>))
                    {
                        property.SetValue(this, Activator.CreateInstance(property.PropertyType, this.nfguid));
                    }

                    if (propertyGenericType == typeof(InnerTable<>))
                    {
                        property.SetValue(this, Activator.CreateInstance(property.PropertyType));
                    }
                }
            }
        }

        private Dictionary<string, object> MergeDefaultParameter(ulong nfguid, IDictionary<string, object> parameters)
        {
            var result = new Dictionary<string, object>() { { "nfguid", nfguid } };
            if (parameters != null)
            {
                parameters.ToList().ForEach(param => result[param.Key] = param.Value);
            }
            return result;
        }

        protected KeyValuePair<string, object> BuildParam(string key, object value)
        {
            return new KeyValuePair<string, object>(key, value);
        }

        protected void ClearTable(string tableName)
        {
            this.deletedTables.Add(tableName);
        }

        protected T MakeData<T>(params KeyValuePair<string, object>[] values) where T : RowData
        {
            T item = (T)Activator.CreateInstance(typeof(T));
            var initParam = this.nfguid != 0 ? new Dictionary<string, object>() { { "nfguid", this.nfguid } } : new Dictionary<string, object>();
            IDictionary<string, object> param = initParam.Union(values).ToDictionary(pair => pair.Key, pair => pair.Value);
            item.BuildData(param);
            return item;
        }

        protected T MakeData<T>(IDictionary<string, object> data = null, bool isIncludeNfguid = true) where T : RowData
        {
            T item = (T)Activator.CreateInstance(typeof(T));
            IDictionary<string, object> initData = new Dictionary<string, object>();
            if (isIncludeNfguid)
            {
                initData["nfguid"] = this.nfguid;
            }
            IDictionary<string, object> param = new Dictionary<string, object>().Union(data).ToDictionary(pair => pair.Key, pair => pair.Value);
            item.BuildData(param);
            return item;
        }

        protected bool SelectData<T>(IRowStorage<T> storage, IDictionary<string, object> whereParams = null, string orderDescColumn = null, ReadStaticDataExceptionDelegate failed = null) where T : RowData
        {
            try
            {
                storage.Clear();

                var metas = RowUtil.GetMetas(typeof(T), this.nfguid);
                var parameters = this.nfguid == 0 ? whereParams : MergeDefaultParameter(this.nfguid, whereParams);
                var dataObjects = DatabaseApplier.Instance.SelectRows(typeof(T), metas, parameters, orderDescColumn);
                foreach (var data in dataObjects)
                {
                    storage.BuildRow(data);
                }

                return true;
            }
            catch (Exception e)
            {
                if (failed != null)
                {
                    failed(e);
                }

                Console.WriteLine(e.Message);

                return false;
            }
        }

        protected bool SelectData(string tableName, CallbackDelegate fn, IDictionary<string, object> whereParams = null, string orderDescColumn = null, int limit = -1, ReadStaticDataExceptionDelegate failed = null)
        {
            try
            {
                var dataObjects = DatabaseApplier.Instance.SelectRows(tableName, whereParams, orderDescColumn, limit);
                foreach (var data in dataObjects)
                {
                    fn(data);
                }

                return true;
            }
            catch (Exception e)
            {
                if (failed != null)
                {
                    failed(e);
                }

                return false;
            }
        }

        protected void UpdateDataByQuery(string tableName, string setClause, string whereClause = null)
        {
            DatabaseApplier.Instance.UpdateRowsByNfguid(tableName, setClause, whereClause, this.nfguid);
        }

        public virtual void Commit()
        {
            var properties = GetType().GetProperties();
            foreach (var property in properties)
            {
                var storage = property.GetValue(this) as IStorage;
                if (storage != null && storage.IsDirty())
                {
                    storage.Commit();
                }
            }

            foreach (var deleteTable in this.deletedTables)
            {
                DatabaseApplier.Instance.DeleteTable(deleteTable, this.nfguid);
            }
            this.deletedTables.Clear();
        }

        public string GetShardingName(string tableName)
        {
            int shardKey = Convert.ToInt32(this.nfguid % 32);
            return String.Format("{0}_{1}", tableName, shardKey);
        }

        public abstract void ReadAllData();

        public virtual void Restore()
        {
            var properties = GetType().GetProperties();
            foreach (var property in properties)
            {
                var storage = property.GetValue(this) as IDatabaseStorage;
                if (storage != null)
                {
                    storage.Restore();
                }
            }
        }
    }
}