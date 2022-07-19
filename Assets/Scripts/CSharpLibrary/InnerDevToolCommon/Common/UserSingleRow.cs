using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using InnerDevToolCommon.Data;
using InnerDevToolCommon.Database;

namespace InnerDevToolCommon.Common
{
    public class UserSingleRow<T> : IRowStorage<T> where T : RowData
    {
        private bool added = false;
        private bool dataChanged = false;
        private ulong nfguid = 0;
        private bool removed = false;
        private T row = null;

        public T Data
        {
            get
            {
                return row;
            }
        }

        public UserSingleRow(ulong nfguid)
        {
            this.nfguid = nfguid;
        }

        private void AddRows(IEnumerable<InnerTableMeta> metas)
        {
            DatabaseApplier.Instance.AddRows<T>(metas, new List<T>() { this.row }, (meta, _, lastId) =>
            {
                var autoIncreaseKey = meta.AutoIncreaseKey;
                if (!String.IsNullOrWhiteSpace(autoIncreaseKey))
                {
                    this.row.BuildData(new Dictionary<string, object>() { { autoIncreaseKey, lastId } });
                }
            });
        }

        private void DeleteRows(IEnumerable<InnerTableMeta> metas)
        {
            DatabaseApplier.Instance.DeleteRows<T>(metas, new List<T>() { this.row });
        }

        private void UpdateRows(IEnumerable<InnerTableMeta> metas)
        {
            DatabaseApplier.Instance.UpdateRows<T>(metas, new List<T>() { this.row });
        }

        public void BuildRow(IDictionary<string, object> data)
        {
            if (this.row == null)
            {
                this.row = (T)Activator.CreateInstance(typeof(T));
            }
            this.row.BuildData(data);
        }

        public void Commit()
        {
            if (IsDirty())
            {
                var metas = RowUtil.GetMetas(typeof(T), this.nfguid);
                if (!this.removed && !this.added && this.row != null)
                {
                    UpdateRows(metas);
                }
                else if (this.added && this.row != null)
                {
                    AddRows(metas);
                }
                else if (this.removed && this.row != null)
                {
                    DeleteRows(metas);
                }

                if (this.row != null && this.row.IsDirty())
                {
                    this.row.BuildData(this.row.GetDatabaseValues());
                }
            }
        }

        public void Clear()
        {
            this.added = false;
            this.removed = false;
            this.dataChanged = false;
        }

        public bool IsDirty()
        {
            return this.dataChanged || (this.row != null && this.row.IsDirty());
        }

        public void Remove()
        {
            this.added = false;
            this.removed = true;

            this.dataChanged = true;
        }

        public void Restore()
        {
            this.row.Restore();
            this.dataChanged = false;
        }

        public void Set(T rowData)
        {
            this.row = rowData;
            this.added = true;
            this.removed = false;

            this.dataChanged = true;
        }
    }
}