//race statistics class for storing all the information

using hw04.Car;
using hw04.TrackPoints;

namespace hw04.Race
{
    public class RaceStats
    {
        public record LapData(TimeSpan LapTime, TimeSpan RaceTime);
        public record TrackPointData(string Driver, int LapNumber, TimeSpan DrivintTime, TimeSpan WaitingTime);


        private readonly List<Dictionary<string, LapData>> _lapsData;
        private readonly Dictionary<ITrackPoint, List<TrackPointData>> _trackPointsData;


        public RaceStats() 
        {
            _trackPointsData = new();
            _lapsData = new();
        }


        public void AddStats(LapReport lapReport)
        {
            //add lap data
            var lapNum = lapReport.LapNumber;
            if (lapNum > _lapsData.Count)
                _lapsData.Add(new());

            var driver = lapReport.Car.Driver;
            var raceTime = lapReport.CurrentRaceTime;
            var lapTime = lapNum - 2 < 0 ? raceTime : raceTime - _lapsData[lapNum - 2][driver].RaceTime;
            _lapsData[lapNum - 1][driver] = new(lapTime, raceTime);

            //add trackpoint data
            foreach (var trackPointStat in lapReport.TrackPointStats)
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
