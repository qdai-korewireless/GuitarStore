using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using log4net;
using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Event;
using NHibernate.SqlCommand;
using Configuration = NHibernate.Cfg.Configuration;

namespace NHibernate.GuitarStore.DataAccess
{
    public class NHibernateBase
    {
        private static Configuration Configuration { get; set; }
        protected static ISessionFactory SessionFactory { get; set; }
        private static ISession session = null;
        private static IStatelessSession statelessSession = null;


        public NHibernateBase()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        public static Configuration ConfigureNHibernate(string assembly)
        {
            Configuration = new Configuration();
            Configuration.DataBaseIntegration(dbi =>
            {
                dbi.Dialect<MsSql2008Dialect>();
                dbi.Driver<SqlClientDriver>();
                dbi.ConnectionProvider<DriverConnectionProvider>();
                dbi.IsolationLevel = IsolationLevel.ReadCommitted;
                dbi.Timeout = 15;
                dbi.ConnectionStringName = "GuitarStore";
            });

            //add interceptors
            Configuration.SetInterceptor(new SQLInterceptor());

            //add events
            Configuration.EventListeners.PostDeleteEventListeners =
new IPostDeleteEventListener[] { new AuditDeleteEvent() };


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
    public class SQLInterceptor : EmptyInterceptor, IInterceptor
    {
        SqlString IInterceptor.OnPrepareStatement(SqlString sql)
        {
            Utils.NHibernateGeneratedSQL = sql.ToString();
            Utils.QueryCounter++;
            return sql;
        }
    }

    public class AuditDeleteEvent : IPostDeleteEventListener
    {
        private static ILog log = LogManager.GetLogger("NHBase.SQL");

        public void OnPostDelete(PostDeleteEvent @event)
        {
            log.Info(@event.Id.ToString() + " has been deleted.");
        }
    }
}
