using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Shiftup.CommonLib.Logger;

namespace Shiftup.CommonLib.Data.Attributes
{
    public abstract class AttributeSelector
    {
        protected abstract bool checkReject(BaseAttribute attr);
        protected abstract bool checkSelect(BaseAttribute attr);
        public IEnumerable<BaseAttribute> SelectAttributes(MemberInfo mi)
        {
            var attributes = mi.GetCustomAttributes(true)
                                .Where(attr => attr is BaseAttribute)
                                .Cast<BaseAttribute>()
                                .ToList();

            if (attributes.Where(attr => checkReject(attr)).Any())
                return Enumerable.Empty<BaseAttribute>();

            return attributes.Where(attr => checkSelect(attr));
        }
    }

    public class AttributeTypeSelector<T> : AttributeSelector where T : BaseAttribute
    {
        protected override bool checkReject(BaseAttribute attr)
        {
            return false;
        }

        protected override bool checkSelect(BaseAttribute attr)
        {
            return attr is T;
        }
    }

    public class Scanner<T>
    {
        public delegate T AttributeTypeGetter(BaseAttribute attr);

        private readonly AttributeTypeGetter attributeTypeGetter;
        private readonly AttributeSelector attributeSelector;

        public Scanner(AttributeSelector selector, AttributeTypeGetter typeGetter)
        {
            this.attributeSelector = selector;
            this.attributeTypeGetter = typeGetter;
        }


        public ReadOnlyDictionary<T, IEnumerable<AttributeInfo>> Scan(IEnumerable<Table> tables, T referenceType, bool databaseFieldsOnly)
        {
            var dict = Scan(tables, databaseFieldsOnly);
            var refDict = buildRefereceFields(dict[referenceType]);
            // Check Refernce Fields 
            var noRefTables = tables.Select(table => table.Meta.Name).Except(refDict.Keys);
            foreach (var name in noRefTables)
                Log.Error("테이블({0})의 PK 필드 설명이 없습니다.", name);
            // Update ReferenceFields
            foreach (var kvp in dict)
            {
                if (kvp.Key.Equals(referenceType) == true)
                    continue;

                foreach (var info in kvp.Value)
                {
                    if (attributeTypeGetter(info.Attribute).Equals(referenceType) == true)
                        continue;

                    info.UpdateRefFields(refDict);
                }
            }

            return dict;
        }

        public ReadOnlyDictionary<T, IEnumerable<AttributeInfo>> Scan(IEnumerable<Table> tables, bool databaseFieldsOnly)
        {
            var ret = tables
                .SelectMany(
                    table =>
                        table.GetAttributeInfo(attributeSelector, databaseFieldsOnly))
                .GroupBy(info => attributeTypeGetter(info.Attribute))
                .ToDictionary(
                    group => group.Key,
                    group => (IEnumerable<AttributeInfo>)group);

            var empties = Enum.GetValues(typeof(T))
                                    .Cast<T>()
                                    .Except(ret.Keys).ToList();

            foreach (var key in empties)
                ret.Add(key, Enumerable.Empty<AttributeInfo>());

            return ret.AsReadOnly();
        }

        private ReadOnlyDictionary<string, IEnumerable<FieldInfo>> buildRefereceFields(IEnumerable<AttributeInfo> refInfos)
        {
            return refInfos
                        .GroupBy(info => info.Table.Meta.Name)
                        .ToDictionary(
                            group => group.Key,
                            group => (IEnumerable<FieldInfo>)group.Select(info => info.Field).ToList())
                        .AsReadOnly();
        }
    }
}
