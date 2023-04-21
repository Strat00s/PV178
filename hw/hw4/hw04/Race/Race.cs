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
        var lapStatsQ = new ConcurrentQueue<(LapStats, List<TrackPointStats>)>();    //queue for cars to report back their lap statistics
        var finishRace = new ThreadSafeBool(false);

        //"global" timer to fix the fact that some cars have better times but due to overhead arrive later
        //https://discord.com/channels/1063366519255470100/1097849859337355354/1097849863686848565
        //this take overhead into account, but that a possible sollution too
        var raceTimer = new Stopwatch();

        var carTasks = new List<Task>();  //list of car tasks to wait for once the race ends

        //start car threads and let them wait for start of the race
        foreach (RaceCar car in _cars)
        {
            carTasks.Add(car.StartAsync(_numberOfLaps, _track, startEvent, lapStatsQ, raceTimer, finishRace));
        }

        //prepare everything else 
        (LapStats, List<TrackPointStats>) lapStats;  //var for storing lap statistics from cars
        var currentRaceTime = TimeSpan.Zero;  //how long the race is taking
        int prevLap = 0;
        var fastestLaps = new List<Lap>();  //no idea data is the List<Lap>() suppose to contain
        var raceDone = Task.WhenAll(carTasks); //task for checking if all cars ended

        //start the race
        raceTimer.Start();
        startEvent.Release(_cars.Count());
        while (!raceDone.IsCompleted || !lapStatsQ.IsEmpty)
        {
            if (lapStatsQ.TryDequeue(out lapStats!))
            {
                //save data for later analytics
                _raceStats.AddDriverStats(lapStats.Item1.Car.Driver, lapStats.Item1.LapNum, lapStats.Item1.CurrentRaceTime);
                foreach (var trackPointStat in lapStats.Item2)
                    _raceStats.AddTrackPointStats(trackPointStat, lapStats.Item1.Car.Driver, lapStats.Item1.LapNum);

                if (prevLap < lapStats.Item1.LapNum)
                {
                    prevLap = lapStats.Item1.LapNum;
                    currentRaceTime = lapStats.Item1.CurrentRaceTime;
                    Console.WriteLine($"Lap: {prevLap}");
                    Console.WriteLine($"{lapStats.Item1.Car.Driver}: {currentRaceTime}");
                    fastestLaps.Add(new(lapStats.Item1.Car, lapStats.Item1.LapNum));
                    continue;
                }

                var diff = lapStats.Item1.CurrentRaceTime - currentRaceTime;
                Console.WriteLine($"{lapStats.Item1.Car.Driver}: +{diff}");
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