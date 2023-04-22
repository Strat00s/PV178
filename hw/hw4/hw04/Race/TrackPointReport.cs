using hw04.TrackPoints;

namespace hw04.Race
{
    public class TrackPointReport
    {
        public ITrackPoint TrackPoint { get; }
        public TimeSpan WaitingTime { get; }
        public TimeSpan DrivingTime { get; }

        public TrackPointReport(ITrackPoint trackPoint, TimeSpan drivingTime, TimeSpan waitingTime)
        {
            TrackPoint = trackPoint;
            WaitingTime = waitingTime;
            DrivingTime = drivingTime;
        }
    }
}
