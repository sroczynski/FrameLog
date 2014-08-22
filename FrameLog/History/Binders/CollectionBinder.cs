using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FrameLog.History.Binders
{
    public class CollectionBinder : IBinder
    {
        private BindManager manager;

        public CollectionBinder(BindManager manager)
        {
            this.manager = manager;
        }

        public bool Supports(Type type)
        {
            return type.IsGenericType
                && typeof(ICollection<>).MakeGenericType(type.GetGenericArguments().First()).IsAssignableFrom(type);
        }

        public virtual object Bind(string raw, Type type)
        {
            var itemType = type.GetGenericArguments().First();
            object collection = createCollection(type, itemType);
            GetType().GetMethod("fillCollection", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy)
                .MakeGenericMethod(new Type[] { itemType })
                .Invoke(this, new object[] { collection, raw });
            return collection;
        }

        protected virtual object createCollection(Type type, Type itemType)
        {
            if (type.IsInterface)
            {
                var concreteType = typeof(List<>).MakeGenericType(itemType);
                return Activator.CreateInstance(concreteType);
            }
            else
            {
                return Activator.CreateInstance(type);
            }
        }

        protected virtual void fillCollection<ItemType>(ICollection<ItemType> collection, string raw)
        {
            foreach (var reference in raw.Split(new char[] { ',' }))
            {
                collection.Add(manager.Bind<ItemType>(reference));
            }
        }
    }
}
