using System;
using NHibernate.GuitarStore.DataAccess;
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
    }
}
