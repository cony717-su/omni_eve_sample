using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnerDevToolCommon
{
    public abstract class Singleton<T> where T : new()
    {
        private static readonly Lazy<T> instanceHolder = new Lazy<T>(() => new T());

        public static T Instance
        {
            get { return instanceHolder.Value; }
        }
    }
}
