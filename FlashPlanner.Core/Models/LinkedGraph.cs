namespace FlashPlanner.Core.Models
{
    public class LinkedGraph
    {
        private readonly BitMask[] _matrix;

        public LinkedGraph(uint size)
        {
            _matrix = new BitMask[size];
            for (uint i = 0; i < size; i++)
                _matrix[i] = new BitMask(size);
        }

        public LinkedGraph(BitMask[] other)
        {
            _matrix = other;
        }

        public BitMask this[uint from] => _matrix[from];

        public void Link(uint from, uint to)
        {
            if (to != from)
                _matrix[from][to] = true;
        }

        public void LinkAll(uint from, List<uint> tos)
        {
            foreach (var to in tos)
                Link(from, to);
        }

        public void GenerateBounds()
        {
            for (int i = 0; i < _matrix.Length; i++)
                _matrix[i].GenerateBounds();
        }

        public float Count => _matrix.Sum(x => (float)x.GetTrueBits());
    }
}
