using hw04.Car;
using hw04.Race;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace hw04.TrackPoints;

public class PitLane : ITrackPoint
{
    public string Description { get; set; }

    public int NextPoint { get; }

    private ConcurrentDictionary<string, SemaphoreSlim> _boxSemaphores;

    public PitLane(string description, List<Team> teams, int nextPoint)
    {
        Description = description;
        NextPoint = nextPoint;
        _boxSemaphores = new(); 

        foreach(Team team in teams)
        {
            _boxSemaphores[team.Name] = new SemaphoreSlim(1);
        }
    }


    public Task<TrackPointPass> PassAsync(RaceCar car)
    {
        return Task.Run(async () =>
        {
            var random = new Random();
            var sw = new Stopwatch();

            //wait to enter
            sw.Start();
            await _boxSemaphores[car.Team.Name].WaitAsync();
            _boxSemaphores[car.Team.Name].Release();

            //start tire change
            Parallel.For(0, 4, async _ =>
            {
                await Task.Delay(random.Next(50, 1000));
            });
            sw.Stop();

            return new TrackPointPass(this, sw.Elapsed, TimeSpan.Zero);
        });
    }
}