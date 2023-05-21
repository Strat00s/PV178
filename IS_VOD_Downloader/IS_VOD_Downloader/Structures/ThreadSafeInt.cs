using System.Threading;

namespace IS_VOD_Downloader.Structures
{
    public class ThreadSafeInt
    {
        private int _value;

        public ThreadSafeInt(int value = 0)
        {
            _value = value;
        }

        public int Value
        {
            get { return Interlocked.CompareExchange(ref _value, 0, 0); }
            set { Interlocked.Exchange(ref _value, value); }
        }

        public void Increment()
        {
            Interlocked.Increment(ref _value);
        }

        public void Decrement()
        {
            Interlocked.Decrement(ref _value);
        }

        public void Add(int amount)
        {
            Interlocked.Add(ref _value, amount);
        }
    }
}