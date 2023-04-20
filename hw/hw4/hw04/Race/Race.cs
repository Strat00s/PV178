using hw04.Car;
using hw04.TrackPoints;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace hw04.Race;

public class Race
{
    private List<RaceCar> _cars;
    private Track _track;
    private int _numberOfLaps;

    public Race(List<RaceCar> cars, Track track, int numberOfLaps)
    {
        _cars = cars;
        _track = track;
        _numberOfLaps = numberOfLaps;
    }
    
    public List<Lap> StartRace()
    {
        var startEvent = new SemaphoreSlim(0);   //starting "semaphore"
        var carTasks   = new List<Task>();  //list of car tasks to wait for once the race ends
        var lapStatsQ = new ConcurrentQueue<LapStats>();    //queue for cars to report back their lap times

        //"global" timer to fix the fact that some cars have better times but due to overhead arrive later
        //https://discord.com/channels/1063366519255470100/1097849859337355354/1097849863686848565
        //this take overhead into account, but that a possible sollution too
        var raceTimer = new Stopwatch();

        //start car threads and let them wait for start of the race
        foreach (RaceCar car in _cars)
        {
            car.StartEvent = startEvent;
            carTasks.Add(car.StartAsync(_numberOfLaps, _track, lapStatsQ, raceTimer));
        }

        //prepare everything
        LapStats lapStats;
        TimeSpan lapTime = TimeSpan.Zero;
        bool finishRace = false;
        int prevLap = 0;

        List<Lap> fastestLaps = new List<Lap>();

        Task raceDone = Task.WhenAll(carTasks);


        //start the race
        raceTimer.Start();
        startEvent.Release(_cars.Count());

        //TODO what if second lap starts before all other laps are done?
        while (!raceDone.IsCompleted)
        {
            if (lapStatsQ.TryDequeue(out lapStats!))
            {
                //maybe to this as an event to remove overhead of reading the queue
                //check if finished lap is done and end the race
                if (lapStats.LapNum == _numberOfLaps && !finishRace)
                {
                    //invoke event to tell cars to finish this lap (winning car should have returned by now)
                    //TODO
                    finishRace = true;
                }

                if (prevLap < lapStats.LapNum)
                {
                    prevLap = lapStats.LapNum;
                    lapTime = lapStats.RaceTime;
                    Console.WriteLine($"Lap: {prevLap}");
                    Console.WriteLine($"{lapStats.Car.Driver}: {lapTime}");
                    fastestLaps.Add(new(lapStats.Car, lapStats.LapNum));
                    continue;
                }

                var diff = lapStats.RaceTime - lapTime;
                Console.WriteLine($"{lapStats.Car.Driver}: +{diff}");
            }
        }

        raceTimer.Stop();

        //Console.WriteLine("Race done");

        return fastestLaps;
    }
}