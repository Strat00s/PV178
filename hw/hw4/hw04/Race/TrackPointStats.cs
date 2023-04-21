using hw04.TrackPoints;

namespace hw04.Race
{
    public class TrackPointStats
    {
        public ITrackPoint TrackPoint { get; }
        public TimeSpan WaitingTime { get; }
        public TimeSpan DrivingTime { get; }

        public TrackPointStats(ITrackPoint trackPoint, TimeSpan waitingTime, TimeSpan drivingTime)
        {
            TrackPoint = trackPoint;
            WaitingTime = waitingTime;
            DrivingTime = drivingTime;
        }
    }
}
