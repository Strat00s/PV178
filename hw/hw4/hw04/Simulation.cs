using hw04.Car;
using hw04.Race;
using hw04.TrackPoints;

namespace hw04;

public class Simulation
{
    public event EventHandler RaceDone;

    Track _track;
    public Simulation(Track track)
    {
        _track = track;
    }

    //simulate the race
    public Race.Race SimulateRaceAsync(List<RaceCar> cars, int numberOfLaps)
    {
        //TODO ready the track
        //create list of track parts, pit is the last (easy to index)
        //TODO ready the cars
        //give them tires, track and so on

        Race.Race race = new Race.Race(cars, _track, numberOfLaps);
        race.StartRace();
        return race;
    }

    //simulate single car - strategy
    public List<Lap> SimulateLapsAsync(RaceCar car, int numberOfLaps)
    {
        throw new NotImplementedException();
    }
}