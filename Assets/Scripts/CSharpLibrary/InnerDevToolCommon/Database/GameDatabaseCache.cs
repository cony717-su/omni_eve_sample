using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Shiftup.CommonLib.Data;
using Shiftup.CommonLib.Data.Attributes;
using Shiftup.CommonLib.Logger;

namespace InnerDevToolCommon.Database
{
    public class GameDatabaseCache
    {
        private Dictionary<string, TableMeta> caches = new Dictionary<string, TableMeta>();

        public IEnumerable<TableMeta> GetTables()
        {
            return this.caches.Values;
        }

        public void Load(string connectionInfo)
        {
            var loader = new GameDBLoader();
            try
            {
                var msg = loader.Init(connectionInfo);
                Log.Info("Start to load database : {0}", msg);

                var meta = loader.GetMeta();
                foreach (var pair in meta)
                {
                    caches.Add(pair.Key, pair.Value);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                loader.Close();
            }
        }
    }
}