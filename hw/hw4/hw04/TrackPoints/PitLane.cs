using hw04.Car;
using hw04.Race;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace hw04.TrackPoints;

public class PitLane : ITrackPoint
{
    public string Description { get; set; }

    public int NextPoint { get; }

    private readonly ConcurrentDictionary<string, SemaphoreSlim> _boxSemaphores;

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
            var sw = new Stopwatch();

            //wait to enter
            sw.Start();
            await _boxSemaphores[car.Team.Name].WaitAsync();

            //start tire change (parallel.for blocks)
            var tireTasks = new List<Task>();
            var random = new ThreadLocal<Random>(() => new Random());
            for (int i = 0; i < 4; i++)
            {
                tireTasks.Add(Task.Delay(random.Value!.Next(50, 100)));
            }

            await Task.WhenAll(tireTasks);

            _boxSemaphores[car.Team.Name].Release();
            sw.Stop();

            return new TrackPointPass(this, sw.Elapsed, TimeSpan.Zero);
        });
    }
}