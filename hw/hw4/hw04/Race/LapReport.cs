//class for passign lap reports between tasks

using hw04.Car;

namespace hw04.Race;

public record LapReport(RaceCar Car, int LapNumber, TimeSpan CurrentRaceTime, List<TrackPointPass> TrackPointStats);
