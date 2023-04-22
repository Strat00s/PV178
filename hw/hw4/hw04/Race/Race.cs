using hw04.Car;
using hw04.TrackPoints;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Channels;

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

    //added as an afterthought
    private void PrintOrder(List<LapReport> reports, ref TimeSpan currentRaceTime)
    {
        bool firstCar = true;
        foreach (var report in reports)
        {
            if (firstCar)
            {
                firstCar = false;
                Console.WriteLine($"Lap: {report.LapNumber}");
                Console.WriteLine($"{report.Car.Driver}: {report.CurrentRaceTime:mm\\:ss\\.fff}");
                currentRaceTime = report.CurrentRaceTime;
                continue;
            }
            var diff = report.CurrentRaceTime - currentRaceTime;
            Console.WriteLine($"{report.Car.Driver}: +{diff:mm\\:ss\\.fff}");

        }
    }
    
    public async Task<List<Lap>> StartRaceAsync()
    {
        //"global" and important variables
        var startEvent = new SemaphoreSlim(0);
        var lapStatsCh = Channel.CreateUnbounded<LapReport>();
        var finishRace = new ThreadSafeBool(false);
        var raceTimer = new Stopwatch();
        var carTasks = new List<Task>();
        var lapResults = new Dictionary<int, List<LapReport>>();

        //start cars and let them wait for start of the race
        foreach (RaceCar car in _cars)
            carTasks.Add(car.StartAsync(_numberOfLaps, _track, startEvent, lapStatsCh, raceTimer, finishRace));

        //prepare everything else 
        LapReport lapReport;
        var currentRaceTime = TimeSpan.Zero;
        var fastestLaps = new List<Lap>();
        var raceDone = Task.WhenAll(carTasks);
        int lastPrintedLap = 0;

        //start the race
        raceTimer.Start();
        startEvent.Release(_cars.Count);
        while (!raceDone.IsCompleted || lapStatsCh.Reader.Count > 0)
        {
            if (lapStatsCh.Reader.TryRead(out lapReport!))
            {
                //save data for later
                _raceStats.AddStats(lapReport);

                //accumulate car data
                if (!lapResults.ContainsKey(lapReport.LapNumber))
                    lapResults[lapReport.LapNumber] = new();
                lapResults[lapReport.LapNumber].Add(lapReport);

                //print them once all cars finish the lap
                if (lapResults[lapReport.LapNumber].Count == _cars.Count)
                {
                    lastPrintedLap = lapReport.LapNumber;
                    PrintOrder(lapResults[lapReport.LapNumber], ref currentRaceTime);
                }
            }
            //yield control back and wait some time when there are no new data
            else
                await lapStatsCh.Reader.WaitToReadAsync();
        }
        raceTimer.Stop();
        
        //print the rest of the laps
        for (lastPrintedLap++; lastPrintedLap < lapResults.Count + 1; lastPrintedLap++)
        {
            PrintOrder(lapResults[lastPrintedLap], ref currentRaceTime);
        }

        return fastestLaps;
    }

    public RaceStats GetRaceStats()
    {
        return _raceStats;
    }
}