using hw04.Car;
using hw04.TrackPoints;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace hw04.Race;

public class Race
{
    private RaceStats _raceStats;
    private List<RaceCar> _cars;
    private Track _track;
    private int _numberOfLaps;

    public Race(List<RaceCar> cars, Track track, int numberOfLaps)
    {
        _cars = cars;
        _track = track;
        _numberOfLaps = numberOfLaps;
        _raceStats = new RaceStats(numberOfLaps);
    }
    
    public async Task<List<Lap>> StartRaceAsync()
    {
        //"global" variables for synchronizing data between tasks
        var startEvent = new SemaphoreSlim(0);   //starting semaphore
        var lapStatsQ = new ConcurrentQueue<LapReport>();    //queue for cars to report back their lap statistics
        var finishRace = new ThreadSafeBool(false);
        var raceTimer = new Stopwatch();    //global timer for lap tracking

        var carTasks = new List<Task>();  //list of car tasks to wait for once the race ends

        //start car threads and let them wait for start of the race
        foreach (RaceCar car in _cars)
        {
            carTasks.Add(car.StartAsync(_numberOfLaps, _track, startEvent, lapStatsQ, raceTimer, finishRace));
        }

        //prepare everything else 
        LapReport lapReport;  //var for storing lap statistics from cars
        var currentRaceTime = TimeSpan.Zero;  //how long the race is taking
        int prevLap = 0;
        var fastestLaps = new List<Lap>();  //no idea data is the List<Lap>() suppose to contain
        var raceDone = Task.WhenAll(carTasks); //task for checking if all cars ended

        //start the race
        raceTimer.Start();
        startEvent.Release(_cars.Count());
        while (!raceDone.IsCompleted || !lapStatsQ.IsEmpty)
        {
            if (lapStatsQ.TryDequeue(out lapReport!))
            {
                //save data for later
                _raceStats.AddStats(lapReport);

                if (prevLap < lapReport.LapNum)
                {
                    prevLap = lapReport.LapNum;
                    currentRaceTime = lapReport.CurrentRaceTime;
                    Console.WriteLine($"Lap: {prevLap}");
                    Console.WriteLine($"{lapReport.Car.Driver}: {currentRaceTime}");
                    fastestLaps.Add(new(lapReport.Car, lapReport.LapNum));
                    continue;
                }

                var diff = lapReport.CurrentRaceTime - currentRaceTime;
                Console.WriteLine($"{lapReport.Car.Driver}: +{diff}");
            }
            //yield control back and wait some time when there are no new data
            else
                await Task.Delay(10);
        }

        raceTimer.Stop();
        return fastestLaps;
    }

    public RaceStats GetRaceStats()
    {
        return _raceStats;
    }
}