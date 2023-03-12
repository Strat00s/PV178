namespace Patterns_03
{
    internal abstract class Car
    {
        protected Car(double accelerationTime)
        {
            AccelerationTime = accelerationTime;
        }

        public double AccelerationTime { get; }
    }
}
