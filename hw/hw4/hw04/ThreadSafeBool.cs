//Thread safe bool used for informing the cars that the race is over

namespace hw04
{
    public class ThreadSafeBool
    {
        private int _value;

        public ThreadSafeBool(bool value)
        {
            _value = value ? 1 : 0;
        }

        public bool Value
        {
            get {return _value != 0;}
            set {Interlocked.Exchange(ref _value, value ? 1 : 0);}
        }
    }
}
