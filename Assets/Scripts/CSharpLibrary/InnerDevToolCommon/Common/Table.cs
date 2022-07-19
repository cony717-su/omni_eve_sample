using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Shiftup.CommonLib.Data.Attributes;
using Shiftup.CommonLib.Data.Bulk;
using Shiftup.CommonLib.Logger;

using InnerDevToolCommon.Attributes;
using InnerDevToolCommon.Common;
using InnerDevToolCommon.Data;
using InnerDevToolCommon.Database;

namespace InnerDevToolCommon.Common
{
    public class InnerTable<T> : IEnumerable<T>, IRowStorage<T> where T : RowData
    {
        protected List<T> addedRows = new List<T>();
        protected bool dataChanged = false;
        protected List<T> deletedRows = new List<T>();
        protected List<T> rows = new List<T>();

        public int Count
        {
            get
            {
                return this.rows.Count;
            }
        }

        protected virtual IEnumerable<InnerTableMeta> GetRowMetas()
        {
            return RowUtil.GetMetas(typeof(T));
        }

        public void BuildRow(IDictionary<string, object> data)
        {
            T newRow = (T)Activator.CreateInstance(typeof(T));
            newRow.BuildData(data);
            this.rows.Add(newRow);
        }

        public virtual void Commit()
        {
            if (IsDirty())
            {
                var metas = GetRowMetas();
                DatabaseApplier.Instance.DeleteRows(metas, this.deletedRows);
                DatabaseApplier.Instance.AddRows(metas, this.addedRows, (meta, row, lastId) =>
                {
                    var autoIncreaseKey = meta.AutoIncreaseKey;
                    if (!String.IsNullOrWhiteSpace(autoIncreaseKey))
                    {
                        row.BuildData(new Dictionary<string, object>() { { autoIncreaseKey, (ulong)lastId } });
                    }
                    row.BuildData(row.GetDatabaseValues());
                });
                var changedRows = this.rows.Where(row => row.IsDirty());
                DatabaseApplier.Instance.UpdateRows(metas, changedRows);

                foreach (var item in changedRows)
                {
                    item.BuildData(item.GetDatabaseValues());
                }
                this.addedRows.Clear();
                this.deletedRows.Clear();

                this.dataChanged = false;
            }
        }

        public void Clear()
        {
            this.rows.Clear();
            this.addedRows.Clear();
            this.deletedRows.Clear();

            this.dataChanged = false;
        }

        public void Except(Func<T, bool> match)
        {
            this.rows = this.rows.Except(this.rows.Where(match)).ToList();
        }

        public T Find(Predicate<T> match)
        {
            var result = this.rows.FindAll(x => !x.IsExcept()).Find(match);
            return result;
        }

        public List<T> FindAll(Predicate<T> match)
        {
            var result = this.rows.FindAll(x => !x.IsExcept()).FindAll(match);
            return result;
        }

