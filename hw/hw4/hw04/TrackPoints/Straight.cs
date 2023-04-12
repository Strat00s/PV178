using hw04.Car;
using hw04.Race;

namespace hw04.TrackPoints;

public class Straight : ITrackPoint
{
    public string Description { get; set; }
    public TimeSpan AvetageTime { get; set; }

    public Straight(string description, TimeSpan averageTime)
    {
        Description = description;
        AvetageTime = averageTime;
    }
    
    public Task<TrackPointPass> PassAsync(RaceCar car)
    {
        throw new NotImplementedException();
    }
}