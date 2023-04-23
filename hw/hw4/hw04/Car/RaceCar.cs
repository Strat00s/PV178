using hw04.Car.Tires;
using hw04.Race;
using hw04.TrackPoints;
using System.Diagnostics;
using System.Threading.Channels;

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


    public async Task StartAsync(int numberOfLaps, Track track, SemaphoreSlim startSemaphore, Channel<LapReport> lapReportsCh,
        Stopwatch raceTimer, ThreadSafeBool raceIsDone)
    {
        //set starting values
        _currentTireIndex = 0;
        var lapTrackPoints = track.GetLap(this, false);
        var tackPointReports = new List<TrackPointPass>();
        bool inPit = false;

        //wait for the start of the race
        await startSemaphore.WaitAsync();

        //driving the number of laps
        for (int lapNum = 1; lapNum < numberOfLaps + 1; lapNum++)
        {
            for (int i = 0; i < lapTrackPoints.Count; i++)
            {
                //drive through track point and save report
                tackPointReports.Add(await lapTrackPoints[i].PassAsync(this));
                
                //change tires
                if (lapTrackPoints[i] is PitLane)
                {
                    if (_currentTireIndex < TireStrategy.Count - 1)
                        _currentTireIndex++;
                    inPit = false;
                }
            }

            lapReportsCh.Writer.TryWrite(new(this, lapNum, raceTimer.Elapsed, tackPointReports.ToList()));

            //exit on last lap or if race is done
            if (lapNum == numberOfLaps || raceIsDone.Value)
            {
                if (!raceIsDone.Value)
                    raceIsDone.Value = true;

                break;
            }

            //add lap when not in pit
            TireStrategy[_currentTireIndex].AddLap();

            if (lapTrackPoints.Last().Description == "PitLane Entry")
                inPit = true;

            lapTrackPoints = track.GetLap(this, inPit).ToList();    //get new track for new lap
            
            //reset everything
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