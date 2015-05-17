using System;
using System.Collections.Generic;

namespace NHibernate.GuitarStore.Common
{
    public class Guitar
    {
        public Guitar() { }
       
        public virtual Guid Id { get; set; }
        public virtual String Type { get; set; }
        IList<Inventory> Inventory { get; set; }
    }
}
