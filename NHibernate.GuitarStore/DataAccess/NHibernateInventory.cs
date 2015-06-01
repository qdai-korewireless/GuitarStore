using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Criterion;
using NHibernate.GuitarStore.Common;
using NHibernate.Impl;

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
                ("select Builder, Model, Price, Id,Profit from Inventory order by Builder");
                return query.List();
            }
        }
        public IList GetDynamicInventory(Guid TypeId)
        {
            using (ITransaction transaction = Session.BeginTransaction())
            {
                string hqlQuery = "select Builder, Model, Price, Id,Profit " +
                "from Inventory " +
                "where TypeId = :TypeId order by Builder";
                IQuery query = Session.CreateQuery(hqlQuery).SetGuid("TypeId", TypeId);
                return query.List();
            }
        }

        public IList GetPagedInventory(int maxValue, int firstValue)
        {
            var query = "select Builder, Model, Price, Id,Profit From Inventory order by Builder";
            using (var tran = Session.BeginTransaction())
            {
                var s = Session.CreateQuery(query)
                    .SetFirstResult(firstValue).SetMaxResults(maxValue);
                return s.List();
            }
        }
        public int GetInventoryCount()
        {
            using (ITransaction transaction = Session.BeginTransaction())
            {
                IQuery query = Session.CreateQuery("select count(*) from Inventory");
                return Convert.ToInt32(query.UniqueResult());
            }
        }
        public IList<T> ExecuteHQL<T>(string hqlQuery)
        {
            using (ITransaction transaction = Session.BeginTransaction())
            {
                IQuery query = Session.CreateQuery(hqlQuery);
                return query.List<T>();
            }
        }


        public int GetInventoryPaging(int MaxResult, int FirstResult, out IList resultSet)
        {
            using (ITransaction transaction = Session.BeginTransaction())
            {
                var hqlQuery = "select Builder, Model, Price, Id,Profit " +
                "from Inventory order by Builder";
                IQuery query = Session.CreateQuery(hqlQuery)
                .SetMaxResults(MaxResult)
                .SetFirstResult(FirstResult);
                IQuery count = Session.CreateQuery("select count(*) from Inventory");
                IMultiQuery mQuery = Session.CreateMultiQuery()
                .Add("result", query)
                .Add<long>("RowCount", count);
                resultSet = (IList)mQuery.GetResult("result");
                int totalCount = (int)((IList<long>)mQuery.GetResult("RowCount")).Single();
                return totalCount;
            }
        }

        public IList ExecuteNamedQuery(string queryName)
        {
            using (var transaction = Session.BeginTransaction())
            {
                IQuery query = Session.GetNamedQuery(queryName);
                return query.List();
            }
        }
        public IList ExecuteDetachedQuery(string searchParameter)
        {
            using (ITransaction transaction = Session.BeginTransaction())
            {
                string hqlQuery = "select Builder, Model, Price, Id " +
                "from Inventory " +
                "where Model like :search " +
                "order by Builder";
                IDetachedQuery detachedQuery = new DetachedQuery(hqlQuery)
                .SetString("search", searchParameter);
                IQuery executableQuery = detachedQuery.GetExecutableQuery(Session);
                return executableQuery.List();
            }
        }
    }
}
