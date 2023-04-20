﻿using hw04.Car;
using hw04.TrackPoints;

namespace hw04.Race
{
    public class LapStats
    {
        public RaceCar Car{ get; }
        public int LapNum { get; }
        public TimeSpan CurrentRaceTime { get; }
        public List<(ITrackPoint, TimeSpan, TimeSpan)> TrackPointTimes { get; }

        public LapStats(RaceCar car, int lapNum, TimeSpan lapTime, List<(ITrackPoint, TimeSpan, TimeSpan)> trackPointTimes)
        {
            Car = car;
            LapNum = lapNum;
            CurrentRaceTime = lapTime;
            TrackPointTimes = trackPointTimes;
        }
    }
}
