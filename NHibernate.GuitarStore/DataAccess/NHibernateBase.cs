using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Cfg;

namespace NHibernate.GuitarStore.DataAccess
{
    public class NHibernateBase
    {
        private static Configuration Configuration { get; set; }
        protected static ISessionFactory SessionFactory { get; set; }
        private static ISession session = null;
        private static IStatelessSession statelessSession = null;

        public static Configuration ConfigureNHibernate(string assembly)
        {
            Configuration = new Configuration();
            Configuration.AddAssembly(assembly);
            return Configuration;
        }
        public void Initialize(string assembly)
        {
            Configuration = ConfigureNHibernate(assembly);
            SessionFactory = Configuration.BuildSessionFactory();
        }

        public static ISession Session
        {
            get { return session ?? (session = SessionFactory.OpenSession()); }
        }

        public static IStatelessSession StatelessSession
        {
            get { return statelessSession ?? (statelessSession = SessionFactory.OpenStatelessSession()); }
        }

        public IList<T> ExecuteICriteria<T>()
        {
            using (ITransaction transaction = Session.BeginTransaction())
            {
                try
                {
                    IList<T> result = Session.CreateCriteria(typeof(T)).List<T>();
                    transaction.Commit();
                    return result;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

    }
}
