using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using InnerDevTool.Data.Game;
using InnerDevTool.Data.Main;

using InnerDevToolCommon;
using InnerDevToolCommon.Common;
using InnerDevToolCommon.Data;
using InnerDevToolCommon.Database;

using MySql.Data;
using MySql.Data.MySqlClient;

using Shiftup.CommonLib;
using Shiftup.CommonLib.Data.Attributes;

namespace InnerDevTool.Data
{
    public class StaticData
    {
        /*
         * ���� ���
         * try
         * {
         *    string query = "SELECT * FROM static_area";
         *    MySqlDataReader staticAreaDataReader = DBLookup.Instance.mainSql.QueryWithReader(query);
         *
         *    IEnumerable<IDictionary<string, Object>> dataReader = staticAreaDataReader.DataRecord();
         *    foreach (var data in dataReader)
         *    {
         *        Area staticArea = new Area(data);
         *        dictArea.Add(staticArea.area_idx, staticArea);
         *    }
         *
         *    staticAreaDataReader.Close();
         * }
         * catch
         * {
         * }
         *
         * ����� ���
         * Init() �Լ����� static row type's InnerTable Ÿ���� Property �� ���� DB �����͸� �ڵ����� ����
         */
        public InnerTable<StaticOmniEveFloor> StaticOmniEveFloorTable { get; set; }

        public InnerTable<StaticOmniEveItem> StaticOmniEveItemTable { get; set; }

        public InnerTable<StaticOmniEveMob> StaticOmniEveMobTable { get; set; }

        /**
         * Ÿ���� �������� �ʰ� DB �����͸� ���� ���
         *
         * ReadStaticData("static_characters", (data) => {
         *    ~~~
         * }, (e) => {
         *   // fail
         * });
         *
         */

        private delegate void AddStaticDataDelegate(IDictionary<string, object> data);

        private delegate void ReadStaticDataExceptionDelegate(Exception e);

        private bool ReadStaticData(string tableName, AddStaticDataDelegate fn, ReadStaticDataExceptionDelegate failed = null)
        {
            try
            {
                var dataObjects = DatabaseApplier.Instance.SelectRows(tableName);
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

        public void Init()
        {
            var properties = GetType().GetProperties();
            foreach (var property in properties)
            {
                if (property.PropertyType.IsGenericType)
                {
                    var propertyGenericType = property.PropertyType.GetGenericTypeDefinition();
                    if (propertyGenericType == typeof(InnerTable<>))
                    {
                        property.SetValue(this, Activator.CreateInstance(property.PropertyType));
                        Type staticRowType = property.PropertyType.GetGenericArguments()[0];
                        var metas = RowData.GetMetas(staticRowType);
                        var dataObjects = DatabaseApplier.Instance.SelectRows(staticRowType, metas);
                        var table = property.GetValue(this) as IRowStorage;
                        foreach (var data in dataObjects)
                        {
                            table.BuildRow(data);
                        }
                    }
                }
            }
        }
    }
}