        public T Get(params object[] keys)
        {
            var primaryKeyInfos = AttributeUtil.SelectAttributeInProperty<PrimaryKeyAttribute>(typeof(T));
            var orderedKeys = primaryKeyInfos.Where(info => (info.Attribute as PrimaryKeyAttribute).Order > 0).OrderBy(info => (info.Attribute as PrimaryKeyAttribute).Order);
            if (orderedKeys.Count() != keys.Length)
            {
                Log.Error("{0} Type Table has {1} ordered key, but given key count is {2}", typeof(T), orderedKeys.Count(), keys.Length);
            }

            List<T> result = this.rows;
            for (var i = 0; i < orderedKeys.Count(); i++)
            {
                var databaseField = orderedKeys.ElementAt(i).Property;
                if (keys[i].GetType() != databaseField.PropertyType)
                {
                    Log.Error("given type {0} - original type {1}", keys[i].GetType(), databaseField.PropertyType);
                }

                result = result.FindAll(data => data[databaseField.Name].Equals(keys[i]));
            }

            if (result.Count > 1)
            {
                Log.Warning("Result data count is {0}", result.Count);
            }

            if (result.Count == 0)
            {
                return null;
            }

            return result[0];
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int index = 0; index < this.rows.Count; index++)
            {
                if (rows[index].IsExcept())
                {
                    continue;
                }

                yield return rows[index];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<T> Gets(params object[] keys)
        {
            var primaryKeyInfos = AttributeUtil.SelectAttributeInProperty<PrimaryKeyAttribute>(typeof(T));
            var orderedKeys = primaryKeyInfos.Where(info => (info.Attribute as PrimaryKeyAttribute).Order > 0).OrderBy(info => (info.Attribute as PrimaryKeyAttribute).Order);
            if (orderedKeys.Count() < keys.Length)
            {
                Log.Error("{0} Type Table has {1} ordered key, but given key count is {2}", typeof(T), orderedKeys.Count(), keys.Length);
            }

            List<T> result = this.rows;
            for (var i = 0; i < keys.Count(); i++)
            {
                var databaseField = orderedKeys.ElementAt(i).Property;
                if (keys[i].GetType() != databaseField.PropertyType)
                {
                    Log.Error("given type {0} - original type {1}", keys[i].GetType(), databaseField.PropertyType);
                }

                result = result.FindAll(data => data[databaseField.Name].Equals(keys[i]));
            }

            return result;
        }

        public T Insert(T item)
        {
            this.addedRows.Add(item);
            this.rows.Add(item);

            this.dataChanged = true;

            return item;
        }

        public T InsertOnDuplicateUpdate(T item)
        {
            if (this.rows.Exists(row => row.Equals(item)))
            {
                var existItem = this.rows.Find(row => row.Equals(item));
                existItem.SetData(item.GetDatabaseValues());
                if (!this.dataChanged)
                {
                    this.dataChanged = existItem.IsDirty();
                }
                return existItem;
            }

            this.addedRows.Add(item);
            this.rows.Add(item);

            this.dataChanged = true;

            return item;
        }

        public bool IsDirty()
        {
            return this.dataChanged || this.rows.Any(row => row.IsDirty());
        }

        public int Max(Func<T, int> selector)
        {
            if (this.rows.FindAll(x => !x.IsExcept()).Count > 0)
            {
                return this.rows.FindAll(x => !x.IsExcept()).Max(selector);
            }
            else
            {
                return 0;
            }
        }

        public bool Remove(params object[] keys)
        {
            var removedData = this.Get(keys);
            if (removedData == null)
            {
                Log.Warning("Getting {0} data in list is null", String.Join(", ", keys));
                return false;
            }

            this.dataChanged = true;

            this.deletedRows.Add(removedData);
            return this.rows.Remove(removedData);
        }

        public bool Remove(Predicate<T> match)
        {
            var removedData = this.rows.Find(match);
            if (removedData == null)
            {
                return false;
            }

            this.dataChanged = true;

            this.deletedRows.Add(removedData);
            return this.rows.Remove(removedData);
        }

        public void RemoveAll()
        {
            this.deletedRows.AddRange(this.rows);
            this.rows.Clear();
            this.addedRows.Clear();

            this.dataChanged = true;
        }

        public int RemoveAll(Predicate<T> match)
        {
            var removedList = this.rows.FindAll(match);
            this.deletedRows.AddRange(removedList);
            this.addedRows.RemoveAll(match);

            this.dataChanged = true;

            return this.rows.RemoveAll(match);
        }

        public void Restore()
        {
            foreach (var row in this.rows)
            {
                row.Restore();
            }

            this.dataChanged = false;
        }
    }

    public class UserTable<T> : InnerTable<T> where T : RowData
    {
        private ulong nfguid = 0;

        public UserTable(ulong nfguid)
        {
            this.nfguid = nfguid;
        }

        protected override IEnumerable<InnerTableMeta> GetRowMetas()
        {
            return RowUtil.GetMetas(typeof(T), this.nfguid);
        }
    }
}