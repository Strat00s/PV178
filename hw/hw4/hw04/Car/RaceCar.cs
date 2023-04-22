using hw04.Car.Tires;
using hw04.Race;
using hw04.TrackPoints;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace hw04.Car;

public class RaceCar
{
    private int _currentTireIndex;


    public string Driver { get; }
    public Team Team { get; }
    public double TurnSpeed { get; }
    public double StraigtSpeed { get; }


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


    public async Task StartAsync(int lapCount, Track track, SemaphoreSlim startEvent, ConcurrentQueue<LapReport> lapStats,
        Stopwatch raceTimer, ThreadSafeBool finishRace)
    {
        //set starting values
        _currentTireIndex = 0;
        var lapTrackPoints = track.GetLap(this).ToList();
        int nextPoint = 0;
        var tackPointReports = new List<TrackPointPass>();

        //wait for the start of the race
        await startEvent.WaitAsync();

        //driving the number of laps
        for (int lapNum = 1; lapNum < lapCount + 1; lapNum++)
        {
            for (int i = 0; i < lapTrackPoints.Count; i++)
            {
                //drive through track point and save report
                //var passData = await lapTrackPoints[i].PassAsync(this);
                tackPointReports.Add(await lapTrackPoints[i].PassAsync(this));
                
                //change tires
                if (lapTrackPoints[i] is PitLane lane)
                {
                    if (_currentTireIndex < TireStrategy.Count - 1)
                        _currentTireIndex++;
                    nextPoint = lane.NextPoint;
                }
            }

            lapStats.Enqueue(new(this, lapNum, raceTimer.Elapsed, tackPointReports.ToList()));  //report back the lap results

            //exit on last lap or if race is done
            if (lapNum == lapCount || finishRace.Value)
            {
                if (!finishRace.Value)
                    finishRace.Value = true;

                break;
            }

            //add lap to tires if we were not in pit
            if (nextPoint == 0)
                TireStrategy[_currentTireIndex].AddLap();

            lapTrackPoints = track.GetLap(this, nextPoint).ToList();    //get new track for new lap

            //reset everything
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
        return _currentTireIndex != TireStrategy.Count - 1 && TireStrategy[_currentTireIndex].NeedsChange();
    }
}