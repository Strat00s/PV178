using hw04.Car.Tires;
using hw04.Race;
using hw04.TrackPoints;
using System.Collections.Concurrent;

namespace hw04.Car;

public class RaceCar
{
    public String Driver { get; }
    public Team Team { get; }
    public double TurnSpeed { get; }
    public double StraigtSpeed { get; }
    private int _currentTire;

    public SemaphoreSlim StartEvent;


    int _finishedLap;


    /// <summary>
    /// Seznam pneumatik v pořadí, v jakém je bude auto při závodu měnit. 
    /// </summary>
    public List<Tire> TireStrategy { get; set; }
    

    /// <param name="driver">Jméno řidiče formule</param>
    /// <param name="team">Tým, pod který formule patří</param>
    /// <param name="turnSpeed">Rychlost auta v zatáčce</param>
    /// <param name="straightSpeed">Rychlost auta na rovince</param>
    public RaceCar(string driver, Team team, double turnSpeed, double straightSpeed)
    {
        Driver = driver;
        Team = team;
        TurnSpeed = turnSpeed;
        StraigtSpeed = straightSpeed;
    }


    //TODO tirestrategy firstordefault when empty fix!
    public async Task StartAsync(int lapCount, Track track, ConcurrentQueue<LapStats> lapStats)
    {
        _currentTire = 0;
        var lap = track.GetLap(this).ToList();
        int nextPoint = 0;

        //TimeSpan lapTime = TimeSpan.Zero;
        TimeSpan raceTime = TimeSpan.Zero;

        //wait for the start of the race
        await StartEvent.WaitAsync();

        //driving the number of laps
        for (int lapNum = 0; lapNum < lapCount; lapNum++)
        {
            for (int i = 0; i < lap.Count(); i++)
            {
                var passData = await lap[i].PassAsync(this);  //wait for the car to enter the track piece
                await Task.Delay((int)passData.DrivingTime.TotalMilliseconds);  //drive through the track piece
                raceTime += passData.WaitingTime + passData.DrivingTime;

                //change the tires
                //save next starting piece when going through pitlane
                if (lap[i] is PitLane)
                {
                    _currentTire++; //if tires run out, fix your strategy
                    nextPoint = ((PitLane)lap[i]).NextPoint;
                }
            }
            //Console.WriteLine($"{Driver}: {raceTime}");
            //Log the race
            lapStats.Enqueue(new(this, lapNum + 1, raceTime));

            //if race is over
            //break;
            if (lapNum + 1 == lapCount) 
            {
                break;
            }

            //add lap to tires if we were not in pit
            if (nextPoint == 0)
                TireStrategy[_currentTire].AddLap();
            
            //get new lap
            lap = track.GetLap(this, nextPoint).ToList();

            //"reset" everything
            nextPoint = 0;
            //lapTime = TimeSpan.Zero;
        }

        //inform race that you won
        //if someone already won, skip informing tha race

        _currentTire = 0;
    }

    public Tire GetTire()
    {
        return TireStrategy[_currentTire];
    }
}