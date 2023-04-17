using hw04.Car;

namespace hw04.Race
{
    public class LapStats
    {
        public RaceCar Car{ get; }
        public int LapNum { get; }
        public TimeSpan LapTime { get; }

        public LapStats(RaceCar car, int lapNum, TimeSpan lapTime)
        {
            Car = car;
            LapNum = lapNum;
            LapTime = lapTime;
        }
    }
}
