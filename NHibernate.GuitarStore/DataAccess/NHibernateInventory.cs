using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Criterion;
using NHibernate.GuitarStore.Common;

namespace NHibernate.GuitarStore.DataAccess
{
    public class NHibernateInventory:NHibernateBase
    {
        public IList<Inventory> ExecuteICriteriaOrderBy(string orderBy)
        {
            using (var transaction = Session.BeginTransaction())
            {
                try
                {
                    var result = Session.CreateCriteria(typeof(Inventory))
                    .AddOrder(Order.Asc(orderBy))
                    .List<Inventory>();
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
        public IList<Inventory> ExecuteICriteria(Guid id)
        {
            using (var transaction = Session.BeginTransaction())
            {
                try
                {
                    var result = Session.CreateCriteria(typeof(Inventory))
                    .Add(Restrictions.Eq("TypeId", id))
                    .List<Inventory>();
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
        public bool DeleteInventoryItem(Guid Id)
        {
            using (ITransaction transaction = Session.BeginTransaction())
            {
                try
                {
                    IQuery query = Session.CreateQuery("from Inventory where Id = :Id")
                    .SetGuid("Id", Id);
                    Inventory inventory = query.List<Inventory>()[0];
                    Session.Delete(inventory);
                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }
        public IList GetDynamicInventory()
        {
            using (ITransaction transaction = Session.BeginTransaction())
            {
                IQuery query = Session.CreateQuery
                ("select Builder, Model, Price, Id from Inventory order by Builder");
                return query.List();
            }
        }
        public IList GetDynamicInventory(Guid TypeId)
        {
            using (ITransaction transaction = Session.BeginTransaction())
            {
                string hqlQuery = "select Builder, Model, Price, Id " +
                "from Inventory " +
                "where TypeId = :TypeId order by Builder";
                IQuery query = Session.CreateQuery(hqlQuery).SetGuid("TypeId", TypeId);
                return query.List();
            }
        }

        
    }
}
