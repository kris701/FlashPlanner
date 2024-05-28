namespace FlashPlanner.Tools
{
    /// <summary>
    /// A priority queue, where you can still check if a given value exists in the queue.
    /// </summary>
    public class RefPriorityQueue<T>
    {
        private readonly HashSet<T> _referenceList;
        private readonly PriorityQueue<T, int> _queue;

        /// <summary>
        /// Main constructor.
        /// </summary>
        public RefPriorityQueue()
        {
            _referenceList = new HashSet<T>();
            _queue = new PriorityQueue<T, int>();
        }

        /// <summary>
        /// Peek the top item in the queue
        /// </summary>
        /// <returns></returns>
        public T Peek() => _queue.Peek();
        /// <summary>
        /// Check if the queue contains a given item
        /// </summary>
        /// <param name="move"></param>
        /// <returns></returns>
        public bool Contains(T move) => _referenceList.Contains(move);
        /// <summary>
        /// Get the size of the queue
        /// </summary>
        public int Count => _queue.Count;

        /// <summary>
        /// Enqueue a new item with a given priority
        /// </summary>
        /// <param name="move"></param>
        /// <param name="priority"></param>
        public void Enqueue(T move, int priority)
        {
            _queue.Enqueue(move, priority);
            _referenceList.Add(move);
        }

        /// <summary>
        /// Clears the content of the queue.
        /// </summary>
        public void Clear()
        {
            _referenceList.Clear();
            _queue.Clear();
        }

        /// <summary>
        /// Dequeue the next item
        /// </summary>
        /// <returns></returns>
        public T Dequeue()
        {
            var item = _queue.Dequeue();
            _referenceList.Remove(item);
            return item;
        }
    }
}
