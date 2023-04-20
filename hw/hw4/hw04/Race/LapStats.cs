using hw04.Car;

namespace hw04.Race
{
    public class LapStats
    {
        public RaceCar Car{ get; }
        public int LapNum { get; }
        public TimeSpan RaceTime { get; }

        public LapStats(RaceCar car, int lapNum, TimeSpan lapTime)
        {
            Car = car;
            LapNum = lapNum;
            RaceTime = lapTime;
        }
    }
}
