//race statistics class for storing all the information

using hw04.Car;
using hw04.TrackPoints;

namespace hw04.Race
{
    public class RaceStats
    {
        private Dictionary<RaceCar, List<TimeSpan>> _driverStats;  //driver statistics
        private Dictionary<ITrackPoint, List<(TimeSpan, TimeSpan)>> _trackPointStats;   //trackpoint statistics

        //all racecars that we care about, all trackpoints that we care about and number of laps that will be driven
        public RaceStats() 
        {
            _driverStats = new();
            _trackPointStats = new();
        }

        public void AddDriverStats(RaceCar car, TimeSpan currentRaceTime)
        {
            if (!_driverStats.ContainsKey(car))
                _driverStats[car] = new List<TimeSpan>();

            var currentLapTime = currentRaceTime - _driverStats[car].Aggregate(TimeSpan.Zero, (sum, time) => sum + time);
            _driverStats[car].Add(currentLapTime);
        }
        public void AddTrackPointStats(ITrackPoint trackPoint, TimeSpan drivingTime, TimeSpan waitingTime)
        {
            if (!_trackPointStats.ContainsKey(trackPoint))
                _trackPointStats[trackPoint] = new List<(TimeSpan, TimeSpan)>();
            
            _trackPointStats[trackPoint].Add((drivingTime, waitingTime));
        }
        public Dictionary<RaceCar, List<TimeSpan>> GetDriverStats()
        {
            return _driverStats;
        }

        public Dictionary<ITrackPoint, List<(TimeSpan, TimeSpan)>> GetTrackPointStat()
        {
            return _trackPointStats;
        }

        public void Clear()
        {
            _driverStats.Clear();
            _trackPointStats.Clear();
        }
    }
}
