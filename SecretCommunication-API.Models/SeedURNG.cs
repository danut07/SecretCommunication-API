namespace SecretCommunication_API.Models
{
    public class SeedURNG
    {
        private uint seed { get; set; }
        private List<uint> obtained;
        private uint limit { get; set; }
        private Random r;
        private List<uint> exp = new List<uint>();
        public SeedURNG(uint seed, uint limit, bool b = false)
        {
            this.seed = seed;
            this.limit = limit;
            obtained = new List<uint>();
            r = new Random((int)seed);
            if (b)
            {
                for (uint i = 0; i < limit; i++)
                {
                    exp.Add(i);
                }
            }
        }

        public uint Next
        {
            get
            {
                uint x = 0;
                do
                {
                    x = (uint)r.Next(0, (int)limit);
                } while (obtained.Contains(x));
                obtained.Add(x);
                return x;
            }
        }

        public uint NextN
        {
            get
            {
                uint x = 0;
                int n = r.Next(0, exp.Count);
                x = exp[n];
                exp.RemoveAt(n);
                return x;
            }
        }
    }
}
