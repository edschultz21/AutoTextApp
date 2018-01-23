using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TenK.InfoSparks.Common
{
    public class ObjectPool<T>
    {
        private readonly ConcurrentBag<ObjectPoolItem<T>> _objects;
        private readonly Func<T> _objectGenerator;

        public ObjectPool(Func<T> objectGenerator)
        {
            if (objectGenerator == null) throw new ArgumentNullException("objectGenerator");
            _objects = new ConcurrentBag<ObjectPoolItem<T>>();
            _objectGenerator = objectGenerator;
        }

        public ObjectPoolItem<T> GetObject()
        {
            ObjectPoolItem<T> item;

            if (!_objects.TryTake(out item))
            {
                item = new ObjectPoolItem<T>(this,_objectGenerator());    
            }

            return item;
        }

        public void PutObject(ObjectPoolItem<T> item)
        {
            _objects.Add(item);
        }
    }

    public class ObjectPoolItem<T> : IDisposable
    {
        private readonly ObjectPool<T> _parentPool;
        public T Item { get; private set; }

        internal ObjectPoolItem(ObjectPool<T> parentPool, T theObject)
        {
            _parentPool = parentPool;
            Item = theObject;
        }
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            _parentPool.PutObject(this);
        }
    }

}
