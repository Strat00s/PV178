using hw04.Car;

namespace hw04;

public static class RaceAnalytics
{
    public static List<(RaceCar Car, TimeSpan TotalTime)> GetOrder(this Race.Race race)
    {
        var raceStats = race.GetRaceStats().GetDriverStats();
        return raceStats
            .Select(pair => (
                pair.Key,
                pair.Value.Aggregate(TimeSpan.Zero, (sum, ts) => sum + ts)
                )
            )
            .OrderBy(pair => pair.Item2)
            .ToList();
    }
    //public static List<(RaceCar Car, TimeSpan CurrentRaceTime)> GetFastestLaps(this Race.Race race)
    //{
    //    return race.RaceStats;
    //}
    //
    //public static List<(RaceCar Car, TimeSpan CurrentRaceTime)> GetOrderAt(this Race.Race race)
    //{
    //    return race.RaceStats;
    //}
    //
    //public static List<(RaceCar Car, TimeSpan CurrentRaceTime)> GetTrackPointTimes(this Race.Race race)
    //{
    //    return race.RaceStats;
    //}

}