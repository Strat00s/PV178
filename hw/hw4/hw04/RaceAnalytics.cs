//get and return analytics data

using System.Runtime.CompilerServices;
using static hw04.Race.RaceStats;

namespace hw04;

public static class RaceAnalytics
{
    public static List<(string, TimeSpan)> GetOrder(this Race.Race race, int lapNum = 0)
    {
        var data = race.GetRaceStats().GetLapsData();
        if (lapNum < 1)
            lapNum = data.Count;

        var finishers = data.ElementAt(lapNum - 1)
            .Select(pair => (pair.Key, pair.Value.RaceTime))
            .OrderBy(pair => pair.RaceTime)
            .ToList();

        var dnf = data.Take(lapNum)
            .SelectMany(lap => lap)
            .Where(driverTime => !finishers.Any(fin => fin.Key == driverTime.Key))
            .OrderByDescending(pair => pair.Value.RaceTime)
            .GroupBy(driverTime => driverTime.Key)
            .Select(group => (group.Key, TimeSpan.MinValue))
            .Reverse()
            .ToList();

        return finishers.Concat(dnf).ToList();
    }

    public static List<(string, TimeSpan)> GetFastestLaps(this Race.Race race)
    {
        var data = race.GetRaceStats().GetLapsData();
        return data.Select(dict => dict
                .Select(pair => (pair.Key, pair.Value.LapTime))
                .OrderBy(pair => pair.LapTime)
                .First()
            )
            .ToList();
    }

    public static List<(string, TimeSpan)> GetOrderAt(this Race.Race race, int lapNum)
    {
        return GetOrder(race, lapNum);
    }

    public static List<(string, string, int, TimeSpan, string, int, TimeSpan)> GetTrackPointTimes(this Race.Race race)
    {
        var data = race.GetRaceStats().GetTrackPointsData();

        return data.Select(dict => (
                dict.Key.Description,
                dict.Value.OrderBy(stats => stats.DrivintTime).First(),    //shortest drive time
                dict.Value.OrderBy(stats => stats.WaitingTime).Last()    //longest wait time
            ))
            .Select(tup => (
                tup.Description,
                tup.Item2.Driver,
                tup.Item2.LapNumber,
                tup.Item2.DrivintTime,
                tup.Item3.Driver,
                tup.Item3.LapNumber,
                tup.Item3.WaitingTime
            ))
            .ToList();
    }
}