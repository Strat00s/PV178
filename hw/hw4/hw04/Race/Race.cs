using hw04.Car;
using hw04.TrackPoints;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace hw04.Race;

public class Race
{
    private readonly RaceStats _raceStats;
    private readonly List<RaceCar> _cars;
    private readonly Track _track;
    private readonly int _numberOfLaps;

    public Race(List<RaceCar> cars, Track track, int numberOfLaps)
    {
        _cars = cars;
        _track = track;
        _numberOfLaps = numberOfLaps;
        _raceStats = new RaceStats();
    }
    
    public async Task<List<Lap>> StartRaceAsync()
    {
        //"global" and important variables
        var startEvent = new SemaphoreSlim(0);
        var lapStatsQ = new ConcurrentQueue<LapReport>();
        var finishRace = new ThreadSafeBool(false);
        var raceTimer = new Stopwatch();
        var carTasks = new List<Task>();

        //start cars and let them wait for start of the race
        foreach (RaceCar car in _cars)
            carTasks.Add(car.StartAsync(_numberOfLaps, _track, startEvent, lapStatsQ, raceTimer, finishRace));

        //prepare everything else 
        LapReport lapReport;
        var currentRaceTime = TimeSpan.Zero;
        int prevLap = 0;
        var fastestLaps = new List<Lap>();
        var raceDone = Task.WhenAll(carTasks);

        //start the race
        raceTimer.Start();
        startEvent.Release(_cars.Count);
        while (!raceDone.IsCompleted || !lapStatsQ.IsEmpty)
        {
            if (lapStatsQ.TryDequeue(out lapReport!))
            {
                //save data for later
                _raceStats.AddStats(lapReport);

                if (prevLap < lapReport.LapNumber)
                {
                    prevLap = lapReport.LapNumber;
                    currentRaceTime = lapReport.CurrentRaceTime;
                    Console.WriteLine($"Lap: {prevLap}");
                    Console.WriteLine($"{lapReport.Car.Driver}: {currentRaceTime:mm\\:ss\\:fff}");
                    fastestLaps.Add(new(lapReport.Car, lapReport.LapNumber));
                    continue;
                }

                var diff = lapReport.CurrentRaceTime - currentRaceTime;
                Console.WriteLine($"{lapReport.Car.Driver}: +{diff:mm\\:ss\\:fff}");
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