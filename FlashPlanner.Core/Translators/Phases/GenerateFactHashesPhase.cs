using FlashPlanner.Core.Models;
using FlashPlanner.Core.Models.SAS;

namespace FlashPlanner.Core.Translators.Phases
{
    /// <summary>
    /// Generate a more in depth hash code for each <seealso cref="Fact"/> that exists.
    /// This is to reduce <seealso href="https://en.wikipedia.org/wiki/Hash_collision">Hash Collisions</seealso>.
    /// </summary>
    public class GenerateFactHashesPhase : BaseTranslatorPhase
    {
        public override event LogEventHandler? DoLog;
        public GenerateFactHashesPhase(LogEventHandler? doLog)
        {
            DoLog = doLog;
        }

        public override TranslatorContext ExecutePhase(TranslatorContext from) => new TranslatorContext(from) { FactHashes = GenerateFactHashes(from.SAS) };

        private int[] GenerateFactHashes(SASDecl decl)
        {
            var factHashes = new int[decl.Facts];
            foreach (var fact in decl.Init)
                if (factHashes[fact.ID] == 0)
                    factHashes[fact.ID] = Hash32shiftmult((int)fact.ID);
            foreach (var fact in decl.Goal)
                if (factHashes[fact.ID] == 0)
                    factHashes[fact.ID] = Hash32shiftmult((int)fact.ID);
            foreach (var op in decl.Operators)
            {
                var all = new List<Fact>(op.Pre);
                all.AddRange(op.Add);
                all.AddRange(op.Del);
                foreach (var fact in all)
                    if (factHashes[fact.ID] == 0)
                        factHashes[fact.ID] = Hash32shiftmult((int)fact.ID);
            }
            return factHashes;
        }

        // https://gist.github.com/badboy/6267743
        private int Hash32shiftmult(int key)
        {
            int c2 = 0x27d4eb2d; // a prime or an odd constant
            key = key ^ 61 ^ (key >>> 16);
            key = key + (key << 3);
            key = key ^ (key >>> 4);
            key = key * c2;
            key = key ^ (key >>> 15);
            return key;
        }
    }
}
