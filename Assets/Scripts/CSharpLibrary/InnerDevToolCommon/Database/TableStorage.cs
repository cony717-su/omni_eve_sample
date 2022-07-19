using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InnerDevToolCommon.Database
{
    public interface IStorage
    {
        void Commit();

        bool IsDirty();
    }

    public interface IRestorableStorage : IStorage
    {
        void Restore();
    }

    public interface IDatabaseStorage : IRestorableStorage
    {
        void ReadAllData();
    }

    public interface IRowStorage : IRestorableStorage
    {
        void BuildRow(IDictionary<string, object> data);

        void Clear();
    }

    public interface IRowStorage<T> : IRowStorage
    {
    }

    public interface IWriteOnlyRowStorage : IStorage
    {
    }

    public interface IShardingStorages : IWriteOnlyRowStorage
    {
        IList<string> Names { get; }
    }

    public interface IShardingStorages<T> : IShardingStorages
    {

    }
}