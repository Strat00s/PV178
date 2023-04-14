using hw04.Car;
using hw04.Race;

namespace hw04.TrackPoints;

public interface ITrackPoint
{
    public string Description { get; set; }
    public async Task<TrackPointPass> PassAsync (RaceCar car);
}