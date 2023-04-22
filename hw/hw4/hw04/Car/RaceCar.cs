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
    private int _currentTireIndex;


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
    public async Task StartAsync(int lapCount, Track track, SemaphoreSlim startEvent, ConcurrentQueue<LapReport> lapStats,
                                 Stopwatch raceTimer, ThreadSafeBool finishRace)
    {
        _currentTireIndex = 0;   //set current tire index
        var lapTrackPoints = track.GetLap(this).ToList();   //get current lap
        int nextPoint = 0;  //next track piece from which next lap track should start
        var tackPointReports = new List<TrackPointReport>();

        TimeSpan tmp = TimeSpan.Zero;

        //wait for the start of the race
        await startEvent.WaitAsync();

        //driving the number of laps
        for (int lapNum = 1; lapNum < lapCount + 1; lapNum++)
        {
            for (int i = 0; i < lapTrackPoints.Count(); i++)
            {
                var passData = await lapTrackPoints[i].PassAsync(this);  //drive through track point
                tackPointReports.Add(new(lapTrackPoints[i], passData.DrivingTime, passData.WaitingTime));  //save trackpoint data
                
                //change the tires
                //save next starting piece when going through pitlane
                if (lapTrackPoints[i] is PitLane)
                {
                    if (_currentTireIndex < TireStrategy.Count - 1)
                        _currentTireIndex++;
                    nextPoint = ((PitLane)lapTrackPoints[i]).NextPoint;
                }
            }
            //Log the lap result
            lapStats.Enqueue(new(this, lapNum, raceTimer.Elapsed, tackPointReports.ToList()));

            //exit on last lap or if race is done
            if (lapNum == lapCount || finishRace.Value)
            {
                //inform everyone that race is done
                if (!finishRace.Value)
                    finishRace.Value = true;
                break;
            }

            //add lap to tires if we were not in pit
            if (nextPoint == 0)
                TireStrategy[_currentTireIndex].AddLap();

            lapTrackPoints = track.GetLap(this, nextPoint).ToList();    //get new track for new lap

            //reset everything and go for another lap
            nextPoint = 0;
            tackPointReports.Clear();
        }
    }

    public Tire GetTire()
    {
        return TireStrategy[_currentTireIndex];
    }

    public bool NeedsChange()
    {
        //dont change tires on last set
        return _currentTireIndex == TireStrategy.Count - 1 ? false : TireStrategy[_currentTireIndex].NeedsChange();
    }
}