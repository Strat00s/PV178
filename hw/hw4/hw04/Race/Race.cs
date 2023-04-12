using hw04.Car;
using hw04.TrackPoints;
using System.Collections.Concurrent;
using static hw04.CurrentF1;

namespace hw04.Race;

public class Race
{
    private List<RaceCar> _cars;
    private Track _track;
    private int _numberOfLaps;
    private int _prevLap;

    public Race(List<RaceCar> cars, Track track, int numberOfLaps)
    {
        _cars = cars;
        _track = track;
        _numberOfLaps = numberOfLaps;
        _prevLap = 0;
    }
    
    public void StartRace()
    {
        var startEvent = new ManualResetEventSlim(false);   //starting "semaphore"
        var carTasks   = new List<Task>();                  //list of car tasks to wait for once the race ends

        //queue for cars to report back their lap times
        //TODO create own class
        ConcurrentQueue<Tuple<string, int, TimeSpan>> lapStats = new ConcurrentQueue<Tuple<string, int, TimeSpan>>();

        //start car threads and let them wait for start of the race
        foreach (RaceCar car in _cars)
        {
            car.StartEvent = startEvent;
            carTasks.Add(car.StartAsync(_numberOfLaps));
        }

        //prepare everything
        Tuple<string, int, TimeSpan>? stats;
        TimeSpan lapTime = TimeSpan.Zero;
        bool finishRace = false;

        Task raceDone = Task.WhenAll(carTasks);
        //start the race
        startEvent.Set();

        //TODO what if second lap starts before all other laps are done?
        while (!raceDone.IsCompleted)
        {
            if (lapStats.TryDequeue(out stats))
            {
                //maybe to this as an event to remove overhead of reading the queue
                //check if finished lap is done and end the race
                if (stats.Item2 == _numberOfLaps && !finishRace)
                {
                    //invoke event to tell cars to finish this lap (winning car should have returned by now)
                    finishRace = true;
                }
                if (_prevLap < stats.Item2)
                {
                    _prevLap = stats.Item2;
                    lapTime = stats.Item3;
                    Console.WriteLine($"Lap: {_prevLap}");
                    Console.WriteLine($"{stats.Item1}: {lapTime.Minutes:02}:{lapTime.Seconds:02}.{lapTime.Microseconds:03}");
                }

                var diff = stats.Item3.Subtract(lapTime);
                Console.WriteLine($"{stats.Item1}: +{diff.Minutes:02}:{diff.Seconds:02}.{diff.Microseconds:03}");
            }
        }
    }
}