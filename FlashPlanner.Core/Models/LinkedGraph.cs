namespace FlashPlanner.Core.Models
{
    public class LinkedGraph
    {
        private readonly BitMask[] _matrix;

        public LinkedGraph(int size)
        {
            _matrix = new BitMask[size];
            for (int i = 0; i < size; i++)
                _matrix[i] = new BitMask(size);
        }

        public LinkedGraph(BitMask[] other)
        {
            _matrix = other;
        }

        public BitMask this[int from] => _matrix[from];

        public void Link(int from, int to)
        {
            if (to != from)
                _matrix[from][to] = true;
        }

        public void LinkAll(int from, List<int> tos)
        {
            foreach (var to in tos)
                Link(from, to);
        }

        public float Count => _matrix.Sum(x => (float)x.GetTrueBits());
    }
}
