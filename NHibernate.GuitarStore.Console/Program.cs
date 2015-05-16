using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.GuitarStore.Common;
using NHibernate.GuitarStore.DataAccess;
using NHibernate.Linq;

namespace NHibernate.GuitarStore.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            { 
                var nhb = new NHibernateBase();
                nhb.Initialize("NHibernate.GuitarStore");
                System.Console.WriteLine("NHibernate.GuitarStore assembly initialized.");
                
                Tests(nhb);
                System.Console.ReadLine();

            }
            catch (Exception ex)
            {
               var message = ex.Message;
                if (ex.InnerException != null)
                {
                    message += " - InnerExcepetion: " + ex.InnerException.Message;
                }
                System.Console.WriteLine();
                System.Console.WriteLine("***** ERROR *****");
                System.Console.WriteLine(message);
                System.Console.WriteLine();
                System.Console.ReadLine();
            }

            
        }

        private static void Tests(NHibernateBase nhb)
        {
            var list1 =
            NHibernateBase.StatelessSession.CreateQuery("from Inventory").List<Inventory>();
            var list2 =
            NHibernateBase.Session.CreateCriteria(typeof(Inventory)).List<Inventory>();
            var linq =(from l in NHibernateBase.Session.Query<Inventory>() select l);
        }

    }
}
