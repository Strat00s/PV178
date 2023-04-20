//This class seems pretty useless

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
    public async Task<Race.Race> SimulateRaceAsync(List<RaceCar> cars, int numberOfLaps)
    {
        Race.Race race = new Race.Race(cars, _track, numberOfLaps);
        await race.StartRaceAsync();
        return race;
    }

    //simulate single car - strategy
    public Task<List<Lap>> SimulateLapsAsync(RaceCar car, int numberOfLaps)
    {
        Race.Race race = new Race.Race(new() { car }, _track, numberOfLaps);
        return race.StartRaceAsync();
    }
}