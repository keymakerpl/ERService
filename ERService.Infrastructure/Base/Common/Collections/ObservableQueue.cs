using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace ERService.Infrastructure.Base.Common.Collections
{
    public class ObservableQueue<T> : INotifyCollectionChanged, IEnumerable<T>
    {
        private readonly Queue<T> _queue;

        public ObservableQueue()
        {
            _queue = new Queue<T>();
        }

        public ObservableQueue(int capacity)
        {
            _queue = new Queue<T>(capacity);
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public T Dequeue()
        {
            var item = _queue.Dequeue();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, 0));
            return item;
        }

        public void Enqueue(T item)
        {
            _queue.Enqueue(item);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _queue.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}