using hw04.Car.Tires;
using hw04.Race;
using hw04.TrackPoints;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace hw04.Car;

public class RaceCar
{
    public string Driver { get; }
    public Team Team { get; }
    public double TurnSpeed { get; }
    public double StraigtSpeed { get; }
    private int _currentTire;


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
    public async Task StartAsync(int lapCount, Track track, SemaphoreSlim startEvent, ConcurrentQueue<(LapStats, List<TrackPointStats>)> lapStats,
                                 Stopwatch raceTimer, ThreadSafeBool finishRace)
    {
        _currentTire = 0;   //set current tire index
        var lapTrackPoints = track.GetLap(this).ToList();   //get current lap
        int nextPoint = 0;  //next track piece from which next lap track should start
        var lapTrackPointStats = new List<TrackPointStats>();

        TimeSpan tmp = TimeSpan.Zero;

        //wait for the start of the race
        await startEvent.WaitAsync();

        //driving the number of laps
        for (int lapNum = 1; lapNum < lapCount + 1; lapNum++)
        {
            for (int i = 0; i < lapTrackPoints.Count(); i++)
            {
                var passData = await lapTrackPoints[i].PassAsync(this);  //wait for the car to enter the track piece
                await Task.Delay((int)passData.DrivingTime.TotalMilliseconds);  //drive through the track piece
                lapTrackPointStats.Add(new (lapTrackPoints[i], passData.DrivingTime, passData.WaitingTime));    //generate track point data
                
                //change the tires
                //save next starting piece when going through pitlane
                if (lapTrackPoints[i] is PitLane)
                {
                    _currentTire++; //if tires run out, fix your strategy
                    nextPoint = ((PitLane)lapTrackPoints[i]).NextPoint;
                }
            }
            //Log the lap
            lapStats.Enqueue( (new(this, lapNum, raceTimer.Elapsed), lapTrackPointStats.ToList()) );

            //exit if race is over or tell everyone that you finished first and they shoudl finish their current lap
            if (lapNum == lapCount || finishRace.Value)
            {
                //if this is the first car to finish the race, signal everyone that the race is done
                if (!finishRace.Value)
                    finishRace.Value = true;
                break;
            }

            //add lap to tires if we were not in pit
            if (nextPoint == 0)
                TireStrategy[_currentTire].AddLap();

            lapTrackPoints = track.GetLap(this, nextPoint).ToList();    //get new track for new lap

            //reset everything and go for another lap
            nextPoint = 0;
            lapTrackPointStats.Clear();
        }
    }

    public Tire GetTire()
    {
        return TireStrategy[_currentTire];
    }
}