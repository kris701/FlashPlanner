namespace FlashPlanner.Tools
{
    public class RefPriorityQueue
    {
        private readonly HashSet<StateMove> _referenceList;
        private readonly PriorityQueue<StateMove, int> _queue;

        public RefPriorityQueue()
        {
            _referenceList = new HashSet<StateMove>();
            _queue = new PriorityQueue<StateMove, int>();
        }

        public StateMove Peek() => _queue.Peek();
        public void EnsureCapacity(int value) => _queue.EnsureCapacity(value);
        public bool Contains(StateMove move) => _referenceList.Contains(move);
        public int Count => _queue.Count;
        public void Clear()
        {
            _referenceList.Clear();
            _queue.Clear();
        }

        public void Enqueue(StateMove move, int priority)
        {
            _queue.Enqueue(move, priority);
            _referenceList.Add(move);
        }

        public StateMove Dequeue()
        {
            var item = _queue.Dequeue();
            _referenceList.Remove(item);
            return item;
        }
    }
}
