namespace FlashPlanner.Models
{
    /// <summary>
    /// In a max priority queue, elements are inserted in the order in which they arrive the queue and the maximum value is always removed first from the queue. For example, assume that we insert in the order 8, 3, 2, 5 and they are removed in the order 8, 5, 3, 2.
    /// When the queue is full, the largest element is removed first.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FixedMaxPriorityQueue<T>
    {
        /// <summary>
        /// Amount of items in the queue
        /// </summary>
        public int Count => _keys.Count;
        /// <summary>
        /// The max size of the queue
        /// </summary>
        public int Size { get; }

        private readonly List<int> _keys;
        private readonly List<T> _values;

        /// <summary>
        /// Main constructor
        /// </summary>
        /// <param name="size">Maximum Size of the queue</param>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="size"/> is lower than zero.</exception>
        public FixedMaxPriorityQueue(int size)
        {
            if (size <= 0)
                throw new ArgumentOutOfRangeException("Size must be larger than 0!");
            Size = size;
            _keys = new List<int>(size);
            _values = new List<T>(size);
        }

        /// <summary>
        /// Enqueue a new <paramref name="value"/> with the priority key <paramref name="key"/>
        /// </summary>
        /// <param name="value">The value</param>
        /// <param name="key">The priority</param>
        public void Enqueue(T value, int key)
        {
            int insertIndex = GetInsertIndex(key);
            _keys.Insert(insertIndex, key);
            _values.Insert(insertIndex, value);

            if (Count > Size)
            {
                _keys.RemoveAt(0);
                _values.RemoveAt(0);
            }
        }

        private int GetInsertIndex(int key)
        {
            for (int i = 0; i < Count; i++)
                if (key > _keys[i])
                    return i;
            return Count;
        }

        /// <summary>
        /// Get the item at the top of the queue
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException">If the queue is empty</exception>
        public T Dequeue()
        {
            if (_keys.Count <= 0)
                throw new ArgumentException("The queue is empty!");
            var returnValue = _values[0];
            _values.RemoveAt(0);
            _keys.RemoveAt(0);
            return returnValue;
        }
    }
}
