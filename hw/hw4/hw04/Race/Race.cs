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
    private static void PrintOrder(List<LapReport> reports, ref TimeSpan currentRaceTime)
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
        var startSemaphore = new SemaphoreSlim(0);
        var lapReportsCh = Channel.CreateUnbounded<LapReport>();
        var raceIsDone = new ThreadSafeBool(false);
        var raceTimer = new Stopwatch();
        var carTasks = new List<Task>();

        //start cars and let them wait for start of the race
        foreach (RaceCar car in _cars)
            carTasks.Add(car.StartAsync(_numberOfLaps, _track, startSemaphore, lapReportsCh, raceTimer, raceIsDone));

        //prepare everything else 
        LapReport lapReport;
        var currentRaceTime = TimeSpan.Zero;
        var lapList = new List<Lap>();
        var carsDone = Task.WhenAll(carTasks);
        int lastPrintedLap = 0;
        var lapResults = new Dictionary<int, List<LapReport>>();

        await Task.Delay(10);   //this should be more than enough for all cars to get ready

        //start the race
        raceTimer.Start();
        startSemaphore.Release(_cars.Count);    //maybe barrier would be better?
        while (!carsDone.IsCompleted || lapReportsCh.Reader.Count > 0)
        {
            if (lapReportsCh.Reader.TryRead(out lapReport!))
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
                    lapList.Add(new(lapReport.Car, lapReport.LapNumber));
                    lastPrintedLap = lapReport.LapNumber;
                    PrintOrder(lapResults[lapReport.LapNumber], ref currentRaceTime);
                }
            }
            else
                await Task.WhenAny(lapReportsCh.Reader.WaitToReadAsync().AsTask(), carsDone);
        }

        raceTimer.Stop();

        //print the rest of the laps
        for (lastPrintedLap++; lastPrintedLap < lapResults.Count + 1; lastPrintedLap++)
        {
            PrintOrder(lapResults[lastPrintedLap], ref currentRaceTime);
        }

        return lapList;
    }

    public RaceStats GetRaceStats()
    {
        return _raceStats;
    }
}
