using hw04.Car;
using hw04.Race;
using System.Diagnostics;

namespace hw04.TrackPoints;

public class Turn : ITrackPoint
{
    private static readonly TimeSpan DriveInTime = TimeSpan.FromMilliseconds(5);
    public TimeSpan AverageTime { get; set; }
    public string Description { get; set; }


    private SemaphoreSlim _semaphore;


    public Turn(string description, TimeSpan averageTime, int carsAllowed)
    {
        Description = description;
        AverageTime = averageTime;
        _semaphore = new SemaphoreSlim(carsAllowed);
    }

    public Task<TrackPointPass> PassAsync(RaceCar car)
    {
        return Task.Run(async () =>
        {
            //wait to enter the turn
            var sw = new Stopwatch();
            sw.Start();
            await _semaphore.WaitAsync();
            _semaphore.Release();
            sw.Stop();
            return new TrackPointPass(this, sw.Elapsed, AverageTime * car.TurnSpeed * car.GetTire().GetSpeed() + DriveInTime);
        });

        //return new TrackPointPass(this, sw.Elapsed, AverageTime * car.TurnSpeed * car.GetTire().GetSpeed() + DriveInTime);
    }
}