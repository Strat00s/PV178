//race statistics class for storing all the information

using hw04.Car;
using hw04.TrackPoints;

namespace hw04.Race
{
    public class RaceStats
    {
        private Dictionary<string, List<(int, TimeSpan)>> _driverStats;  //driver statistics
        private Dictionary<ITrackPoint, List<(string, int, TimeSpan, TimeSpan)>> _trackPointStats;   //trackpoint statistics
        //private List<LapStats> _lapStats;

        //all racecars that we care about, all trackpoints that we care about and number of laps that will be driven
        public RaceStats(int lapNum) 
        {
            _driverStats = new();
            _trackPointStats = new();
            //_lapStats = new List<LapStats>(lapNum);
        }

        public void AddDriverStats(string driver, int lapNum, TimeSpan currentRaceTime)
        {
            if (!_driverStats.ContainsKey(driver))
                _driverStats[driver] = new List<(int, TimeSpan)>();

            var currentLapTime = currentRaceTime - _driverStats[driver].Aggregate(TimeSpan.Zero, (sum, time) => sum + time.Item2);
            _driverStats[driver].Add((lapNum, currentLapTime));
        }
        public void AddTrackPointStats(TrackPointStats trackPointStats, string driver, int lapNum)
        {
            if (!_trackPointStats.ContainsKey(trackPointStats.TrackPoint))
                _trackPointStats[trackPointStats.TrackPoint] = new List<(string, int, TimeSpan, TimeSpan)>();
            
            _trackPointStats[trackPointStats.TrackPoint].Add((driver, lapNum, trackPointStats.DrivingTime, trackPointStats.WaitingTime));
        }
        public Dictionary<string, List<(int, TimeSpan)>> GetDriverStats()
        {
            return _driverStats;
        }

        public Dictionary<ITrackPoint, List<(string, int, TimeSpan, TimeSpan)>> GetTrackPointStats()
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
