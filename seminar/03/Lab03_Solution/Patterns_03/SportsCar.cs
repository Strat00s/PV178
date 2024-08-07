﻿namespace Patterns_03
{
    internal class SportsCar : Car
    {
        public bool HasLaunchControl { get; }

        public SportsCar(double accelerationTime, bool hasLaunchControl) : base(accelerationTime)
        {
            HasLaunchControl = hasLaunchControl;
        }
    }
}
