using hw04.Car;

namespace hw04.TrackPoints;

public class Track
{
    private readonly List<ITrackPoint> _trackPoints;
    private Turn? _pitLaneEntry;
    private Turn? _pitLaneExit;
    private PitLane? _pitLane;

    public Track()
    {
        _trackPoints = new List<ITrackPoint>();
    }

    public Track AddTurn(string description, TimeSpan averageTime, int carsAllowed)
    {
        _trackPoints.Add(new Turn(description, averageTime, carsAllowed));
        return this;
    }

    public Track AddStraight(string description, TimeSpan averageTime)
    {
        _trackPoints.Add(new Straight(description, averageTime));
        return this;
    }

    public Track AddPitLane(TimeSpan entryTime, TimeSpan exitTime, List<Team> teams,
        int nextPoint)
    {
        _pitLane = new PitLane("PitLane", teams, nextPoint);
        _pitLaneEntry = new Turn("PitLane Entry", entryTime, 1);
        _pitLaneExit = new Turn("PitLane Exit", exitTime, 1);
        return this;
    }

    /// <summary>
    /// Vrací seznam trackpoints s ohledem na to,
    /// zda má auto jít na konci kola vyměnit pneumatiky nebo ne
    /// </summary>
    /// <param name="car"></param>
    /// <returns></returns>
    public List<ITrackPoint> GetLap(RaceCar car, bool inPit)
    {
        var trackPoints = _trackPoints.ToList();
        if (inPit)
        {
            return trackPoints
                .Skip(_pitLane!.NextPoint)
                .Prepend(_pitLaneExit!)
                .Prepend(_pitLane)
                .ToList();
        }

        //I expect tires to last at least a single lap
        if (car.NeedsChange() && !inPit)
        {
            return trackPoints
                .TakeWhile(p => p.Description != "22 / No name Straight")
                .Append(_pitLaneEntry!)
                .ToList();
        }

        return trackPoints;
    }

}