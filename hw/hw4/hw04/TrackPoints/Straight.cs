using hw04.Car;
using hw04.Race;
using System.Diagnostics;
using System.Threading;

namespace hw04.TrackPoints;

public class Straight : ITrackPoint
{
    public string Description { get; set; }
    public TimeSpan AverageTime { get; set; }

    public Straight(string description, TimeSpan averageTime)
    {
        Description = description;
        AverageTime = averageTime;
    }
    
    public Task<TrackPointPass> PassAsync(RaceCar car)
    {
        return Task.Run(async () =>
        {
            var drivingTime = AverageTime * car.StraigtSpeed * car.GetTire().GetSpeed();
            await Task.Delay(drivingTime);  //drive through
            return (new TrackPointPass(this, TimeSpan.Zero, drivingTime));
        });
    }
}