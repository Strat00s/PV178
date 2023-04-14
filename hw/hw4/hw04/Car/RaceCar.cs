using hw04.Car.Tires;
using hw04.TrackPoints;

namespace hw04.Car;

public class RaceCar
{
    public String Driver { get; }
    public Team Team { get; }
    public double TurnSpeed { get; }
    public double StraigtSpeed { get; }
    private Tire CurrentTire;

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


    public async Task StartAsync(int lapCount, Track track)
    {
        CurrentTire = TireStrategy.First();

        //wait for the start of the race
        StartEvent.Wait();

        //driving the number of laps
        for (int lapNum = 0; lapNum < lapCount; lapNum++)
        {
            var lapPoints = track.GetLap(this).ToList();
            for (int i = 0; i < lapPoints.Count(); i++)
            {
                var passData = await lapPoints[i].PassAsync(this);  //wait for the car to enter the track piece
                await Task.Delay(((int)passData.DrivingTime.TotalMilliseconds));  //drive through the track piece
                _raceTime += passData.WaitingTime + passData.DrivingTime;

                //if in pit, change tires
                //if in finish, add age to tires
            }

            //if race is over
            //break;
        }

        //inform race that you won
        //if someone already won, skip informing tha race
    }

    public Tire getTire()
    {
        return CurrentTire;
    }
}