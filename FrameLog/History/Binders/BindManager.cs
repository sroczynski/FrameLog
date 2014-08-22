using FrameLog.Contexts;
using System;
using System.Collections.Generic;

namespace FrameLog.History.Binders
{
    public class BindManager : IBindManager
    {
        protected List<IBinder> binders;
        private IHistoryContext db;

        public BindManager(IHistoryContext db)
        {
            this.db = db;
            binders = new List<IBinder>()
            {
                new PrimitiveBinder(),
                new GuidBinder(),
                new DateTimeBinder(),
                new CollectionBinder(this),
            };
        }

        public virtual TValue Bind<TValue>(string reference)
        {
            return (TValue)Bind(reference, typeof(TValue));
        }
        public virtual object Bind(string raw, Type type)
        {
            foreach (var binder in binders)
            {
                if (binder.Supports(type))
                    return binder.Bind(raw, type);
            }
            if (raw == null)
                return null;
            else
                return db.GetObjectByReference(type, raw);
        }
    }
}
