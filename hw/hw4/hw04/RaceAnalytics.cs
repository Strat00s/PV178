//get and return analytics data

namespace hw04;

public static class RaceAnalytics
{
    public static List<(string, TimeSpan)> GetOrder(this Race.Race race)
    {
        var data = race.GetRaceStats().GetLapsData();
        return data.Last()
            .Select(pair => (pair.Key,pair.Value.RaceTime))
            .OrderBy(pair => pair.Item2)
            .ToList();
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
        var data = race.GetRaceStats().GetLapsData();
        return data.ElementAt(lapNum - 1)
            .Select(lapData => (
                lapData.Key,
                lapData.Value.RaceTime
            ))
            .OrderBy(driverTime => driverTime.RaceTime)
            .ToList();
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