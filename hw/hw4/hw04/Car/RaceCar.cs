using hw04.Car.Tires;
using hw04.TrackPoints;

namespace hw04.Car;

public class RaceCar
{
    public String Driver { get; }
    public Team Team { get; }
    public double TurnSpeed { get; }
    public double StraigtSpeed { get; }
    private Tire _currentTire;

    public ManualResetEventSlim StartEvent;


    int _finishedLap;
    TimeSpan _raceTime;


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

        _raceTime = TimeSpan.Zero;
    }


    //TODO tirestrategy firstordefault when empty fix!
    public async Task StartAsync(int lapCount, Track track)
    {
        _currentTire = TireStrategy.First();
        TireStrategy.RemoveAt(0);
        var lap = track.GetLap(this).ToList();
        int nextPoint = 0;

        //wait for the start of the race
        StartEvent.Wait();

        //driving the number of laps
        for (int lapNum = 0; lapNum < lapCount; lapNum++)
        {
            for (int i = 0; i < lap.Count(); i++)
            {
                var passData = await lap[i].PassAsync(this);  //wait for the car to enter the track piece
                await Task.Delay(((int)passData.DrivingTime.TotalMilliseconds));  //drive through the track piece
                _raceTime += passData.WaitingTime + passData.DrivingTime;

                //change the tires
                //save next starting piece when going through pitlane
                if (lap[i] is PitLane)
                {
                    _currentTire = TireStrategy.First();    //if the tires run out, fix your strategy!
                    TireStrategy.RemoveAt(0);
                    nextPoint = ((PitLane)lap[i]).NextPoint;
                }
            }


            //if race is over
            //break;

            //add lap if we were not in pit
            if (nextPoint == 0)
                _currentTire.AddLap();
            
            lap = track.GetLap(this, nextPoint).ToList();
            
            nextPoint = 0;
        }

        //inform race that you won
        //if someone already won, skip informing tha race
    }

    public Tire GetTire()
    {
        return _currentTire;
    }
}