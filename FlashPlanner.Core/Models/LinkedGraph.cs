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

        public BitMask this[int from] => _matrix[from];

        public void LinkAll(int from, List<int> tos)
        {
            foreach (var to in tos)
                if (to != from)
                    _matrix[from][to] = true;
        }

        public int Count => _matrix.Sum(x => x.GetTrueBits());
    }
}
