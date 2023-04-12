using hw04.Car.Tires;
using hw04.TrackPoints;

namespace hw04.Car;

public class RaceCar
{
    int _finishedLap;
    String _driver;
    Team _team;
    double _turnSpeed;
    double _straigtSpeed;
    TimeSpan _raceTime;
    List<ITrackPoint> _track;


    public ManualResetEventSlim StartEvent;



    /// <summary>
    /// Seznam pneumatik v pořadí, v jakém je bude auto při závodu měnit. 
    /// </summary>
    public List<Tire> TireStrategy { get; set; }
    

    /// <param name="driver">Jméno řidiče formule</param>
    /// <param name="team">Tým, pod který formule patří</param>
    /// <param name="turnSpeed">Rychlost auta v zatáčce</param>
    /// <param name="straightSpeed">Rychlost auta na rovince</param>
    public RaceCar(string driver, Team team, double turnSpeed, double straightSpeed, List<ITrackPoint> track)
    {
        _driver = driver;
        _team = team;
        _turnSpeed = turnSpeed;
        _straigtSpeed = straightSpeed;

        _finishedLap = 0;
        _raceTime = TimeSpan.Zero;
    }

    public async Task StartAsync(int lapCount)
    {

        int trackIndex = 0;
        bool tireChangeRequired = false;

        //wait for the start of the race
        StartEvent.Wait();

        while (true)
        {
            //keep removing time from tires
            if (tireChangeRequired && trackIndex > 10)
            {
                //go to pit
                //check if it is empty, otherwise wait
                //change the tires
                //continue and skip some parts of the track
                trackIndex = 5;
                continue;
            }

            if (_track[trackIndex] is Straight)
            {
                Straight trackPart = (Straight)_track[trackIndex];
                trackPart.AvetageTime
            }

            //race is done, exit
            if (_finishedLap == lapCount)
            {
                break;
            }
        }
    }
}