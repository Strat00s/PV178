//race statistics class for storing all the information

using hw04.Car;
using hw04.TrackPoints;

namespace hw04.Race
{
    public class RaceStats
    {
        public class LapData
        {
            public TimeSpan LapTime { get; }
            public TimeSpan RaceTime { get; }
            public LapData(TimeSpan lapTime, TimeSpan raceTime)
            {
                LapTime = lapTime;
                RaceTime = raceTime;
            }
        }
        public class TrackPointData
        {
            public string Driver { get; }
            public int LapNumber { get; }
            public TimeSpan DrivintTime { get; }
            public TimeSpan WaitingTime { get; }

            public TrackPointData(string driver, int lapNumber, TimeSpan drivintTime, TimeSpan waitingTime)
            {
                Driver = driver;
                LapNumber = lapNumber;
                DrivintTime = drivintTime;
                WaitingTime = waitingTime;
            }
        }
        //list of laps: driver: lap time and race time
        private readonly List<Dictionary<string, LapData>> _lapsData;    //lap statistics
        private readonly Dictionary<ITrackPoint, List<TrackPointData>> _trackPointsData;   //trackpoint statistics

        //all racecars that we care about, all trackpoints that we care about and number of laps that will be driven
        public RaceStats() 
        {
            _trackPointsData = new();
            _lapsData = new();
        }

        public void AddStats(LapReport lapReport)
        {
            //add lap data
            var lapNum = lapReport.LapNum;
            if (lapNum > _lapsData.Count)
                _lapsData.Add(new());

            var driver = lapReport.Car.Driver;
            var raceTime = lapReport.CurrentRaceTime;
            var lapTime = lapNum - 2 < 0 ? raceTime : raceTime - _lapsData[lapNum - 2][driver].RaceTime;
            _lapsData[lapNum - 1][driver] = new(lapTime, raceTime);

            //add trackpoint data
            foreach (var trackPointStat in lapReport.TrackPointReports)
            {
                if (!_trackPointsData.ContainsKey(trackPointStat.TrackPoint))
                    _trackPointsData[trackPointStat.TrackPoint] = new();

                _trackPointsData[trackPointStat.TrackPoint].Add(new(driver, lapNum, trackPointStat.DrivingTime, trackPointStat.WaitingTime));
            }
        }

        public List<Dictionary<string, LapData>> GetLapsData()
        {
            return _lapsData;
        }

        public Dictionary<ITrackPoint, List<TrackPointData>> GetTrackPointsData()
        {
            return _trackPointsData;
        }

        public void Clear()
        {
            _lapsData.Clear();
            _trackPointsData.Clear();
        }
    }
}
