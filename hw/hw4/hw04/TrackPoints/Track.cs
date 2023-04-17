using hw04.Car;
using System.Numerics;

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
    public IEnumerable<ITrackPoint> GetLap(RaceCar car, int startIndex = 0)
    {
        //return normal track from index
        if (!car.GetTire().NeedsChange())
            return _trackPoints.Skip(startIndex);   //linq creates copy
        
        return _trackPoints
            .Skip(startIndex)
            .TakeWhile(p => p.Description != "22 / No name Straight")
            .Append(_pitLaneEntry!) //if any of these are null, fix your track!
            .Append(_pitLane!)
            .Append(_pitLaneExit!);
    }

}