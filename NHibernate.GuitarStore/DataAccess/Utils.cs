using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.AdoNet.Util;

namespace NHibernate.GuitarStore.DataAccess
{
    public class Utils
    {
        public static string NHibernateGeneratedSQL { get; set; }
        public static int QueryCounter { get; set; }
        public static string FormatSQL()
        {
            var formatter = new BasicFormatter();
            return formatter.Format(NHibernateGeneratedSQL.ToUpper());
        }
    }
}
