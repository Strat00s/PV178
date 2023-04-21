//This is awful. Don't look at it. I didn't want to deal with it so I just started adding stuff up into everything to get the data I need
//Analytics bad...

using hw04.Car;
using hw04.TrackPoints;

namespace hw04;

public static class RaceAnalytics
{
    //Finishing order
    public static List<(string, TimeSpan)> GetOrder(this Race.Race race)
    {
        var raceStats = race.GetRaceStats().GetDriverStats();
        return raceStats
            .Select(pair => (
                pair.Key,
                pair.Value.Aggregate(TimeSpan.Zero, (sum, pair) => sum + pair.Item2)
                )
            )
            .OrderBy(pair => pair.Item2)
            .ToList();
    }

    //Fastest lap
    public static List<(string, int)> GetFastestLaps(this Race.Race race)
    {
        var raceStats = race.GetRaceStats().GetDriverStats();
        return raceStats
            .Select(pair => (
                pair.Key,
                pair.Value.MinBy(lapTs => lapTs.Item2).Item1
                )
            )
            .ToList();
    }
    
    //Order at specific lap
    public static List<(String, TimeSpan)> GetOrderAt(this Race.Race race, int lapNum)
    {
        var raceStats = race.GetRaceStats().GetDriverStats();
        return raceStats
            .Select(pair => (
                pair.Key,
                pair.Value
                    .Take(lapNum)
                    .Aggregate(TimeSpan.Zero, (sum, ts) => sum + ts.Item2)
                )
            )
            .OrderBy(pair => pair.Item2)
            .ToList();
    }
    
    //Fastest driver at a trackpoint
    public static List<(ITrackPoint, string, TimeSpan, string, TimeSpan)> GetTrackPointTimes(this Race.Race race)
    {
        var data = race.GetRaceStats().GetTrackPointStats();

        List<(ITrackPoint, string, TimeSpan, string, TimeSpan)> result = data.Select(kv => {
            ITrackPoint key = kv.Key;
            List<(string, int, TimeSpan, TimeSpan)> valueList = kv.Value;

            TimeSpan minTimeSpan1 = valueList.Min(item => item.Item3);
            (string, int, TimeSpan, TimeSpan) minTuple = valueList.First(item => item.Item3 == minTimeSpan1);

            TimeSpan maxTimeSpan2 = valueList.Max(item => item.Item4);
            (string, int, TimeSpan, TimeSpan) maxTuple = valueList.First(item => item.Item4 == maxTimeSpan2);

            return (key, minTuple.Item1, minTuple.Item3, maxTuple.Item1, maxTuple.Item4);
        }).ToList();
        return result;
    }

}