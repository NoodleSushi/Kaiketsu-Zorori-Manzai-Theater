using Godot;
using Godot.Collections;

namespace Utils
{
    public class RandomUniqueClass
    {
        readonly private RandomNumberGenerator RNG = new RandomNumberGenerator();
        readonly private Array<int> Pool = new Array<int>();
        readonly private int UniqueSize = 0;
        readonly private int MaxInt = 0;

        public RandomUniqueClass(int UniqueSizeIn = 5, int MaxIntIn = 5)
        {
            RNG.Randomize();
            UniqueSize = UniqueSizeIn;
            MaxInt = MaxIntIn;
            Fill();
        }

        public void Fill()
        {
            while (Pool.Count < UniqueSize)
            {
                int _NewInt = RNG.RandiRange(0, MaxInt - 1);
                // GD.Print(_NewInt);
                // GD.Print(Pool);
                if (!Pool.Contains(_NewInt))
                {
                    Pool.Add(_NewInt);
                }
            }
        }

        public int Cast()
        {
            int _X = Pool[0];
            Pool.RemoveAt(0);
            Fill();
            return _X;
        }
    }
}