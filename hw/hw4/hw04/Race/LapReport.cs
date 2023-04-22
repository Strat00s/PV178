using hw04.Car;

namespace hw04.Race
{
    public class LapReport
    {
        public RaceCar Car { get; }
        public int LapNum { get; }
        public TimeSpan CurrentRaceTime { get; }
        public List<TrackPointReport> TrackPointReports { get; }

        public LapReport(RaceCar car, int lapNum, TimeSpan currentRaceTime, List<TrackPointReport> trackPointStats)
        {
            Car = car;
            LapNum = lapNum;
            CurrentRaceTime = currentRaceTime;
            TrackPointReports = trackPointStats;
        }
    }
}
