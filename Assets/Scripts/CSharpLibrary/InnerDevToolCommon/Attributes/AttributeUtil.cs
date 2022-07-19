using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Shiftup.CommonLib.Data.Attributes;

namespace InnerDevToolCommon.Attributes
{
    public static class AttributeUtil
    {
        public static IEnumerable<InnerAttributeInfo> SelectAttributeInProperty(Type t, AttributeSelector selector)
        {
            return t.GetProperties().SelectMany(pi => selector.SelectAttributes(pi).Select(attr => new InnerAttributeInfo(attr, pi)));
        }

        public static IEnumerable<InnerAttributeInfo> SelectAttributeInProperty<T>(Type t) where T : BaseAttribute
        {
            return t.GetProperties().SelectMany(pi => (new AttributeTypeSelector<T>()).SelectAttributes(pi).Select(attr => new InnerAttributeInfo(attr, pi)));
        }

        public static IEnumerable<string> SelectPropertyNames(Type t, AttributeSelector selector)
        {
            return SelectAttributeInProperty(t, selector).Select(attr => attr.Property.Name);
        }

        public static IEnumerable<string> SelectPropertyNames<T>(Type t) where T : BaseAttribute
        {
            return SelectAttributeInProperty<T>(t).Select(attr => attr.Property.Name);
        }
    }
}
